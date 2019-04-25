// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.PooledObjects;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark.Simplification
{
    internal partial class CSharpExtensionMethodReducer
    {
        private class Rewriter : AbstractReductionRewriter
        {
            public Rewriter(ObjectPool<IReductionRewriter> pool)
                : base(pool)
            {
            }

            public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                return SimplifyExpression(
                    node,
                    newNode: base.VisitInvocationExpression(node),
                    simplifier: s_simplifyExtensionMethod);
            }
        }
    }
}
