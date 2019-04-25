// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.CodeGeneration;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;

namespace StarkPlatform.Compiler.Stark.CodeGeneration
{
    [ExportLanguageServiceFactory(typeof(ICodeGenerationService), LanguageNames.Stark), Shared]
    internal partial class CSharpCodeGenerationServiceFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices provider)
        {
            return new CSharpCodeGenerationService(provider);
        }
    }
}
