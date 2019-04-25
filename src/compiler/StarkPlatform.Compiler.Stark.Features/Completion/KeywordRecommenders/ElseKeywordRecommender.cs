// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.Compiler.Stark.Extensions.ContextQuery;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Shared.Extensions;

namespace StarkPlatform.Compiler.Stark.Completion.KeywordRecommenders
{
    internal class ElseKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        public ElseKeywordRecommender()
            : base(SyntaxKind.ElseKeyword, isValidInPreprocessorContext: true)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            if (context.IsPreProcessorKeywordContext)
            {
                return true;
            }

            var token = context.TargetToken;

            // We have to consider all ancestor if statements of the last token until we find a match for this 'else':
            // while (true)
            //     if (true)
            //         while (true)
            //             if (true)
            //                 Console.WriteLine();
            //             else
            //                 Console.WriteLine();
            //     $$
            foreach (var ifStatement in token.GetAncestors<IfStatementSyntax>())
            {
                // If there's a missing token at the end of the statement, it's incomplete and we do not offer 'else'.
                // context.TargetToken does not include zero width so in that case these will never be equal.
                if (ifStatement.Statement.GetLastToken(includeZeroWidth: true) == token)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
