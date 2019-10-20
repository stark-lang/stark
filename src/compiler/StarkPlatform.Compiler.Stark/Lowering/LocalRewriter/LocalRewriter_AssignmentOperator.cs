﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            // Assume value of expression is used.
            return VisitAssignmentOperator(node, used: true);
        }

        private BoundExpression VisitAssignmentOperator(BoundAssignmentOperator node, bool used)
        {
            var loweredRight = VisitExpression(node.Right);

            BoundExpression left = node.Left;
            BoundExpression loweredLeft;
            switch (left.Kind)
            {
                case BoundKind.PropertyAccess:
                    loweredLeft = VisitPropertyAccess((BoundPropertyAccess)left, isLeftOfAssignment: true);
                    break;

                case BoundKind.IndexerAccess:
                    loweredLeft = VisitIndexerAccess((BoundIndexerAccess)left, isLeftOfAssignment: true);
                    break;

                default:
                    loweredLeft = VisitExpression(left);
                    break;
            }

            return MakeStaticAssignmentOperator(node.Syntax, loweredLeft, loweredRight, node.IsRef, node.Type, used);
        }

        /// <summary>
        /// Generates a lowered form of the assignment operator for the given left and right sub-expressions.
        /// Left and right sub-expressions must be in lowered form.
        /// </summary>
        private BoundExpression MakeAssignmentOperator(SyntaxNode syntax, BoundExpression rewrittenLeft, BoundExpression rewrittenRight, TypeSymbol type,
            bool used, bool isChecked, bool isCompoundAssignment)
        {
            switch (rewrittenLeft.Kind)
            {
                case BoundKind.EventAccess:
                    var eventAccess = (BoundEventAccess)rewrittenLeft;
                    Debug.Assert(eventAccess.IsUsableAsField);

                    // Only Windows Runtime field-like events can come through here:
                    // - Assignment operation is not supported for custom (non-field like) events.
                    // - Access to regular field-like events is expected to be lowered to at least a field access
                    //   when we reach here.
                    throw ExceptionUtilities.Unreachable;

                default:
                    return MakeStaticAssignmentOperator(syntax, rewrittenLeft, rewrittenRight, isRef: false, type: type, used: used);
            }
        }

        /// <summary>
        /// Generates a lowered form of the assignment operator for the given left and right sub-expressions.
        /// Left and right sub-expressions must be in lowered form.
        /// </summary>
        private BoundExpression MakeStaticAssignmentOperator(
            SyntaxNode syntax,
            BoundExpression rewrittenLeft,
            BoundExpression rewrittenRight,
            bool isRef,
            TypeSymbol type,
            bool used)
        {
            switch (rewrittenLeft.Kind)
            {
                case BoundKind.PropertyAccess:
                    {
                        Debug.Assert(!isRef);
                        BoundPropertyAccess propertyAccess = (BoundPropertyAccess)rewrittenLeft;
                        BoundExpression rewrittenReceiver = propertyAccess.ReceiverOpt;
                        PropertySymbol property = propertyAccess.PropertySymbol;
                        Debug.Assert(!property.IsIndexer);
                        return MakePropertyAssignment(
                            syntax,
                            rewrittenReceiver,
                            property,
                            ImmutableArray<BoundExpression>.Empty,
                            default(ImmutableArray<RefKind>),
                            false,
                            default(ImmutableArray<int>),
                            rewrittenRight,
                            type,
                            used);
                    }

                case BoundKind.IndexerAccess:
                    {
                        Debug.Assert(!isRef);
                        BoundIndexerAccess indexerAccess = (BoundIndexerAccess)rewrittenLeft;
                        BoundExpression rewrittenReceiver = indexerAccess.ReceiverOpt;
                        ImmutableArray<BoundExpression> rewrittenArguments = indexerAccess.Arguments;
                        PropertySymbol indexer = indexerAccess.Indexer;
                        Debug.Assert(indexer.IsIndexer || indexer.IsIndexedProperty);
                        return MakePropertyAssignment(
                            syntax,
                            rewrittenReceiver,
                            indexer,
                            rewrittenArguments,
                            indexerAccess.ArgumentRefKindsOpt,
                            indexerAccess.Expanded,
                            indexerAccess.ArgsToParamsOpt,
                            rewrittenRight,
                            type,
                            used);
                    }

                case BoundKind.Local:
                    {
                        Debug.Assert(!isRef || ((BoundLocal)rewrittenLeft).LocalSymbol.RefKind != RefKind.None);
                        return new BoundAssignmentOperator(
                            syntax,
                            rewrittenLeft,
                            rewrittenRight,
                            type,
                            isRef: isRef);
                    }

                case BoundKind.Parameter:
                    {
                        Debug.Assert(!isRef || rewrittenLeft.GetRefKind() != RefKind.None);
                        return new BoundAssignmentOperator(
                            syntax,
                            rewrittenLeft,
                            rewrittenRight,
                            isRef,
                            type);
                    }

                case BoundKind.DiscardExpression:
                    {
                        return rewrittenRight;
                    }

                default:
                    {
                        Debug.Assert(!isRef);
                        return new BoundAssignmentOperator(
                            syntax,
                            rewrittenLeft,
                            rewrittenRight,
                            type);
                    }
            }
        }

        private BoundExpression MakePropertyAssignment(
            SyntaxNode syntax,
            BoundExpression rewrittenReceiver,
            PropertySymbol property,
            ImmutableArray<BoundExpression> rewrittenArguments,
            ImmutableArray<RefKind> argumentRefKindsOpt,
            bool expanded,
            ImmutableArray<int> argsToParamsOpt,
            BoundExpression rewrittenRight,
            TypeSymbol type,
            bool used)
        {
            // Rewrite property assignment into call to setter.
            var setMethod = property.GetOwnOrInheritedSetMethod();

            if ((object)setMethod == null)
            {
                Debug.Assert((property as SourcePropertySymbol)?.IsAutoProperty == true,
                    "only autoproperties can be assignable without having setters");

                var backingField = (property as SourcePropertySymbol).BackingField;
                return _factory.AssignmentExpression(
                    _factory.Field(rewrittenReceiver, backingField),
                    rewrittenRight);
            }

            // We have already lowered each argument, but we may need some additional rewriting for the arguments,
            // such as generating a params array, re-ordering arguments based on argsToParamsOpt map, inserting arguments for optional parameters, etc.
            ImmutableArray<LocalSymbol> argTemps;
            rewrittenArguments = MakeArguments(
                syntax,
                rewrittenArguments,
                property,
                setMethod,
                expanded,
                argsToParamsOpt,
                ref argumentRefKindsOpt,
                out argTemps,
                invokedAsExtensionMethod: false,
                enableCallerInfo: ThreeState.True);

            if (used)
            {
                // Save expression value to a temporary before calling the
                // setter, and restore the temporary after the setter, so the
                // assignment can be used as an embedded expression.
                TypeSymbol exprType = rewrittenRight.Type;

                LocalSymbol rhsTemp = _factory.SynthesizedLocal(exprType);

                BoundExpression boundRhs = new BoundLocal(syntax, rhsTemp, null, exprType);

                BoundExpression rhsAssignment = new BoundAssignmentOperator(
                    syntax,
                    boundRhs,
                    rewrittenRight,
                    exprType);

                BoundExpression setterCall = BoundCall.Synthesized(
                    syntax,
                    rewrittenReceiver,
                    setMethod,
                    AppendToPossibleNull(rewrittenArguments, rhsAssignment));

                return new BoundSequence(
                    syntax,
                    AppendToPossibleNull(argTemps, rhsTemp),
                    ImmutableArray.Create(setterCall),
                    boundRhs,
                    type);
            }
            else
            {
                BoundCall setterCall = BoundCall.Synthesized(
                    syntax,
                    rewrittenReceiver,
                    setMethod,
                    AppendToPossibleNull(rewrittenArguments, rewrittenRight));

                if (argTemps.IsDefaultOrEmpty)
                {
                    return setterCall;
                }
                else
                {
                    return new BoundSequence(
                        syntax,
                        argTemps,
                        ImmutableArray<BoundExpression>.Empty,
                        setterCall,
                        setMethod.ReturnType.TypeSymbol);
                }
            }
        }

        private static ImmutableArray<T> AppendToPossibleNull<T>(ImmutableArray<T> possibleNull, T newElement)
        {
            Debug.Assert(newElement != null);
            return possibleNull.NullToEmpty().Add(newElement);
        }
    }
}
