// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeActions;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.UseExplicitTupleName
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal partial class UseExplicitTupleNameCodeFixProvider : SyntaxEditorBasedCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(IDEDiagnosticIds.UseExplicitTupleNameDiagnosticId);

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            context.RegisterCodeFix(new MyCodeAction(
                c => FixAsync(context.Document, context.Diagnostics[0], c)),
                context.Diagnostics);
            return Task.CompletedTask;
        }

        protected override Task FixAllAsync(
            Document document, ImmutableArray<Diagnostic> diagnostics,
            SyntaxEditor editor, CancellationToken cancellationToken)
        {
            var root = editor.OriginalRoot;
            var generator = editor.Generator;

            foreach (var diagnostic in diagnostics)
            {
                var oldNameNode = diagnostic.Location.FindNode(
                    getInnermostNodeForTie: true, cancellationToken: cancellationToken);

                var preferredName = diagnostic.Properties[nameof(UseExplicitTupleNameDiagnosticAnalyzer.ElementName)];
                var newNameNode = generator.IdentifierName(preferredName).WithTriviaFrom(oldNameNode);

                editor.ReplaceNode(oldNameNode, newNameNode);
            }

            return Task.CompletedTask;
        }

        private class MyCodeAction : CodeAction.DocumentChangeAction
        {
            public MyCodeAction(Func<CancellationToken, Task<Document>> createChangedDocument)
                : base(FeaturesResources.Use_explicitly_provided_tuple_name,
                       createChangedDocument,
                       FeaturesResources.Use_explicitly_provided_tuple_name)
            {
            }
        }
    }
}
