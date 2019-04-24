// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis.Stark.CodeStyle;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;

namespace StarkPlatform.CodeAnalysis.Stark.UseExpressionBody
{
    internal class UseExpressionBodyForAccessorsHelper :
        UseExpressionBodyHelper<AccessorDeclarationSyntax>
    {
        public static readonly UseExpressionBodyForAccessorsHelper Instance = new UseExpressionBodyForAccessorsHelper();

        private UseExpressionBodyForAccessorsHelper()
            : base(IDEDiagnosticIds.UseExpressionBodyForAccessorsDiagnosticId,
                   new LocalizableResourceString(nameof(FeaturesResources.Use_expression_body_for_accessors), FeaturesResources.ResourceManager, typeof(FeaturesResources)),
                   new LocalizableResourceString(nameof(FeaturesResources.Use_block_body_for_accessors), FeaturesResources.ResourceManager, typeof(FeaturesResources)),
                   CSharpCodeStyleOptions.PreferExpressionBodiedAccessors,
                   ImmutableArray.Create(SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration))
        {
        }

        protected override BlockSyntax GetBody(AccessorDeclarationSyntax declaration)
            => declaration.Body;

        protected override ArrowExpressionClauseSyntax GetExpressionBody(AccessorDeclarationSyntax declaration)
            => declaration.ExpressionBody;

        protected override SyntaxToken GetEosToken(AccessorDeclarationSyntax declaration)
            => declaration.EosToken;

        protected override AccessorDeclarationSyntax WithSemicolonToken(AccessorDeclarationSyntax declaration, SyntaxToken token)
            => declaration.WithEosToken(token);

        protected override AccessorDeclarationSyntax WithExpressionBody(AccessorDeclarationSyntax declaration, ArrowExpressionClauseSyntax expressionBody)
            => declaration.WithExpressionBody(expressionBody);

        protected override AccessorDeclarationSyntax WithBody(AccessorDeclarationSyntax declaration, BlockSyntax body)
            => declaration.WithBody(body);

        protected override bool CreateReturnStatementForExpression(SemanticModel semanticModel, AccessorDeclarationSyntax declaration)
            => declaration.IsKind(SyntaxKind.GetAccessorDeclaration);
    }
}
