// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Classification.Classifiers;
using StarkPlatform.CodeAnalysis.EmbeddedLanguages.LanguageServices;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.EmbeddedLanguages.RegularExpressions.LanguageServices
{
    internal class RegexEmbeddedLanguage : IEmbeddedLanguage
    {
        public readonly EmbeddedLanguageInfo Info;

        public ISyntaxClassifier Classifier { get; }

        public RegexEmbeddedLanguage(EmbeddedLanguageInfo info)
        {
            Info = info;
            Classifier = new RegexSyntaxClassifier(info);
        }

        internal async Task<(RegexTree tree, SyntaxToken token)> TryGetTreeAndTokenAtPositionAsync(
            Document document, int position, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(position);
            var syntaxFacts = document.GetLanguageService<ISyntaxFactsService>();
            if (RegexPatternDetector.IsDefinitelyNotPattern(token, syntaxFacts))
            {
                return default;
            }

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var detector = RegexPatternDetector.TryGetOrCreate(semanticModel, this.Info);
            var tree = detector?.TryParseRegexPattern(token, cancellationToken);
            return tree == null ? default : (tree, token);
        }

        internal async Task<RegexTree> TryGetTreeAtPositionAsync(
            Document document, int position, CancellationToken cancellationToken)
        {
            var (tree, _) = await TryGetTreeAndTokenAtPositionAsync(
                document, position, cancellationToken).ConfigureAwait(false);
            return tree;
        }
    }
}
