// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitRangeVariable(BoundRangeVariable node)
        {
            return VisitExpression(node.Value);
        }

        public override BoundNode VisitQueryClause(BoundQueryClause node)
        {
            return VisitExpression(node.Value);
        }
    }
}
