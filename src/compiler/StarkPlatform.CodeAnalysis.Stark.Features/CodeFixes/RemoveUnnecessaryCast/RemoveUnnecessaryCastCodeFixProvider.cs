// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeActions;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.Formatting;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.Simplification;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.CodeFixes.RemoveUnnecessaryCast
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.RemoveUnnecessaryCast), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.ImplementInterface)]
    internal partial class RemoveUnnecessaryCastCodeFixProvider : SyntaxEditorBasedCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(IDEDiagnosticIds.RemoveUnnecessaryCastDiagnosticId);

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            context.RegisterCodeFix(new MyCodeAction(
                FeaturesResources.Remove_Unnecessary_Cast,
                c => FixAsync(context.Document, context.Diagnostics.First(), c)),
                context.Diagnostics);
            return Task.CompletedTask;
        }

        protected override async Task FixAllAsync(
            Document document, ImmutableArray<Diagnostic> diagnostics,
            SyntaxEditor editor, CancellationToken cancellationToken)
        {
            var castNodes = diagnostics.SelectAsArray(
                d => (CastExpressionSyntax)d.AdditionalLocations[0].FindNode(getInnermostNodeForTie: true, cancellationToken));

            await editor.ApplyExpressionLevelSemanticEditsAsync(
                document, castNodes,
                (semanticModel, castExpression) => castExpression.IsUnnecessaryCast(semanticModel, cancellationToken),
                (_, currentRoot, castExpression) =>
                {
                    var oldParent = castExpression.WalkUpParentheses();
                    var newParent = Recurse(oldParent);

                    return currentRoot.ReplaceNode(oldParent, newParent);
                },
                cancellationToken).ConfigureAwait(false);
        }

        private ExpressionSyntax Recurse(ExpressionSyntax old)
        {
            if (old is ParenthesizedExpressionSyntax parenthesizedExpression)
            {
                // It's common in C# to have to write  ((Goo)expr).Etc(). we don't just want to
                // remove the cast and produce (expr).Etc().  So we mark all parent parenthesized
                // expressions as worthy of simplification.  The simplifier will remove these
                // if possible, or leave them alone if not.
                return parenthesizedExpression.ReplaceNode(parenthesizedExpression.Expression, Recurse(parenthesizedExpression.Expression))
                                              .WithAdditionalAnnotations(Simplifier.Annotation);
            }
            else if (old is CastExpressionSyntax castExpression)
            {
                // parenthesize the uncasted value to help ensure any proper parsing. The excess
                // parens will be removed if unnecessary. 
                return castExpression.Uncast().WithAdditionalAnnotations(Formatter.Annotation)
                                     .Parenthesize();
            }
            else
            {
                throw ExceptionUtilities.UnexpectedValue(old);
            }
        }

        private class MyCodeAction : CodeAction.DocumentChangeAction
        {
            public MyCodeAction(string title, Func<CancellationToken, Task<Document>> createChangedDocument) :
                base(title, createChangedDocument)
            {
            }
        }
    }
}
