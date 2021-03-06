﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Text;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    internal sealed partial class LocalRewriter
    {
        /// <summary>
        /// This rewriter lowers pre-/post- increment/decrement operations (initially represented as
        /// unary operators). We use BoundSequenceExpressions because we need to capture the RHS of the
        /// assignment in a temp variable.
        /// </summary>
        /// <remarks>
        /// This rewriter assumes that it will be run before decimal rewriting (so that it does not have
        /// to lower decimal constants and operations) and call rewriting (so that it does not have to
        /// lower property accesses).
        /// </remarks>
        public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
        {
            switch (node.OperatorKind.Operator())
            {
                case UnaryOperatorKind.PrefixDecrement:
                case UnaryOperatorKind.PrefixIncrement:
                case UnaryOperatorKind.PostfixDecrement:
                case UnaryOperatorKind.PostfixIncrement:
                    Debug.Assert(false); // these should have been represented as a BoundIncrementOperator
                    return base.VisitUnaryOperator(node);
            }

            BoundExpression loweredOperand = VisitExpression(node.Operand);
            return MakeUnaryOperator(node, node.OperatorKind, node.Syntax, node.MethodOpt, loweredOperand, node.Type);
        }

        private BoundExpression MakeUnaryOperator(
            UnaryOperatorKind kind,
            SyntaxNode syntax,
            MethodSymbol method,
            BoundExpression loweredOperand,
            TypeSymbol type)
        {
            return MakeUnaryOperator(null, kind, syntax, method, loweredOperand, type);
        }

        private BoundExpression MakeUnaryOperator(
            BoundUnaryOperator oldNode,
            UnaryOperatorKind kind,
            SyntaxNode syntax,
            MethodSymbol method,
            BoundExpression loweredOperand,
            TypeSymbol type)
        {
            if (kind.IsLifted())
            {
                return LowerLiftedUnaryOperator(kind, syntax, method, loweredOperand, type);
            }
            else if (kind.IsUserDefined())
            {
                Debug.Assert((object)method != null);
                Debug.Assert(TypeSymbol.Equals(type, method.ReturnType.TypeSymbol, TypeCompareKind.ConsiderEverything2));
                return BoundCall.Synthesized(syntax, null, method, loweredOperand);
            }
            else if (kind.Operator() == UnaryOperatorKind.UnaryPlus)
            {
                // We do not call the operator even for decimal; we simply optimize it away entirely.
                return loweredOperand;
            }

            if (kind == UnaryOperatorKind.EnumBitwiseComplement)
            {
                var underlyingType = loweredOperand.Type.GetEnumUnderlyingType();
                var upconvertSpecialType = Binder.GetEnumPromotedType(underlyingType.SpecialType);
                var upconvertType = upconvertSpecialType == underlyingType.SpecialType ?
                    underlyingType :
                    _compilation.GetSpecialType(upconvertSpecialType);


                var newOperand = MakeConversionNode(loweredOperand, upconvertType, false);
                UnaryOperatorKind newKind = kind.Operator().WithType(upconvertSpecialType);

                var newNode = (oldNode != null) ?
                    oldNode.Update(
                        newKind,
                        newOperand,
                        oldNode.ConstantValueOpt,
                        method,
                        newOperand.ResultKind,
                        upconvertType) :
                    new BoundUnaryOperator(
                        syntax,
                        newKind,
                        newOperand,
                        null,
                        method,
                        LookupResultKind.Viable,
                        upconvertType);

                return MakeConversionNode(newNode.Syntax, newNode, Conversion.ExplicitEnumeration, type, @checked: false);
            }

            return (oldNode != null) ?
                oldNode.Update(kind, loweredOperand, oldNode.ConstantValueOpt, method, oldNode.ResultKind, type) :
                new BoundUnaryOperator(syntax, kind, loweredOperand, null, method, LookupResultKind.Viable, type);
        }

        private BoundExpression LowerLiftedUnaryOperator(
            UnaryOperatorKind kind,
            SyntaxNode syntax,
            MethodSymbol method,
            BoundExpression loweredOperand,
            TypeSymbol type)
        {
            // First, an optimization. If we know that the operand is always null then
            // we can simply lower to the alternative.

            BoundExpression optimized = OptimizeLiftedUnaryOperator(kind, syntax, method, loweredOperand, type);
            if (optimized != null)
            {
                return optimized;
            }

            // We do not know whether the operand is null or non-null, so we generate:
            //
            // S? temp = operand;
            // R? r = temp.HasValue ? 
            //        new R?(OP(temp.GetValueOrDefault())) :
            //        default(R?);

            BoundAssignmentOperator tempAssignment;
            BoundLocal boundTemp = _factory.StoreToTemp(loweredOperand, out tempAssignment);
            MethodSymbol getValueOrDefault = UnsafeGetNullableMethod(syntax, boundTemp.Type, SpecialMember.System_Nullable_T_GetValueOrDefault);

            // temp.HasValue
            BoundExpression condition = MakeNullableHasValue(syntax, boundTemp);

            // temp.GetValueOrDefault()
            BoundExpression call_GetValueOrDefault = BoundCall.Synthesized(syntax, boundTemp, getValueOrDefault);

            // new R?(temp.GetValueOrDefault())
            BoundExpression consequence = GetLiftedUnaryOperatorConsequence(kind, syntax, method, type, call_GetValueOrDefault);

            // default(R?)
            BoundExpression alternative = new BoundDefaultExpression(syntax, null, type);

            // temp.HasValue ? 
            //          new R?(OP(temp.GetValueOrDefault())) : 
            //          default(R?);
            BoundExpression conditionalExpression = RewriteConditionalOperator(
                syntax: syntax,
                rewrittenCondition: condition,
                rewrittenConsequence: consequence,
                rewrittenAlternative: alternative,
                constantValueOpt: null,
                rewrittenType: type,
                isRef: false);

            // temp = operand; 
            // temp.HasValue ? 
            //          new R?(OP(temp.GetValueOrDefault())) : 
            //          default(R?);
            return new BoundSequence(
                syntax: syntax,
                locals: ImmutableArray.Create<LocalSymbol>(boundTemp.LocalSymbol),
                sideEffects: ImmutableArray.Create<BoundExpression>(tempAssignment),
                value: conditionalExpression,
                type: type);
        }

        private BoundExpression OptimizeLiftedUnaryOperator(
            UnaryOperatorKind operatorKind,
            SyntaxNode syntax,
            MethodSymbol method,
            BoundExpression loweredOperand,
            TypeSymbol type)
        {
            if (NullableNeverHasValue(loweredOperand))
            {
                return new BoundDefaultExpression(syntax, null, type);
            }

            // Second, another simple optimization. If we know that the operand is never null
            // then we can obtain the non-null value and skip generating the temporary. That is,
            // "~(new int?(M()))" is the same as "new int?(~M())".

            BoundExpression neverNull = NullableAlwaysHasValue(loweredOperand);
            if (neverNull != null)
            {
                return GetLiftedUnaryOperatorConsequence(operatorKind, syntax, method, type, neverNull);
            }

            var conditionalLeft = loweredOperand as BoundLoweredConditionalAccess;

            // NOTE: we could in theory handle side-effecting loweredRight here too
            //       by including it as a part of whenNull, but there is a concern 
            //       that it can lead to code duplication
            var optimize = conditionalLeft != null &&
                (conditionalLeft.WhenNullOpt == null || conditionalLeft.WhenNullOpt.IsDefaultValue());

            if (optimize)
            {
                var result = LowerLiftedUnaryOperator(operatorKind, syntax, method, conditionalLeft.WhenNotNull, type);

                return conditionalLeft.Update(
                    conditionalLeft.Receiver,
                    conditionalLeft.HasValueMethodOpt,
                    whenNotNull: result,
                    whenNullOpt: null,
                    id: conditionalLeft.Id,
                    type: result.Type
                );
            }

            // This optimization is analogous to DistributeLiftedConversionIntoLiftedOperand.

            // Suppose we have a lifted unary conversion whose operand is itself a lifted operation.
            // That is, we have something like:
            //
            // int? r = - (M() + N());
            // 
            // where M() and N() return nullable ints. We would simply codegen this as first
            // creating the nullable int result of M() + N(), then checking it for nullity,
            // and then doing the unary minus. That is:
            //
            // int? m = M();
            // int? n = N();
            // int? t = m.HasValue && n.HasValue ? new int?(m.Value + n.Value) : new int?();
            // int? r = t.HasValue ? new int?(-t.Value) : new int?();
            //
            // However, we also observe that we can distribute the unary minus into both branches of
            // the conditional:
            //
            // int? m = M();
            // int? n = N();
            // int? r = m.HasValue && n.HasValue ? - (new int?(m.Value + n.Value))) : - new int?();
            //
            // And we already optimize those! So we could reduce this to:
            //
            // int? m = M();
            // int? n = N();
            // int? r = m.HasValue && n.HasValue ? new int?(- (m.Value + n.Value)) : new int?());
            //
            // which avoids entirely the creation of the unnecessary nullable int and the unnecessary
            // extra null check.

            if (loweredOperand.Kind == BoundKind.Sequence)
            {
                BoundSequence seq = (BoundSequence)loweredOperand;
                if (seq.Value.Kind == BoundKind.ConditionalOperator)
                {
                    BoundConditionalOperator conditional = (BoundConditionalOperator)seq.Value;
                    Debug.Assert(TypeSymbol.Equals(seq.Type, conditional.Type, TypeCompareKind.ConsiderEverything2));
                    Debug.Assert(TypeSymbol.Equals(conditional.Type, conditional.Consequence.Type, TypeCompareKind.ConsiderEverything2));
                    Debug.Assert(TypeSymbol.Equals(conditional.Type, conditional.Alternative.Type, TypeCompareKind.ConsiderEverything2));

                    if (NullableAlwaysHasValue(conditional.Consequence) != null && NullableNeverHasValue(conditional.Alternative))
                    {
                        return new BoundSequence(
                            syntax,
                            seq.Locals,
                            seq.SideEffects,
                            RewriteConditionalOperator(
                                syntax,
                                conditional.Condition,
                                MakeUnaryOperator(operatorKind, syntax, method, conditional.Consequence, type),
                                MakeUnaryOperator(operatorKind, syntax, method, conditional.Alternative, type),
                                ConstantValue.NotAvailable,
                                type,
                                isRef: false),
                            type);
                    }
                }
            }

            return null;
        }

        private BoundExpression GetLiftedUnaryOperatorConsequence(UnaryOperatorKind kind, SyntaxNode syntax, MethodSymbol method, TypeSymbol type, BoundExpression nonNullOperand)
        {
            MethodSymbol ctor = UnsafeGetNullableMethod(syntax, type, SpecialMember.System_Nullable_T__ctor);

            // OP(temp.GetValueOrDefault())
            BoundExpression unliftedOp = MakeUnaryOperator(
                oldNode: null,
                kind: kind.Unlifted(),
                syntax: syntax,
                method: method,
                loweredOperand: nonNullOperand,
                type: type.GetNullableUnderlyingType());

            // new R?(OP(temp.GetValueOrDefault()))
            BoundExpression consequence = new BoundObjectCreationExpression(
                    syntax,
                    ctor,
                    null,
                    unliftedOp);
            return consequence;
        }

        private static bool IsIncrement(BoundIncrementOperator node)
        {
            var op = node.OperatorKind.Operator();
            return op == UnaryOperatorKind.PostfixIncrement || op == UnaryOperatorKind.PrefixIncrement;
        }

        private static bool IsPrefix(BoundIncrementOperator node)
        {
            var op = node.OperatorKind.Operator();
            return op == UnaryOperatorKind.PrefixIncrement || op == UnaryOperatorKind.PrefixDecrement;
        }

        /// <summary>
        /// The rewrites are as follows: suppose the operand x is a variable of type X. The
        /// chosen increment/decrement operator is modelled as a static method on a type T,
        /// which takes a value of type T and returns the result of incrementing or decrementing
        /// that value.
        /// 
        /// x++
        ///     X temp = x
        ///     x = (X)(T.Increment((T)temp))
        ///     return temp
        /// x--
        ///     X temp = x
        ///     x = (X)(T.Decrement((T)temp))
        ///     return temp
        /// ++x
        ///     X temp = (X)(T.Increment((T)x))
        ///     x = temp
        ///     return temp
        /// --x
        ///     X temp = (X)(T.Decrement((T)x))
        ///     x = temp
        ///     return temp
        /// 
        /// Note: 
        /// Dev11 implements dynamic prefix operators incorrectly.
        /// 
        ///   result = ++x.P  is emitted as  result = SetMember{"P"}(t, UnaryOperation{Inc}(GetMember{"P"}(x)))
        /// 
        /// The difference is that Dev11 relies on SetMember returning the same value as it was given as an argument.
        /// Failing to do so changes the semantics of ++/-- operator which is undesirable. We emit the same pattern for
        /// both dynamic and static operators.
        ///    
        /// For example, we might have a class X with user-defined implicit conversions
        /// to and from short, but no user-defined increment or decrement operators. We
        /// would bind x++ as "X temp = x; x = (X)(short)((int)(short)temp + 1); return temp;"
        /// </summary>
        /// <param name="node">The unary operator expression representing the increment/decrement.</param>
        /// <returns>A bound sequence that uses a temp to achieve the correct side effects and return value.</returns>
        public override BoundNode VisitIncrementOperator(BoundIncrementOperator node)
        {
            bool isPrefix = IsPrefix(node);
            bool isChecked = node.OperatorKind.IsChecked();

            ArrayBuilder<LocalSymbol> tempSymbols = ArrayBuilder<LocalSymbol>.GetInstance();
            ArrayBuilder<BoundExpression> tempInitializers = ArrayBuilder<BoundExpression>.GetInstance();

            SyntaxNode syntax = node.Syntax;

            // This will be filled in with the LHS that uses temporaries to prevent
            // double-evaluation of side effects.
            BoundExpression transformedLHS = TransformCompoundAssignmentLHS(node.Operand, tempInitializers, tempSymbols);
            TypeSymbol operandType = transformedLHS.Type; //type of the variable being incremented
            Debug.Assert(TypeSymbol.Equals(operandType, node.Type, TypeCompareKind.ConsiderEverything2));

            LocalSymbol tempSymbol = _factory.SynthesizedLocal(operandType);
            tempSymbols.Add(tempSymbol);
            // Not adding an entry to tempInitializers because the initial value depends on the case.

            BoundExpression boundTemp = new BoundLocal(
                syntax: syntax,
                localSymbol: tempSymbol,
                constantValueOpt: null,
                type: operandType);

            // prefix:  (X)(T.Increment((T)operand)))
            // postfix: (X)(T.Increment((T)temp)))
            var newValue = MakeIncrementOperator(node, rewrittenValueToIncrement: (isPrefix ? MakeRValue(transformedLHS) : boundTemp));

            // there are two strategies for completing the rewrite.
            // The reason is that indirect assignments read the target of the assignment before evaluating 
            // of the assignment value and that may cause reads of operand and boundTemp to cross which 
            // in turn would require one of them to be a real temp (not a stack local)
            //
            // To avoid this issue, in a case of ByRef operand, we perform a "nested sequence" rewrite.
            //
            // Ex: 
            //    Seq{..., operand = Seq{temp = operand + 1, temp}, ...}       
            //  instead of 
            //    Seq{.... temp = operand + 1, operand = temp, ...}              
            //
            // Such rewrite will nest reads of boundTemp relative to reads of operand so both 
            // operand and boundTemp could be optimizable (subject to all other conditions of course).
            //
            // In a case of the non-byref operand we use a single-sequence strategy as it results in shorter 
            // overall life time of temps and as such more appropriate. (problem of crossed reads does not affect that case)
            //
            if (IsIndirectOrInstanceField(transformedLHS))
            {
                return RewriteWithRefOperand(isPrefix, isChecked, tempSymbols, tempInitializers, syntax, transformedLHS, operandType, boundTemp, newValue);
            }
            else
            {
                return RewriteWithNotRefOperand(isPrefix, isChecked, tempSymbols, tempInitializers, syntax, transformedLHS, operandType, boundTemp, newValue);
            }
        }

        private static bool IsIndirectOrInstanceField(BoundExpression expression)
        {
            switch (expression.Kind)
            {
                case BoundKind.Local:
                    return ((BoundLocal)expression).LocalSymbol.RefKind != RefKind.None;

                case BoundKind.Parameter:
                    return ((BoundParameter)expression).ParameterSymbol.RefKind != RefKind.None;

                case BoundKind.FieldAccess:
                    return !((BoundFieldAccess)expression).FieldSymbol.IsStatic;
            }

            return false;
        }

        private BoundNode RewriteWithNotRefOperand(
            bool isPrefix,
            bool isChecked,
            ArrayBuilder<LocalSymbol> tempSymbols,
            ArrayBuilder<BoundExpression> tempInitializers,
            SyntaxNode syntax,
            BoundExpression transformedLHS,
            TypeSymbol operandType,
            BoundExpression boundTemp,
            BoundExpression newValue)
        {
            // prefix:  temp = (X)(T.Increment((T)operand)));  operand = temp; 
            // postfix: temp = operand;                        operand = (X)(T.Increment((T)temp)));
            ImmutableArray<BoundExpression> assignments = ImmutableArray.Create<BoundExpression>(
                MakeAssignmentOperator(syntax, boundTemp, isPrefix ? newValue : MakeRValue(transformedLHS), operandType, used: false, isChecked: isChecked, isCompoundAssignment: false),
                MakeAssignmentOperator(syntax, transformedLHS, isPrefix ? boundTemp : newValue, operandType, used: false, isChecked: isChecked, isCompoundAssignment: false));

            // prefix:  Seq( operand initializers; temp = (T)(operand + 1); operand = temp;          result: temp)
            // postfix: Seq( operand initializers; temp = operand;          operand = (T)(temp + 1); result: temp)
            return new BoundSequence(
                syntax: syntax,
                locals: tempSymbols.ToImmutableAndFree(),
                sideEffects: tempInitializers.ToImmutableAndFree().Concat(assignments),
                value: boundTemp,
                type: operandType);
        }

        private BoundNode RewriteWithRefOperand(
            bool isPrefix,
            bool isChecked,
            ArrayBuilder<LocalSymbol> tempSymbols,
            ArrayBuilder<BoundExpression> tempInitializers,
            SyntaxNode syntax,
            BoundExpression operand,
            TypeSymbol operandType,
            BoundExpression boundTemp,
            BoundExpression newValue)
        {
            var tempValue = isPrefix ? newValue : MakeRValue(operand);
            var tempAssignment = MakeAssignmentOperator(syntax, boundTemp, tempValue, operandType, used: false, isChecked: isChecked, isCompoundAssignment: false);

            var operandValue = isPrefix ? boundTemp : newValue;
            var tempAssignedAndOperandValue = new BoundSequence(
                    syntax,
                    ImmutableArray<LocalSymbol>.Empty,
                    ImmutableArray.Create<BoundExpression>(tempAssignment),
                    operandValue,
                    tempValue.Type);

            // prefix:  operand = Seq{temp = (T)(operand + 1);  temp;}
            // postfix: operand = Seq{temp = operand;        ;  (T)(temp + 1);}
            BoundExpression operandAssignment = MakeAssignmentOperator(syntax, operand, tempAssignedAndOperandValue, operandType, used: false, isChecked: isChecked, isCompoundAssignment: false);

            // prefix:  Seq{operand initializers; operand = Seq{temp = (T)(operand + 1);  temp;}          result: temp}
            // postfix: Seq{operand initializers; operand = Seq{temp = operand;        ;  (T)(temp + 1);} result: temp}
            tempInitializers.Add(operandAssignment);
            return new BoundSequence(
                syntax: syntax,
                locals: tempSymbols.ToImmutableAndFree(),
                sideEffects: tempInitializers.ToImmutableAndFree(),
                value: boundTemp,
                type: operandType);
        }

        private BoundExpression MakeIncrementOperator(BoundIncrementOperator node, BoundExpression rewrittenValueToIncrement)
        {
            BoundExpression result;
            if (node.OperatorKind.OperandTypes() == UnaryOperatorKind.UserDefined)
            {
                result = MakeUserDefinedIncrementOperator(node, rewrittenValueToIncrement);
            }
            else
            {
                result = MakeBuiltInIncrementOperator(node, rewrittenValueToIncrement);
            }

            // Generate the conversion back to the type of the original expression.

            // (X)(short)((int)(short)x + 1)
            if (!node.ResultConversion.IsIdentity)
            {
                result = MakeConversionNode(
                    syntax: node.Syntax,
                    rewrittenOperand: result,
                    conversion: node.ResultConversion,
                    rewrittenType: node.Type,
                    @checked: node.OperatorKind.IsChecked());
            }

            return result;
        }

        private BoundExpression MakeUserDefinedIncrementOperator(BoundIncrementOperator node, BoundExpression rewrittenValueToIncrement)
        {
            Debug.Assert((object)node.MethodOpt != null);
            Debug.Assert(node.MethodOpt.ParameterCount == 1);

            bool isLifted = node.OperatorKind.IsLifted();
            bool @checked = node.OperatorKind.IsChecked();

            BoundExpression rewrittenArgument = rewrittenValueToIncrement;
            SyntaxNode syntax = node.Syntax;

            TypeSymbol type = node.MethodOpt.ParameterTypes[0].TypeSymbol;
            if (isLifted)
            {
                type = _compilation.GetSpecialType(SpecialType.core_Option_T).Construct(type);
                Debug.Assert(TypeSymbol.Equals(node.MethodOpt.ParameterTypes[0].TypeSymbol, node.MethodOpt.ReturnType.TypeSymbol, TypeCompareKind.ConsiderEverything2));
            }

            if (!node.OperandConversion.IsIdentity)
            {
                rewrittenArgument = MakeConversionNode(
                    syntax: syntax,
                    rewrittenOperand: rewrittenValueToIncrement,
                    conversion: node.OperandConversion,
                    rewrittenType: type,
                    @checked: @checked);
            }

            if (!isLifted)
            {
                return BoundCall.Synthesized(syntax, null, node.MethodOpt, rewrittenArgument);
            }

            // S? temp = operand;
            // S? r = temp.HasValue ? 
            //        new S?(op_Increment(temp.GetValueOrDefault())) :
            //        default(S?);

            // Unlike the other unary operators, we do not attempt to optimize nullable user-defined 
            // increment or decrement. The operand is a variable (or property), and so we do not know if
            // it is always null/never null.

            BoundAssignmentOperator tempAssignment;
            BoundLocal boundTemp = _factory.StoreToTemp(rewrittenArgument, out tempAssignment);

            MethodSymbol getValueOrDefault = UnsafeGetNullableMethod(syntax, type, SpecialMember.System_Nullable_T_GetValueOrDefault);
            MethodSymbol ctor = UnsafeGetNullableMethod(syntax, type, SpecialMember.System_Nullable_T__ctor);

            // temp.HasValue
            BoundExpression condition = MakeNullableHasValue(node.Syntax, boundTemp);

            // temp.GetValueOrDefault()
            BoundExpression call_GetValueOrDefault = BoundCall.Synthesized(syntax, boundTemp, getValueOrDefault);

            // op_Increment(temp.GetValueOrDefault())
            BoundExpression userDefinedCall = BoundCall.Synthesized(syntax, null, node.MethodOpt, call_GetValueOrDefault);

            // new S?(op_Increment(temp.GetValueOrDefault()))
            BoundExpression consequence = new BoundObjectCreationExpression(syntax, ctor, null, userDefinedCall);

            // default(S?)
            BoundExpression alternative = new BoundDefaultExpression(syntax, null, type);

            // temp.HasValue ? 
            //          new S?(op_Increment(temp.GetValueOrDefault())) : 
            //          default(S?);
            BoundExpression conditionalExpression = RewriteConditionalOperator(
                syntax: syntax,
                rewrittenCondition: condition,
                rewrittenConsequence: consequence,
                rewrittenAlternative: alternative,
                constantValueOpt: null,
                rewrittenType: type,
                isRef: false);

            // temp = operand; 
            // temp.HasValue ? 
            //          new S?(op_Increment(temp.GetValueOrDefault())) : 
            //          default(S?);
            return new BoundSequence(
                syntax: syntax,
                locals: ImmutableArray.Create<LocalSymbol>(boundTemp.LocalSymbol),
                sideEffects: ImmutableArray.Create<BoundExpression>(tempAssignment),
                value: conditionalExpression,
                type: type);
        }

        private BoundExpression MakeBuiltInIncrementOperator(BoundIncrementOperator node, BoundExpression rewrittenValueToIncrement)
        {
            BoundExpression result;
            // If we have a built-in increment or decrement then things get a bit trickier. Suppose for example we have
            // a user-defined conversion from X to short and from short to X, but no user-defined increment operator on
            // X.  The increment portion of "++x" is then: (X)(short)((int)(short)x + 1). That is, first x must be
            // converted to short via an implicit user- defined conversion, then to int via an implicit numeric
            // conversion, then the addition is performed in integers. The resulting integer is converted back to short,
            // and then the short is converted to X.

            // This is the input and output type of the unary increment operator we're going to call.
            // That is, "short" in the example above.
            TypeSymbol unaryOperandType = GetUnaryOperatorType(node);

            // This is the kind of binary operator that we're going to realize the unary operator
            // as. That is, "int + int --> int" in the example above.
            BinaryOperatorKind binaryOperatorKind = GetCorrespondingBinaryOperator(node);
            binaryOperatorKind |= IsIncrement(node) ? BinaryOperatorKind.Addition : BinaryOperatorKind.Subtraction;

            // The "1" in the example above.
            ConstantValue constantOne = GetConstantOneForBinOp(binaryOperatorKind);

            Debug.Assert(constantOne != null);
            Debug.Assert(constantOne.SpecialType != SpecialType.None);
            Debug.Assert(binaryOperatorKind.OperandTypes() != 0);

            // The input/output type of the binary operand. "int" in the example. 
            TypeSymbol binaryOperandType = _compilation.GetSpecialType(constantOne.SpecialType);

            // 1
            BoundExpression boundOne = MakeLiteral(
                syntax: node.Syntax,
                constantValue: constantOne,
                type: binaryOperandType);

            if (binaryOperatorKind.IsLifted())
            {
                binaryOperandType = _compilation.GetSpecialType(SpecialType.core_Option_T).Construct(binaryOperandType);
                MethodSymbol ctor = UnsafeGetNullableMethod(node.Syntax, binaryOperandType, SpecialMember.System_Nullable_T__ctor);
                boundOne = new BoundObjectCreationExpression(node.Syntax, ctor, null, boundOne);
            }

            // Now we construct the other operand to the binary addition. We start with just plain "x".
            BoundExpression binaryOperand = rewrittenValueToIncrement;

            bool @checked = node.OperatorKind.IsChecked();

            // If we need to make a conversion from the original operand type to the operand type of the
            // underlying increment operation, do it now.
            if (!node.OperandConversion.IsIdentity)
            {
                // (short)x
                binaryOperand = MakeConversionNode(
                    syntax: node.Syntax,
                    rewrittenOperand: binaryOperand,
                    conversion: node.OperandConversion,
                    rewrittenType: unaryOperandType,
                    @checked: @checked);
            }

            // Early-out for pointer increment - we don't need to convert the operands to a common type.
            if (node.OperatorKind.OperandTypes() == UnaryOperatorKind.Pointer)
            {
                Debug.Assert(binaryOperatorKind.OperandTypes() == BinaryOperatorKind.PointerAndInt32);
                Debug.Assert(binaryOperand.Type.IsPointerType());
                Debug.Assert(boundOne.Type.SpecialType == SpecialType.System_Int32);
                return MakeBinaryOperator(node.Syntax, binaryOperatorKind, binaryOperand, boundOne, binaryOperand.Type, method: null);
            }

            // If we need to make a conversion from the unary operator type to the binary operator type,
            // do it now.

            // (int)(short)x
            binaryOperand = MakeConversionNode(binaryOperand, binaryOperandType, @checked);

            // Perform the addition.

            // (int)(short)x + 1            
            BoundExpression binOp;
            binOp = MakeBinaryOperator(node.Syntax, binaryOperatorKind, binaryOperand, boundOne, binaryOperandType, method: null);

            // Generate the conversion back to the type of the unary operator.

            // (short)((int)(short)x + 1)
            result = MakeConversionNode(binOp, unaryOperandType, @checked);
            return result;
        }


        /// <summary>
        /// Transform an expression from a form suitable as an lvalue to a form suitable as an rvalue.
        /// </summary>
        /// <param name="transformedExpression">The children of this node must already be lowered.</param>
        /// <returns>Fully lowered node.</returns>
        private BoundExpression MakeRValue(BoundExpression transformedExpression)
        {
            switch (transformedExpression.Kind)
            {
                case BoundKind.PropertyAccess:
                    var propertyAccess = (BoundPropertyAccess)transformedExpression;
                    return MakePropertyGetAccess(transformedExpression.Syntax, propertyAccess.ReceiverOpt, propertyAccess.PropertySymbol, propertyAccess);

                case BoundKind.IndexerAccess:
                    var indexerAccess = (BoundIndexerAccess)transformedExpression;
                    return MakePropertyGetAccess(transformedExpression.Syntax, indexerAccess.ReceiverOpt, indexerAccess.Indexer, indexerAccess.Arguments);

                default:
                    return transformedExpression;
            }
        }

        // There are ++ and -- operators defined on sbyte, byte, short, ushort, int,
        // uint, long, ulong, char, float, double, decimal and any enum type.
        // Given a built-in increment operator, get the associated type.  Note
        // that this need not be the result type or the operand type of the node!
        // We could have a user-defined conversion from the type of the operand
        // to short, and a user-defined conversion from short to the result
        // type.
        private TypeSymbol GetUnaryOperatorType(BoundIncrementOperator node)
        {
            UnaryOperatorKind kind = node.OperatorKind.OperandTypes();

            // If overload resolution chose an enum operator then the operand
            // type and the return type really are an enum; we are not in a user-
            // defined conversion scenario. 
            if (kind == UnaryOperatorKind.Enum)
            {
                return node.Type;
            }

            SpecialType specialType;

            switch (kind)
            {
                case UnaryOperatorKind.Int32:
                    specialType = SpecialType.System_Int32;
                    break;
                case UnaryOperatorKind.Int8:
                    specialType = SpecialType.System_Int8;
                    break;
                case UnaryOperatorKind.Int16:
                    specialType = SpecialType.System_Int16;
                    break;
                case UnaryOperatorKind.UInt8:
                    specialType = SpecialType.System_UInt8;
                    break;
                case UnaryOperatorKind.UShort:
                    specialType = SpecialType.System_UInt16;
                    break;
                case UnaryOperatorKind.Rune:
                    specialType = SpecialType.System_Rune;
                    break;
                case UnaryOperatorKind.UInt32:
                    specialType = SpecialType.System_UInt32;
                    break;
                case UnaryOperatorKind.Int64:
                    specialType = SpecialType.System_Int64;
                    break;
                case UnaryOperatorKind.UInt64:
                    specialType = SpecialType.System_UInt64;
                    break;
                case UnaryOperatorKind.Float32:
                    specialType = SpecialType.System_Float32;
                    break;
                case UnaryOperatorKind.Float64:
                    specialType = SpecialType.System_Float64;
                    break;
                case UnaryOperatorKind.Int:
                    specialType = SpecialType.System_Int;
                    break;
                case UnaryOperatorKind.UInt:
                    specialType = SpecialType.System_UInt;
                    break;
                case UnaryOperatorKind.Pointer:
                    return node.Type;
                case UnaryOperatorKind.UserDefined:
                case UnaryOperatorKind.Bool:
                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }

            NamedTypeSymbol type = _compilation.GetSpecialType(specialType);
            if (node.OperatorKind.IsLifted())
            {
                type = _compilation.GetSpecialType(SpecialType.core_Option_T).Construct(type);
            }

            return type;
        }

        private static BinaryOperatorKind GetCorrespondingBinaryOperator(BoundIncrementOperator node)
        {
            // We need to create expressions that have the semantics of incrementing or decrementing:
            // sbyte, byte, short, ushort, int, uint, long, ulong, char, float, double, decimal and
            // any enum.  However, the binary addition operators we have at our disposal are just
            // int, uint, long, ulong, float, double and decimal.

            UnaryOperatorKind unaryOperatorKind = node.OperatorKind;
            BinaryOperatorKind result;

            switch (unaryOperatorKind.OperandTypes())
            {
                case UnaryOperatorKind.Int32:
                case UnaryOperatorKind.Int8:
                case UnaryOperatorKind.Int16:
                    result = BinaryOperatorKind.Int32;
                    break;
                case UnaryOperatorKind.UInt8:
                case UnaryOperatorKind.UShort:
                case UnaryOperatorKind.Rune:
                case UnaryOperatorKind.UInt32:
                    result = BinaryOperatorKind.UInt32;
                    break;
                case UnaryOperatorKind.Int:
                    result = BinaryOperatorKind.Int;
                    break;
                case UnaryOperatorKind.UInt:
                    result = BinaryOperatorKind.UInt;
                    break;
                case UnaryOperatorKind.Int64:
                    result = BinaryOperatorKind.Int64;
                    break;
                case UnaryOperatorKind.UInt64:
                    result = BinaryOperatorKind.UInt64;
                    break;
                case UnaryOperatorKind.Float32:
                    result = BinaryOperatorKind.Float32;
                    break;
                case UnaryOperatorKind.Float64:
                    result = BinaryOperatorKind.Float64;
                    break;
                case UnaryOperatorKind.Enum:
                    {
                        TypeSymbol underlyingType = node.Type;
                        if (underlyingType.IsNullableType())
                        {
                            underlyingType = underlyingType.GetNullableUnderlyingType();
                        }
                        Debug.Assert(underlyingType.IsEnumType());
                        underlyingType = underlyingType.GetEnumUnderlyingType();

                        // Operator overload resolution will not have chosen the enumerated type
                        // unless the operand actually is of the enumerated type (or nullable enum type.)

                        switch (underlyingType.SpecialType)
                        {
                            case SpecialType.System_Int8:
                            case SpecialType.System_Int16:
                            case SpecialType.System_Int32:
                                result = BinaryOperatorKind.Int32;
                                break;
                            case SpecialType.System_UInt8:
                            case SpecialType.System_UInt16:
                            case SpecialType.System_UInt32:
                                result = BinaryOperatorKind.UInt32;
                                break;
                            case SpecialType.System_Int64:
                                result = BinaryOperatorKind.Int64;
                                break;
                            case SpecialType.System_UInt64:
                                result = BinaryOperatorKind.UInt64;
                                break;
                            default:
                                throw ExceptionUtilities.UnexpectedValue(underlyingType.SpecialType);
                        }
                    }
                    break;
                case UnaryOperatorKind.Pointer:
                    result = BinaryOperatorKind.PointerAndInt32;
                    break;
                case UnaryOperatorKind.UserDefined:
                case UnaryOperatorKind.Bool:
                default:
                    throw ExceptionUtilities.UnexpectedValue(unaryOperatorKind.OperandTypes());
            }

            switch (result)
            {
                case BinaryOperatorKind.UInt32:
                case BinaryOperatorKind.Int32:
                case BinaryOperatorKind.UInt64:
                case BinaryOperatorKind.Int64:
                case BinaryOperatorKind.Int:
                case BinaryOperatorKind.PointerAndInt32:
                    result |= (BinaryOperatorKind)unaryOperatorKind.OverflowChecks();
                    break;
            }

            if (unaryOperatorKind.IsLifted())
            {
                result |= BinaryOperatorKind.Lifted;
            }

            return result;
        }

        private static ConstantValue GetConstantOneForBinOp(
            BinaryOperatorKind binaryOperatorKind)
        {
            switch (binaryOperatorKind.OperandTypes())
            {
                case BinaryOperatorKind.PointerAndInt32:
                case BinaryOperatorKind.Int32:
                    return ConstantValue.Create(1);
                case BinaryOperatorKind.Int:
                case BinaryOperatorKind.UInt:
                    return ConstantValue.CreateInt(1);
                case BinaryOperatorKind.UInt32:
                    return ConstantValue.Create(1U);
                case BinaryOperatorKind.Int64:
                    return ConstantValue.Create(1L);
                case BinaryOperatorKind.UInt64:
                    return ConstantValue.Create(1LU);
                case BinaryOperatorKind.Float32:
                    return ConstantValue.Create(1f);
                case BinaryOperatorKind.Float64:
                    return ConstantValue.Create(1.0);
                default:
                    throw ExceptionUtilities.UnexpectedValue(binaryOperatorKind.OperandTypes());
            }
        }
    }
}
