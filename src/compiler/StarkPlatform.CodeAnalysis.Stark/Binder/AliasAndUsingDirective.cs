// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal struct AliasAndImportDirective
    {
        public readonly AliasSymbol Alias;
        public readonly ImportDirectiveSyntax ImportDirective;

        public AliasAndImportDirective(AliasSymbol alias, ImportDirectiveSyntax usingDirective)
        {
            this.Alias = alias;
            this.ImportDirective = usingDirective;
        }
    }
}
