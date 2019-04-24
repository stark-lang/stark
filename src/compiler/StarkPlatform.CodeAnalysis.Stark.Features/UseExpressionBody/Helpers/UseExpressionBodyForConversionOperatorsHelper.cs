// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis.Stark.CodeStyle;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;

namespace StarkPlatform.CodeAnalysis.Stark.UseExpressionBody
{
    internal class UseExpressionBodyForConversionOperatorsHelper :
        UseExpressionBodyHelper<ConversionOperatorDeclarationSyntax>
    {
        public static readonly UseExpressionBodyForConversionOperatorsHelper Instance = new UseExpressionBodyForConversionOperatorsHelper();

        private UseExpressionBodyForConversionOperatorsHelper()
            : base(IDEDiagnosticIds.UseExpressionBodyForConversionOperatorsDiagnosticId,
                   new LocalizableResourceString(nameof(FeaturesResources.Use_expression_body_for_operators), FeaturesResources.ResourceManager, typeof(FeaturesResources)),
                   new LocalizableResourceString(nameof(FeaturesResources.Use_block_body_for_operators), FeaturesResources.ResourceManager, typeof(FeaturesResources)),
                   CSharpCodeStyleOptions.PreferExpressionBodiedOperators,
                   ImmutableArray.Create(SyntaxKind.ConversionOperatorDeclaration))
        {
        }

        protected override BlockSyntax GetBody(ConversionOperatorDeclarationSyntax declaration)
            => declaration.Body;

        protected override ArrowExpressionClauseSyntax GetExpressionBody(ConversionOperatorDeclarationSyntax declaration)
            => declaration.ExpressionBody;

        protected override SyntaxToken GetEosToken(ConversionOperatorDeclarationSyntax declaration)
            => declaration.EosToken;

        protected override ConversionOperatorDeclarationSyntax WithSemicolonToken(ConversionOperatorDeclarationSyntax declaration, SyntaxToken token)
            => declaration.WithEosToken(token);

        protected override ConversionOperatorDeclarationSyntax WithExpressionBody(ConversionOperatorDeclarationSyntax declaration, ArrowExpressionClauseSyntax expressionBody)
            => declaration.WithExpressionBody(expressionBody);

        protected override ConversionOperatorDeclarationSyntax WithBody(ConversionOperatorDeclarationSyntax declaration, BlockSyntax body)
            => declaration.WithBody(body);

        protected override bool CreateReturnStatementForExpression(SemanticModel semanticModel, ConversionOperatorDeclarationSyntax declaration)
            => true;
    }
}
