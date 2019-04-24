// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.UseCompoundAssignment;

namespace StarkPlatform.CodeAnalysis.Stark.UseCompoundAssignment
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpUseCompoundAssignmentDiagnosticAnalyzer
        : AbstractUseCompoundAssignmentDiagnosticAnalyzer<SyntaxKind, AssignmentExpressionSyntax, BinaryExpressionSyntax>
    {
        public CSharpUseCompoundAssignmentDiagnosticAnalyzer()
            : base(CSharpSyntaxFactsService.Instance, Utilities.Kinds)
        {
        }

        protected override SyntaxKind GetKind(int rawKind)
            => (SyntaxKind)rawKind;

        protected override SyntaxKind GetAnalysisKind()
            => SyntaxKind.SimpleAssignmentExpression;

        protected override bool IsSupported(SyntaxKind assignmentKind, ParseOptions options)
            => assignmentKind != SyntaxKind.CoalesceExpression ||
            ((CSharpParseOptions)options).LanguageVersion >= LanguageVersion.CSharp8;
    }
}
