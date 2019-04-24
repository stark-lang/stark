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
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Editing;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Diagnostics.AddBraces
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.AddBraces), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.AddAwait)]
    internal sealed class CSharpAddBracesCodeFixProvider : SyntaxEditorBasedCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(IDEDiagnosticIds.AddBracesDiagnosticId);

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            context.RegisterCodeFix(
                new MyCodeAction(c => FixAsync(context.Document, context.Diagnostics.First(), c)),
                context.Diagnostics);

            return Task.CompletedTask;
        }

        protected override Task FixAllAsync(
            Document document, ImmutableArray<Diagnostic> diagnostics,
            SyntaxEditor editor, CancellationToken cancellationToken)
        {
            var root = editor.OriginalRoot;
            foreach (var diagnostic in diagnostics)
            {
                var statement = root.FindNode(diagnostic.Location.SourceSpan);

                // Use the callback version of ReplaceNode so that we see the effects
                // of other replace calls.  i.e. we may have statements nested in statements,
                // we need to make sure that any inner edits are seen when we make the outer
                // replacement.
                editor.ReplaceNode(statement, (currentStatement, g) =>
                {
                    var embeddedStatement = currentStatement.GetEmbeddedStatement();
                    return currentStatement.ReplaceNode(embeddedStatement, SyntaxFactory.Block(embeddedStatement));
                });
            }

            return Task.CompletedTask;
        }

        private sealed class MyCodeAction : CodeAction.DocumentChangeAction
        {
            public MyCodeAction(Func<CancellationToken, Task<Document>> createChangedDocument) :
                base(FeaturesResources.Add_braces, createChangedDocument, FeaturesResources.Add_braces)
            {
            }
        }
    }
}
