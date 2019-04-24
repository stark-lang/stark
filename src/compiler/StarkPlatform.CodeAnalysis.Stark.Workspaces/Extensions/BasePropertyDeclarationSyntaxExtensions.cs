// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class BasePropertyDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Available if <paramref name="node"/> is <see cref="PropertyDeclarationSyntax"/> or <see cref="IndexerDeclarationSyntax"/>.
        /// </summary>
        public static SyntaxToken TryGetSemicolonToken(this BasePropertyDeclarationSyntax node)
        {
            if (node != null)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.PropertyDeclaration: return ((PropertyDeclarationSyntax)node).EosToken;
                    case SyntaxKind.IndexerDeclaration: return ((IndexerDeclarationSyntax)node).EosToken;
                }
            }

            return default;
        }

        /// <summary>
        /// Available if <paramref name="node"/> is <see cref="PropertyDeclarationSyntax"/> or <see cref="IndexerDeclarationSyntax"/>.
        /// </summary>
        public static BasePropertyDeclarationSyntax TryWithSemicolonToken(this BasePropertyDeclarationSyntax node, SyntaxToken semicolonToken)
        {
            if (node != null)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.PropertyDeclaration: return ((PropertyDeclarationSyntax)node).WithEosToken(semicolonToken);
                    case SyntaxKind.IndexerDeclaration: return ((IndexerDeclarationSyntax)node).WithEosToken(semicolonToken);
                }
            }

            return node;
        }

        /// <summary>
        /// Available if <paramref name="node"/> is <see cref="PropertyDeclarationSyntax"/> or <see cref="IndexerDeclarationSyntax"/>.
        /// </summary>
        public static BasePropertyDeclarationSyntax TryWithExpressionBody(this BasePropertyDeclarationSyntax node, ArrowExpressionClauseSyntax expressionBody)
        {
            if (node != null)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.PropertyDeclaration: return ((PropertyDeclarationSyntax)node).WithExpressionBody(expressionBody);
                    case SyntaxKind.IndexerDeclaration: return ((IndexerDeclarationSyntax)node).WithExpressionBody(expressionBody);
                }
            }

            return node;
        }
    }
}
