// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using StarkPlatform.CodeAnalysis.Formatting.Rules;
using StarkPlatform.CodeAnalysis.Text;

#if !CODE_STYLE
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Options;
#endif

namespace StarkPlatform.CodeAnalysis.Formatting
{
    internal interface ISyntaxFormattingService
#if !CODE_STYLE
        : ILanguageService
#endif
    {
        IEnumerable<IFormattingRule> GetDefaultFormattingRules();
        IFormattingResult Format(SyntaxNode node, IEnumerable<TextSpan> spans, OptionSet options, IEnumerable<IFormattingRule> rules, CancellationToken cancellationToken);
    }
}
