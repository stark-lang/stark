// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis.Stark.CodeStyle;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;

namespace StarkPlatform.CodeAnalysis.Stark.UseExpressionBody
{
    internal class UseExpressionBodyForOperatorsHelper :
        UseExpressionBodyHelper<OperatorDeclarationSyntax>
    {
        public static readonly UseExpressionBodyForOperatorsHelper Instance = new UseExpressionBodyForOperatorsHelper();

        private UseExpressionBodyForOperatorsHelper()
            : base(IDEDiagnosticIds.UseExpressionBodyForOperatorsDiagnosticId,
                   new LocalizableResourceString(nameof(FeaturesResources.Use_expression_body_for_operators), FeaturesResources.ResourceManager, typeof(FeaturesResources)),
                   new LocalizableResourceString(nameof(FeaturesResources.Use_block_body_for_operators), FeaturesResources.ResourceManager, typeof(FeaturesResources)),
                   CSharpCodeStyleOptions.PreferExpressionBodiedOperators,
                   ImmutableArray.Create(SyntaxKind.OperatorDeclaration))
        {
        }

        protected override BlockSyntax GetBody(OperatorDeclarationSyntax declaration)
            => declaration.Body;

        protected override ArrowExpressionClauseSyntax GetExpressionBody(OperatorDeclarationSyntax declaration)
            => declaration.ExpressionBody;

        protected override SyntaxToken GetEosToken(OperatorDeclarationSyntax declaration)
            => declaration.EosToken;

        protected override OperatorDeclarationSyntax WithSemicolonToken(OperatorDeclarationSyntax declaration, SyntaxToken token)
            => declaration.WithEosToken(token);

        protected override OperatorDeclarationSyntax WithExpressionBody(OperatorDeclarationSyntax declaration, ArrowExpressionClauseSyntax expressionBody)
            => declaration.WithExpressionBody(expressionBody);

        protected override OperatorDeclarationSyntax WithBody(OperatorDeclarationSyntax declaration, BlockSyntax body)
            => declaration.WithBody(body);

        protected override bool CreateReturnStatementForExpression(SemanticModel semanticModel, OperatorDeclarationSyntax declaration)
            => true;
    }
}
