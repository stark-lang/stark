// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Stark.Utilities;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.Completion.KeywordRecommenders
{
    internal class PartialKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        private static readonly ISet<SyntaxKind> s_validMemberModifiers = new HashSet<SyntaxKind>(SyntaxFacts.EqualityComparer)
        {
            SyntaxKind.AsyncKeyword,
            SyntaxKind.StaticKeyword
        };

        public PartialKeywordRecommender()
            : base(SyntaxKind.PartialKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return
                context.IsGlobalStatementContext ||
                IsMemberDeclarationContext(context, cancellationToken) ||
                IsTypeDeclarationContext(context, cancellationToken);
        }

        private bool IsMemberDeclarationContext(CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            if (context.IsMemberDeclarationContext(validModifiers: s_validMemberModifiers, validTypeDeclarations: SyntaxKindSet.ClassStructTypeDeclarations, canBePartial: false, cancellationToken: cancellationToken))
            {
                var token = context.LeftToken;
                var decl = token.GetAncestor<TypeDeclarationSyntax>();

                // partial methods must be in partial types
                if (!decl.Modifiers.Any(t => t.IsKindOrHasMatchingText(SyntaxKind.PartialKeyword)))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private bool IsTypeDeclarationContext(CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return context.IsTypeDeclarationContext(
                validModifiers: SyntaxKindSet.AllTypeModifiers,
                validTypeDeclarations: SyntaxKindSet.ClassStructTypeDeclarations,
                canBePartial: false,
                cancellationToken: cancellationToken);
        }
    }
}
