// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Structure;

namespace StarkPlatform.CodeAnalysis.Stark.Structure
{
    internal class AccessorDeclarationStructureProvider : AbstractSyntaxNodeStructureProvider<AccessorDeclarationSyntax>
    {
        protected override void CollectBlockSpans(
            AccessorDeclarationSyntax accessorDeclaration,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            CSharpStructureHelpers.CollectCommentBlockSpans(accessorDeclaration, spans);

            // fault tolerance
            if (accessorDeclaration.Body == null ||
                accessorDeclaration.Body.OpenBraceToken.IsMissing ||
                accessorDeclaration.Body.CloseBraceToken.IsMissing)
            {
                return;
            }

            spans.AddIfNotNull(CSharpStructureHelpers.CreateBlockSpan(
                accessorDeclaration,
                accessorDeclaration.Keyword,
                autoCollapse: true,
                type: BlockTypes.Member,
                isCollapsible: true));
        }
    }
}
