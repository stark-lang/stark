// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.CodeRefactorings;
using StarkPlatform.Compiler.CodeRefactorings.PullMemberUp;
using StarkPlatform.Compiler.CodeRefactorings.PullMemberUp.Dialog;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.CodeRefactorings.PullMemberUp
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = nameof(PredefinedCodeRefactoringProviderNames.PullMemberUp)), Shared]
    internal class CSharpPullMemberUpCodeRefactoringProvider : AbstractPullMemberUpRefactoringProvider
    {
        /// <summary>
        /// Test purpose only.
        /// </summary>
        public CSharpPullMemberUpCodeRefactoringProvider(IPullMemberUpOptionsService service) : base(service)
        {
        }

        [ImportingConstructor]
        public CSharpPullMemberUpCodeRefactoringProvider() : base(null)
        {
        }

        protected override bool IsSelectionValid(TextSpan span, SyntaxNode selectedNode)
        {
            var identifier = GetIdentifier(selectedNode);
            if (identifier == default)
            {
                return false;
            }
            else if (identifier.FullSpan.Contains(span) && span.Contains(identifier.Span))
            {
                // Selection lies within the identifier's span
                return true;
            }
            else if (identifier.Span.Contains(span) && span.IsEmpty)
            {
                // Cursor stands on the identifier
                return true;
            }
            else
            {
                return false;
            }
        }

        private SyntaxToken GetIdentifier(SyntaxNode selectedNode)
        {
            switch (selectedNode)
            {
                case MemberDeclarationSyntax memberDeclarationSyntax:
                    // Nested type is checked in before this method is called.
                    return memberDeclarationSyntax.GetNameToken();
                case VariableDeclarationSyntax variableDeclaratorSyntax:
                    // It handles multiple fields or events declared in one line
                    return variableDeclaratorSyntax.Identifier;
                default:
                    return default;
            }
        }
    }
}
