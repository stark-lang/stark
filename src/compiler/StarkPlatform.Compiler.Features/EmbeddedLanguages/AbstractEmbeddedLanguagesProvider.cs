// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.Compiler.EmbeddedLanguages.LanguageServices;
using StarkPlatform.Compiler.Features.EmbeddedLanguages.RegularExpressions;

namespace StarkPlatform.Compiler.Features.EmbeddedLanguages
{
    /// <summary>
    /// Abstract implementation of the C# and VB embedded language providers.
    /// </summary>
    internal abstract class AbstractEmbeddedLanguageFeaturesProvider : AbstractEmbeddedLanguagesProvider, IEmbeddedLanguageFeaturesProvider
    {
        new public ImmutableArray<IEmbeddedLanguageFeatures> Languages { get; }

        protected AbstractEmbeddedLanguageFeaturesProvider(EmbeddedLanguageInfo info) : base(info)
        {
            // No 'Fallback' language added here.  That's because the Fallback language doesn't
            // support any of the IEmbeddedLanguageFeatures or IEmbeddedLanguageEditorFeatures
            // capabilities.
            Languages = ImmutableArray.Create<IEmbeddedLanguageFeatures>(
                new RegexEmbeddedLanguageFeatures(info));
        }
    }
}
