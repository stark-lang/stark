// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.Shared.Utilities;

namespace StarkPlatform.Compiler.CodeCleanup
{
    internal interface ICodeCleanupService : ILanguageService
    {
        Task<Document> CleanupAsync(Document document, EnabledDiagnosticOptions enabledDiagnostics, IProgressTracker progressTracker, CancellationToken cancellationToken);
        EnabledDiagnosticOptions GetAllDiagnostics();
        EnabledDiagnosticOptions GetEnabledDiagnostics(OptionSet optionSet);
    }
}
