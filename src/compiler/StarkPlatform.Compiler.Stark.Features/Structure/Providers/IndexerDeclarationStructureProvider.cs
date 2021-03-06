﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Structure;

namespace StarkPlatform.Compiler.Stark.Structure
{
    internal class IndexerDeclarationStructureProvider : AbstractSyntaxNodeStructureProvider<IndexerDeclarationSyntax>
    {
        protected override void CollectBlockSpans(
            IndexerDeclarationSyntax indexerDeclaration,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            CSharpStructureHelpers.CollectCommentBlockSpans(indexerDeclaration, spans);

            // fault tolerance
            if (indexerDeclaration.AccessorList == null ||
                indexerDeclaration.AccessorList.IsMissing ||
                indexerDeclaration.AccessorList.OpenBraceToken.IsMissing ||
                indexerDeclaration.AccessorList.CloseBraceToken.IsMissing)
            {
                return;
            }

            spans.AddIfNotNull(CSharpStructureHelpers.CreateBlockSpan(
                indexerDeclaration,
                indexerDeclaration.ParameterList.GetLastToken(includeZeroWidth: true),
                autoCollapse: true,
                type: BlockTypes.Member,
                isCollapsible: true));
        }
    }
}
