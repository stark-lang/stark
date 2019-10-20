// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitConditionalAccess(BoundConditionalAccess node)
        {
            return RewriteConditionalAccess(node, used: true);
        }

        public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        // null when currently enclosing conditional access node
        // is not supposed to be lowered.
        private BoundExpression _currentConditionalAccessTarget;
        private int _currentConditionalAccessID;

        private enum ConditionalAccessLoweringKind
        {
            LoweredConditionalAccess,
            Ternary,
            TernaryCaptureReceiverByVal
        }

        // IL gen can generate more compact code for certain conditional accesses 
        // by utilizing stack dup/pop instructions 
        internal BoundExpression RewriteConditionalAccess(BoundConditionalAccess node, bool used)
        {
            var loweredReceiver = this.VisitExpression(node.Receiver);
            var receiverType = loweredReceiver.Type;

            // Check trivial case
            if (loweredReceiver.IsDefaultValue() && receiverType.IsReferenceType)
            {
                return _factory.Default(node.Type);
            }

            ConditionalAccessLoweringKind loweringKind;

            // trivial cases are directly supported in IL gen
            loweringKind = ConditionalAccessLoweringKind.LoweredConditionalAccess;

            var previousConditionalAccessTarget = _currentConditionalAccessTarget;
            var currentConditionalAccessID = ++_currentConditionalAccessID;

            LocalSymbol temp = null;

            switch (loweringKind)
            {
                case ConditionalAccessLoweringKind.LoweredConditionalAccess:
                    _currentConditionalAccessTarget = new BoundConditionalReceiver(
                        loweredReceiver.Syntax,
                        currentConditionalAccessID,
                        receiverType);

                    break;

                case ConditionalAccessLoweringKind.Ternary:
                    _currentConditionalAccessTarget = loweredReceiver;
                    break;

                case ConditionalAccessLoweringKind.TernaryCaptureReceiverByVal:
                    temp = _factory.SynthesizedLocal(receiverType);
                    _currentConditionalAccessTarget = _factory.Local(temp);
                    break;

                default:
                    throw ExceptionUtilities.UnexpectedValue(loweringKind);
            }

            BoundExpression loweredAccessExpression;

            if (used)
            {
                loweredAccessExpression = this.VisitExpression(node.AccessExpression);
            }
            else
            {
                loweredAccessExpression = this.VisitUnusedExpression(node.AccessExpression);
                if (loweredAccessExpression == null)
                {
                    return null;
                }
            }

            Debug.Assert(loweredAccessExpression != null);
            _currentConditionalAccessTarget = previousConditionalAccessTarget;

            TypeSymbol type = this.VisitType(node.Type);

            TypeSymbol nodeType = node.Type;
            TypeSymbol accessExpressionType = loweredAccessExpression.Type;

            if (accessExpressionType.SpecialType == SpecialType.System_Void)
            {
                type = nodeType = accessExpressionType;
            }

            if (!TypeSymbol.Equals(accessExpressionType, nodeType, TypeCompareKind.ConsiderEverything2) && nodeType.IsNullableType())
            {
                Debug.Assert(TypeSymbol.Equals(accessExpressionType, nodeType.GetNullableUnderlyingType(), TypeCompareKind.ConsiderEverything2));
                loweredAccessExpression = _factory.New((NamedTypeSymbol)nodeType, loweredAccessExpression);
            }
            else
            {
                Debug.Assert(TypeSymbol.Equals(accessExpressionType, nodeType, TypeCompareKind.ConsiderEverything2) ||
                    (nodeType.SpecialType == SpecialType.System_Void && !used));
            }

            BoundExpression result;
            var objectType = _compilation.GetSpecialType(SpecialType.System_Object);

            switch (loweringKind)
            {
                case ConditionalAccessLoweringKind.LoweredConditionalAccess:
                    result = new BoundLoweredConditionalAccess(
                        node.Syntax,
                        loweredReceiver,
                        receiverType.IsNullableType() ?
                                 UnsafeGetNullableMethod(node.Syntax, loweredReceiver.Type, SpecialMember.core_Option_T_get_has_value) :
                                 null,
                        loweredAccessExpression,
                        null,
                        currentConditionalAccessID,
                        type);

                    break;

                case ConditionalAccessLoweringKind.TernaryCaptureReceiverByVal:
                    // capture the receiver into a temp
                    loweredReceiver = _factory.MakeSequence(
                                            _factory.AssignmentExpression(_factory.Local(temp), loweredReceiver),
                                            _factory.Local(temp));

                    goto case ConditionalAccessLoweringKind.Ternary;

                case ConditionalAccessLoweringKind.Ternary:
                    {
                        // (object)r != null ? access : default(T)
                        var condition = _factory.ObjectNotEqual(
                                _factory.Convert(objectType, loweredReceiver),
                                _factory.Null(objectType));

                        var consequence = loweredAccessExpression;

                        result = RewriteConditionalOperator(node.Syntax,
                            condition,
                            consequence,
                            _factory.Default(nodeType),
                            null,
                            nodeType,
                            isRef: false);

                        if (temp != null)
                        {
                            result = _factory.MakeSequence(temp, result);
                        }
                    }
                    break;

                default:
                    throw ExceptionUtilities.UnexpectedValue(loweringKind);
            }

            return result;
        }

        public override BoundNode VisitConditionalReceiver(BoundConditionalReceiver node)
        {
            var newtarget = _currentConditionalAccessTarget;

            if (newtarget.Type.IsNullableType())
            {
                newtarget = MakeOptimizedGetValueOrDefault(node.Syntax, newtarget);
            }

            return newtarget;
        }
    }
}
