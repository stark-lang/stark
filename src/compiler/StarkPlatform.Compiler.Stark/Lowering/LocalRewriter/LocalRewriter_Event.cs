// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.Compiler.Stark;
using StarkPlatform.Compiler.Stark.Symbols.Metadata.PE;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.RuntimeMembers;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            BoundExpression rewrittenReceiverOpt = VisitExpression(node.ReceiverOpt);
            BoundExpression rewrittenArgument = VisitExpression(node.Argument);

            var rewrittenArguments = ImmutableArray.Create<BoundExpression>(rewrittenArgument);

            MethodSymbol method = node.IsAddition ? node.Event.AddMethod : node.Event.RemoveMethod;
            return MakeCall(node.Syntax, rewrittenReceiverOpt, method, rewrittenArguments, node.Type);
        }

        private enum EventAssignmentKind
        {
            Assignment,
            Addition,
            Subtraction,
        }

        public override BoundNode VisitEventAccess(BoundEventAccess node)
        {
            // We didn't get here via VisitEventAssignmentOperator (i.e. += or -=),
            // so the event better be field-like.
            Debug.Assert(node.IsUsableAsField);

            BoundExpression rewrittenReceiver = VisitExpression(node.ReceiverOpt);
            return MakeEventAccess(node.Syntax, rewrittenReceiver, node.EventSymbol, node.ConstantValue, node.ResultKind, node.Type);
        }

        private BoundExpression MakeEventAccess(
            SyntaxNode syntax,
            BoundExpression rewrittenReceiver,
            EventSymbol eventSymbol,
            ConstantValue constantValueOpt,
            LookupResultKind resultKind,
            TypeSymbol type)
        {
            Debug.Assert(eventSymbol.HasAssociatedField);

            FieldSymbol fieldSymbol = eventSymbol.AssociatedField;
            Debug.Assert((object)fieldSymbol != null);

            return MakeFieldAccess(syntax, rewrittenReceiver, fieldSymbol, constantValueOpt, resultKind, type);
        }
    }
}
