// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Structure;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Structure
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
