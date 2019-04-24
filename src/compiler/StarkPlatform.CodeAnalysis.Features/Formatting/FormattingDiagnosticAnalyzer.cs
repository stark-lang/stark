// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.CodeStyle;
using StarkPlatform.CodeAnalysis.Diagnostics;

namespace StarkPlatform.CodeAnalysis.Formatting
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class FormattingDiagnosticAnalyzer
        : AbstractBuiltInCodeStyleDiagnosticAnalyzer
    {
        public FormattingDiagnosticAnalyzer()
            : base(
                IDEDiagnosticIds.FormattingDiagnosticId,
                new LocalizableResourceString(nameof(FeaturesResources.Fix_formatting), FeaturesResources.ResourceManager, typeof(FeaturesResources)),
                new LocalizableResourceString(nameof(FeaturesResources.Fix_formatting), FeaturesResources.ResourceManager, typeof(FeaturesResources)))
        {
        }

        public override DiagnosticAnalyzerCategory GetAnalyzerCategory()
            => DiagnosticAnalyzerCategory.SyntaxTreeWithoutSemanticsAnalysis;

        public override bool OpenFileOnly(Workspace workspace)
            => false;

        protected override void InitializeWorker(AnalysisContext context)
            => context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);

        private void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            if (!(context.Options is WorkspaceAnalyzerOptions workspaceAnalyzerOptions))
            {
                return;
            }

            var tree = context.Tree;
            var cancellationToken = context.CancellationToken;

            var options = context.Options.GetDocumentOptionSetAsync(tree, cancellationToken).GetAwaiter().GetResult();
            if (options == null)
            {
                return;
            }

            var workspace = workspaceAnalyzerOptions.Services.Workspace;
            FormattingAnalyzerHelper.AnalyzeSyntaxTree(context, workspace, Descriptor, options);
        }
    }
}
