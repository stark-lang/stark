// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.PooledObjects;

namespace StarkPlatform.Compiler.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            Debug.Assert(node.MethodOpt != null);

            //NamedTypeSymbol booleanType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            //BoundExpression fromEnd = MakeLiteral(node.Syntax, ConstantValue.Create(true), booleanType);

            BoundExpression operand = VisitExpression(node.Operand);

            if (NullableNeverHasValue(operand))
            {
                operand = new BoundDefaultExpression(operand.Syntax, operand.Type.GetNullableUnderlyingType());
            }

            operand = NullableAlwaysHasValue(operand) ?? operand;

            if (!node.Type.IsNullableType())
            {
                var notOperand = new BoundUnaryOperator(node.Syntax, UnaryOperatorKind.IntBitwiseComplement, operand, null, null, LookupResultKind.Viable, ImmutableArray<MethodSymbol>.Empty, operand.Type);
                return new BoundObjectCreationExpression(node.Syntax, node.MethodOpt, binderOpt: null, notOperand);
            }

            throw new NotImplementedException("TODO: rewriter of BoundFromEndIndexExpression for ?int is not implemented");

            ArrayBuilder<BoundExpression> sideeffects = ArrayBuilder<BoundExpression>.GetInstance();
            ArrayBuilder<LocalSymbol> locals = ArrayBuilder<LocalSymbol>.GetInstance();

            // operand.HasValue
            operand = CaptureExpressionInTempIfNeeded(operand, sideeffects, locals);
            BoundExpression condition = MakeOptimizedHasValue(operand.Syntax, operand);


            // new Index(operand, fromEnd: true)
            BoundExpression boundOperandGetValueOrDefault = MakeOptimizedGetValueOrDefault(operand.Syntax, operand);
            BoundExpression indexCreation = new BoundObjectCreationExpression(node.Syntax, node.MethodOpt, binderOpt: null, boundOperandGetValueOrDefault);

            if (!TryGetNullableMethod(node.Syntax, node.Type, SpecialMember.System_Nullable_T__ctor, out MethodSymbol nullableCtor))
            {
                return BadExpression(node.Syntax, node.Type, operand);
            }

            // new Nullable(new Index(operand, fromEnd: true))
            BoundExpression consequence = new BoundObjectCreationExpression(node.Syntax, nullableCtor, binderOpt: null, indexCreation);

            // default
            BoundExpression alternative = new BoundDefaultExpression(node.Syntax, constantValueOpt: null, node.Type);

            // operand.HasValue ? new Nullable(new Index(operand, fromEnd: true)) : default
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
    }
}
