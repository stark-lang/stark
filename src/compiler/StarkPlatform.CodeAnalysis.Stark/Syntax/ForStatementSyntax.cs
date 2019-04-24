// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.Syntax
{
    public partial class ForStatementSyntax
    {
        public ForStatementSyntax Update(SyntaxToken forEachKeyword, ExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax expression, BlockSyntax statement)
        {
            return Update(awaitKeyword: default, forEachKeyword, variable, inKeyword, expression, statement);
        }
    }
}

namespace StarkPlatform.CodeAnalysis.Stark
{
    public partial class SyntaxFactory
    {
        public static ForStatementSyntax ForStatement(SyntaxToken forEachKeyword, ExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax expression, BlockSyntax statement)
        {
            return ForStatement(awaitKeyword: default, forEachKeyword, variable, inKeyword, expression, statement);
        }
    }
}
