// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Reflection.Metadata;

namespace StarkPlatform.Compiler.Stark.CodeGen
{
    internal partial class CodeGenerator
    {
        private enum ArrayInitializerStyle
        {
            // Initialize every element
            Element,

            // Initialize all elements at once from a metadata blob
            Block,

            // Mixed case where there are some initializers that are constants and
            // there is enough of them so that it makes sense to use block initialization
            // followed by individual initialization of non-constant elements
            Mixed,
        }

        /// <summary>
        /// Entry point to the array initialization.
        /// Assumes that we have newly created array on the stack.
        /// 
        /// inits could be an array of values for a single dimensional array
        /// or an array (of array)+ of values for a multidimensional case
        /// 
        /// in either case it is expected that number of leaf values will match number 
        /// of elements in the array and nesting level should match the rank of the array.
        /// </summary>
        private void EmitArrayInitializers(TypeSymbol arrayType, BoundArrayInitialization inits)
        {
            var initExprs = inits.Initializers;
            var initializationStyle = ShouldEmitBlockInitializer(arrayType.GetArrayElementType().TypeSymbol, initExprs);

            if (initializationStyle == ArrayInitializerStyle.Element)
            {
                this.EmitElementInitializers(arrayType, initExprs, true);
            }
            else
            {
                ImmutableArray<byte> data = this.GetRawData(initExprs);
                _builder.EmitArrayBlockInitializer(data, inits.Syntax, _diagnostics);

                if (initializationStyle == ArrayInitializerStyle.Mixed)
                {
                    EmitElementInitializers(arrayType, initExprs, false);
                }
            }
        }

        private void EmitElementInitializers(TypeSymbol arrayType,
                                            ImmutableArray<BoundExpression> inits,
                                            bool includeConstants)
        {
            EmitVectorElementInitializers(arrayType, inits, includeConstants);
        }

        private void EmitVectorElementInitializers(TypeSymbol arrayType,
                    ImmutableArray<BoundExpression> inits,
                    bool includeConstants)
        {
            for (int i = 0; i < inits.Length; i++)
            {
                var init = inits[i];
                if (ShouldEmitInitExpression(includeConstants, init))
                {
                    _builder.EmitOpCode(ILOpCode.Dup);
                    _builder.EmitIntConstant(i);
                    EmitExpression(init, true);
                    EmitArrayElementStore(arrayType, init.Syntax);
                }
            }
        }

        // if element init is not a constant we have no choice - we need to emit it
        // if element is a default value - no need to emit initializer, arrays are created zero inited.
        // if element is a not a constant or includeConstants flag is set, return true
        private static bool ShouldEmitInitExpression(bool includeConstants, BoundExpression init)
        {
            if (init.IsDefaultValue())
            {
                return false;
            }

            return includeConstants || init.ConstantValue == null;
        }

        private static ConstantValue AsConstOrDefault(BoundExpression init)
        {
            ConstantValue initConstantValueOpt = init.ConstantValue;

            if (initConstantValueOpt != null)
            {
                return initConstantValueOpt;
            }

            TypeSymbol type = init.Type.EnumUnderlyingType();
            return ConstantValue.Default(type.SpecialType);
        }

        private ArrayInitializerStyle ShouldEmitBlockInitializer(TypeSymbol elementType, ImmutableArray<BoundExpression> inits)
        {
            if (!_module.SupportsPrivateImplClass)
            {
                return ArrayInitializerStyle.Element;
            }

            if (elementType.IsEnumType())
            {
                if (!_module.Compilation.EnableEnumArrayBlockInitialization)
                {
                    return ArrayInitializerStyle.Element;
                }
                elementType = elementType.EnumUnderlyingType();
            }

            if (elementType.SpecialType.IsBlittable())
            {
                if (_module.GetInitArrayHelper() == null)
                {
                    return ArrayInitializerStyle.Element;
                }

                int initCount = 0;
                int constCount = 0;
                InitializerCountRecursive(inits, ref initCount, ref constCount);

                if (initCount > 2)
                {
                    if (initCount == constCount)
                    {
                        return ArrayInitializerStyle.Block;
                    }

                    int thresholdCnt = Math.Max(3, (initCount / 3));

                    if (constCount >= thresholdCnt)
                    {
                        return ArrayInitializerStyle.Mixed;
                    }
                }
            }

            return ArrayInitializerStyle.Element;
        }

