// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.UseConditionalExpression;

namespace StarkPlatform.CodeAnalysis.Stark.UseConditionalExpression
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpUseConditionalExpressionForAssignmentDiagnosticAnalyzer
        : AbstractUseConditionalExpressionForAssignmentDiagnosticAnalyzer<IfStatementSyntax>
    {
        public CSharpUseConditionalExpressionForAssignmentDiagnosticAnalyzer()
            : base(new LocalizableResourceString(nameof(CSharpFeaturesResources.if_statement_can_be_simplified), CSharpFeaturesResources.ResourceManager, typeof(CSharpFeaturesResources)))
        {
        }

        protected override ISyntaxFactsService GetSyntaxFactsService()
            => CSharpSyntaxFactsService.Instance;
    }
}
