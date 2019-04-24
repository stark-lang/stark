// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Syntax;
using System;
using System.ComponentModel;

namespace StarkPlatform.CodeAnalysis.Stark.Syntax
{
    // backwards compatibility for API extension
    public sealed partial class AccessorDeclarationSyntax : CSharpSyntaxNode
    {
        public AccessorDeclarationSyntax Update(SyntaxList<AttributeSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, BlockSyntax body, SyntaxToken semicolonToken)
            => Update(attributeLists, modifiers, keyword, body, default(ArrowExpressionClauseSyntax), semicolonToken);
    }
}

namespace StarkPlatform.CodeAnalysis.Stark
{
    public partial class SyntaxFactory
    {
        /// <summary>Creates a new AccessorDeclarationSyntax instance.</summary>
        public static AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind, BlockSyntax body)
        {
            return SyntaxFactory.AccessorDeclaration(kind, default(SyntaxList<AttributeSyntax>), default(SyntaxTokenList), SyntaxFactory.Token(GetAccessorDeclarationKeywordKind(kind)), body, default(ArrowExpressionClauseSyntax), default(SyntaxToken));
        }
    }
}

