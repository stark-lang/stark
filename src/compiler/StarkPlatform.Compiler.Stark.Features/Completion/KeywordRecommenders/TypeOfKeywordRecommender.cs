// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions.ContextQuery;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Shared.Extensions;

namespace StarkPlatform.Compiler.Stark.Completion.KeywordRecommenders
{
    internal class TypeOfKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public TypeOfKeywordRecommender()
            : base(SyntaxKind.TypeOfKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return
                (context.IsAnyExpressionContext && !context.IsConstantExpressionContext) ||
                context.IsStatementContext ||
                context.IsGlobalStatementContext ||
                IsAttributeArgumentContext(context);
        }

        private bool IsAttributeArgumentContext(CSharpSyntaxContext context)
        {
            return
                context.IsAnyExpressionContext &&
                context.LeftToken.GetAncestor<AttributeSyntax>() != null;
        }
    }
}
