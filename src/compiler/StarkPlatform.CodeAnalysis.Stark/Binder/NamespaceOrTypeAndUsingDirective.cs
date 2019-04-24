// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal struct NamespaceOrTypeAndImportDirective
    {
        public readonly NamespaceOrTypeSymbol NamespaceOrType;
        public readonly ImportDirectiveSyntax ImportDirective;

        public NamespaceOrTypeAndImportDirective(NamespaceOrTypeSymbol namespaceOrType, ImportDirectiveSyntax usingDirective)
        {
            this.NamespaceOrType = namespaceOrType;
            this.ImportDirective = usingDirective;
        }
    }
}
