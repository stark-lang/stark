// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Host;

namespace StarkPlatform.CodeAnalysis.SolutionCrawler
{
    /// <summary>
    /// Register a solution crawler for a particular workspace
    /// </summary>
    internal interface ISolutionCrawlerRegistrationService : IWorkspaceService
    {
        void Register(Workspace workspace);
        void Unregister(Workspace workspace, bool blockingShutdown = false);

        void AddAnalyzerProvider(IIncrementalAnalyzerProvider provider, IncrementalAnalyzerProviderMetadata metadata);
    }
}
