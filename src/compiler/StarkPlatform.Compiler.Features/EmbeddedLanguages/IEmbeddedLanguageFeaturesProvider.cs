// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.Compiler.EmbeddedLanguages.LanguageServices;
using StarkPlatform.Compiler.Host;

namespace StarkPlatform.Compiler.Features.EmbeddedLanguages
{
    /// <summary>
    /// Service that returns all the embedded languages supported.  Each embedded language can expose
    /// individual language services through the <see cref="IEmbeddedLanguageFeatures"/> interface.
    /// </summary>
    internal interface IEmbeddedLanguageFeaturesProvider : IEmbeddedLanguagesProvider
    {
        new ImmutableArray<IEmbeddedLanguageFeatures> Languages { get; }
    }
}
