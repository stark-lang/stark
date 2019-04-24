// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.CommentSelection
{
    internal interface ICommentSelectionService : ILanguageService
    {
        Task<CommentSelectionInfo> GetInfoAsync(Document document, TextSpan textSpan, CancellationToken cancellationToken);

        Task<Document> FormatAsync(Document document, ImmutableArray<TextSpan> changes, CancellationToken cancellationToken);
    }
}
