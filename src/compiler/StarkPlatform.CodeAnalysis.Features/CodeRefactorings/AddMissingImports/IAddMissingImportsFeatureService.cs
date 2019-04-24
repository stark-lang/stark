// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.AddMissingImports
{
    internal interface IAddMissingImportsFeatureService : ILanguageService
    {
        Task<bool> HasMissingImportsAsync(Document document, TextSpan textSpan, CancellationToken cancellationToken);

        Task<Project> AddMissingImportsAsync(Document document, TextSpan textSpan, CancellationToken cancellationToken);
    }
}
