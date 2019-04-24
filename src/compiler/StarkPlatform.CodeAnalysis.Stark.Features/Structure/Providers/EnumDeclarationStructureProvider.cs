// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Structure;

namespace StarkPlatform.CodeAnalysis.Stark.Structure
{
    internal class EnumDeclarationStructureProvider : AbstractSyntaxNodeStructureProvider<EnumDeclarationSyntax>
    {
        protected override void CollectBlockSpans(
            EnumDeclarationSyntax enumDeclaration,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            CSharpStructureHelpers.CollectCommentBlockSpans(enumDeclaration, spans);

            if (!enumDeclaration.OpenBraceToken.IsMissing &&
                !enumDeclaration.CloseBraceToken.IsMissing)
            {
                spans.AddIfNotNull(CSharpStructureHelpers.CreateBlockSpan(
                    enumDeclaration,
                    enumDeclaration.Identifier,
                    autoCollapse: false,
                    type: BlockTypes.Member,
                    isCollapsible: true));
            }

            // add any leading comments before the end of the type block
            if (!enumDeclaration.CloseBraceToken.IsMissing)
            {
                var leadingTrivia = enumDeclaration.CloseBraceToken.LeadingTrivia;
                CSharpStructureHelpers.CollectCommentBlockSpans(leadingTrivia, spans);
            }
        }
    }
}
