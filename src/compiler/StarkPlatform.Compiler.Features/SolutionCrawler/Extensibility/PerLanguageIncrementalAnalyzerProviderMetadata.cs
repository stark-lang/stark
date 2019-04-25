// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using StarkPlatform.Compiler.Host.Mef;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.SolutionCrawler
{
    internal class PerLanguageIncrementalAnalyzerProviderMetadata : LanguageMetadata
    {
        public string Name { get; }

        public PerLanguageIncrementalAnalyzerProviderMetadata(IDictionary<string, object> data)
            : base(data)
        {
            this.Name = (string)data.GetValueOrDefault("Name");
        }
    }
}