        /// <summary>
        /// Count of all nontrivial initializers and count of those that are constants.
        /// </summary>
        private void InitializerCountRecursive(ImmutableArray<BoundExpression> inits, ref int initCount, ref int constInits)
        {
            if (inits.Length == 0)
            {
                return;
            }

            foreach (var init in inits)
            {
                var asArrayInit = init as BoundArrayInitialization;

                if (asArrayInit != null)
                {
                    InitializerCountRecursive(asArrayInit.Initializers, ref initCount, ref constInits);
                }
                else
                {
                    // NOTE: default values do not need to be initialized. 
                    //       .Net arrays are always zero-inited.
                    if (!init.IsDefaultValue())
                    {
                        initCount += 1;
                        if (init.ConstantValue != null)
                        {
                            constInits += 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Produces a serialized blob of all constant initializers.
        /// Non-constant initializers are matched with a zero of corresponding size.
        /// </summary>
        private ImmutableArray<byte> GetRawData(ImmutableArray<BoundExpression> initializers)
        {
            // the initial size is a guess.
            // there is no point to be precise here as MemoryStream always has N + 1 storage 
            // and will need to be trimmed regardless
            var writer = new BlobBuilder(initializers.Length * 4);

            SerializeArrayRecursive(writer, initializers);

            return writer.ToImmutableArray();
        }

        private void SerializeArrayRecursive(BlobBuilder bw, ImmutableArray<BoundExpression> inits)
        {
            if (inits.Length != 0)
            {
                if (inits[0].Kind == BoundKind.ArrayInitialization)
                {
                    foreach (var init in inits)
                    {
                        SerializeArrayRecursive(bw, ((BoundArrayInitialization)init).Initializers);
                    }
                }
                else
                {
                    foreach (var init in inits)
                    {
                        AsConstOrDefault(init).Serialize(bw);
                    }
                }
            }
        }

        /// <summary>
        /// Check if it is a regular collection of expressions or there are nested initializers.
        /// </summary>
        private static bool IsMultidimensionalInitializer(ImmutableArray<BoundExpression> inits)
        {
            Debug.Assert(inits.All((init) => init.Kind != BoundKind.ArrayInitialization) ||
                         inits.All((init) => init.Kind == BoundKind.ArrayInitialization),
                         "all or none should be nested");

            return inits.Length != 0 && inits[0].Kind == BoundKind.ArrayInitialization;
        }

        private bool TryEmitReadonlySpanAsBlobWrapper(NamedTypeSymbol spanType, BoundExpression wrappedExpression, bool used, bool inPlace)
        {
            ImmutableArray<byte> data = default;
            int elementCount = -1;
            TypeSymbol elementType = null;

            if (!_module.SupportsPrivateImplClass)
            {
                return false;
            }

            var ctor = ((MethodSymbol)this._module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_ReadOnlySpan_T__ctor));
            if (ctor == null)
            {
                return false;
            }

            if (wrappedExpression is BoundArrayCreation ac)
            {
                elementType = ac.Type.GetArrayElementType().TypeSymbol.EnumUnderlyingType();

                // NB: we cannot use this approach for element types larger than one byte
                //     the issue is that metadata stores blobs in little-endian format
                //     so anything that is larger than one byte will be incorrect on a big-endian machine
                //     With additional runtime support it might be possible, but not yet.
                //     See: https://github.com/dotnet/corefx/issues/26948 for more details
                if (elementType.SpecialType.SizeInBytes() != 1)
                {
                    return false;
                }

                elementCount = TryGetRawDataForArrayInit(ac.InitializerOpt, out data);
            }

            if (elementCount < 0)
            {
                return false;
            }

            if (!inPlace && !used)
            {
                // emitting a value that no one will see
                return true;
            }

            if (elementCount == 0)
            {
                if (inPlace)
                {
                    _builder.EmitOpCode(ILOpCode.Initobj);
                    EmitSymbolToken(spanType, wrappedExpression.Syntax);
                }
                else
                {
                    EmitDefaultValue(spanType, used, wrappedExpression.Syntax);
                }
            }
            else
            {
                if (IsPeVerifyCompatEnabled())
                {
                    return false;
                }

                _builder.EmitArrayBlockFieldRef(data, elementType, wrappedExpression.Syntax, _diagnostics);
                _builder.EmitIntConstant(elementCount);

                if (inPlace)
                {
                    // consumes target ref, data ptr and size, pushes nothing
                    _builder.EmitOpCode(ILOpCode.Call, stackAdjustment: -3);
                }
                else
                {
                    // consumes data ptr and size, pushes the instance
                    _builder.EmitOpCode(ILOpCode.Newobj, stackAdjustment: -1);
                }

                EmitSymbolToken(ctor.AsMember(spanType), wrappedExpression.Syntax, optArgList: null);
            }

            return true;
        }

        /// <summary>
        ///  Returns a byte blob that matches serialized content of single array initializer.    
        ///  returns -1 if the initializer is null or not an array of literals
        /// </summary>
        private int TryGetRawDataForArrayInit(BoundArrayInitialization initializer, out ImmutableArray<byte> data)
        {
            data = default;

            if (initializer == null)
            {
                return -1;
            }

            var initializers = initializer.Initializers;
            if (initializers.Any(init => init.ConstantValue == null))
            {
                return -1;
            }

            var elementCount = initializers.Length;
            if (elementCount == 0)
            {
                data = ImmutableArray<byte>.Empty;
                return 0;
            }

            var writer = new BlobBuilder(initializers.Length * 4);

            foreach (var init in initializer.Initializers)
            {
                init.ConstantValue.Serialize(writer);
            }

            data = writer.ToImmutableArray();
            return elementCount;
        }
    }
}
