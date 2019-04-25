// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.ConvertAnonymousTypeToTuple;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Diagnostics;

namespace StarkPlatform.Compiler.Stark.ConvertAnonymousTypeToTuple
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpConvertAnonymousTypeToTupleDiagnosticAnalyzer
        : AbstractConvertAnonymousTypeToTupleDiagnosticAnalyzer<
            SyntaxKind,
            AnonymousObjectCreationExpressionSyntax>
    {
        protected override SyntaxKind GetAnonymousObjectCreationExpressionSyntaxKind()
            => SyntaxKind.AnonymousObjectCreationExpression;

        protected override int GetInitializerCount(AnonymousObjectCreationExpressionSyntax anonymousType)
            => anonymousType.Initializers.Count;
    }
}
