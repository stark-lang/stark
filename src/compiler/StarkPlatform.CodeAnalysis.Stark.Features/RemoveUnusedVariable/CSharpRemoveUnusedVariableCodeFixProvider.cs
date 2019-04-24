// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.RemoveUnusedVariable;

namespace StarkPlatform.CodeAnalysis.Stark.RemoveUnusedVariable
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.RemoveUnusedVariable), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.AddImport)]
    internal partial class CSharpRemoveUnusedVariableCodeFixProvider : AbstractRemoveUnusedVariableCodeFixProvider<LocalDeclarationStatementSyntax, VariableDeclarationSyntax>
    {
        public const string CS0168 = nameof(CS0168);
        public const string CS0219 = nameof(CS0219);

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(CS0168, CS0219);

        protected override bool IsCatchDeclarationIdentifier(SyntaxToken token)
            => token.Parent is CatchDeclarationSyntax catchDeclaration && catchDeclaration.Identifier == token;

        protected override SyntaxNode GetNodeToRemoveOrReplace(SyntaxNode node)
        {
            node = node.Parent;
            if (node.Kind() == SyntaxKind.SimpleAssignmentExpression)
            {
                var parent = node.Parent;
                if (parent.Kind() == SyntaxKind.ExpressionStatement)
                {
                    return parent;
                }
                else
                {
                    return node;
                }
            }

            return null;
        }

        protected override void RemoveOrReplaceNode(SyntaxEditor editor, SyntaxNode node, ISyntaxFactsService syntaxFacts)
        {
            switch (node.Kind())
            {
                case SyntaxKind.SimpleAssignmentExpression:
                    editor.ReplaceNode(node, ((AssignmentExpressionSyntax)node).Right);
                    return;
                default:
                    RemoveNode(editor, node, syntaxFacts);
                    return;
            }
        }

        protected override SyntaxNode GetVariables(LocalDeclarationStatementSyntax localDeclarationStatement)
            => localDeclarationStatement.Declaration;
    }
}
