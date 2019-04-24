// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery;

namespace StarkPlatform.CodeAnalysis.Stark.Completion.KeywordRecommenders
{
    internal class IsKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public IsKeywordRecommender()
            : base(SyntaxKind.IsKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            // cases:
            //    expr |
            return !context.IsInNonUserCode && context.IsIsOrAsOrSwitchExpressionContext;
        }
    }
}
