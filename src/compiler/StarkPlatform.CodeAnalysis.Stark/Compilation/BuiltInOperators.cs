// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark
{
    /// <summary>
    /// Internal cache of built-in operators.
    /// Cache is compilation-specific because it uses compilation-specific SpecialTypes.
    /// </summary>
    internal class BuiltInOperators
    {
        private readonly CSharpCompilation _compilation;

        //actual lazily-constructed caches of built-in operators.
        private ImmutableArray<UnaryOperatorSignature>[] _builtInUnaryOperators;
        private ImmutableArray<BinaryOperatorSignature>[][] _builtInOperators;

        internal BuiltInOperators(CSharpCompilation compilation)
        {
            _compilation = compilation;
        }

        // PERF: Use int instead of UnaryOperatorKind so the compiler can use array literal initialization.
        //       The most natural type choice, Enum arrays, are not blittable due to a CLR limitation.
        private ImmutableArray<UnaryOperatorSignature> GetSignaturesFromUnaryOperatorKinds(int[] operatorKinds)
        {
            var builder = ArrayBuilder<UnaryOperatorSignature>.GetInstance();
            foreach (var kind in operatorKinds)
            {
                builder.Add(GetSignature((UnaryOperatorKind)kind));
            }

            return builder.ToImmutableAndFree();
        }

        internal void GetSimpleBuiltInOperators(UnaryOperatorKind kind, ArrayBuilder<UnaryOperatorSignature> operators)
        {
            if (_builtInUnaryOperators == null)
            {
                var allOperators = new ImmutableArray<UnaryOperatorSignature>[]
                {
                    GetSignaturesFromUnaryOperatorKinds(new []
                    {
                        (int)UnaryOperatorKind.Int8PostfixIncrement,
                        (int)UnaryOperatorKind.UInt8PostfixIncrement,
                        (int)UnaryOperatorKind.Int16PostfixIncrement,
                        (int)UnaryOperatorKind.UInt16PostfixIncrement,
                        (int)UnaryOperatorKind.Int32PostfixIncrement,
                        (int)UnaryOperatorKind.UInt32PostfixIncrement,
                        (int)UnaryOperatorKind.Int64PostfixIncrement,
                        (int)UnaryOperatorKind.UInt64PostfixIncrement,
                        (int)UnaryOperatorKind.CharPostfixIncrement,
                        (int)UnaryOperatorKind.Float32PostfixIncrement,
                        (int)UnaryOperatorKind.Float64PostfixIncrement,
                        (int)UnaryOperatorKind.DecimalPostfixIncrement,
                        (int)UnaryOperatorKind.IntPostfixIncrement,
                        (int)UnaryOperatorKind.UIntPostfixIncrement,
                        (int)UnaryOperatorKind.LiftedInt8PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedUInt8PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedInt16PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedUInt16PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedInt32PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedUInt32PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedInt64PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedUInt64PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedCharPostfixIncrement,
                        (int)UnaryOperatorKind.LiftedFloat32PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedFloat64PostfixIncrement,
                        (int)UnaryOperatorKind.LiftedDecimalPostfixIncrement,
                        (int)UnaryOperatorKind.LiftedIntPostfixIncrement,
                        (int)UnaryOperatorKind.LiftedUIntPostfixIncrement,
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new []
                    {
                        (int)UnaryOperatorKind.Int8PostfixDecrement,
                        (int)UnaryOperatorKind.UInt8PostfixDecrement,
                        (int)UnaryOperatorKind.Int16PostfixDecrement,
                        (int)UnaryOperatorKind.UInt16PostfixDecrement,
                        (int)UnaryOperatorKind.Int32PostfixDecrement,
                        (int)UnaryOperatorKind.UInt32PostfixDecrement,
                        (int)UnaryOperatorKind.Int64PostfixDecrement,
                        (int)UnaryOperatorKind.UInt64PostfixDecrement,
                        (int)UnaryOperatorKind.CharPostfixDecrement,
                        (int)UnaryOperatorKind.Float32PostfixDecrement,
                        (int)UnaryOperatorKind.Float64PostfixDecrement,
                        (int)UnaryOperatorKind.DecimalPostfixDecrement,
                        (int)UnaryOperatorKind.IntPostfixDecrement,
                        (int)UnaryOperatorKind.UIntPostfixDecrement,
                        (int)UnaryOperatorKind.LiftedInt8PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedUInt8PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedInt16PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedUInt16PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedInt32PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedUInt32PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedInt64PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedUInt64PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedCharPostfixDecrement,
                        (int)UnaryOperatorKind.LiftedFloat32PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedFloat64PostfixDecrement,
                        (int)UnaryOperatorKind.LiftedDecimalPostfixDecrement,
                        (int)UnaryOperatorKind.LiftedIntPostfixDecrement,
                        (int)UnaryOperatorKind.LiftedUIntPostfixDecrement,
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new []
                    {
                        (int)UnaryOperatorKind.Int8PrefixIncrement,
                        (int)UnaryOperatorKind.UInt8PrefixIncrement,
                        (int)UnaryOperatorKind.Int16PrefixIncrement,
                        (int)UnaryOperatorKind.UInt16PrefixIncrement,
                        (int)UnaryOperatorKind.Int32PrefixIncrement,
                        (int)UnaryOperatorKind.UInt32PrefixIncrement,
                        (int)UnaryOperatorKind.Int64PrefixIncrement,
                        (int)UnaryOperatorKind.UInt64PrefixIncrement,
                        (int)UnaryOperatorKind.CharPrefixIncrement,
                        (int)UnaryOperatorKind.Float32PrefixIncrement,
                        (int)UnaryOperatorKind.Float64PrefixIncrement,
                        (int)UnaryOperatorKind.DecimalPrefixIncrement,
                        (int)UnaryOperatorKind.IntPrefixIncrement,
                        (int)UnaryOperatorKind.UIntPrefixIncrement,
                        (int)UnaryOperatorKind.LiftedInt8PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedUInt8PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedInt16PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedUInt16PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedInt32PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedUInt32PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedInt64PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedUInt64PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedCharPrefixIncrement,
                        (int)UnaryOperatorKind.LiftedFloat32PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedFloat64PrefixIncrement,
                        (int)UnaryOperatorKind.LiftedDecimalPrefixIncrement,
                        (int)UnaryOperatorKind.LiftedIntPrefixIncrement,
                        (int)UnaryOperatorKind.LiftedUIntPrefixIncrement,
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new []
                    {
                        (int)UnaryOperatorKind.Int8PrefixDecrement,
                        (int)UnaryOperatorKind.UInt8PrefixDecrement,
                        (int)UnaryOperatorKind.Int16PrefixDecrement,
                        (int)UnaryOperatorKind.UInt16PrefixDecrement,
                        (int)UnaryOperatorKind.Int32PrefixDecrement,
                        (int)UnaryOperatorKind.UInt32PrefixDecrement,
                        (int)UnaryOperatorKind.Int64PrefixDecrement,
                        (int)UnaryOperatorKind.UInt64PrefixDecrement,
                        (int)UnaryOperatorKind.CharPrefixDecrement,
                        (int)UnaryOperatorKind.Float32PrefixDecrement,
                        (int)UnaryOperatorKind.Float64PrefixDecrement,
                        (int)UnaryOperatorKind.DecimalPrefixDecrement,
                        (int)UnaryOperatorKind.IntPrefixDecrement,
                        (int)UnaryOperatorKind.UIntPrefixDecrement,
                        (int)UnaryOperatorKind.LiftedInt8PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedUInt8PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedInt16PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedUInt16PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedInt32PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedUInt32PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedInt64PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedUInt64PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedCharPrefixDecrement,
                        (int)UnaryOperatorKind.LiftedFloat32PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedFloat64PrefixDecrement,
                        (int)UnaryOperatorKind.LiftedDecimalPrefixDecrement,
                        (int)UnaryOperatorKind.LiftedIntPrefixDecrement,
                        (int)UnaryOperatorKind.LiftedUIntPrefixDecrement,
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new []
                    {
                        (int)UnaryOperatorKind.Int32UnaryPlus,
                        (int)UnaryOperatorKind.UInt32UnaryPlus,
                        (int)UnaryOperatorKind.Int64UnaryPlus,
                        (int)UnaryOperatorKind.UInt64UnaryPlus,
                        (int)UnaryOperatorKind.Float32UnaryPlus,
                        (int)UnaryOperatorKind.Float64UnaryPlus,
                        (int)UnaryOperatorKind.DecimalUnaryPlus,
                        (int)UnaryOperatorKind.IntUnaryPlus,
                        (int)UnaryOperatorKind.UIntUnaryPlus,
                        (int)UnaryOperatorKind.LiftedInt32UnaryPlus,
                        (int)UnaryOperatorKind.LiftedUInt32UnaryPlus,
                        (int)UnaryOperatorKind.LiftedInt64UnaryPlus,
                        (int)UnaryOperatorKind.LiftedUInt64UnaryPlus,
                        (int)UnaryOperatorKind.LiftedFloat32UnaryPlus,
                        (int)UnaryOperatorKind.LiftedFloat64UnaryPlus,
                        (int)UnaryOperatorKind.LiftedDecimalUnaryPlus,
                        (int)UnaryOperatorKind.LiftedIntUnaryPlus,
                        (int)UnaryOperatorKind.LiftedUIntUnaryPlus,
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new []
                    {
                        (int)UnaryOperatorKind.Int32UnaryMinus,
                        (int)UnaryOperatorKind.Int64UnaryMinus,
                        (int)UnaryOperatorKind.Float32UnaryMinus,
                        (int)UnaryOperatorKind.Float64UnaryMinus,
                        (int)UnaryOperatorKind.DecimalUnaryMinus,
                        (int)UnaryOperatorKind.IntUnaryMinus,
                        (int)UnaryOperatorKind.LiftedInt32UnaryMinus,
                        (int)UnaryOperatorKind.LiftedInt64UnaryMinus,
                        (int)UnaryOperatorKind.LiftedFloat32UnaryMinus,
                        (int)UnaryOperatorKind.LiftedFloat64UnaryMinus,
                        (int)UnaryOperatorKind.LiftedDecimalUnaryMinus,
                        (int)UnaryOperatorKind.LiftedIntUnaryMinus,
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new []
                    {
                        (int)UnaryOperatorKind.BoolLogicalNegation,
                        (int)UnaryOperatorKind.LiftedBoolLogicalNegation,
                    }),
                    GetSignaturesFromUnaryOperatorKinds(new []
                    {
                        (int)UnaryOperatorKind.Int32BitwiseComplement,
                        (int)UnaryOperatorKind.UInt32BitwiseComplement,
                        (int)UnaryOperatorKind.Int64BitwiseComplement,
                        (int)UnaryOperatorKind.UInt64BitwiseComplement,
                        (int)UnaryOperatorKind.IntBitwiseComplement,
                        (int)UnaryOperatorKind.LiftedInt32BitwiseComplement,
                        (int)UnaryOperatorKind.LiftedUInt32BitwiseComplement,
                        (int)UnaryOperatorKind.LiftedInt64BitwiseComplement,
                        (int)UnaryOperatorKind.LiftedUInt64BitwiseComplement,
                        (int)UnaryOperatorKind.LiftedIntBitwiseComplement,
                    }),
                    // No built-in operator true or operator false
                    ImmutableArray<UnaryOperatorSignature>.Empty,
                    ImmutableArray<UnaryOperatorSignature>.Empty,
                };

                Interlocked.CompareExchange(ref _builtInUnaryOperators, allOperators, null);
            }

            operators.AddRange(_builtInUnaryOperators[kind.OperatorIndex()]);
        }

        internal UnaryOperatorSignature GetSignature(UnaryOperatorKind kind)
        {
            TypeSymbol opType;
            switch (kind.OperandTypes())
            {
                case UnaryOperatorKind.Int8: opType = _compilation.GetSpecialType(SpecialType.System_Int8); break;
                case UnaryOperatorKind.UInt8: opType = _compilation.GetSpecialType(SpecialType.System_UInt8); break;
                case UnaryOperatorKind.Int16: opType = _compilation.GetSpecialType(SpecialType.System_Int16); break;
                case UnaryOperatorKind.UShort: opType = _compilation.GetSpecialType(SpecialType.System_UInt16); break;
                case UnaryOperatorKind.Int32: opType = _compilation.GetSpecialType(SpecialType.System_Int32); break;
                case UnaryOperatorKind.UInt32: opType = _compilation.GetSpecialType(SpecialType.System_UInt32); break;
                case UnaryOperatorKind.Int64: opType = _compilation.GetSpecialType(SpecialType.System_Int64); break;
                case UnaryOperatorKind.UInt64: opType = _compilation.GetSpecialType(SpecialType.System_UInt64); break;
                case UnaryOperatorKind.Char: opType = _compilation.GetSpecialType(SpecialType.System_Char); break;
                case UnaryOperatorKind.Float32: opType = _compilation.GetSpecialType(SpecialType.System_Float32); break;
                case UnaryOperatorKind.Float64: opType = _compilation.GetSpecialType(SpecialType.System_Float64); break;
                case UnaryOperatorKind.Decimal: opType = _compilation.GetSpecialType(SpecialType.System_Decimal); break;
                case UnaryOperatorKind.Bool: opType = _compilation.GetSpecialType(SpecialType.System_Boolean); break;
                case UnaryOperatorKind.Int: opType = _compilation.GetSpecialType(SpecialType.System_Int); break;
                case UnaryOperatorKind.UInt: opType = _compilation.GetSpecialType(SpecialType.System_UInt); break;
                default: throw ExceptionUtilities.UnexpectedValue(kind.OperandTypes());
            }

            if (kind.IsLifted())
            {
                opType = _compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(opType);
            }

            return new UnaryOperatorSignature(kind, opType, opType);
        }

        // PERF: Use int instead of BinaryOperatorKind so the compiler can use array literal initialization.
        //       The most natural type choice, Enum arrays, are not blittable due to a CLR limitation.
        private ImmutableArray<BinaryOperatorSignature> GetSignaturesFromBinaryOperatorKinds(int[] operatorKinds)
        {
            var builder = ArrayBuilder<BinaryOperatorSignature>.GetInstance();
            foreach (var kind in operatorKinds)
            {
                builder.Add(GetSignature((BinaryOperatorKind)kind));
            }

            return builder.ToImmutableAndFree();
        }

        internal void GetSimpleBuiltInOperators(BinaryOperatorKind kind, ArrayBuilder<BinaryOperatorSignature> operators)
        {
            if (_builtInOperators == null)
            {
                var logicalOperators = new ImmutableArray<BinaryOperatorSignature>[]
                {
                    ImmutableArray<BinaryOperatorSignature>.Empty, //multiplication
                    ImmutableArray<BinaryOperatorSignature>.Empty, //addition
                    ImmutableArray<BinaryOperatorSignature>.Empty, //subtraction
                    ImmutableArray<BinaryOperatorSignature>.Empty, //division
                    ImmutableArray<BinaryOperatorSignature>.Empty, //remainder
                    ImmutableArray<BinaryOperatorSignature>.Empty, //left shift
                    ImmutableArray<BinaryOperatorSignature>.Empty, //right shift
                    ImmutableArray<BinaryOperatorSignature>.Empty, //equal
                    ImmutableArray<BinaryOperatorSignature>.Empty, //not equal
                    ImmutableArray<BinaryOperatorSignature>.Empty, //greater than
                    ImmutableArray<BinaryOperatorSignature>.Empty, //less than
                    ImmutableArray<BinaryOperatorSignature>.Empty, //greater than or equal
                    ImmutableArray<BinaryOperatorSignature>.Empty, //less than or equal
                    ImmutableArray.Create<BinaryOperatorSignature>(GetSignature(BinaryOperatorKind.LogicalBoolAnd)), //and
                    ImmutableArray<BinaryOperatorSignature>.Empty, //xor
                    ImmutableArray.Create<BinaryOperatorSignature>(GetSignature(BinaryOperatorKind.LogicalBoolOr)), //or
                };

                var nonLogicalOperators = new ImmutableArray<BinaryOperatorSignature>[]
                {
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32Multiplication,
                        (int)BinaryOperatorKind.UInt32Multiplication,
                        (int)BinaryOperatorKind.Int64Multiplication,
                        (int)BinaryOperatorKind.UInt64Multiplication,
                        (int)BinaryOperatorKind.Float32Multiplication,
                        (int)BinaryOperatorKind.Float64Multiplication,
                        (int)BinaryOperatorKind.DecimalMultiplication,
                        (int)BinaryOperatorKind.IntMultiplication,
                        (int)BinaryOperatorKind.UIntMultiplication,
                        (int)BinaryOperatorKind.LiftedInt32Multiplication,
                        (int)BinaryOperatorKind.LiftedUInt32Multiplication,
                        (int)BinaryOperatorKind.LiftedInt64Multiplication,
                        (int)BinaryOperatorKind.LiftedUInt64Multiplication,
                        (int)BinaryOperatorKind.LiftedFloat32Multiplication,
                        (int)BinaryOperatorKind.LiftedFloat64Multiplication,
                        (int)BinaryOperatorKind.LiftedDecimalMultiplication,
                        (int)BinaryOperatorKind.LiftedIntMultiplication,
                        (int)BinaryOperatorKind.LiftedUIntMultiplication,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32Addition,
                        (int)BinaryOperatorKind.UInt32Addition,
                        (int)BinaryOperatorKind.Int64Addition,
                        (int)BinaryOperatorKind.UInt64Addition,
                        (int)BinaryOperatorKind.Float32Addition,
                        (int)BinaryOperatorKind.Float64Addition,
                        (int)BinaryOperatorKind.DecimalAddition,
                        (int)BinaryOperatorKind.IntAddition,
                        (int)BinaryOperatorKind.UIntAddition,
                        (int)BinaryOperatorKind.LiftedInt32Addition,
                        (int)BinaryOperatorKind.LiftedUInt32Addition,
                        (int)BinaryOperatorKind.LiftedInt64Addition,
                        (int)BinaryOperatorKind.LiftedUInt64Addition,
                        (int)BinaryOperatorKind.LiftedFloat32Addition,
                        (int)BinaryOperatorKind.LiftedFloat64Addition,
                        (int)BinaryOperatorKind.LiftedDecimalAddition,
                        (int)BinaryOperatorKind.LiftedIntAddition,
                        (int)BinaryOperatorKind.LiftedUIntAddition,
                        (int)BinaryOperatorKind.StringConcatenation,
                        (int)BinaryOperatorKind.StringAndObjectConcatenation,
                        (int)BinaryOperatorKind.ObjectAndStringConcatenation,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32Subtraction,
                        (int)BinaryOperatorKind.UInt32Subtraction,
                        (int)BinaryOperatorKind.Int64Subtraction,
                        (int)BinaryOperatorKind.UInt64Subtraction,
                        (int)BinaryOperatorKind.Float32Subtraction,
                        (int)BinaryOperatorKind.Float64Subtraction,
                        (int)BinaryOperatorKind.DecimalSubtraction,
                        (int)BinaryOperatorKind.IntSubtraction,
                        (int)BinaryOperatorKind.UIntSubtraction,
                        (int)BinaryOperatorKind.Lifted32IntSubtraction,
                        (int)BinaryOperatorKind.LiftedUInt32Subtraction,
                        (int)BinaryOperatorKind.LiftedInt64Subtraction,
                        (int)BinaryOperatorKind.LiftedUInt64Subtraction,
                        (int)BinaryOperatorKind.LiftedFloat32Subtraction,
                        (int)BinaryOperatorKind.LiftedFloat64Subtraction,
                        (int)BinaryOperatorKind.LiftedDecimalSubtraction,
                        (int)BinaryOperatorKind.LiftedIntSubtraction,
                        (int)BinaryOperatorKind.LiftedUIntSubtraction,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32Division,
                        (int)BinaryOperatorKind.UInt32Division,
                        (int)BinaryOperatorKind.Int64Division,
                        (int)BinaryOperatorKind.UInt64Division,
                        (int)BinaryOperatorKind.Float32Division,
                        (int)BinaryOperatorKind.Float64Division,
                        (int)BinaryOperatorKind.DecimalDivision,
                        (int)BinaryOperatorKind.IntDivision,
                        (int)BinaryOperatorKind.UIntDivision,
                        (int)BinaryOperatorKind.LiftedInt32Division,
                        (int)BinaryOperatorKind.LiftedUInt32Division,
                        (int)BinaryOperatorKind.LiftedInt64Division,
                        (int)BinaryOperatorKind.LiftedUInt64Division,
                        (int)BinaryOperatorKind.LiftedFloat32Division,
                        (int)BinaryOperatorKind.LiftedFloat64Division,
                        (int)BinaryOperatorKind.LiftedDecimalDivision,
                        (int)BinaryOperatorKind.LiftedIntDivision,
                        (int)BinaryOperatorKind.LiftedUIntDivision,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32Remainder,
                        (int)BinaryOperatorKind.UInt32Remainder,
                        (int)BinaryOperatorKind.Int64Remainder,
                        (int)BinaryOperatorKind.UInt64Remainder,
                        (int)BinaryOperatorKind.Float32Remainder,
                        (int)BinaryOperatorKind.Float64Remainder,
                        (int)BinaryOperatorKind.DecimalRemainder,
                        (int)BinaryOperatorKind.IntRemainder,
                        (int)BinaryOperatorKind.UIntRemainder,
                        (int)BinaryOperatorKind.LiftedInt32Remainder,
                        (int)BinaryOperatorKind.LiftedUInt32Remainder,
                        (int)BinaryOperatorKind.LiftedInt64Remainder,
                        (int)BinaryOperatorKind.LiftedUInt64Remainder,
                        (int)BinaryOperatorKind.LiftedFloat32Remainder,
                        (int)BinaryOperatorKind.LiftedFloat64Remainder,
                        (int)BinaryOperatorKind.LiftedDecimalRemainder,
                        (int)BinaryOperatorKind.LiftedIntRemainder,
                        (int)BinaryOperatorKind.LiftedUIntRemainder,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32LeftShift,
                        (int)BinaryOperatorKind.UInt32LeftShift,
                        (int)BinaryOperatorKind.Int64LeftShift,
                        (int)BinaryOperatorKind.UInt64LeftShift,
                        (int)BinaryOperatorKind.IntLeftShift,
                        (int)BinaryOperatorKind.UIntLeftShift,
                        (int)BinaryOperatorKind.LiftedInt32LeftShift,
                        (int)BinaryOperatorKind.LiftedUInt32LeftShift,
                        (int)BinaryOperatorKind.LiftedInt64LeftShift,
                        (int)BinaryOperatorKind.LiftedUInt64LeftShift,
                        (int)BinaryOperatorKind.LiftedIntLeftShift,
                        (int)BinaryOperatorKind.LiftedUIntLeftShift,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32RightShift,
                        (int)BinaryOperatorKind.UInt32RightShift,
                        (int)BinaryOperatorKind.Int64RightShift,
                        (int)BinaryOperatorKind.UInt64RightShift,
                        (int)BinaryOperatorKind.IntRightShift,
                        (int)BinaryOperatorKind.UIntRightShift,
                        (int)BinaryOperatorKind.LiftedInt32RightShift,
                        (int)BinaryOperatorKind.LiftedUInt32RightShift,
                        (int)BinaryOperatorKind.LiftedInt64RightShift,
                        (int)BinaryOperatorKind.LiftedUInt64RightShift,
                        (int)BinaryOperatorKind.LiftedIntRightShift,
                        (int)BinaryOperatorKind.LiftedUIntRightShift,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32Equal,
                        (int)BinaryOperatorKind.UInt32Equal,
                        (int)BinaryOperatorKind.Int64Equal,
                        (int)BinaryOperatorKind.UInt64Equal,
                        (int)BinaryOperatorKind.Float32Equal,
                        (int)BinaryOperatorKind.Float64Equal,
                        (int)BinaryOperatorKind.DecimalEqual,
                        (int)BinaryOperatorKind.IntEqual,
                        (int)BinaryOperatorKind.UIntEqual,
                        (int)BinaryOperatorKind.BoolEqual,
                        (int)BinaryOperatorKind.LiftedInt32Equal,
                        (int)BinaryOperatorKind.LiftedUInt32Equal,
                        (int)BinaryOperatorKind.LiftedInt64Equal,
                        (int)BinaryOperatorKind.LiftedUInt64Equal,
                        (int)BinaryOperatorKind.LiftedFloat32Equal,
                        (int)BinaryOperatorKind.LiftedFloat64Equal,
                        (int)BinaryOperatorKind.LiftedDecimalEqual,
                        (int)BinaryOperatorKind.LiftedIntEqual,
                        (int)BinaryOperatorKind.LiftedUIntEqual,
                        (int)BinaryOperatorKind.LiftedBoolEqual,
                        (int)BinaryOperatorKind.ObjectEqual,
                        (int)BinaryOperatorKind.StringEqual,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32NotEqual,
                        (int)BinaryOperatorKind.UInt32NotEqual,
                        (int)BinaryOperatorKind.Int64NotEqual,
                        (int)BinaryOperatorKind.UInt64NotEqual,
                        (int)BinaryOperatorKind.Float32NotEqual,
                        (int)BinaryOperatorKind.Float64NotEqual,
                        (int)BinaryOperatorKind.DecimalNotEqual,
                        (int)BinaryOperatorKind.IntNotEqual,
                        (int)BinaryOperatorKind.UIntNotEqual,
                        (int)BinaryOperatorKind.BoolNotEqual,
                        (int)BinaryOperatorKind.LiftedInt32NotEqual,
                        (int)BinaryOperatorKind.LiftedUInt32NotEqual,
                        (int)BinaryOperatorKind.LiftedInt64NotEqual,
                        (int)BinaryOperatorKind.LiftedUInt64NotEqual,
                        (int)BinaryOperatorKind.LiftedFloat32NotEqual,
                        (int)BinaryOperatorKind.LiftedFloat64NotEqual,
                        (int)BinaryOperatorKind.LiftedDecimalNotEqual,
                        (int)BinaryOperatorKind.LiftedIntNotEqual,
                        (int)BinaryOperatorKind.LiftedUIntNotEqual,
                        (int)BinaryOperatorKind.LiftedBoolNotEqual,
                        (int)BinaryOperatorKind.ObjectNotEqual,
                        (int)BinaryOperatorKind.StringNotEqual,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32GreaterThan,
                        (int)BinaryOperatorKind.UInt32GreaterThan,
                        (int)BinaryOperatorKind.Int64GreaterThan,
                        (int)BinaryOperatorKind.UInt64GreaterThan,
                        (int)BinaryOperatorKind.Float32GreaterThan,
                        (int)BinaryOperatorKind.Float64GreaterThan,
                        (int)BinaryOperatorKind.DecimalGreaterThan,
                        (int)BinaryOperatorKind.IntGreaterThan,
                        (int)BinaryOperatorKind.UIntGreaterThan,
                        (int)BinaryOperatorKind.Lifted32IntGreaterThan,
                        (int)BinaryOperatorKind.LiftedUInt32GreaterThan,
                        (int)BinaryOperatorKind.LiftedInt64GreaterThan,
                        (int)BinaryOperatorKind.LiftedUInt64GreaterThan,
                        (int)BinaryOperatorKind.LiftedFloat32GreaterThan,
                        (int)BinaryOperatorKind.LiftedFloat64GreaterThan,
                        (int)BinaryOperatorKind.LiftedDecimalGreaterThan,
                        (int)BinaryOperatorKind.LiftedIntGreaterThan,
                        (int)BinaryOperatorKind.LiftedUIntGreaterThan,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32LessThan,
                        (int)BinaryOperatorKind.UInt32LessThan,
                        (int)BinaryOperatorKind.Int64LessThan,
                        (int)BinaryOperatorKind.UInt64LessThan,
                        (int)BinaryOperatorKind.Float32LessThan,
                        (int)BinaryOperatorKind.Float64LessThan,
                        (int)BinaryOperatorKind.DecimalLessThan,
                        (int)BinaryOperatorKind.IntLessThan,
                        (int)BinaryOperatorKind.UIntLessThan,
                        (int)BinaryOperatorKind.LiftedInt32LessThan,
                        (int)BinaryOperatorKind.LiftedUInt32LessThan,
                        (int)BinaryOperatorKind.LiftedInt64LessThan,
                        (int)BinaryOperatorKind.LiftedUInt64LessThan,
                        (int)BinaryOperatorKind.LiftedFloat32LessThan,
                        (int)BinaryOperatorKind.LiftedFloat64LessThan,
                        (int)BinaryOperatorKind.LiftedDecimalLessThan,
                        (int)BinaryOperatorKind.LiftedIntLessThan,
                        (int)BinaryOperatorKind.LiftedUIntLessThan,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32GreaterThanOrEqual,
                        (int)BinaryOperatorKind.UInt32GreaterThanOrEqual,
                        (int)BinaryOperatorKind.Int64GreaterThanOrEqual,
                        (int)BinaryOperatorKind.UInt64GreaterThanOrEqual,
                        (int)BinaryOperatorKind.Float32GreaterThanOrEqual,
                        (int)BinaryOperatorKind.Float64GreaterThanOrEqual,
                        (int)BinaryOperatorKind.DecimalGreaterThanOrEqual,
                        (int)BinaryOperatorKind.IntGreaterThanOrEqual,
                        (int)BinaryOperatorKind.UIntGreaterThanOrEqual,
                        (int)BinaryOperatorKind.LiftedInt32GreaterThanOrEqual,
                        (int)BinaryOperatorKind.LiftedUInt32GreaterThanOrEqual,
                        (int)BinaryOperatorKind.LiftedInt64GreaterThanOrEqual,
                        (int)BinaryOperatorKind.LiftedUInt64GreaterThanOrEqual,
                        (int)BinaryOperatorKind.LiftedFloat32GreaterThanOrEqual,
                        (int)BinaryOperatorKind.LiftedFloat64GreaterThanOrEqual,
                        (int)BinaryOperatorKind.LiftedDecimalGreaterThanOrEqual,
                        (int)BinaryOperatorKind.LiftedIntGreaterThanOrEqual,
                        (int)BinaryOperatorKind.LiftedUIntGreaterThanOrEqual,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32LessThanOrEqual,
                        (int)BinaryOperatorKind.UInt32LessThanOrEqual,
                        (int)BinaryOperatorKind.Int64LessThanOrEqual,
                        (int)BinaryOperatorKind.UInt64LessThanOrEqual,
                        (int)BinaryOperatorKind.Float32LessThanOrEqual,
                        (int)BinaryOperatorKind.Float64LessThanOrEqual,
                        (int)BinaryOperatorKind.DecimalLessThanOrEqual,
                        (int)BinaryOperatorKind.IntLessThanOrEqual,
                        (int)BinaryOperatorKind.UIntLessThanOrEqual,
                        (int)BinaryOperatorKind.LiftedInt32LessThanOrEqual,
                        (int)BinaryOperatorKind.LiftedUInt32LessThanOrEqual,
                        (int)BinaryOperatorKind.LiftedInt64LessThanOrEqual,
                        (int)BinaryOperatorKind.LiftedUInt64LessThanOrEqual,
                        (int)BinaryOperatorKind.LiftedFloat32LessThanOrEqual,
                        (int)BinaryOperatorKind.LiftedFloat64LessThanOrEqual,
                        (int)BinaryOperatorKind.LiftedDecimalLessThanOrEqual,
                        (int)BinaryOperatorKind.LiftedIntLessThanOrEqual,
                        (int)BinaryOperatorKind.LiftedUIntLessThanOrEqual,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32And,
                        (int)BinaryOperatorKind.UInt32And,
                        (int)BinaryOperatorKind.Int64And,
                        (int)BinaryOperatorKind.UInt64And,
                        (int)BinaryOperatorKind.IntAnd,
                        (int)BinaryOperatorKind.UIntAnd,
                        (int)BinaryOperatorKind.BoolAnd,
                        (int)BinaryOperatorKind.LiftedInt32And,
                        (int)BinaryOperatorKind.LiftedUInt32And,
                        (int)BinaryOperatorKind.LiftedInt64And,
                        (int)BinaryOperatorKind.LiftedUInt64And,
                        (int)BinaryOperatorKind.LiftedIntAnd,
                        (int)BinaryOperatorKind.LiftedUIntAnd,
                        (int)BinaryOperatorKind.LiftedBoolAnd,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32Xor,
                        (int)BinaryOperatorKind.UInt32Xor,
                        (int)BinaryOperatorKind.Int64Xor,
                        (int)BinaryOperatorKind.UInt64Xor,
                        (int)BinaryOperatorKind.IntXor,
                        (int)BinaryOperatorKind.UIntXor,
                        (int)BinaryOperatorKind.BoolXor,
                        (int)BinaryOperatorKind.LiftedInt32Xor,
                        (int)BinaryOperatorKind.LiftedUInt32Xor,
                        (int)BinaryOperatorKind.LiftedInt64Xor,
                        (int)BinaryOperatorKind.LiftedUInt64Xor,
                        (int)BinaryOperatorKind.LiftedIntXor,
                        (int)BinaryOperatorKind.LiftedUIntXor,
                        (int)BinaryOperatorKind.LiftedBoolXor,
                    }),
                    GetSignaturesFromBinaryOperatorKinds(new []
                    {
                        (int)BinaryOperatorKind.Int32Or,
                        (int)BinaryOperatorKind.UInt32Or,
                        (int)BinaryOperatorKind.Int64Or,
                        (int)BinaryOperatorKind.UInt64Or,
                        (int)BinaryOperatorKind.IntOr,
                        (int)BinaryOperatorKind.UIntOr,
                        (int)BinaryOperatorKind.BoolOr,
                        (int)BinaryOperatorKind.LiftedInt32Or,
                        (int)BinaryOperatorKind.LiftedUInt32Or,
                        (int)BinaryOperatorKind.LiftedInt64Or,
                        (int)BinaryOperatorKind.LiftedUInt64Or,
                        (int)BinaryOperatorKind.LiftedIntOr,
                        (int)BinaryOperatorKind.LiftedUIntOr,
                        (int)BinaryOperatorKind.LiftedBoolOr,
                    }),
                };

                var allOperators = new[] { nonLogicalOperators, logicalOperators };

                Interlocked.CompareExchange(ref _builtInOperators, allOperators, null);
            }

            operators.AddRange(_builtInOperators[kind.IsLogical() ? 1 : 0][kind.OperatorIndex()]);
        }

        internal BinaryOperatorSignature GetSignature(BinaryOperatorKind kind)
        {
            var left = LeftType(kind);
            switch (kind.Operator())
            {
                case BinaryOperatorKind.Multiplication:
                case BinaryOperatorKind.Division:
                case BinaryOperatorKind.Subtraction:
                case BinaryOperatorKind.Remainder:
                case BinaryOperatorKind.And:
                case BinaryOperatorKind.Or:
                case BinaryOperatorKind.Xor:
                    return new BinaryOperatorSignature(kind, left, left, left);
                case BinaryOperatorKind.Addition:
                    return new BinaryOperatorSignature(kind, left, RightType(kind), ReturnType(kind));
                case BinaryOperatorKind.LeftShift:

                case BinaryOperatorKind.RightShift:
                    TypeSymbol returnType = _compilation.GetSpecialType(SpecialType.System_Int32);

                    if (kind.IsLifted())
                    {
                        returnType = _compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(returnType);
                    }

                    return new BinaryOperatorSignature(kind, left, returnType, left);

                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThanOrEqual:
                    return new BinaryOperatorSignature(kind, left, left, _compilation.GetSpecialType(SpecialType.System_Boolean));
            }
            return new BinaryOperatorSignature(kind, left, RightType(kind), ReturnType(kind));
        }

        private TypeSymbol LeftType(BinaryOperatorKind kind)
        {
            if (kind.IsLifted())
            {
                return LiftedType(kind);
            }
            else
            {
                switch (kind.OperandTypes())
                {
                    case BinaryOperatorKind.Int32: return _compilation.GetSpecialType(SpecialType.System_Int32);
                    case BinaryOperatorKind.UInt32: return _compilation.GetSpecialType(SpecialType.System_UInt32);
                    case BinaryOperatorKind.Int64: return _compilation.GetSpecialType(SpecialType.System_Int64);
                    case BinaryOperatorKind.UInt64: return _compilation.GetSpecialType(SpecialType.System_UInt64);
                    case BinaryOperatorKind.Float32: return _compilation.GetSpecialType(SpecialType.System_Float32);
                    case BinaryOperatorKind.Float64: return _compilation.GetSpecialType(SpecialType.System_Float64);
                    case BinaryOperatorKind.Decimal: return _compilation.GetSpecialType(SpecialType.System_Decimal);
                    case BinaryOperatorKind.Int: return _compilation.GetSpecialType(SpecialType.System_Int);
                    case BinaryOperatorKind.UInt: return _compilation.GetSpecialType(SpecialType.System_UInt);
                    case BinaryOperatorKind.Bool: return _compilation.GetSpecialType(SpecialType.System_Boolean);
                    case BinaryOperatorKind.ObjectAndString:
                    case BinaryOperatorKind.Object:
                        return _compilation.GetSpecialType(SpecialType.System_Object);
                    case BinaryOperatorKind.String:
                    case BinaryOperatorKind.StringAndObject:
                        return _compilation.GetSpecialType(SpecialType.System_String);
                }
            }
            Debug.Assert(false, "Bad operator kind in left type");
            return null;
        }

        private TypeSymbol RightType(BinaryOperatorKind kind)
        {
            if (kind.IsLifted())
            {
                return LiftedType(kind);
            }
            else
            {
                switch (kind.OperandTypes())
                {
                    case BinaryOperatorKind.Int32: return _compilation.GetSpecialType(SpecialType.System_Int32);
                    case BinaryOperatorKind.UInt32: return _compilation.GetSpecialType(SpecialType.System_UInt32);
                    case BinaryOperatorKind.Int64: return _compilation.GetSpecialType(SpecialType.System_Int64);
                    case BinaryOperatorKind.UInt64: return _compilation.GetSpecialType(SpecialType.System_UInt64);
                    case BinaryOperatorKind.Float32: return _compilation.GetSpecialType(SpecialType.System_Float32);
                    case BinaryOperatorKind.Float64: return _compilation.GetSpecialType(SpecialType.System_Float64);
                    case BinaryOperatorKind.Decimal: return _compilation.GetSpecialType(SpecialType.System_Decimal);
                    case BinaryOperatorKind.Int: return _compilation.GetSpecialType(SpecialType.System_Int);
                    case BinaryOperatorKind.UInt: return _compilation.GetSpecialType(SpecialType.System_UInt);
                    case BinaryOperatorKind.Bool: return _compilation.GetSpecialType(SpecialType.System_Boolean);
                    case BinaryOperatorKind.ObjectAndString:
                    case BinaryOperatorKind.String:
                        return _compilation.GetSpecialType(SpecialType.System_String);
                    case BinaryOperatorKind.StringAndObject:
                    case BinaryOperatorKind.Object:
                        return _compilation.GetSpecialType(SpecialType.System_Object);
                }
            }
            Debug.Assert(false, "Bad operator kind in right type");
            return null;
        }

        private TypeSymbol ReturnType(BinaryOperatorKind kind)
        {
            if (kind.IsLifted())
            {
                return LiftedType(kind);
            }
            else
            {
                switch (kind.OperandTypes())
                {
                    case BinaryOperatorKind.Int32: return _compilation.GetSpecialType(SpecialType.System_Int32);
                    case BinaryOperatorKind.UInt32: return _compilation.GetSpecialType(SpecialType.System_UInt32);
                    case BinaryOperatorKind.Int64: return _compilation.GetSpecialType(SpecialType.System_Int64);
                    case BinaryOperatorKind.UInt64: return _compilation.GetSpecialType(SpecialType.System_UInt64);
                    case BinaryOperatorKind.Float32: return _compilation.GetSpecialType(SpecialType.System_Float32);
                    case BinaryOperatorKind.Float64: return _compilation.GetSpecialType(SpecialType.System_Float64);
                    case BinaryOperatorKind.Decimal: return _compilation.GetSpecialType(SpecialType.System_Decimal);
                    case BinaryOperatorKind.Int: return _compilation.GetSpecialType(SpecialType.System_Int);
                    case BinaryOperatorKind.UInt: return _compilation.GetSpecialType(SpecialType.System_UInt);
                    case BinaryOperatorKind.Bool: return _compilation.GetSpecialType(SpecialType.System_Boolean);
                    case BinaryOperatorKind.Object: return _compilation.GetSpecialType(SpecialType.System_Object);
                    case BinaryOperatorKind.ObjectAndString:
                    case BinaryOperatorKind.StringAndObject:
                    case BinaryOperatorKind.String:
                        return _compilation.GetSpecialType(SpecialType.System_String);
                }
            }
            Debug.Assert(false, "Bad operator kind in return type");
            return null;
        }

        private TypeSymbol LiftedType(BinaryOperatorKind kind)
        {
            Debug.Assert(kind.IsLifted());

            var nullable = _compilation.GetSpecialType(SpecialType.System_Nullable_T);

            switch (kind.OperandTypes())
            {
                case BinaryOperatorKind.Int32: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_Int32));
                case BinaryOperatorKind.UInt32: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_UInt32));
                case BinaryOperatorKind.Int64: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_Int64));
                case BinaryOperatorKind.UInt64: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_UInt64));
                case BinaryOperatorKind.Float32: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_Float32));
                case BinaryOperatorKind.Float64: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_Float64));
                case BinaryOperatorKind.Decimal: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_Decimal));
                case BinaryOperatorKind.Int: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_Int));
                case BinaryOperatorKind.UInt: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_UInt));
                case BinaryOperatorKind.Bool: return nullable.Construct(_compilation.GetSpecialType(SpecialType.System_Boolean));
            }
            Debug.Assert(false, "Bad operator kind in lifted type");
            return null;
        }

        internal static bool IsValidObjectEquality(Conversions Conversions, TypeSymbol leftType, bool leftIsNullOrDefault, TypeSymbol rightType, bool rightIsNullOrDefault, ref HashSet<DiagnosticInfo> useSiteDiagnostics)
        {
            // SPEC: The predefined reference type equality operators require one of the following:

            // SPEC: (1) Both operands are a value of a type known to be a reference-type or the literal null. 
            // SPEC:     Furthermore, an explicit reference conversion exists from the type of either 
            // SPEC:     operand to the type of the other operand. Or:
            // SPEC: (2) One operand is a value of type T where T is a type-parameter and the other operand is 
            // SPEC:     the literal null. Furthermore T does not have the value type constraint.

            // SPEC ERROR: Notice that the spec calls out that an explicit reference conversion must exist;
            // SPEC ERROR: in fact it should say that an explicit reference conversion, implicit reference
            // SPEC ERROR: conversion or identity conversion must exist. The conversion from object to object
            // SPEC ERROR: is not classified as a reference conversion at all; it is an identity conversion.

            // Dev10 does not follow the spec exactly for type parameters. Specifically, in Dev10,
            // if a type parameter argument is known to be a value type, or if a type parameter
            // argument is not known to be either a value type or reference type and the other
            // argument is not null, reference type equality cannot be applied. Otherwise, the
            // effective base class of the type parameter is used to determine the conversion
            // to the other argument type. (See ExpressionBinder::GetRefEqualSigs.)

            if (((object)leftType != null) && leftType.IsTypeParameter())
            {
                if (leftType.IsValueType || (!leftType.IsReferenceType && !rightIsNullOrDefault))
                {
                    return false;
                }

                leftType = ((TypeParameterSymbol)leftType).EffectiveBaseClass(ref useSiteDiagnostics);
                Debug.Assert((object)leftType != null);
            }

            if (((object)rightType != null) && rightType.IsTypeParameter())
            {
                if (rightType.IsValueType || (!rightType.IsReferenceType && !leftIsNullOrDefault))
                {
                    return false;
                }

                rightType = ((TypeParameterSymbol)rightType).EffectiveBaseClass(ref useSiteDiagnostics);
                Debug.Assert((object)rightType != null);
            }

            var leftIsReferenceType = ((object)leftType != null) && leftType.IsReferenceType;
            if (!leftIsReferenceType && !leftIsNullOrDefault)
            {
                return false;
            }

            var rightIsReferenceType = ((object)rightType != null) && rightType.IsReferenceType;
            if (!rightIsReferenceType && !rightIsNullOrDefault)
            {
                return false;
            }

            // If at least one side is null then clearly a conversion exists.
            if (leftIsNullOrDefault || rightIsNullOrDefault)
            {
                return true;
            }

            var leftConversion = Conversions.ClassifyConversionFromType(leftType, rightType, ref useSiteDiagnostics);
            if (leftConversion.IsIdentity || leftConversion.IsReference)
            {
                return true;
            }

            var rightConversion = Conversions.ClassifyConversionFromType(rightType, leftType, ref useSiteDiagnostics);
            if (rightConversion.IsIdentity || rightConversion.IsReference)
            {
                return true;
            }

            return false;
        }
    }
}
