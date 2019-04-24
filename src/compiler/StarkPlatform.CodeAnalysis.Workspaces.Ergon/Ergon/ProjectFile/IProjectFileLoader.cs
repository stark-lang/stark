// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Ergon.Build;
using StarkPlatform.CodeAnalysis.Host;

namespace StarkPlatform.CodeAnalysis.Ergon.ProjectFile
{
    internal interface IProjectFileLoader : ILanguageService
    {
        string Language { get; }
        Task<IProjectFile> LoadProjectFileAsync(
            string path,
            ProjectBuildManager buildManager,
            CancellationToken cancellationToken);
    }
}
