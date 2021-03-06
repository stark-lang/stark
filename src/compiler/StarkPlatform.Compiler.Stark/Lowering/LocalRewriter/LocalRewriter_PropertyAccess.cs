﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
        {
            return VisitPropertyAccess(node, isLeftOfAssignment: false);
        }

        private BoundExpression VisitPropertyAccess(BoundPropertyAccess node, bool isLeftOfAssignment)
        {
            var rewrittenReceiverOpt = VisitExpression(node.ReceiverOpt);
            return MakePropertyAccess(node.Syntax, rewrittenReceiverOpt, node.PropertySymbol, node.ResultKind, node.Type, isLeftOfAssignment, node);
        }

        private BoundExpression MakePropertyAccess(
            SyntaxNode syntax,
            BoundExpression rewrittenReceiverOpt,
            PropertySymbol propertySymbol,
            LookupResultKind resultKind,
            TypeSymbol type,
            bool isLeftOfAssignment,
            BoundPropertyAccess oldNodeOpt = null)
        {
            // check for System.Array.[Length|LongLength] on a single dimensional array,
            // we have a special node for such cases.
            if (rewrittenReceiverOpt != null && rewrittenReceiverOpt.Type.IsArray() && !isLeftOfAssignment)
            {
                // NOTE: we are not interested in potential badness of Array.Length property.
                // If it is bad reference compare will not succeed.

                if (ReferenceEquals(propertySymbol, _compilation.GetSpecialTypeMember(SpecialMember.core_Array__size)))
                {
                    return new BoundArraySize(syntax, rewrittenReceiverOpt, type);
                }
            }

            if (isLeftOfAssignment && propertySymbol.RefKind == RefKind.None)
            {
                // This is a property set access. We return a BoundPropertyAccess node here.
                // This node will be rewritten with MakePropertyAssignment when rewriting the enclosing BoundAssignmentOperator.

                return oldNodeOpt != null ?
                    oldNodeOpt.Update(rewrittenReceiverOpt, propertySymbol, resultKind, type) :
                    new BoundPropertyAccess(syntax, rewrittenReceiverOpt, propertySymbol, resultKind, type);
            }
            else
            {
                // This is a property get access
                return MakePropertyGetAccess(syntax, rewrittenReceiverOpt, propertySymbol, oldNodeOpt);
            }
        }

        private BoundExpression MakePropertyGetAccess(SyntaxNode syntax, BoundExpression rewrittenReceiver, PropertySymbol property, BoundPropertyAccess oldNodeOpt)
        {
            return MakePropertyGetAccess(syntax, rewrittenReceiver, property, ImmutableArray<BoundExpression>.Empty, null, oldNodeOpt);
        }

        private BoundExpression MakePropertyGetAccess(
            SyntaxNode syntax,
            BoundExpression rewrittenReceiver,
            PropertySymbol property,
            ImmutableArray<BoundExpression> rewrittenArguments,
            MethodSymbol getMethodOpt = null,
            BoundPropertyAccess oldNodeOpt = null)
        {
            var getMethod = getMethodOpt ?? property.GetOwnOrInheritedGetMethod();

            Debug.Assert((object)getMethod != null);
            Debug.Assert(getMethod.ParameterCount == rewrittenArguments.Length);
            Debug.Assert(((object)getMethodOpt == null) || ReferenceEquals(getMethod, getMethodOpt));

            return BoundCall.Synthesized(
                syntax,
                rewrittenReceiver,
                getMethod,
                rewrittenArguments);
        }
    }
}
