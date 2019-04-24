// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.Completion.KeywordRecommenders
{
    internal class AddKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public AddKeywordRecommender()
            : base(SyntaxKind.AddKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return context.TargetToken.IsAccessorDeclarationContext<EventDeclarationSyntax>(position, SyntaxKind.AddKeyword);
        }
    }
}
