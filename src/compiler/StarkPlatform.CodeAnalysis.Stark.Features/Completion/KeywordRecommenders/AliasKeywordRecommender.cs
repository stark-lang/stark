// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.Completion.KeywordRecommenders
{
    internal class AliasKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public AliasKeywordRecommender()
            : base(SyntaxKind.AliasKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            // cases:
            //   extern |
            //   extern a|
            var token = context.TargetToken;

            if (token.Kind() == SyntaxKind.ExternKeyword)
            {
                // members can be 'extern' but we don't want
                // 'alias' to show up in a 'type'.
                return token.GetAncestor<TypeDeclarationSyntax>() == null;
            }

            return false;
        }
    }
}
