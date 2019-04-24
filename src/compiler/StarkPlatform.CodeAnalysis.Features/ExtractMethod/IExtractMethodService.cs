// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.ExtractMethod
{
    internal interface IExtractMethodService : ILanguageService
    {
        Task<ExtractMethodResult> ExtractMethodAsync(Document document, TextSpan textSpan, OptionSet options = null, CancellationToken cancellationToken = default);
    }
}
