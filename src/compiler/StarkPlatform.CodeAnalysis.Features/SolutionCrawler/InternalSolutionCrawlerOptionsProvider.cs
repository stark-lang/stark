// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.Options.Providers;

namespace StarkPlatform.CodeAnalysis.SolutionCrawler
{
    [ExportOptionProvider, Shared]
    internal class InternalSolutionCrawlerOptionsProvider : IOptionProvider
    {
        public ImmutableArray<IOption> Options { get; } = ImmutableArray.Create<IOption>(
            InternalSolutionCrawlerOptions.SolutionCrawler,
            InternalSolutionCrawlerOptions.ActiveFileWorkerBackOffTimeSpanInMS,
            InternalSolutionCrawlerOptions.AllFilesWorkerBackOffTimeSpanInMS,
            InternalSolutionCrawlerOptions.EntireProjectWorkerBackOffTimeSpanInMS,
            InternalSolutionCrawlerOptions.SemanticChangeBackOffTimeSpanInMS,
            InternalSolutionCrawlerOptions.ProjectPropagationBackOffTimeSpanInMS,
            InternalSolutionCrawlerOptions.PreviewBackOffTimeSpanInMS);
    }
}
