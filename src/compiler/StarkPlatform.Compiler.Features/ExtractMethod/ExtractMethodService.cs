// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.Shared.Extensions;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.ExtractMethod
{
    internal static class ExtractMethodService
    {
        public static Task<ExtractMethodResult> ExtractMethodAsync(Document document, TextSpan textSpan, OptionSet options = null, CancellationToken cancellationToken = default)
        {
            return document.GetLanguageService<IExtractMethodService>().ExtractMethodAsync(document, textSpan, options, cancellationToken);
        }
    }
}
