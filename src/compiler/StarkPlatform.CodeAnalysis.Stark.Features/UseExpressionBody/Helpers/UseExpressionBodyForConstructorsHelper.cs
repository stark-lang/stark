// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis.Stark.CodeStyle;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;

namespace StarkPlatform.CodeAnalysis.Stark.UseExpressionBody
{
    internal class UseExpressionBodyForConstructorsHelper :
        UseExpressionBodyHelper<ConstructorDeclarationSyntax>
    {
        public static readonly UseExpressionBodyForConstructorsHelper Instance = new UseExpressionBodyForConstructorsHelper();

        private UseExpressionBodyForConstructorsHelper()
            : base(IDEDiagnosticIds.UseExpressionBodyForConstructorsDiagnosticId,
                   new LocalizableResourceString(nameof(FeaturesResources.Use_expression_body_for_constructors), FeaturesResources.ResourceManager, typeof(FeaturesResources)),
                   new LocalizableResourceString(nameof(FeaturesResources.Use_block_body_for_constructors), FeaturesResources.ResourceManager, typeof(FeaturesResources)),
                   CSharpCodeStyleOptions.PreferExpressionBodiedConstructors,
                   ImmutableArray.Create(SyntaxKind.ConstructorDeclaration))
        {
        }

        protected override BlockSyntax GetBody(ConstructorDeclarationSyntax declaration)
            => declaration.Body;

        protected override ArrowExpressionClauseSyntax GetExpressionBody(ConstructorDeclarationSyntax declaration)
            => declaration.ExpressionBody;

        protected override SyntaxToken GetEosToken(ConstructorDeclarationSyntax declaration)
            => declaration.EosToken;

        protected override ConstructorDeclarationSyntax WithSemicolonToken(ConstructorDeclarationSyntax declaration, SyntaxToken token)
            => declaration.WithEosToken(token);

        protected override ConstructorDeclarationSyntax WithExpressionBody(ConstructorDeclarationSyntax declaration, ArrowExpressionClauseSyntax expressionBody)
            => declaration.WithExpressionBody(expressionBody);

        protected override ConstructorDeclarationSyntax WithBody(ConstructorDeclarationSyntax declaration, BlockSyntax body)
            => declaration.WithBody(body);

        protected override bool CreateReturnStatementForExpression(SemanticModel semanticModel, ConstructorDeclarationSyntax declaration) => false;
    }
}
