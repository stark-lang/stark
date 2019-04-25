// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.CodeFixes;
using StarkPlatform.Compiler.Stark.Diagnostics.SimplifyTypeNames;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.Formatting;
using StarkPlatform.Compiler.Simplification;
using StarkPlatform.Compiler.SimplifyTypeNames;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark.SimplifyTypeNames
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.SimplifyNames), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.RemoveUnnecessaryCast)]
    internal partial class SimplifyTypeNamesCodeFixProvider : AbstractSimplifyTypeNamesCodeFixProvider<SyntaxKind>
    {
        public SimplifyTypeNamesCodeFixProvider()
            : base(new CSharpSimplifyTypeNamesDiagnosticAnalyzer())
        {
        }

        protected override string GetTitle(string diagnosticId, string nodeText)
        {
            switch (diagnosticId)
            {
                case IDEDiagnosticIds.SimplifyNamesDiagnosticId:
                case IDEDiagnosticIds.PreferBuiltInOrFrameworkTypeDiagnosticId:
                    return string.Format(CSharpFeaturesResources.Simplify_name_0, nodeText);

                case IDEDiagnosticIds.SimplifyMemberAccessDiagnosticId:
                    return string.Format(CSharpFeaturesResources.Simplify_member_access_0, nodeText);

                case IDEDiagnosticIds.RemoveQualificationDiagnosticId:
                    return CSharpFeaturesResources.Remove_this_qualification;

                default:
                    throw ExceptionUtilities.UnexpectedValue(diagnosticId);
            }
        }

        protected override SyntaxNode AddSimplificationAnnotationTo(SyntaxNode expressionSyntax)
        {
            var annotatedexpressionSyntax = expressionSyntax.WithAdditionalAnnotations(Simplifier.Annotation, Formatter.Annotation);

            if (annotatedexpressionSyntax.Kind() == SyntaxKind.IsExpression || annotatedexpressionSyntax.Kind() == SyntaxKind.AsOptExpression)
            {
                var right = ((BinaryExpressionSyntax)annotatedexpressionSyntax).Right;
                annotatedexpressionSyntax = annotatedexpressionSyntax.ReplaceNode(right, right.WithAdditionalAnnotations(Simplifier.Annotation));
            }

            return annotatedexpressionSyntax;
        }
    }
}
