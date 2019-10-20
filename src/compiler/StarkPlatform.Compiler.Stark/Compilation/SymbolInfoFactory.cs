// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using StarkPlatform.Compiler.Stark.Symbols;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    internal static class SymbolInfoFactory
    {
        internal static SymbolInfo Create(ImmutableArray<Symbol> symbols, LookupResultKind resultKind)
        {
            if (resultKind == LookupResultKind.Viable)
            {
                if (symbols.Length > 0)
                {
                    Debug.Assert(symbols.Length == 1);
                    return new SymbolInfo(symbols[0]);
                }
                else
                {
                    return SymbolInfo.None;
                }
            }
            else
            {
                return new SymbolInfo(StaticCast<ISymbol>.From(symbols), (symbols.Length > 0) ? resultKind.ToCandidateReason() : CandidateReason.None);
            }
        }
    }
}
