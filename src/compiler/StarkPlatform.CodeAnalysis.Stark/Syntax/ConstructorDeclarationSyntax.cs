// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Syntax
{
    public partial class ConstructorDeclarationSyntax
    {
        public ConstructorDeclarationSyntax Update(
            SyntaxList<AttributeSyntax> attributeLists,
            SyntaxTokenList modifiers,
            SyntaxToken constructorKeyword,
            ParameterListSyntax parameterList,
            ConstructorInitializerSyntax initializer,
            SyntaxList<ContractClauseSyntax> contractClauses,
            BlockSyntax body,
            SyntaxToken eosToken)
            => Update(
                attributeLists,
                modifiers,
                constructorKeyword,
                parameterList,
                initializer,
                contractClauses,
                body,
                default(ArrowExpressionClauseSyntax),
                eosToken);
    }
}

namespace StarkPlatform.CodeAnalysis.Stark
{
    public partial class SyntaxFactory
    {
        public static ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxList<AttributeSyntax> attributeLists, SyntaxTokenList modifiers, ParameterListSyntax parameterList, ConstructorInitializerSyntax initializer, BlockSyntax body, ArrowExpressionClauseSyntax expressionBody, SyntaxToken eosToken)
        {
            return SyntaxFactory.ConstructorDeclaration(attributeLists, modifiers, SyntaxFactory.Token(SyntaxKind.ConstructorKeyword), parameterList, initializer, default, body, expressionBody, eosToken);
        }

        public static ConstructorDeclarationSyntax ConstructorDeclaration(
            SyntaxList<AttributeSyntax> attributeLists,
            SyntaxTokenList modifiers,
            SyntaxToken identifier,
            ParameterListSyntax parameterList,
            ConstructorInitializerSyntax initializer,
            SyntaxList<ContractClauseSyntax> contractClauses,
            BlockSyntax body)
            => ConstructorDeclaration(
                attributeLists,
                modifiers,
                identifier,
                parameterList,
                initializer,
                contractClauses,
                body,
                default(ArrowExpressionClauseSyntax),
                default(SyntaxToken));

        public static ConstructorDeclarationSyntax ConstructorDeclaration(
            SyntaxList<AttributeSyntax> attributeLists,
            SyntaxTokenList modifiers,
            SyntaxToken identifier,
            ParameterListSyntax parameterList,
            ConstructorInitializerSyntax initializer,
            SyntaxList<ContractClauseSyntax> contractClauses,
            BlockSyntax body,
            SyntaxToken semicolonToken)
            => ConstructorDeclaration(
                attributeLists,
                modifiers,
                identifier,
                parameterList,
                initializer,
                contractClauses,
                body,
                default(ArrowExpressionClauseSyntax),
                semicolonToken);

        public static ConstructorDeclarationSyntax ConstructorDeclaration(
            SyntaxList<AttributeSyntax> attributeLists,
            SyntaxTokenList modifiers,
            SyntaxToken identifier,
            ParameterListSyntax parameterList,
            ConstructorInitializerSyntax initializer,
            SyntaxList<ContractClauseSyntax> contractClauses,
            ArrowExpressionClauseSyntax expressionBody)
            => ConstructorDeclaration(
                attributeLists,
                modifiers,
                identifier,
                parameterList,
                initializer,
                contractClauses,
                default(BlockSyntax),
                expressionBody,
                default(SyntaxToken));

        public static ConstructorDeclarationSyntax ConstructorDeclaration(
            SyntaxList<AttributeSyntax> attributeLists,
            SyntaxTokenList modifiers,
            SyntaxToken identifier,
            ParameterListSyntax parameterList,
            ConstructorInitializerSyntax initializer,
            SyntaxList<ContractClauseSyntax> contractClauses,
            ArrowExpressionClauseSyntax expressionBody,
            SyntaxToken semicolonToken)
            => ConstructorDeclaration(
                attributeLists,
                modifiers,
                identifier,
                parameterList,
                initializer,
                contractClauses,
                default(BlockSyntax),
                expressionBody,
                semicolonToken);

    }
}
