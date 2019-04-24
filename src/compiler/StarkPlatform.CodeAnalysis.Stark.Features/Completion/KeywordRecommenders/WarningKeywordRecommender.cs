// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery;

namespace StarkPlatform.CodeAnalysis.Stark.Completion.KeywordRecommenders
{
    internal class WarningKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public WarningKeywordRecommender()
            : base(SyntaxKind.WarningKeyword, isValidInPreprocessorContext: true)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            // # warning
            if (context.IsPreProcessorKeywordContext)
            {
                return true;
            }

            // # pragma |
            // # pragma w|
            var previousToken1 = context.TargetToken;
            var previousToken2 = previousToken1.GetPreviousToken(includeSkipped: true);

            return
                previousToken1.Kind() == SyntaxKind.PragmaKeyword &&
                previousToken2.Kind() == SyntaxKind.HashToken;
        }
    }
}
