// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Options;

namespace StarkPlatform.CodeAnalysis.Recommendations
{
    internal interface IRecommendationService : ILanguageService
    {
        Task<ImmutableArray<ISymbol>> GetRecommendedSymbolsAtPositionAsync(
            Workspace workspace,
            SemanticModel semanticModel,
            int position,
            OptionSet options,
            CancellationToken cancellationToken);
    }
}
