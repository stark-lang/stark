﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Formatting;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Simplification;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark.Simplification
{
    internal partial class CSharpNameReducer
    {
        private class Rewriter : AbstractReductionRewriter
        {
            public Rewriter(ObjectPool<IReductionRewriter> pool)
                : base(pool)
            {
            }

            public override SyntaxNode VisitPredefinedType(PredefinedTypeSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitPredefinedType(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitAliasQualifiedName(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitQualifiedName(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitMemberAccessExpression(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitIdentifierName(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitGenericName(GenericNameSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitGenericName(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitQualifiedCref(QualifiedCrefSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitQualifiedCref(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitArrayType(ArrayTypeSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitArrayType(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitNullableType(NullableTypeSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitNullableType(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitTupleType(TupleTypeSyntax node)
            {
                bool oldAlwaysSimplify = this.alwaysSimplify;
                if (!this.alwaysSimplify)
                {
                    this.alwaysSimplify = node.HasAnnotation(Simplifier.Annotation);
                }

                var result = SimplifyExpression(
                    node,
                    newNode: base.VisitTupleType(node),
                    simplifier: s_simplifyName);

                this.alwaysSimplify = oldAlwaysSimplify;

                return result;
            }

            public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
            {
                bool isOrAsNode = node.Kind() == SyntaxKind.AsOptExpression || node.Kind() == SyntaxKind.IsExpression;

                var result = (ExpressionSyntax)base.VisitBinaryExpression(node);

                if (result != node && isOrAsNode)
                {
                    // In order to handle cases in which simplifying a name would result in code
                    // that parses different, we pre-emptively add parentheses that will be
                    // simplified away later.
                    //
                    // For example, this code...
                    //
                    //     var x = y as Nullable<int> + 1;
                    //
                    // ...should simplify as...
                    //
                    //     var x = (y as int?) + 1;
                    return result.Parenthesize().WithAdditionalAnnotations(Formatter.Annotation);
                }

                return result;
            }
        }
    }
}
