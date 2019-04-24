// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeActions;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.RemoveUnusedLocalFunction
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.RemoveUnusedLocalFunction), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.AddImport)]
    internal class CSharpRemoveUnusedLocalFunctionCodeFixProvider : SyntaxEditorBasedCodeFixProvider
    {
        private const string CS8321 = nameof(CS8321); // The local function 'X' is declared but never used

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(CS8321);

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            context.RegisterCodeFix(
                new MyCodeAction(c => FixAsync(context.Document, context.Diagnostics.First(), c)),
                context.Diagnostics);

            return Task.CompletedTask;
        }

        protected override Task FixAllAsync(Document document, ImmutableArray<Diagnostic> diagnostics, SyntaxEditor editor, CancellationToken cancellationToken)
        {
            var root = editor.OriginalRoot;

            // Order diagnostics in reverse (from latest in file to earliest) so that we process
            // all inner local functions before processing outer local functions.  If we don't
            // do this, then SyntaxEditor will fail if it tries to remove an inner local function
            // after already removing the outer one.
            var localFunctions = diagnostics.OrderBy((d1, d2) => d2.Location.SourceSpan.Start - d1.Location.SourceSpan.Start)
                                            .Select(d => root.FindToken(d.Location.SourceSpan.Start))
                                            .Select(t => t.GetAncestor<LocalFunctionStatementSyntax>());

            foreach (var localFunction in localFunctions)
            {
                editor.RemoveNode(localFunction);
            }

            return Task.CompletedTask;
        }

        private class MyCodeAction : CodeAction.DocumentChangeAction
        {
            public MyCodeAction(Func<CancellationToken, Task<Document>> createChangedDocument) :
                base(CSharpFeaturesResources.Remove_unused_function, createChangedDocument, CSharpFeaturesResources.Remove_unused_function)
            {
            }
        }
    }
}
