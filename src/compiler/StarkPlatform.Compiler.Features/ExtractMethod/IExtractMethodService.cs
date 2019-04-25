// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.ExtractMethod
{
    internal interface IExtractMethodService : ILanguageService
    {
        Task<ExtractMethodResult> ExtractMethodAsync(Document document, TextSpan textSpan, OptionSet options = null, CancellationToken cancellationToken = default);
    }
}
