// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.ExtractMethod
{
    internal static class ExtractMethodService
    {
        public static Task<ExtractMethodResult> ExtractMethodAsync(Document document, TextSpan textSpan, OptionSet options = null, CancellationToken cancellationToken = default)
        {
            return document.GetLanguageService<IExtractMethodService>().ExtractMethodAsync(document, textSpan, options, cancellationToken);
        }
    }
}
