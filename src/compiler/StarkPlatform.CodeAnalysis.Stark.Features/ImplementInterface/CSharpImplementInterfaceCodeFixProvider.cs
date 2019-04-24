// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeActions;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.ImplementInterface;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.ImplementInterface
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.ImplementInterface), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.ImplementAbstractClass)]
    internal class CSharpImplementInterfaceCodeFixProvider : CodeFixProvider
    {
        private readonly Func<TypeSyntax, bool> _interfaceName = n => n.Parent is BaseTypeSyntax && n.Parent.Parent is ImplementListSyntax && ((BaseTypeSyntax)n.Parent).Type == n;
        private readonly Func<IEnumerable<CodeAction>, bool> _codeActionAvailable = actions => actions != null && actions.Any();

        private const string CS0535 = nameof(CS0535); // 'Program' does not implement interface member 'System.Collections.IEnumerable.GetEnumerator()'
        private const string CS0737 = nameof(CS0737); // 'Class' does not implement interface member 'IInterface.M()'. 'Class.M()' cannot implement an interface member because it is not public.
        private const string CS0738 = nameof(CS0738); // 'C' does not implement interface member 'I.Method1()'. 'B.Method1()' cannot implement 'I.Method1()' because it does not have the matching return type of 'void'.

        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(CS0535, CS0737, CS0738);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var span = context.Span;
            var cancellationToken = context.CancellationToken;

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var token = root.FindToken(span.Start);
            if (!token.Span.IntersectsWith(span))
            {
                return;
            }

            var service = document.GetLanguageService<IImplementInterfaceService>();
            var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var actions = token.Parent.GetAncestorsOrThis<TypeSyntax>()
                                      .Where(_interfaceName)
                                      .Select(n => service.GetCodeActions(document, model, n, cancellationToken))
                                      .FirstOrDefault(_codeActionAvailable);

            if (_codeActionAvailable(actions))
            {
                context.RegisterFixes(actions, context.Diagnostics);
            }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }
    }
}
