// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Host;

namespace StarkPlatform.CodeAnalysis.RemoveUnnecessaryImports
{
    internal interface IRemoveUnnecessaryImportsService : ILanguageService
    {
        Task<Document> RemoveUnnecessaryImportsAsync(Document document, CancellationToken cancellationToken);
        Task<Document> RemoveUnnecessaryImportsAsync(Document fromDocument, Func<SyntaxNode, bool> predicate, CancellationToken cancellationToken);
    }
}
