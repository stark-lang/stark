// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions.ContextQuery;
using StarkPlatform.Compiler.Stark.Utilities;
using StarkPlatform.Compiler.Shared.Extensions;

namespace StarkPlatform.Compiler.Stark.Completion.KeywordRecommenders
{
    internal class ReturnKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public ReturnKeywordRecommender()
            : base(SyntaxKind.ReturnKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return
                context.IsStatementContext ||
                context.TargetToken.IsAfterYieldKeyword() ||
                IsAttributeContext(context, cancellationToken);
        }

        private static bool IsAttributeContext(CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return
                context.IsMemberAttributeContext(SyntaxKindSet.ClassInterfaceStructTypeDeclarations, cancellationToken) ||
                (context.SyntaxTree.IsScript() && context.IsTypeAttributeContext(cancellationToken));
        }
    }
}
