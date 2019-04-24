// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Structure;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Structure
{
    internal class SwitchStatementStructureProvider : AbstractSyntaxNodeStructureProvider<SwitchStatementSyntax>
    {
        protected override void CollectBlockSpans(
            SwitchStatementSyntax node,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            spans.Add(new BlockSpan(
                isCollapsible: true,
                textSpan: TextSpan.FromBounds((node.CloseParenToken != default) ? node.CloseParenToken.Span.End : node.Expression.Span.End, node.CloseBraceToken.Span.End),
                hintSpan: node.Span,
                type: BlockTypes.Conditional));
        }
    }
}
