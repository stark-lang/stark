// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.Simplification;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class StatementSyntaxExtensions
    {
        public static StatementSyntax GetPreviousStatement(this StatementSyntax statement)
        {
            if (statement != null)
            {
                var previousToken = statement.GetFirstToken().GetPreviousToken();
                return previousToken.GetAncestors<StatementSyntax>().FirstOrDefault(s => s.Parent == statement.Parent);
            }

            return null;
        }

        public static StatementSyntax GetNextStatement(this StatementSyntax statement)
        {
            if (statement != null)
            {
                var nextToken = statement.GetLastToken().GetNextToken();
                return nextToken.GetAncestors<StatementSyntax>().FirstOrDefault(s => s.Parent == statement.Parent);
            }

            return null;
        }
    }
}
