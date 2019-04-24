// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeActions;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.CodeFixes.GenerateMember;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.GenerateMember.GenerateEnumMember;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.CodeFixes.GenerateEnumMember
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.GenerateEnumMember), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.GenerateConstructor)]
    internal class GenerateEnumMemberCodeFixProvider : AbstractGenerateMemberCodeFixProvider
    {
        private const string CS0117 = nameof(CS0117); // error CS0117: 'Color' does not contain a definition for 'Red'

        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CS0117); }
        }

        protected override Task<ImmutableArray<CodeAction>> GetCodeActionsAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var service = document.GetLanguageService<IGenerateEnumMemberService>();
            return service.GenerateEnumMemberAsync(document, node, cancellationToken);
        }

        protected override bool IsCandidate(SyntaxNode node, SyntaxToken token, Diagnostic diagnostic)
        {
            return node is IdentifierNameSyntax;
        }
    }
}
