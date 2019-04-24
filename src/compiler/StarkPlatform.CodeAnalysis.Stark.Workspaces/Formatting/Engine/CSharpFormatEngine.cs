// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using StarkPlatform.CodeAnalysis.Formatting;
using StarkPlatform.CodeAnalysis.Formatting.Rules;
using StarkPlatform.CodeAnalysis.Options;

namespace StarkPlatform.CodeAnalysis.Stark.Formatting
{
    internal class CSharpFormatEngine : AbstractFormatEngine
    {
        public CSharpFormatEngine(
            SyntaxNode node,
            OptionSet optionSet,
            IEnumerable<IFormattingRule> formattingRules,
            SyntaxToken token1,
            SyntaxToken token2) :
            base(TreeData.Create(node),
                 optionSet,
                 formattingRules,
                 token1,
                 token2)
        {
        }

        protected override AbstractTriviaDataFactory CreateTriviaFactory()
        {
            return new TriviaDataFactory(this.TreeData, this.OptionSet);
        }

        protected override AbstractFormattingResult CreateFormattingResult(TokenStream tokenStream)
        {
            return new FormattingResult(this.TreeData, tokenStream, this.SpanToFormat);
        }
    }
}
