// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Formatting;
using StarkPlatform.CodeAnalysis.Formatting.Rules;
using StarkPlatform.CodeAnalysis.Operations;
using StarkPlatform.CodeAnalysis.UseConditionalExpression;

namespace StarkPlatform.CodeAnalysis.Stark.UseConditionalExpression
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal partial class CSharpUseConditionalExpressionForReturnCodeRefactoringProvider
        : AbstractUseConditionalExpressionForReturnCodeFixProvider<StatementSyntax, IfStatementSyntax, ExpressionSyntax, IfExpressionSyntax>
    {
        protected override bool IsRef(IReturnOperation returnOperation)
            => returnOperation.Syntax is ReturnStatementSyntax statement &&
               statement.Expression is RefExpressionSyntax;

        protected override IFormattingRule GetMultiLineFormattingRule()
            => MultiLineConditionalExpressionFormattingRule.Instance;

        protected override StatementSyntax WrapWithBlockIfAppropriate(
            IfStatementSyntax ifStatement, StatementSyntax statement)
        {
            if (ifStatement.Parent is ElseClauseSyntax &&
                ifStatement.Statement is BlockSyntax block)
            {
                return block.WithStatements(SyntaxFactory.SingletonList(statement))
                            .WithAdditionalAnnotations(Formatter.Annotation);
            }

            return statement;
        }
    }
}
