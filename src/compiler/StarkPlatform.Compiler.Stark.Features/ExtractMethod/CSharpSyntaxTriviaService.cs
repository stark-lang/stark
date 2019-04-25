// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.ExtractMethod;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.LanguageServices;

namespace StarkPlatform.Compiler.Stark.ExtractMethod
{
    internal class CSharpSyntaxTriviaService : AbstractSyntaxTriviaService
    {
        public CSharpSyntaxTriviaService(HostLanguageServices provider)
            : base(provider.GetService<ISyntaxFactsService>(), (int)SyntaxKind.EndOfLineTrivia)
        {
        }
    }
}
