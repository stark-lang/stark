// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Completion.Providers;
using StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Completion.KeywordRecommenders
{
    internal class VarKeywordRecommender : IKeywordRecommender<CSharpSyntaxContext>
    {
        public VarKeywordRecommender()
        {
        }

        private bool IsValidContext(CSharpSyntaxContext context)
        {
            if (context.IsStatementContext ||
                context.IsGlobalStatementContext ||
                context.IsPossibleTupleContext ||
                context.IsPatternContext)
            {
                return true;
            }

            return context.IsLocalVariableDeclarationContext;
        }

        public Task<IEnumerable<RecommendedKeyword>> RecommendKeywordsAsync(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            if (IsValidContext(context))
            {
                return Task.FromResult(SpecializedCollections.SingletonEnumerable(new RecommendedKeyword("var")));
            }

            return Task.FromResult<IEnumerable<RecommendedKeyword>>(null);
        }
    }
}
