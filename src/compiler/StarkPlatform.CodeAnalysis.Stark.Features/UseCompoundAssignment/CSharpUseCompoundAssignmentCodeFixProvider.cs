// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.UseCompoundAssignment;

namespace StarkPlatform.CodeAnalysis.Stark.UseCompoundAssignment
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal class CSharpUseCompoundAssignmentCodeFixProvider
        : AbstractUseCompoundAssignmentCodeFixProvider<SyntaxKind, AssignmentExpressionSyntax, ExpressionSyntax>
    {
        public CSharpUseCompoundAssignmentCodeFixProvider()
            : base(Utilities.Kinds)
        {
        }

        protected override SyntaxKind GetSyntaxKind(int rawKind)
            => (SyntaxKind)rawKind;

        protected override SyntaxToken Token(SyntaxKind kind)
            => SyntaxFactory.Token(kind);

        protected override AssignmentExpressionSyntax Assignment(
            SyntaxKind assignmentOpKind, ExpressionSyntax left, SyntaxToken syntaxToken, ExpressionSyntax right)
        {
            return SyntaxFactory.AssignmentExpression(assignmentOpKind, left, syntaxToken, right);
        }
    }
}
