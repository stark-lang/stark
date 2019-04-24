// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.ExtractMethod;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.ExtractMethod
{
    [ExportLanguageServiceFactory(typeof(ISyntaxTriviaService), LanguageNames.Stark), Shared]
    internal class CSharpSyntaxTriviaServiceFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices provider)
        {
            return new CSharpSyntaxTriviaService(provider);
        }
    }
}
