// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.UseInferredMemberName;

namespace StarkPlatform.CodeAnalysis.Stark.UseInferredMemberName
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal sealed class CSharpUseInferredMemberNameCodeFixProvider : AbstractUseInferredMemberNameCodeFixProvider
    {
        protected override void LanguageSpecificRemoveSuggestedNode(SyntaxEditor editor, SyntaxNode node)
        {
            editor.RemoveNode(node, SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.AddElasticMarker);
        }
    }
}
