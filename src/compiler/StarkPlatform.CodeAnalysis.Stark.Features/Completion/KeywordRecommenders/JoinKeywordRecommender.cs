// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery;

namespace StarkPlatform.CodeAnalysis.Stark.Completion.KeywordRecommenders
{
    internal class JoinKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public JoinKeywordRecommender()
            : base(SyntaxKind.JoinKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return context.SyntaxTree.IsValidContextForJoinClause(position, context.LeftToken, cancellationToken);
        }
    }
}
