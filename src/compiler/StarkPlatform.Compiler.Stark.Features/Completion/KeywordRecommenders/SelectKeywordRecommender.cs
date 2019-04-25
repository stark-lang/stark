// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Stark.Extensions.ContextQuery;

namespace StarkPlatform.Compiler.Stark.Completion.KeywordRecommenders
{
    internal class SelectKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public SelectKeywordRecommender()
            : base(SyntaxKind.SelectKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            var token = context.TargetToken;

            // for orderby, ascending is the default so select should be available in the orderby direction context
            if (token.IsOrderByDirectionContext())
            {
                return true;
            }

            // var q = from x in y
            //         |
            if (!token.IntersectsWith(position) &&
                token.IsLastTokenOfQueryClause())
            {
                return true;
            }

            return false;
        }
    }
}
