// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.CodeStyle;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.DocumentHighlighting;
using StarkPlatform.CodeAnalysis.EmbeddedLanguages.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Features.EmbeddedLanguages
{
    /// <summary>
    /// Services related to a specific embedded language.
    /// </summary>
    internal interface IEmbeddedLanguageFeatures : IEmbeddedLanguage
    {
        /// <summary>
        /// A optional highlighter that can highlight spans for an embedded language string.
        /// </summary>
        IDocumentHighlightsService DocumentHighlightsService { get; }

        /// <summary>
        /// An optional analyzer that produces diagnostics for an embedded language string.
        /// </summary>
        AbstractBuiltInCodeStyleDiagnosticAnalyzer DiagnosticAnalyzer { get; }
    }
}
