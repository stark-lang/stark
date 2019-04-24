// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Diagnostics.Analyzers.NamingStyles;

namespace StarkPlatform.CodeAnalysis.Stark.Diagnostics.NamingStyles
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal sealed class CSharpNamingStyleDiagnosticAnalyzer : NamingStyleDiagnosticAnalyzerBase<SyntaxKind>
    {
        protected override ImmutableArray<SyntaxKind> SupportedSyntaxKinds { get; } =
            ImmutableArray.Create(
                SyntaxKind.VariableDeclaration,
                SyntaxKind.CatchDeclaration,
                SyntaxKind.SingleVariableDesignation,
                SyntaxKind.LocalFunctionStatement,
                SyntaxKind.Parameter,
                SyntaxKind.TypeParameter);
    }
}
