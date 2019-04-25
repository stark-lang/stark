// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.LanguageServices;

namespace StarkPlatform.Compiler.Stark
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
