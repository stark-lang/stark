// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Shared.Extensions;
using StarkPlatform.Compiler.Simplification;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark.Simplification
{
    internal partial class CSharpCastReducer : AbstractCSharpReducer
    {
        private static readonly ObjectPool<IReductionRewriter> s_pool = new ObjectPool<IReductionRewriter>(
            () => new Rewriter(s_pool));

        public CSharpCastReducer() : base(s_pool)
        {
        }

        private static readonly Func<CastExpressionSyntax, SemanticModel, OptionSet, CancellationToken, ExpressionSyntax> s_simplifyCast = SimplifyCast;

        private static ExpressionSyntax SimplifyCast(CastExpressionSyntax node, SemanticModel semanticModel, OptionSet optionSet, CancellationToken cancellationToken)
        {
            if (!node.IsUnnecessaryCast(semanticModel, cancellationToken))
            {
                return node;
            }

            return node.Uncast();
        }
    }
}
