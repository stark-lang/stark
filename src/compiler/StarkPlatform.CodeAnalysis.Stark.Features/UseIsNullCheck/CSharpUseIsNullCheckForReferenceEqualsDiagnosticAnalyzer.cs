// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.UseIsNullCheck;

namespace StarkPlatform.CodeAnalysis.Stark.UseIsNullCheck
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpUseIsNullCheckForReferenceEqualsDiagnosticAnalyzer : AbstractUseIsNullCheckForReferenceEqualsDiagnosticAnalyzer<SyntaxKind>
    {
        public CSharpUseIsNullCheckForReferenceEqualsDiagnosticAnalyzer()
            : base(CSharpFeaturesResources.Use_is_null_check)
        {
        }

        protected override bool IsLanguageVersionSupported(ParseOptions options)
            => ((CSharpParseOptions)options).LanguageVersion >= LanguageVersion.CSharp7;

        protected override SyntaxKind GetInvocationExpressionKind()
            => SyntaxKind.InvocationExpression;

        protected override ISyntaxFactsService GetSyntaxFactsService()
            => CSharpSyntaxFactsService.Instance;
    }
}
