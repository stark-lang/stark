// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.CodeStyle;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.DocumentHighlighting;
using StarkPlatform.CodeAnalysis.EmbeddedLanguages.LanguageServices;
using StarkPlatform.CodeAnalysis.EmbeddedLanguages.RegularExpressions.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Features.EmbeddedLanguages.RegularExpressions
{
    internal class RegexEmbeddedLanguageFeatures : RegexEmbeddedLanguage, IEmbeddedLanguageFeatures
    {
        public IDocumentHighlightsService DocumentHighlightsService { get; }
        public AbstractBuiltInCodeStyleDiagnosticAnalyzer DiagnosticAnalyzer { get; }

        public RegexEmbeddedLanguageFeatures(EmbeddedLanguageInfo info) : base(info)
        {
            DocumentHighlightsService = new RegexDocumentHighlightsService(this);
            DiagnosticAnalyzer = new RegexDiagnosticAnalyzer(info);
        }
    }
}
