// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.FindSymbols.Finders
{
    internal class ConstructorInitializerSymbolReferenceFinder : AbstractReferenceFinder<IMethodSymbol>
    {
        protected override bool CanFind(IMethodSymbol symbol)
        {
            return symbol.MethodKind == MethodKind.Constructor;
        }

        protected override Task<ImmutableArray<Document>> DetermineDocumentsToSearchAsync(
            IMethodSymbol symbol,
            Project project,
            IImmutableSet<Document> documents,
            FindReferencesSearchOptions options,
            CancellationToken cancellationToken)
        {
            return FindDocumentsAsync(project, documents, async (d, c) =>
            {
                var index = await SyntaxTreeIndex.GetIndexAsync(d, c).ConfigureAwait(false);
                if (index.ContainsBaseConstructorInitializer)
                {
                    return true;
                }

                if (index.ProbablyContainsIdentifier(symbol.ContainingType.Name))
                {
                    if (index.ContainsThisConstructorInitializer)
                    {
                        return true;
                    }
                }

                return false;
            }, cancellationToken);
        }

        protected override async Task<ImmutableArray<FinderLocation>> FindReferencesInDocumentAsync(
            IMethodSymbol methodSymbol,
            Document document,
            SemanticModel semanticModel,
            FindReferencesSearchOptions options,
            CancellationToken cancellationToken)
        {
            var syntaxFactsService = document.GetLanguageService<ISyntaxFactsService>();
            var typeName = methodSymbol.ContainingType.Name;

            bool tokensMatch(SyntaxToken t)
            {
                if (syntaxFactsService.IsBaseConstructorInitializer(t))
                {
                    var containingType = semanticModel.GetEnclosingNamedType(t.SpanStart, cancellationToken);
                    return containingType != null && containingType.BaseType != null && containingType.BaseType.Name == typeName;
                }
                else if (syntaxFactsService.IsThisConstructorInitializer(t))
                {
                    var containingType = semanticModel.GetEnclosingNamedType(t.SpanStart, cancellationToken);
                    return containingType != null && containingType.Name == typeName;
                }

                return false;
            }

            var tokens = await document.GetConstructorInitializerTokensAsync(semanticModel, cancellationToken).ConfigureAwait(false);

            return await FindReferencesInTokensAsync(
                 methodSymbol,
                 document,
                 semanticModel,
                 tokens,
                 tokensMatch,
                 cancellationToken).ConfigureAwait(false);
        }
    }
}
