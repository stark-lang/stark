// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Stark
{
    [ExportLanguageServiceFactory(typeof(ISyntaxFactsService), LanguageNames.Stark), Shared]
    internal class CSharpSyntaxFactsServiceFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices languageServices)
            => CSharpSyntaxFactsService.Instance;
    }
}
