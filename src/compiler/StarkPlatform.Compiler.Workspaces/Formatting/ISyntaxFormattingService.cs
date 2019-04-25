// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using StarkPlatform.Compiler.Formatting.Rules;
using StarkPlatform.Compiler.Text;

#if !CODE_STYLE
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Options;
#endif

namespace StarkPlatform.Compiler.Formatting
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
