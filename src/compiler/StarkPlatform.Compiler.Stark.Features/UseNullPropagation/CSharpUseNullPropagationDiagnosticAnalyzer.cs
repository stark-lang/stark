// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.LanguageServices;
using StarkPlatform.Compiler.UseNullPropagation;

namespace StarkPlatform.Compiler.Stark.UseNullPropagation
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpUseNullPropagationDiagnosticAnalyzer :
        AbstractUseNullPropagationDiagnosticAnalyzer<
            SyntaxKind,
            ExpressionSyntax,
            IfExpressionSyntax,
            BinaryExpressionSyntax,
            InvocationExpressionSyntax,
            MemberAccessExpressionSyntax,
            ConditionalAccessExpressionSyntax,
            ElementAccessExpressionSyntax>
    {
        protected override bool ShouldAnalyze(ParseOptions options)
            => ((CSharpParseOptions)options).LanguageVersion >= LanguageVersion.CSharp6;

        protected override ISyntaxFactsService GetSyntaxFactsService()
            => CSharpSyntaxFactsService.Instance;

        protected override ISemanticFactsService GetSemanticFactsService()
            => CSharpSemanticFactsService.Instance;

        protected override SyntaxKind GetSyntaxKindToAnalyze()
            => SyntaxKind.IfExpression;

        protected override bool IsEquals(BinaryExpressionSyntax condition)
            => condition.Kind() == SyntaxKind.EqualsExpression;

        protected override bool IsNotEquals(BinaryExpressionSyntax condition)
            => condition.Kind() == SyntaxKind.NotEqualsExpression;

        protected override bool TryAnalyzePatternCondition(
            ISyntaxFactsService syntaxFacts, SyntaxNode conditionNode,
            out SyntaxNode conditionPartToCheck, out bool isEquals)
        {
            conditionPartToCheck = null;
            isEquals = true;

            var patternExpression = conditionNode as IsPatternExpressionSyntax;
            if (patternExpression == null)
            {
                return false;
            }

            var constantPattern = patternExpression.Pattern as ConstantPatternSyntax;
            if (constantPattern == null)
            {
                return false;
            }

            if (!syntaxFacts.IsNullLiteralExpression(constantPattern.Expression))
            {
                return false;
            }

            conditionPartToCheck = patternExpression.Expression;
            return true;
        }
    }
}
