// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using StarkPlatform.Compiler.Stark;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Shared.Extensions;
using StarkPlatform.Compiler.Simplification;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Extensions
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
