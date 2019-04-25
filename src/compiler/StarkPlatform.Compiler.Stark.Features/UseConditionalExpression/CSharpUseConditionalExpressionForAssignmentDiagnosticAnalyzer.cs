// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.LanguageServices;
using StarkPlatform.Compiler.UseConditionalExpression;

namespace StarkPlatform.Compiler.Stark.UseConditionalExpression
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
