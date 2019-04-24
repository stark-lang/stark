// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Structure;

namespace StarkPlatform.CodeAnalysis.Stark.Structure
{
    internal class DestructorDeclarationStructureProvider : AbstractSyntaxNodeStructureProvider<DestructorDeclarationSyntax>
    {
        protected override void CollectBlockSpans(
            DestructorDeclarationSyntax destructorDeclaration,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            CSharpStructureHelpers.CollectCommentBlockSpans(destructorDeclaration, spans);

            // fault tolerance
            if (destructorDeclaration.Body == null ||
                destructorDeclaration.Body.OpenBraceToken.IsMissing ||
                destructorDeclaration.Body.CloseBraceToken.IsMissing)
            {
                return;
            }

            spans.AddIfNotNull(CSharpStructureHelpers.CreateBlockSpan(
                destructorDeclaration,
                destructorDeclaration.ParameterList.GetLastToken(includeZeroWidth: true),
                autoCollapse: true,
                type: BlockTypes.Member,
                isCollapsible: true));
        }
    }
}
