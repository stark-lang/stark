// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Structure;

namespace StarkPlatform.Compiler.Stark.Structure
{
    internal class CompilationUnitStructureProvider : AbstractSyntaxNodeStructureProvider<CompilationUnitSyntax>
    {
        protected override void CollectBlockSpans(
            CompilationUnitSyntax compilationUnit,
            ArrayBuilder<BlockSpan> spans,
            OptionSet options,
            CancellationToken cancellationToken)
        {
            CSharpStructureHelpers.CollectCommentBlockSpans(compilationUnit, spans);

            // extern aliases and usings are outlined in a single region
            var externsAndUsings = new List<SyntaxNode>();
            externsAndUsings.AddRange(compilationUnit.Externs);
            externsAndUsings.AddRange(compilationUnit.Usings);
            externsAndUsings.Sort((node1, node2) => node1.SpanStart.CompareTo(node2.SpanStart));

            spans.AddIfNotNull(CSharpStructureHelpers.CreateBlockSpan(
                externsAndUsings, autoCollapse: true,
                type: BlockTypes.Imports, isCollapsible: true));

            if (compilationUnit.Usings.Count > 0 ||
                compilationUnit.Externs.Count > 0 ||
                compilationUnit.Members.Count > 0 ||
                compilationUnit.AttributeLists.Count > 0)
            {
                CSharpStructureHelpers.CollectCommentBlockSpans(compilationUnit.EndOfFileToken.LeadingTrivia, spans);
            }
        }

        protected override bool SupportedInWorkspaceKind(string kind)
        {
            return true;
        }
    }
}
