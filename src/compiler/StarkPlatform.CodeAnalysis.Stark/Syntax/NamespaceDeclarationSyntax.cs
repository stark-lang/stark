// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Syntax
{
    public partial class NamespaceDeclarationSyntax
    {
        new internal Syntax.InternalSyntax.NamespaceDeclarationSyntax Green
        {
            get
            {
                return (Syntax.InternalSyntax.NamespaceDeclarationSyntax)base.Green;
            }
        }
    }
}
