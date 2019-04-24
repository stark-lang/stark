// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Structure;

namespace StarkPlatform.CodeAnalysis.Stark.Structure
{
    internal class DelegateDeclarationStructureProvider : AbstractSyntaxNodeStructureProvider<DelegateDeclarationSyntax>
    {
        protected override void CollectBlockSpans(
            DelegateDeclarationSyntax delegateDeclaration,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            CSharpStructureHelpers.CollectCommentBlockSpans(delegateDeclaration, spans);
        }
    }
}
