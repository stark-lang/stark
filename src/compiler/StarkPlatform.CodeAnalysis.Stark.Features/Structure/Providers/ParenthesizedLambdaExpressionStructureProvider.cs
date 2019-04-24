// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Structure;

namespace StarkPlatform.CodeAnalysis.Stark.Structure
{
    internal class ParenthesizedLambdaExpressionStructureProvider : AbstractSyntaxNodeStructureProvider<ParenthesizedLambdaExpressionSyntax>
    {
        protected override void CollectBlockSpans(
            ParenthesizedLambdaExpressionSyntax lambdaExpression,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            // fault tolerance
            if (lambdaExpression.Body.IsMissing)
            {
                return;
            }

            var lambdaBlock = lambdaExpression.Body as BlockSyntax;
            if (lambdaBlock == null ||
                lambdaBlock.OpenBraceToken.IsMissing ||
                lambdaBlock.CloseBraceToken.IsMissing)
            {
                return;
            }

            var lastToken = CSharpStructureHelpers.GetLastInlineMethodBlockToken(lambdaExpression);
            if (lastToken.Kind() == SyntaxKind.None)
            {
                return;
            }

            spans.AddIfNotNull(CSharpStructureHelpers.CreateBlockSpan(
                lambdaExpression,
                lambdaExpression.ArrowToken,
                lastToken,
                autoCollapse: false,
                type: BlockTypes.Expression,
                isCollapsible: true));
        }
    }
}
