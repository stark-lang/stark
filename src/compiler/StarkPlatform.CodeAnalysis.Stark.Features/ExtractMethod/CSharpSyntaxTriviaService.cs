// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.ExtractMethod;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Stark.ExtractMethod
{
    internal class CSharpSyntaxTriviaService : AbstractSyntaxTriviaService
    {
        public CSharpSyntaxTriviaService(HostLanguageServices provider)
            : base(provider.GetService<ISyntaxFactsService>(), (int)SyntaxKind.EndOfLineTrivia)
        {
        }
    }
}
