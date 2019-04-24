// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Stark.EmbeddedLanguages.LanguageServices;
using StarkPlatform.CodeAnalysis.Features.EmbeddedLanguages;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.Features.EmbeddedLanguages
{
    [ExportLanguageService(typeof(IEmbeddedLanguageFeaturesProvider), LanguageNames.Stark), Shared]
    internal class CSharpEmbeddedLanguageFeaturesProvider : AbstractEmbeddedLanguageFeaturesProvider
    {
        public static IEmbeddedLanguageFeaturesProvider Instance = new CSharpEmbeddedLanguageFeaturesProvider();

        public CSharpEmbeddedLanguageFeaturesProvider() : base(CSharpEmbeddedLanguagesProvider.Info)
        {
        }
    }
}
