// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Features.EmbeddedLanguages;

namespace StarkPlatform.CodeAnalysis.Stark.Features.EmbeddedLanguages
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpEmbeddedLanguageDiagnosticAnalyzer : AbstractEmbeddedLanguageDiagnosticAnalyzer
    {
        public CSharpEmbeddedLanguageDiagnosticAnalyzer()
            : base(CSharpEmbeddedLanguageFeaturesProvider.Instance)
        {
        }
    }
}
