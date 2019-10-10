// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using System.Collections.Immutable;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.PooledObjects;
using System.Linq;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitRangeExpression(BoundRangeExpression node)
        {
            Debug.Assert(node != null && node.MethodOpt != null);

            bool needLifting = false;
            var operandsBuilder = new ArrayBuilder<BoundExpression>();

            var left = node.LeftOperand;
            if (left != null)
            {
                operandsBuilder.Add(tryOptimizeOperand(left));
            }

            var right = node.RightOperand;
            if (right != null)
            {
                operandsBuilder.Add(tryOptimizeOperand(right));
            }

            ImmutableArray<BoundExpression> operands = operandsBuilder.ToImmutable();

            if (needLifting)
            {
                return LiftRangeExpression(node, operands);
            }
            else
            {
                BoundExpression rangeCreation = MakeRangeExpression(node.MethodOpt, left, right);

                if (node.Type.IsNullableType())
                {
                    if (!TryGetNullableMethod(node.Syntax, node.Type, SpecialMember.System_Nullable_T__ctor, out MethodSymbol nullableCtor))
                    {
                        return BadExpression(node.Syntax, node.Type, node);
                    }

                    return new BoundObjectCreationExpression(node.Syntax, nullableCtor, binderOpt: null, rangeCreation);
                }

                return rangeCreation;
            }

            BoundExpression tryOptimizeOperand(BoundExpression operand)
            {
                Debug.Assert(operand != null);
                operand = VisitExpression(operand);

                if (NullableNeverHasValue(operand))
                {
                    operand = new BoundDefaultExpression(operand.Syntax, operand.Type.GetNullableUnderlyingType());
                }
                else
                {
                    operand = NullableAlwaysHasValue(operand) ?? operand;

                    if (operand.Type.IsNullableType())
                    {
                        needLifting = true;
                    }
                }

                return operand;
            }
        }

        private BoundExpression LiftRangeExpression(BoundRangeExpression node, ImmutableArray<BoundExpression> operands)
        {
            Debug.Assert(node.Type.IsNullableType());
            Debug.Assert(operands.Any(operand => operand.Type.IsNullableType()));
            Debug.Assert(operands.Length == 1 || operands.Length == 2);

            ArrayBuilder<BoundExpression> sideeffects = ArrayBuilder<BoundExpression>.GetInstance();
            ArrayBuilder<LocalSymbol> locals = ArrayBuilder<LocalSymbol>.GetInstance();
            ArrayBuilder<BoundExpression> arguments = ArrayBuilder<BoundExpression>.GetInstance();

            // left.HasValue && right.HasValue
            BoundExpression condition = null;
            foreach (var operand in operands)
            {
                BoundExpression tempOperand = CaptureExpressionInTempIfNeeded(operand, sideeffects, locals);

                if (tempOperand.Type.IsNullableType())
                {
                    BoundExpression operandHasValue = MakeOptimizedHasValue(tempOperand.Syntax, tempOperand);

                    if (condition is null)
                    {
                        condition = operandHasValue;
                    }
                    else
                    {
                        TypeSymbol boolType = _compilation.GetSpecialType(SpecialType.System_Boolean);
                        condition = MakeBinaryOperator(node.Syntax, BinaryOperatorKind.BoolAnd, condition, operandHasValue, boolType, method: null);
                    }

                    arguments.Add(MakeOptimizedGetValueOrDefault(tempOperand.Syntax, tempOperand));
                }
                else
                {
                    arguments.Add(tempOperand);
                }
            }

            Debug.Assert(condition != null);

            // method(left.GetValueOrDefault(), right.GetValueOrDefault())
            BoundExpression rangeCall = MakeCall(
                node.Syntax,
                rewrittenReceiver: null,
                node.MethodOpt,
                arguments.ToImmutableArray(),
                node.MethodOpt.ReturnType.TypeSymbol);

            if (!TryGetNullableMethod(node.Syntax, node.Type, SpecialMember.System_Nullable_T__ctor, out MethodSymbol nullableCtor))
            {
                return BadExpression(node.Syntax, node.Type, node);
            }

            // new Nullable(method(left.GetValueOrDefault(), right.GetValueOrDefault()))
            BoundExpression consequence = new BoundObjectCreationExpression(node.Syntax, nullableCtor, binderOpt: null, rangeCall);

            // default
            BoundExpression alternative = new BoundDefaultExpression(node.Syntax, constantValueOpt: null, node.Type);

            // left.HasValue && right.HasValue ? new Nullable(method(left.GetValueOrDefault(), right.GetValueOrDefault())) : default
            BoundExpression conditionalExpression = RewriteConditionalOperator(
                syntax: node.Syntax,
                rewrittenCondition: condition,
                rewrittenConsequence: consequence,
                rewrittenAlternative: alternative,
                constantValueOpt: null,
                rewrittenType: node.Type,
                isRef: false);

            return new BoundSequence(
                syntax: node.Syntax,
                locals: locals.ToImmutableAndFree(),
                sideEffects: sideeffects.ToImmutableAndFree(),
                value: conditionalExpression,
                type: node.Type);
        }

        private BoundExpression MakeRangeExpression(
    MethodSymbol constructionMethod,
    BoundExpression left,
    BoundExpression right)
        {
            var F = _factory;

            ;

            // The construction method may vary based on what well-known
            // members were available during binding. Depending on which member
            // is chosen we need to change our adjust our calling node.
            switch (constructionMethod.MethodKind)
            {
                case MethodKind.Constructor:
                    // Represents Range..ctor(Index left, Index right)
                    // The constructor can always be used to construct a range,
                    // but if any of the arguments are missing then we need to
                    // construct replacement Indexes
                    left = left ?? F.New(WellKnownMember.core_Index__ctor, ImmutableArray.Create<BoundExpression>(F.Literal(0)));
                    right = right ?? F.New(WellKnownMember.core_Index__ctor, ImmutableArray.Create<BoundExpression>(F.Literal(-1)));

                    return F.New(constructionMethod, ImmutableArray.Create(left, right));

                //case MethodKind.Ordinary:
                //    // Represents either Range.StartAt or Range.EndAt, which
                //    // means that the `..` expression is missing an argument on
                //    // either the left or the right (i.e., `x..` or `..x`)
                //    Debug.Assert(left is null ^ right is null);
                //    Debug.Assert(constructionMethod.MetadataName == "StartAt" ||
                //                 constructionMethod.MetadataName == "EndAt");
                //    Debug.Assert(constructionMethod.IsStatic);
                //    var arg = left ?? right;
                //    return F.StaticCall(constructionMethod, ImmutableArray.Create(arg));

                //case MethodKind.PropertyGet:
                //    // The only property is Range.All, so the expression must
                //    // be `..` with both arguments missing
                //    Debug.Assert(constructionMethod.MetadataName == "get_All");
                //    Debug.Assert(constructionMethod.IsStatic);
                //    Debug.Assert(left is null && right is null);
                //    return F.StaticCall(constructionMethod, ImmutableArray<BoundExpression>.Empty);

                default:
                    throw ExceptionUtilities.UnexpectedValue(constructionMethod.MethodKind);
            }
        }
    }
}
