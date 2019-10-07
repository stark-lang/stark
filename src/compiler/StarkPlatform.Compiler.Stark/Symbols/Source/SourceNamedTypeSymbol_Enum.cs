// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Symbols
{
    internal partial class SourceNamedTypeSymbol
    {
        private NamedTypeSymbol _lazyEnumUnderlyingType = ErrorTypeSymbol.UnknownResultType;

        /// <summary>
        /// For enum types, gets the underlying type. Returns null on all other
        /// kinds of types.
        /// </summary>
        public override NamedTypeSymbol EnumUnderlyingType
        {
            get
            {
                if (ReferenceEquals(_lazyEnumUnderlyingType, ErrorTypeSymbol.UnknownResultType))
                {
                    DiagnosticBag diagnostics = DiagnosticBag.GetInstance();
                    if ((object)Interlocked.CompareExchange(ref _lazyEnumUnderlyingType, this.GetEnumUnderlyingType(diagnostics), ErrorTypeSymbol.UnknownResultType) ==
                        (object)ErrorTypeSymbol.UnknownResultType)
                    {
                        AddDeclarationDiagnostics(diagnostics);
                        this.state.NotePartComplete(CompletionPart.EnumUnderlyingType);
                    }
                    diagnostics.Free();
                }

                return _lazyEnumUnderlyingType;
            }
        }

        private NamedTypeSymbol GetEnumUnderlyingType(DiagnosticBag diagnostics)
        {
            if (this.TypeKind != TypeKind.Enum)
            {
                return null;
            }

            var compilation = this.DeclaringCompilation;
            var decl = this.declaration.Declarations[0];
            var bases = GetImplementListOpt(decl);
            if (bases != null)
            {
                var types = bases.Types;
                if (types.Count > 0)
                {
                    var typeSyntax = types[0].Type;

                    var baseBinder = compilation.GetBinder(bases);
                    var type = baseBinder.BindType(typeSyntax, diagnostics).TypeSymbol;

                    // Error types are not exposed to the caller. In those
                    // cases, the underlying type is treated as int.
                    if (!type.SpecialType.IsValidEnumUnderlyingType())
                    {
                        diagnostics.Add(ErrorCode.ERR_IntegralTypeExpected, typeSyntax.Location);
                        type = compilation.GetSpecialType(SpecialType.System_Int32);
                    }

                    return (NamedTypeSymbol)type;
                }
            }

            NamedTypeSymbol defaultUnderlyingType = compilation.GetSpecialType(SpecialType.System_Int32);
            Binder.ReportUseSiteDiagnostics(defaultUnderlyingType, diagnostics, this.Locations[0]);
            return defaultUnderlyingType;
        }
    }
}
