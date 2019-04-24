// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.CodeStyle;
using StarkPlatform.CodeAnalysis.Stark.CodeStyle;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Operations;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.RemoveUnusedParametersAndValues;

namespace StarkPlatform.CodeAnalysis.Stark.RemoveUnusedParametersAndValues
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpRemoveUnusedParametersAndValuesDiagnosticAnalyzer : AbstractRemoveUnusedParametersAndValuesDiagnosticAnalyzer
    {
        protected override Option<CodeStyleOption<UnusedValuePreference>> UnusedValueExpressionStatementOption
            => CSharpCodeStyleOptions.UnusedValueExpressionStatement;

        protected override Option<CodeStyleOption<UnusedValuePreference>> UnusedValueAssignmentOption
            => CSharpCodeStyleOptions.UnusedValueAssignment;

        protected override bool SupportsDiscard(SyntaxTree tree)
            => ((CSharpParseOptions)tree.Options).LanguageVersion >= LanguageVersion.CSharp7;

        protected override bool MethodHasHandlesClause(IMethodSymbol method)
            => false;

        protected override bool IsIfConditionalDirective(SyntaxNode node)
            => node is IfDirectiveTriviaSyntax;

        // C# does not have an explicit "call" statement syntax for invocations with explicit value discard.
        protected override bool IsCallStatement(IExpressionStatementOperation expressionStatement)
            => false;

        protected override Location GetDefinitionLocationToFade(IOperation unusedDefinition)
        {
            switch (unusedDefinition.Syntax)
            {
                case VariableDeclarationSyntax variableDeclarator:
                    return variableDeclarator.Identifier.GetLocation();

                case DeclarationPatternSyntax declarationPattern:
                    return declarationPattern.Designation.GetLocation();

                default:
                    // C# syntax node for foreach statement has no syntax node for the loop control variable declaration,
                    // so the operation tree has an IVariableDeclarationOperation with the syntax mapped to the type node syntax instead of variable declarator syntax.
                    // Check if the unused definition syntax is the foreach statement's type node.
                    if (unusedDefinition.Syntax.Parent is ForStatementSyntax forEachStatement &&
                        forEachStatement.Variable == unusedDefinition.Syntax)
                    {
                        return forEachStatement.Variable.GetLocation();
                    }

                    return unusedDefinition.Syntax.GetLocation();
            }
        }
    }
}
