// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Stark.UseObjectInitializer;
using StarkPlatform.CodeAnalysis.UseCollectionInitializer;

namespace StarkPlatform.CodeAnalysis.Stark.UseCollectionInitializer
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.UseCollectionInitializer), Shared]
    internal class CSharpUseCollectionInitializerCodeFixProvider :
        AbstractUseCollectionInitializerCodeFixProvider<
            SyntaxKind,
            ExpressionSyntax,
            StatementSyntax,
            ObjectCreationExpressionSyntax,
            MemberAccessExpressionSyntax,
            InvocationExpressionSyntax,
            ExpressionStatementSyntax,
            VariableDeclarationSyntax>
    {
        protected override StatementSyntax GetNewStatement(
            StatementSyntax statement,
            ObjectCreationExpressionSyntax objectCreation,
            ImmutableArray<ExpressionStatementSyntax> matches)
        {
            return statement.ReplaceNode(
                objectCreation,
                GetNewObjectCreation(objectCreation, matches));
        }

        private ObjectCreationExpressionSyntax GetNewObjectCreation(
            ObjectCreationExpressionSyntax objectCreation,
            ImmutableArray<ExpressionStatementSyntax> matches)
        {
            return UseInitializerHelpers.GetNewObjectCreation(
                objectCreation, CreateExpressions(matches));
        }

        private SeparatedSyntaxList<ExpressionSyntax> CreateExpressions(
            ImmutableArray<ExpressionStatementSyntax> matches)
        {
            var nodesAndTokens = new List<SyntaxNodeOrToken>();
            for (int i = 0; i < matches.Length; i++)
            {
                var expressionStatement = matches[i];

                var newExpression = ConvertExpression(expressionStatement.Expression)
                    .WithoutTrivia()
                    .WithLeadingTrivia(expressionStatement.GetLeadingTrivia());

                if (i < matches.Length - 1)
                {
                    nodesAndTokens.Add(newExpression);
                    var commaToken = SyntaxFactory.Token(SyntaxKind.CommaToken)
                        .WithTriviaFrom(expressionStatement.EosToken);

                    nodesAndTokens.Add(commaToken);
                }
                else
                {
                    newExpression = newExpression.WithTrailingTrivia(
                        expressionStatement.GetTrailingTrivia());
                    nodesAndTokens.Add(newExpression);
                }
            }

            return SyntaxFactory.SeparatedList<ExpressionSyntax>(nodesAndTokens);
        }

        private static ExpressionSyntax ConvertExpression(ExpressionSyntax expression)
        {
            if (expression is InvocationExpressionSyntax invocation)
            {
                return ConvertInvocation(invocation);
            }
            else if (expression is AssignmentExpressionSyntax assignment)
            {
                return ConvertAssignment(assignment);
            }

            throw new InvalidOperationException();
        }

        private static ExpressionSyntax ConvertAssignment(AssignmentExpressionSyntax assignment)
        {
            var elementAccess = (ElementAccessExpressionSyntax)assignment.Left;
            return assignment.WithLeft(
                SyntaxFactory.ImplicitElementAccess(elementAccess.ArgumentList));
        }

        private static ExpressionSyntax ConvertInvocation(InvocationExpressionSyntax invocation)
        {
            var arguments = invocation.ArgumentList.Arguments;

            if (arguments.Count == 1)
            {
                // Assignment expressions in a collection initializer will cause the compiler to 
                // report an error.  This is because { a = b } is teh form for an object initializer,
                // and the two forms are not allowed to mix/match.  Parenthesize the assignment to
                // avoid the ambiguity.
                var expression = arguments[0].Expression;
                return SyntaxFacts.IsAssignmentExpression(expression.Kind())
                    ? SyntaxFactory.ParenthesizedExpression(expression)
                    : expression;
            }

            return SyntaxFactory.InitializerExpression(
                SyntaxKind.ComplexElementInitializerExpression,
                SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithoutTrivia(),
                SyntaxFactory.SeparatedList(
                    arguments.Select(a => a.Expression),
                    arguments.GetSeparators()),
                SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithoutTrivia());
        }
    }
}
