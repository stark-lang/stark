// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.Ergon.ProjectFile;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;

namespace StarkPlatform.Compiler.Ergon.CSharp
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
