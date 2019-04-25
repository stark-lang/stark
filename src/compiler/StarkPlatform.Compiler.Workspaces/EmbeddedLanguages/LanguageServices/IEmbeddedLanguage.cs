// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Classification;
using StarkPlatform.Compiler.Classification.Classifiers;

namespace StarkPlatform.Compiler.EmbeddedLanguages.LanguageServices
{
    /// <summary>
    /// Services related to a specific embedded language.
    /// </summary>
    internal interface IEmbeddedLanguage
    {
        /// <summary>
        /// A optional classifier that can produce <see cref="ClassifiedSpan"/>s for an embedded language string.
        /// </summary>
        ISyntaxClassifier Classifier { get; }
    }
}
