// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Structure;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Structure
{
    internal class DocumentationCommentStructureProvider : AbstractSyntaxNodeStructureProvider<DocumentationCommentTriviaSyntax>
    {
        protected override void CollectBlockSpans(
            DocumentationCommentTriviaSyntax documentationComment,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            var startPos = documentationComment.FullSpan.Start;

            // The trailing newline is included in XmlDocCommentSyntax, so we need to strip it.
            var endPos = documentationComment.SpanStart + documentationComment.ToString().TrimEnd().Length;

            var span = TextSpan.FromBounds(startPos, endPos);

            var bannerLength = options.GetOption(BlockStructureOptions.MaximumBannerLength, LanguageNames.Stark);
            var bannerText = CSharpSyntaxFactsService.Instance.GetBannerText(
                documentationComment, bannerLength, cancellationToken);

            spans.Add(new BlockSpan(
                isCollapsible: true,
                textSpan: span,
                type: BlockTypes.Comment,
                bannerText: bannerText,
                autoCollapse: true));
        }
    }
}
