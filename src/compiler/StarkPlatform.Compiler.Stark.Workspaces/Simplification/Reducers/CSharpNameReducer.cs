﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Formatting;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Simplification;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Simplification
{
    internal partial class CSharpNameReducer : AbstractCSharpReducer
    {
        private static readonly ObjectPool<IReductionRewriter> s_pool = new ObjectPool<IReductionRewriter>(
            () => new Rewriter(s_pool));

        public CSharpNameReducer() : base(s_pool)
        {
        }

        private static readonly Func<SyntaxNode, SemanticModel, OptionSet, CancellationToken, SyntaxNode> s_simplifyName = SimplifyName;

        private static SyntaxNode SimplifyName(
            SyntaxNode node,
            SemanticModel semanticModel,
            OptionSet optionSet,
            CancellationToken cancellationToken)
        {
            SyntaxNode replacementNode;
            TextSpan issueSpan;

            if (node.Kind() == SyntaxKind.QualifiedCref)
            {
                var crefSyntax = (QualifiedCrefSyntax)node;
                if (!crefSyntax.TryReduceOrSimplifyExplicitName(semanticModel, out var crefReplacement, out issueSpan, optionSet, cancellationToken))
                {
                    return node;
                }

                replacementNode = crefReplacement;
            }
            else
            {
                var expressionSyntax = (ExpressionSyntax)node;
                if (!expressionSyntax.TryReduceOrSimplifyExplicitName(semanticModel, out var expressionReplacement, out issueSpan, optionSet, cancellationToken))
                {
                    return node;
                }

                replacementNode = expressionReplacement;
            }

            node = node.CopyAnnotationsTo(replacementNode).WithAdditionalAnnotations(Formatter.Annotation);
            return node.WithoutAnnotations(Simplifier.Annotation);
        }
    }
}
