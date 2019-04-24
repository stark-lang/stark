// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeActions;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.Formatting;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.MakeFieldReadonly
{
    internal abstract class AbstractMakeFieldReadonlyCodeFixProvider<TSymbolSyntax, TFieldDeclarationSyntax>
        : SyntaxEditorBasedCodeFixProvider
        where TSymbolSyntax : SyntaxNode
        where TFieldDeclarationSyntax : SyntaxNode
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(IDEDiagnosticIds.MakeFieldReadonlyDiagnosticId);

        protected abstract SyntaxNode GetInitializerNode(TSymbolSyntax declaration);
        protected abstract TSymbolSyntax GetVariableDeclarations(TFieldDeclarationSyntax declaration);

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            context.RegisterCodeFix(new MyCodeAction(
                c => FixAsync(context.Document, context.Diagnostics[0], c)),
                context.Diagnostics);
            return Task.CompletedTask;
        }

        protected override async Task FixAllAsync(
            Document document,
            ImmutableArray<Diagnostic> diagnostics,
            SyntaxEditor editor,
            CancellationToken cancellationToken)
        {
            var declarators = new List<TSymbolSyntax>();

            foreach (var diagnostic in diagnostics)
            {
                var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                var diagnosticSpan = diagnostic.Location.SourceSpan;

                declarators.Add(root.FindNode(diagnosticSpan, getInnermostNodeForTie: true).FirstAncestorOrSelf<TSymbolSyntax>());
            }

            await MakeFieldReadonlyAsync(document, editor, declarators).ConfigureAwait(false);
        }

        private async Task MakeFieldReadonlyAsync(Document document, SyntaxEditor editor, List<TSymbolSyntax> declarators)
        {
            var declaratorsByField = declarators.GroupBy(g => g.FirstAncestorOrSelf<TFieldDeclarationSyntax>());

            foreach (var fieldDeclarators in declaratorsByField)
            {
                //var declarationDeclarators = GetVariableDeclarations(fieldDeclarators.Key);
                // TODO: readonly is not workling like this now
                editor.SetModifiers(fieldDeclarators.Key, editor.Generator.GetModifiers(fieldDeclarators.Key) | DeclarationModifiers.ReadOnly);
            }
        }

        private class MyCodeAction : CodeAction.DocumentChangeAction
        {
            public MyCodeAction(Func<CancellationToken, Task<Document>> createChangedDocument) :
                base(FeaturesResources.Add_readonly_modifier, createChangedDocument)
            {
            }
        }
    }
}
