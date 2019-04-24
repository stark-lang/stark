// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.CodeStyle;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Simplification
{
    internal partial class CSharpDefaultExpressionReducer
    {
        private class Rewriter : AbstractReductionRewriter
        {
            public Rewriter(ObjectPool<IReductionRewriter> pool)
                : base(pool)
            {
                _simplifyDefaultExpression = SimplifyDefaultExpression;
            }

            private readonly Func<DefaultExpressionSyntax, SemanticModel, OptionSet, CancellationToken, SyntaxNode> _simplifyDefaultExpression;

            private SyntaxNode SimplifyDefaultExpression(
                DefaultExpressionSyntax node,
                SemanticModel semanticModel,
                OptionSet optionSet,
                CancellationToken cancellationToken)
            {
                if (node.CanReplaceWithDefaultLiteral(ParseOptions, optionSet, semanticModel, cancellationToken))
                {
                    return SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                                        .WithTriviaFrom(node);
                }

                return node;
            }

            public override SyntaxNode VisitDefaultExpression(DefaultExpressionSyntax node)
            {
                return SimplifyNode(
                    node,
                    newNode: base.VisitDefaultExpression(node),
                    parentNode: node.Parent,
                    simplifier: _simplifyDefaultExpression);
            }
        }
    }
}
