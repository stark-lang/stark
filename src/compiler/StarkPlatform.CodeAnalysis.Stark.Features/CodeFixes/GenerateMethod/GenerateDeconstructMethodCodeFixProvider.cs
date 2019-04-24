// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.GenerateMember.GenerateParameterizedMember;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.CodeFixes.GenerateDeconstructMethod
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.GenerateDeconstructMethod), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.GenerateEnumMember)]
#pragma warning disable RS1016 // Code fix providers should provide FixAll support. https://github.com/dotnet/roslyn/issues/23528
    internal class GenerateDeconstructMethodCodeFixProvider : CodeFixProvider
#pragma warning restore RS1016 // Code fix providers should provide FixAll support.
    {
        private const string CS8129 = nameof(CS8129); // No suitable Deconstruct instance or extension method was found...

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CS8129);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            // Not supported in REPL
            if (context.Project.IsSubmission)
            {
                return;
            }

            var document = context.Document;
            var cancellationToken = context.CancellationToken;
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var span = context.Span;
            var token = root.FindToken(span.Start);

            var deconstruction = token.GetAncestors<SyntaxNode>()
                .FirstOrDefault(n => n.IsKind(SyntaxKind.SimpleAssignmentExpression, SyntaxKind.ForStatement));

            if (deconstruction is null)
            {
                Debug.Fail("The diagnostic can only be produced in context of a deconstruction-assignment or deconstruction-foreach");
                return;
            }

            var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            DeconstructionInfo info;
            ITypeSymbol type;
            ExpressionSyntax target;
            switch (deconstruction)
            {
                case ForStatementSyntax @foreach:
                    info = model.GetDeconstructionInfo(@foreach);
                    type = model.GetForEachStatementInfo(@foreach).ElementType;
                    target = @foreach.Variable;
                    break;
                case AssignmentExpressionSyntax assignment:
                    info = model.GetDeconstructionInfo(assignment);
                    type = model.GetTypeInfo(assignment.Right).Type;
                    target = assignment.Left;
                    break;
                default:
                    throw ExceptionUtilities.Unreachable;
            }

            if (type?.Kind != SymbolKind.NamedType)
            {
                return;
            }

            if (info.Method != null || !info.Nested.IsEmpty)
            {
                // There is already a Deconstruct method, or we have a nesting situation
                return;
            }

            var service = document.GetLanguageService<IGenerateDeconstructMemberService>();
            var codeActions = await service.GenerateDeconstructMethodAsync(document, target, (INamedTypeSymbol)type, cancellationToken).ConfigureAwait(false);

            Debug.Assert(!codeActions.IsDefault);
            context.RegisterFixes(codeActions, context.Diagnostics);
        }
    }
}
