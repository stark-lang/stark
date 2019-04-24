// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeActions;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.CodeFixes.GenerateMember;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.GenerateMember.GenerateParameterizedMember;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.CodeFixes.GenerateMethod
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.GenerateConversion), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.GenerateEnumMember)]
    internal class GenerateConversionCodeFixProvider : AbstractGenerateMemberCodeFixProvider
    {
        private const string CS0029 = nameof(CS0029); // error CS0029: Cannot implicitly convert type 'type' to 'type'
        private const string CS0030 = nameof(CS0030); // error CS0030: Cannot convert type 'type' to 'type'

        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CS0029, CS0030); }
        }

        protected override bool IsCandidate(SyntaxNode node, SyntaxToken token, Diagnostic diagnostic)
        {
            return node.IsKind(SyntaxKind.IdentifierName) ||
                   node.IsKind(SyntaxKind.MethodDeclaration) ||
                   node.IsKind(SyntaxKind.InvocationExpression) ||
                   node.IsKind(SyntaxKind.CastExpression) ||
                   node is LiteralExpressionSyntax ||
                   node is SimpleNameSyntax ||
                   node is ExpressionSyntax;
        }

        protected override SyntaxNode GetTargetNode(SyntaxNode node)
        {
            if (node is InvocationExpressionSyntax invocation)
            {
                return invocation.Expression.GetRightmostName();
            }

            if (node is MemberBindingExpressionSyntax memberBindingExpression)
            {
                return memberBindingExpression.Name;
            }

            return node;
        }

        protected override Task<ImmutableArray<CodeAction>> GetCodeActionsAsync(
            Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var service = document.GetLanguageService<IGenerateConversionService>();
            return service.GenerateConversionAsync(document, node, cancellationToken);
        }
    }
}
