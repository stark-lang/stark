// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Symbols;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal sealed class LazyArrayElementCantBeRefAnyDiagnosticInfo : LazyDiagnosticInfo
    {
        private readonly TypeSymbolWithAnnotations _possiblyRestrictedTypeSymbol;

        internal LazyArrayElementCantBeRefAnyDiagnosticInfo(TypeSymbolWithAnnotations possiblyRestrictedTypeSymbol)
        {
            _possiblyRestrictedTypeSymbol = possiblyRestrictedTypeSymbol;
        }

        protected override DiagnosticInfo ResolveInfo()
        {
            if (_possiblyRestrictedTypeSymbol.IsRestrictedType())
            {
                return new CSDiagnosticInfo(ErrorCode.ERR_ArrayElementCantBeRefAny, _possiblyRestrictedTypeSymbol.TypeSymbol);
            }

            return null;
        }
    }
}
