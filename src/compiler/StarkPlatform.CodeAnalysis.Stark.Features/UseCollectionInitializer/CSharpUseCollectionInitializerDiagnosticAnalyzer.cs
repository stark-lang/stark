// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.UseCollectionInitializer;

namespace StarkPlatform.CodeAnalysis.Stark.UseCollectionInitializer
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpUseCollectionInitializerDiagnosticAnalyzer :
        AbstractUseCollectionInitializerDiagnosticAnalyzer<
            SyntaxKind,
            ExpressionSyntax,
            StatementSyntax,
            ObjectCreationExpressionSyntax,
            MemberAccessExpressionSyntax,
            InvocationExpressionSyntax,
            ExpressionStatementSyntax,
            VariableDeclarationSyntax>
    {
        protected override bool AreCollectionInitializersSupported(SyntaxNodeAnalysisContext context)
            => ((CSharpParseOptions)context.Node.SyntaxTree.Options).LanguageVersion >= LanguageVersion.CSharp3;

        protected override SyntaxKind GetObjectCreationSyntaxKind() => SyntaxKind.ObjectCreationExpression;

        protected override ISyntaxFactsService GetSyntaxFactsService() => CSharpSyntaxFactsService.Instance;
    }
}
