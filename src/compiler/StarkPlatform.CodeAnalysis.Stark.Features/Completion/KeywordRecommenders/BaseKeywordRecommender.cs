// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.Completion.KeywordRecommenders
{
    internal class BaseKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public BaseKeywordRecommender()
            : base(SyntaxKind.BaseKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            // We need to at least be in a type declaration context.  This prevents us from showing
            // calls to 'base' in things like top level repl statements and whatnot.
            if (context.ContainingTypeDeclaration != null)
            {
                return
                    IsConstructorInitializerContext(position, context, cancellationToken) ||
                    IsInstanceExpressionOrStatement(context);
            }

            return false;
        }

        private static bool IsInstanceExpressionOrStatement(CSharpSyntaxContext context)
        {
            if (context.IsInstanceContext)
            {
                return context.IsNonAttributeExpressionContext || context.IsStatementContext;
            }

            return false;
        }

        private bool IsConstructorInitializerContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            // cases:
            //   Goo() : |

            var token = context.TargetToken;

            if (token.Kind() == SyntaxKind.ColonToken &&
                token.Parent is ConstructorInitializerSyntax &&
                token.Parent.IsParentKind(SyntaxKind.ConstructorDeclaration) &&
                token.Parent.GetParent().IsParentKind(SyntaxKind.ClassDeclaration))
            {
                var constructor = token.GetAncestor<ConstructorDeclarationSyntax>();
                if (constructor.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
