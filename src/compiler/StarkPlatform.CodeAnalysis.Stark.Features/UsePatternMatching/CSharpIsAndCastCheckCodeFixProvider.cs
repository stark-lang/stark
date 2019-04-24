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
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.UsePatternMatching
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal partial class CSharpIsAndCastCheckCodeFixProvider : SyntaxEditorBasedCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(IDEDiagnosticIds.InlineIsTypeCheckId);

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            context.RegisterCodeFix(new MyCodeAction(
                c => FixAsync(context.Document, context.Diagnostics.First(), c)),
                context.Diagnostics);
            return Task.CompletedTask;
        }

        protected override Task FixAllAsync(
            Document document, ImmutableArray<Diagnostic> diagnostics,
            SyntaxEditor editor, CancellationToken cancellationToken)
        {
            foreach (var diagnostic in diagnostics)
            {
                cancellationToken.ThrowIfCancellationRequested();
                AddEdits(editor, diagnostic, cancellationToken);
            }

            return Task.CompletedTask;
        }

        private void AddEdits(
            SyntaxEditor editor,
            Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            var ifStatementLocation = diagnostic.AdditionalLocations[0];
            var localDeclarationLocation = diagnostic.AdditionalLocations[1];

            var ifStatement = (IfStatementSyntax)ifStatementLocation.FindNode(cancellationToken);
            var localDeclaration = (LocalDeclarationStatementSyntax)localDeclarationLocation.FindNode(cancellationToken);
            var isExpression = (BinaryExpressionSyntax)ifStatement.Condition;

            var updatedCondition = SyntaxFactory.IsPatternExpression(
                isExpression.Left, SyntaxFactory.DeclarationPattern(
                    ((TypeSyntax)isExpression.Right).WithoutTrivia(),
                    SyntaxFactory.SingleVariableDesignation(
                        localDeclaration.Declaration.Identifier.WithoutTrivia())));

            var trivia = localDeclaration.GetLeadingTrivia().Concat(localDeclaration.GetTrailingTrivia())
                                         .Where(t => t.IsSingleOrMultiLineComment())
                                         .SelectMany(t => ImmutableArray.Create(SyntaxFactory.Space, t, SyntaxFactory.ElasticCarriageReturnLineFeed))
                                         .ToImmutableArray();

            editor.RemoveNode(localDeclaration);
            editor.ReplaceNode(ifStatement,
                (i, g) =>
                {
                    // Because the local declaration is *inside* the 'if', we need to get the 'if' 
                    // statement after it was already modified and *then* update the condition
                    // portion of it.
                    var currentIf = (IfStatementSyntax)i;
                    return GetUpdatedIfStatement(updatedCondition, trivia, ifStatement, currentIf);
                });
        }

        private static IfStatementSyntax GetUpdatedIfStatement(
            IsPatternExpressionSyntax updatedCondition,
            ImmutableArray<SyntaxTrivia> trivia,
            IfStatementSyntax originalIf,
            IfStatementSyntax currentIf)
        {
            var newIf = currentIf.ReplaceNode(currentIf.Condition, updatedCondition);
            newIf = originalIf.IsParentKind(SyntaxKind.ElseClause)
                ? newIf.ReplaceNode(newIf.Condition, newIf.Condition.WithTrailingTrivia(trivia))
                : newIf.WithPrependedLeadingTrivia(trivia);

            return newIf.WithAdditionalAnnotations(Formatter.Annotation);
        }

        private class MyCodeAction : CodeAction.DocumentChangeAction
        {
            public MyCodeAction(Func<CancellationToken, Task<Document>> createChangedDocument)
                : base(FeaturesResources.Use_pattern_matching, createChangedDocument)
            {
            }
        }
    }
}
