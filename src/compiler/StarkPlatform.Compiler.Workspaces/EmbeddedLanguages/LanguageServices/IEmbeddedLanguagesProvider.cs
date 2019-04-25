// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.Compiler.Host;

namespace StarkPlatform.Compiler.EmbeddedLanguages.LanguageServices
{
    /// <summary>
    /// Service that returns all the embedded languages supported.  Each embedded language can expose
    /// individual language services through the <see cref="IEmbeddedLanguage"/> interface.
    /// </summary>
    internal interface IEmbeddedLanguagesProvider : ILanguageService
    {
        ImmutableArray<IEmbeddedLanguage> Languages { get; }
    }
}
