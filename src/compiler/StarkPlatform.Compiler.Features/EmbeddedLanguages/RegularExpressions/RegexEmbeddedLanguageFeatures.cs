// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.CodeStyle;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.DocumentHighlighting;
using StarkPlatform.Compiler.EmbeddedLanguages.LanguageServices;
using StarkPlatform.Compiler.EmbeddedLanguages.RegularExpressions.LanguageServices;

namespace StarkPlatform.Compiler.Features.EmbeddedLanguages.RegularExpressions
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
