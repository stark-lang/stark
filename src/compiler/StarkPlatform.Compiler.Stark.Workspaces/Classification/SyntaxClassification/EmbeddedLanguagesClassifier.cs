// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Classification.Classifiers;
using StarkPlatform.Compiler.Stark.EmbeddedLanguages.LanguageServices;

namespace StarkPlatform.Compiler.Stark.Classification.Classifiers
{
    internal class EmbeddedLanguagesClassifier : AbstractEmbeddedLanguagesClassifier
    {
        public EmbeddedLanguagesClassifier()
            : base(CSharpEmbeddedLanguagesProvider.Instance)
        {
        }
    }
}
