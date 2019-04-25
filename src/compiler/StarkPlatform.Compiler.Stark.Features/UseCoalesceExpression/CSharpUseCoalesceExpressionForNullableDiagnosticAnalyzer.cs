// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.LanguageServices;
using StarkPlatform.Compiler.UseCoalesceExpression;

namespace StarkPlatform.Compiler.Stark.UseCoalesceExpression
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpUseCoalesceExpressionForNullableDiagnosticAnalyzer :
        AbstractUseCoalesceExpressionForNullableDiagnosticAnalyzer<
            SyntaxKind,
            ExpressionSyntax,
            IfExpressionSyntax,
            BinaryExpressionSyntax,
            MemberAccessExpressionSyntax,
            PrefixUnaryExpressionSyntax>
    {
        protected override ISyntaxFactsService GetSyntaxFactsService()
            => CSharpSyntaxFactsService.Instance;

        protected override SyntaxKind GetSyntaxKindToAnalyze()
            => SyntaxKind.IfExpression;
    }
}
