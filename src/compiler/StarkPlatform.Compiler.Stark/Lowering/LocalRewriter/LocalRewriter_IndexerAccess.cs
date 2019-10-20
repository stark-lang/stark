// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitIndexerAccess(BoundIndexerAccess node)
        {
            Debug.Assert(node.Indexer.IsIndexer || node.Indexer.IsIndexedProperty);
            Debug.Assert((object)node.Indexer.GetOwnOrInheritedGetMethod() != null);

            return VisitIndexerAccess(node, isLeftOfAssignment: false);
        }

        private BoundExpression VisitIndexerAccess(BoundIndexerAccess node, bool isLeftOfAssignment)
        {
            PropertySymbol indexer = node.Indexer;
            Debug.Assert(indexer.IsIndexer || indexer.IsIndexedProperty);

            // Rewrite the receiver.
            BoundExpression rewrittenReceiver = VisitExpression(node.ReceiverOpt);

            // Rewrite the arguments.
            // NOTE: We may need additional argument rewriting such as generating a params array, re-ordering arguments based on argsToParamsOpt map, inserting arguments for optional parameters, etc.
            // NOTE: This is done later by MakeArguments, for now we just lower each argument.
            ImmutableArray<BoundExpression> rewrittenArguments = VisitList(node.Arguments);

            return MakeIndexerAccess(
                node.Syntax,
                rewrittenReceiver,
                indexer,
                rewrittenArguments,
                node.ArgumentNamesOpt,
                node.ArgumentRefKindsOpt,
                node.Expanded,
                node.ArgsToParamsOpt,
                node.Type,
                node,
                isLeftOfAssignment);
        }

        private BoundExpression MakeIndexerAccess(
            SyntaxNode syntax,
            BoundExpression rewrittenReceiver,
            PropertySymbol indexer,
            ImmutableArray<BoundExpression> rewrittenArguments,
            ImmutableArray<string> argumentNamesOpt,
            ImmutableArray<RefKind> argumentRefKindsOpt,
            bool expanded,
            ImmutableArray<int> argsToParamsOpt,
            TypeSymbol type,
            BoundIndexerAccess oldNodeOpt,
            bool isLeftOfAssignment)
        {
            // check for System.Array.[Length|LongLength] on a single dimensional array,
            // we have a special node for such cases.
            if (rewrittenReceiver != null && rewrittenReceiver.Type.IsArray() && !isLeftOfAssignment)
            {
                // NOTE: we are not interested in potential badness of Array.Length property.
                // If it is bad reference compare will not succeed.

                var specialIndexer = _compilation.GetSpecialTypeMember(SpecialMember.core_Array_T__item);

                if (ReferenceEquals(indexer.OriginalDefinition, specialIndexer))
                {
                    return new BoundArrayAccess(syntax, rewrittenReceiver, rewrittenArguments[0], indexer.Type.TypeSymbol);
                }
            }


            if (isLeftOfAssignment && indexer.RefKind == RefKind.None)
            {
                // This is an indexer set access. We return a BoundIndexerAccess node here.
                // This node will be rewritten with MakePropertyAssignment when rewriting the enclosing BoundAssignmentOperator.

                return oldNodeOpt != null ?
                    oldNodeOpt.Update(rewrittenReceiver, indexer, rewrittenArguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, null, isLeftOfAssignment, type) :
                    new BoundIndexerAccess(syntax, rewrittenReceiver, indexer, rewrittenArguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, null, isLeftOfAssignment, type);
            }
            else
            {
                var getMethod = indexer.GetOwnOrInheritedGetMethod();
                Debug.Assert((object)getMethod != null);

                // We have already lowered each argument, but we may need some additional rewriting for the arguments,
                // such as generating a params array, re-ordering arguments based on argsToParamsOpt map, inserting arguments for optional parameters, etc.
                ImmutableArray<LocalSymbol> temps;
                rewrittenArguments = MakeArguments(
                    syntax,
                    rewrittenArguments,
                    indexer,
                    getMethod,
                    expanded,
                    argsToParamsOpt,
                    ref argumentRefKindsOpt,
                    out temps,
                    enableCallerInfo: ThreeState.True);

                BoundExpression call = MakePropertyGetAccess(syntax, rewrittenReceiver, indexer, rewrittenArguments, getMethod);

                if (temps.IsDefaultOrEmpty)
                {
                    return call;
                }
                else
                {
                    return new BoundSequence(
                        syntax,
                        temps,
                        ImmutableArray<BoundExpression>.Empty,
                        call,
                        type);
                }
            }
        }
    }
}
