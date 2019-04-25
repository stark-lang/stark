// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Composition;
using StarkPlatform.Compiler.CodeFixes;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.SimplifyThisOrMe;

namespace StarkPlatform.Compiler.Stark.SimplifyThisOrMe
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.SimplifyThisOrMe), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.RemoveUnnecessaryCast)]
    internal partial class CSharpSimplifyThisOrMeCodeFixProvider
        : AbstractSimplifyThisOrMeCodeFixProvider<MemberAccessExpressionSyntax>
    {
        protected override string GetTitle()
            => CSharpFeaturesResources.Remove_this_qualification;

        protected override SyntaxNode Rewrite(
            SemanticModel semanticModel, SyntaxNode root, ISet<MemberAccessExpressionSyntax> memberAccessNodes)
        {
            var rewriter = new Rewriter(memberAccessNodes);
            return rewriter.Visit(root);
        }

        private class Rewriter : CSharpSyntaxRewriter
        {
            private readonly ISet<MemberAccessExpressionSyntax> memberAccessNodes;

            public Rewriter(ISet<MemberAccessExpressionSyntax> memberAccessNodes)
                => this.memberAccessNodes = memberAccessNodes;

            public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
                => memberAccessNodes.Contains(node)
                    ? node.GetNameWithTriviaMoved()
                    : base.VisitMemberAccessExpression(node);
        }
    }
}
