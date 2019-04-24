// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Host;

namespace StarkPlatform.CodeAnalysis.OrganizeImports
{
    internal interface IOrganizeImportsService : ILanguageService
    {
        Task<Document> OrganizeImportsAsync(Document document, CancellationToken cancellationToken);

        string SortAndRemoveUnusedImportsDisplayStringWithAccelerator { get; }
    }
}
