// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.CodeFixes;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.RemoveUnnecessaryParentheses;

namespace StarkPlatform.Compiler.Stark.RemoveUnnecessaryParentheses
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal class CSharpRemoveUnnecessaryParenthesesCodeFixProvider :
        AbstractRemoveUnnecessaryParenthesesCodeFixProvider<ParenthesizedExpressionSyntax>
    {
        protected override bool CanRemoveParentheses(ParenthesizedExpressionSyntax current, SemanticModel semanticModel)
        {
            return CSharpRemoveUnnecessaryParenthesesDiagnosticAnalyzer.CanRemoveParenthesesHelper(
                current, semanticModel, out _, out _);
        }
    }
}
