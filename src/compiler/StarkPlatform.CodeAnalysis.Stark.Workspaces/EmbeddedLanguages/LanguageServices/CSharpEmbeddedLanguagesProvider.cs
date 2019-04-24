// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Stark.EmbeddedLanguages.VirtualChars;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.EmbeddedLanguages.LanguageServices;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.EmbeddedLanguages.LanguageServices
{
    [ExportLanguageService(typeof(IEmbeddedLanguagesProvider), LanguageNames.Stark), Shared]
    internal class CSharpEmbeddedLanguagesProvider : AbstractEmbeddedLanguagesProvider
    {
        public static EmbeddedLanguageInfo Info = new EmbeddedLanguageInfo(
            (int)SyntaxKind.StringLiteralToken,
            (int)SyntaxKind.InterpolatedStringTextToken,
            CSharpSyntaxFactsService.Instance,
            CSharpSemanticFactsService.Instance,
            CSharpVirtualCharService.Instance);
        public static IEmbeddedLanguagesProvider Instance = new CSharpEmbeddedLanguagesProvider();

        public CSharpEmbeddedLanguagesProvider() : base(Info)
        {
        }
    }
}
