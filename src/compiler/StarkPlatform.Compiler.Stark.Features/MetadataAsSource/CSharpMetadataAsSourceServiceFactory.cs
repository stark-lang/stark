// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.MetadataAsSource;

namespace StarkPlatform.Compiler.Stark.MetadataAsSource
{
    [ExportLanguageServiceFactory(typeof(IMetadataAsSourceService), LanguageNames.Stark), Shared]
    internal class CSharpMetadataAsSourceServiceFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices provider)
        {
            return new CSharpMetadataAsSourceService(provider);
        }
    }
}
