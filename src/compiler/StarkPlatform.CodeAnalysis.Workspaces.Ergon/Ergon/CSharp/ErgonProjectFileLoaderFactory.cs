// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Ergon.ProjectFile;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Ergon.CSharp
{
    [Shared]
    [ExportLanguageServiceFactory(typeof(IProjectFileLoader), LanguageNames.Stark)]
    [ProjectFileExtension("toml")]
    internal class ErgonProjectFileLoaderFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices languageServices)
        {
            return new ErgonProjectFileLoader();
        }
    }
}
