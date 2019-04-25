// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.LanguageServices;

namespace StarkPlatform.Compiler.Editor.CSharp.LanguageServices
{
    [ExportLanguageServiceFactory(typeof(ISymbolDisplayService), LanguageNames.Stark), Shared]
    internal partial class CSharpSymbolDisplayServiceFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices provider)
        {
            return new CSharpSymbolDisplayService(provider);
        }
    }
}
