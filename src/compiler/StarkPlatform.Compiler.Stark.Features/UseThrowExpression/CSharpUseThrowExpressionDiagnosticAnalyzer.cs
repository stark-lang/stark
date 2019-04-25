// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.LanguageServices;
using StarkPlatform.Compiler.UseThrowExpression;

namespace StarkPlatform.Compiler.Stark.UseThrowExpression
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpUseThrowExpressionDiagnosticAnalyzer : AbstractUseThrowExpressionDiagnosticAnalyzer
    {
        protected override bool IsSupported(ParseOptions options)
        {
            var csOptions = (CSharpParseOptions)options;
            return csOptions.LanguageVersion >= LanguageVersion.CSharp7;
        }

        protected override ISyntaxFactsService GetSyntaxFactsService()
            => CSharpSyntaxFactsService.Instance;

        protected override ISemanticFactsService GetSemanticFactsService()
            => CSharpSemanticFactsService.Instance;
    }
}
