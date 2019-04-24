// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Structure;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Structure
{
    internal class ArrowExpressionClauseStructureProvider : AbstractSyntaxNodeStructureProvider<ArrowExpressionClauseSyntax>
    {
        protected override void CollectBlockSpans(
            ArrowExpressionClauseSyntax node,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            var previousToken = node.ArrowToken.GetPreviousToken();
            spans.Add(new BlockSpan(
                isCollapsible: true,
                textSpan: TextSpan.FromBounds(previousToken.Span.End, node.Parent.Span.End),
                hintSpan: node.Parent.Span,
                type: BlockTypes.Nonstructural,
                autoCollapse: !node.IsParentKind(SyntaxKind.LocalFunctionStatement)));
        }
    }
}
