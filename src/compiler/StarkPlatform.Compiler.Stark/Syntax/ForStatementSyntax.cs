// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Syntax;

namespace StarkPlatform.Compiler.Stark.Syntax
{
    public partial class ForStatementSyntax
    {
        public ForStatementSyntax Update(SyntaxToken forEachKeyword, ExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax expression, BlockSyntax statement)
        {
            return Update(awaitKeyword: default, forEachKeyword, variable, inKeyword, expression, statement);
        }
    }
}

namespace StarkPlatform.Compiler.Stark
{
    public partial class SyntaxFactory
    {
        public static ForStatementSyntax ForStatement(SyntaxToken forEachKeyword, ExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax expression, BlockSyntax statement)
        {
            return ForStatement(awaitKeyword: default, forEachKeyword, variable, inKeyword, expression, statement);
        }
    }
}
