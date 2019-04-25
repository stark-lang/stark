// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.AddMissingImports
{
    internal interface IAddMissingImportsFeatureService : ILanguageService
    {
        Task<bool> HasMissingImportsAsync(Document document, TextSpan textSpan, CancellationToken cancellationToken);

        Task<Project> AddMissingImportsAsync(Document document, TextSpan textSpan, CancellationToken cancellationToken);
    }
}
