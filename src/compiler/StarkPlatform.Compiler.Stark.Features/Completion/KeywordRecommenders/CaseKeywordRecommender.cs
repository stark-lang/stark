﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions.ContextQuery;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Shared.Extensions;

namespace StarkPlatform.Compiler.Stark.Completion.KeywordRecommenders
{
    internal class CaseKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public CaseKeywordRecommender()
            : base(SyntaxKind.CaseKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            return
                context.TargetToken.IsSwitchLabelContext() ||
                IsAfterGotoInSwitchContext(context);
        }

        internal static bool IsAfterGotoInSwitchContext(CSharpSyntaxContext context)
        {
            var token = context.TargetToken;

            if (token.Kind() == SyntaxKind.GotoKeyword &&
                token.GetAncestor<SwitchStatementSyntax>() != null)
            {
                // todo: what if we're in a lambda... or a try/finally or 
                // something?  Might want to filter this out.
                return true;
            }

            return false;
        }
    }
}
