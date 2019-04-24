// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.ValidateFormatString;


namespace StarkPlatform.CodeAnalysis.Stark.ValidateFormatString
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpValidateFormatStringDiagnosticAnalyzer :
        AbstractValidateFormatStringDiagnosticAnalyzer<SyntaxKind>
    {
        protected override ISyntaxFactsService GetSyntaxFactsService()
            => CSharpSyntaxFactsService.Instance;

        protected override SyntaxKind GetInvocationExpressionSyntaxKind()
            => SyntaxKind.InvocationExpression;

        protected override SyntaxNode TryGetMatchingNamedArgument(
            SeparatedSyntaxList<SyntaxNode> arguments,
            string searchArgumentName)
        {
            foreach (var argument in arguments.Cast<ArgumentSyntax>())
            {
                if (argument.NameColon != null && argument.NameColon.Name.Identifier.ValueText.Equals(searchArgumentName))
                {
                    return argument;
                }
            }

            return null;
        }

        protected override SyntaxNode GetArgumentExpression(SyntaxNode syntaxNode)
            => ((ArgumentSyntax)syntaxNode).Expression;
    }
}