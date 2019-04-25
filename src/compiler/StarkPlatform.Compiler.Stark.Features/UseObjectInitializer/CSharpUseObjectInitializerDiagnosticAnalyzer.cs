// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.LanguageServices;
using StarkPlatform.Compiler.UseObjectInitializer;

namespace StarkPlatform.Compiler.Stark.UseObjectInitializer
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpUseObjectInitializerDiagnosticAnalyzer :
        AbstractUseObjectInitializerDiagnosticAnalyzer<
            SyntaxKind,
            ExpressionSyntax,
            StatementSyntax,
            ObjectCreationExpressionSyntax,
            MemberAccessExpressionSyntax,
            ExpressionStatementSyntax,
            VariableDeclarationSyntax>
    {
        protected override bool FadeOutOperatorToken => true;

        protected override bool AreObjectInitializersSupported(SyntaxNodeAnalysisContext context)
        {
            // object initializers are only available in C# 3.0 and above.  Don't offer this refactoring
            // in projects targeting a lesser version.
            return ((CSharpParseOptions)context.Node.SyntaxTree.Options).LanguageVersion >= LanguageVersion.CSharp3;
        }

        protected override SyntaxKind GetObjectCreationSyntaxKind() => SyntaxKind.ObjectCreationExpression;

        protected override ISyntaxFactsService GetSyntaxFactsService() => CSharpSyntaxFactsService.Instance;
    }
}
