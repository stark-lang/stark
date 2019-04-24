// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Stark
{
    [ExportLanguageService(typeof(ISymbolDeclarationService), LanguageNames.Stark), Shared]
    internal class CSharpSymbolDeclarationService : ISymbolDeclarationService
    {
        public ImmutableArray<SyntaxReference> GetDeclarations(ISymbol symbol)
            => symbol != null
                ? symbol.DeclaringSyntaxReferences
                : ImmutableArray<SyntaxReference>.Empty;
    }
}
