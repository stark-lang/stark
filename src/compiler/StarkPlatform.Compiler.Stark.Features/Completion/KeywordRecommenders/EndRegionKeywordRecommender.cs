// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions.ContextQuery;

namespace StarkPlatform.Compiler.Stark.Completion.KeywordRecommenders
{
    internal class EndRegionKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public EndRegionKeywordRecommender()
            : base(SyntaxKind.EndRegionKeyword, isValidInPreprocessorContext: true, shouldFormatOnCommit: true)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return context.IsPreProcessorKeywordContext;
        }
    }
}
