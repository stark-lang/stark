// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.Completion.KeywordRecommenders
{
    internal class AwaitKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public AwaitKeywordRecommender()
            : base(SyntaxKind.AwaitKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            if (context.IsGlobalStatementContext)
            {
                return true;
            }

            if (context.IsAnyExpressionContext || context.IsStatementContext)
            {
                foreach (var node in context.LeftToken.GetAncestors<SyntaxNode>())
                {
                    if (node.IsAnyLambdaOrAnonymousMethod())
                    {
                        return true;
                    }

                    if (node.IsKind(SyntaxKind.QueryExpression))
                    {
                        return false;
                    }

                    if (node.IsKind(SyntaxKind.LockStatement))
                    {
                        var lockStatement = (LockStatementSyntax)node;
                        if (lockStatement.Statement != null &&
                            !lockStatement.Statement.IsMissing &&
                            lockStatement.Statement.Span.Contains(position))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }
    }
}
