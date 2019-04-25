// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.CodeCleanup;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;

namespace StarkPlatform.Compiler.Stark.CodeCleanup
{
    [ExportLanguageServiceFactory(typeof(ICodeCleanerService), LanguageNames.Stark), Shared]
    internal class CSharpCodeCleanerServiceFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices provider)
        {
            return new CSharpCodeCleanerService();
        }
    }
}
