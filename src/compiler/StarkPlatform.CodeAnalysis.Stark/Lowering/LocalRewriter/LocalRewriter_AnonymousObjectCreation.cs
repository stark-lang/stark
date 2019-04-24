// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            // Rewrite the arguments.
            var rewrittenArguments = VisitList(node.Arguments);

            return new BoundObjectCreationExpression(
                syntax: node.Syntax,
                constructor: node.Constructor,
                arguments: rewrittenArguments,
                argumentNamesOpt: default(ImmutableArray<string>),
                argumentRefKindsOpt: default(ImmutableArray<RefKind>),
                expanded: false,
                argsToParamsOpt: default(ImmutableArray<int>),
                constantValueOpt: null,
                initializerExpressionOpt: null,
                binderOpt: null,
                type: node.Type);
        }
    }
}
