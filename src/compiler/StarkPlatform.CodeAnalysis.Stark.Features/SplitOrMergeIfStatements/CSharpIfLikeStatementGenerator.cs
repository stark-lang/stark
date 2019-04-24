// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.SplitOrMergeIfStatements;

namespace StarkPlatform.CodeAnalysis.Stark.SplitOrMergeIfStatements
{
    [ExportLanguageService(typeof(IIfLikeStatementGenerator), LanguageNames.Stark), Shared]
    internal sealed class CSharpIfLikeStatementGenerator : IIfLikeStatementGenerator
    {
        public bool IsIfOrElseIf(SyntaxNode node) => node is IfStatementSyntax;

        public bool IsCondition(SyntaxNode expression, out SyntaxNode ifOrElseIf)
        {
            if (expression.Parent is IfStatementSyntax ifStatement && ifStatement.Condition == expression)
            {
                ifOrElseIf = ifStatement;
                return true;
            }

            ifOrElseIf = null;
            return false;
        }

        public bool IsElseIfClause(SyntaxNode node, out SyntaxNode parentIfOrElseIf)
        {
            if (node is IfStatementSyntax && node.Parent is ElseClauseSyntax)
            {
                parentIfOrElseIf = (IfStatementSyntax)node.Parent.Parent;
                return true;
            }

            parentIfOrElseIf = null;
            return false;
        }

        public bool HasElseIfClause(SyntaxNode ifOrElseIf, out SyntaxNode elseIfClause)
        {
            var ifStatement = (IfStatementSyntax)ifOrElseIf;
            if (ifStatement.Else?.Statement is IfStatementSyntax elseIfStatement)
            {
                elseIfClause = elseIfStatement;
                return true;
            }

            elseIfClause = null;
            return false;
        }

        public SyntaxNode GetCondition(SyntaxNode ifOrElseIf)
        {
            var ifStatement = (IfStatementSyntax)ifOrElseIf;
            return ifStatement.Condition;
        }

        public SyntaxNode GetRootIfStatement(SyntaxNode ifOrElseIf)
        {
            var ifStatement = (IfStatementSyntax)ifOrElseIf;

            while (ifStatement.Parent is ElseClauseSyntax elseClause)
            {
                ifStatement = (IfStatementSyntax)elseClause.Parent;
            }

            return ifStatement;
        }

        public ImmutableArray<SyntaxNode> GetElseIfAndElseClauses(SyntaxNode ifOrElseIf)
        {
            var ifStatement = (IfStatementSyntax)ifOrElseIf;

            var builder = ImmutableArray.CreateBuilder<SyntaxNode>();

            while (ifStatement.Else?.Statement is IfStatementSyntax elseIfStatement)
            {
                builder.Add(elseIfStatement);
                ifStatement = elseIfStatement;
            }

            if (ifStatement.Else != null)
            {
                builder.Add(ifStatement.Else);
            }

            return builder.ToImmutable();
        }

        public SyntaxNode WithCondition(SyntaxNode ifOrElseIf, SyntaxNode condition)
        {
            var ifStatement = (IfStatementSyntax)ifOrElseIf;
            return ifStatement.WithCondition((ExpressionSyntax)condition);
        }

        public SyntaxNode WithStatementInBlock(SyntaxNode ifOrElseIf, SyntaxNode statement)
        {
            var ifStatement = (IfStatementSyntax)ifOrElseIf;
            return ifStatement.WithStatement(SyntaxFactory.Block((StatementSyntax)statement));
        }

        public SyntaxNode WithStatementsOf(SyntaxNode ifOrElseIf, SyntaxNode otherIfOrElseIf)
        {
            var ifStatement = (IfStatementSyntax)ifOrElseIf;
            var otherIfStatement = (IfStatementSyntax)otherIfOrElseIf;
            return ifStatement.WithStatement(otherIfStatement.Statement);
        }

        public SyntaxNode WithElseIfAndElseClausesOf(SyntaxNode ifStatement, SyntaxNode otherIfStatement)
        {
            return ((IfStatementSyntax)ifStatement).WithElse(((IfStatementSyntax)otherIfStatement).Else);
        }

        public SyntaxNode ToIfStatement(SyntaxNode ifOrElseIf)
            => ifOrElseIf;

        public SyntaxNode ToElseIfClause(SyntaxNode ifOrElseIf)
            => ((IfStatementSyntax)ifOrElseIf).WithElse(null);

        public void InsertElseIfClause(SyntaxEditor editor, SyntaxNode afterIfOrElseIf, SyntaxNode elseIfClause)
        {
            editor.ReplaceNode(afterIfOrElseIf, (currentNode, _) =>
            {
                var ifStatement = (IfStatementSyntax)currentNode;
                var elseIfStatement = (IfStatementSyntax)elseIfClause;

                var newElseIfStatement = elseIfStatement.WithElse(ifStatement.Else);
                var newIfStatement = ifStatement.WithElse(SyntaxFactory.ElseClause(newElseIfStatement));

                return newIfStatement;
            });
        }

        public void RemoveElseIfClause(SyntaxEditor editor, SyntaxNode elseIfClause)
        {
            editor.ReplaceNode(elseIfClause.Parent.Parent, (currentNode, _) =>
            {
                var parentIfStatement = (IfStatementSyntax)currentNode;
                var elseClause = parentIfStatement.Else;
                var elseIfStatement = (IfStatementSyntax)elseClause.Statement;
                return parentIfStatement.WithElse(elseIfStatement.Else);
            });
        }
    }
}
