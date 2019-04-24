// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using StarkPlatform.CodeAnalysis.Host.Mef;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Diagnostics
{
    internal class DiagnosticProviderMetadata : ILanguageMetadata
    {
        public string Name { get; }
        public string Language { get; }

        public DiagnosticProviderMetadata(IDictionary<string, object> data)
        {
            this.Name = (string)data.GetValueOrDefault("Name");
            this.Language = (string)data.GetValueOrDefault("Language");
        }
    }
}
