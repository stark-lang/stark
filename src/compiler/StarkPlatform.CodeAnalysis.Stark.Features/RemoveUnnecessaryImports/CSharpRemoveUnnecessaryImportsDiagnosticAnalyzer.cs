// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.RemoveUnnecessaryImports;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.RemoveUnnecessaryImports;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.RemoveUnnecessaryImports
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal sealed class CSharpRemoveUnnecessaryImportsDiagnosticAnalyzer :
        AbstractRemoveUnnecessaryImportsDiagnosticAnalyzer
    {
        private static readonly LocalizableString s_TitleAndMessageFormat =
            new LocalizableResourceString(nameof(CSharpFeaturesResources.Using_directive_is_unnecessary), CSharpFeaturesResources.ResourceManager, typeof(CSharpFeaturesResources));

        protected override LocalizableString GetTitleAndMessageFormatForClassificationIdDescriptor()
            => s_TitleAndMessageFormat;

        // C# has no need to do any merging of using statements.  Only VB needs to
        // merge import clauses to an import statement if it all the import clauses
        // are unnecessary.
        protected override ImmutableArray<SyntaxNode> MergeImports(ImmutableArray<SyntaxNode> unnecessaryImports)
            => unnecessaryImports;

        protected override IEnumerable<TextSpan> GetFixableDiagnosticSpans(
            IEnumerable<SyntaxNode> nodes, SyntaxTree tree, CancellationToken cancellationToken)
        {
            Contract.ThrowIfFalse(nodes.Any());

            var nodesContainingUnnecessaryUsings = new HashSet<SyntaxNode>();
            foreach (var node in nodes)
            {
                var nodeContainingUnnecessaryUsings = node.GetAncestors().First(n => n is NamespaceDeclarationSyntax || n is CompilationUnitSyntax);
                if (!nodesContainingUnnecessaryUsings.Add(nodeContainingUnnecessaryUsings))
                {
                    continue;
                }

                yield return nodeContainingUnnecessaryUsings is NamespaceDeclarationSyntax ?
                    ((NamespaceDeclarationSyntax)nodeContainingUnnecessaryUsings).Usings.GetContainedSpan() :
                    ((CompilationUnitSyntax)nodeContainingUnnecessaryUsings).Usings.GetContainedSpan();
            }
        }
    }
}
