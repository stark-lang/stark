﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Syntax;

namespace StarkPlatform.Compiler.Stark.Syntax
{
    public partial class StackAllocArrayCreationExpressionSyntax
    {
        public StackAllocArrayCreationExpressionSyntax Update(SyntaxToken stackAllocKeyword, TypeSyntax type)
            => Update(StackAllocKeyword, type, default(InitializerExpressionSyntax));
    }

    public partial class LocalDeclarationStatementSyntax
    {
        public LocalDeclarationStatementSyntax Update(SyntaxTokenList modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
            => Update(default(SyntaxToken), default(SyntaxToken), declaration, semicolonToken);
    }
}

namespace StarkPlatform.Compiler.Stark
{
    public partial class SyntaxFactory
    {
        public static StackAllocArrayCreationExpressionSyntax StackAllocArrayCreationExpression(SyntaxToken stackAllocKeyword, TypeSyntax type)
            => StackAllocArrayCreationExpression(stackAllocKeyword, type, default(InitializerExpressionSyntax));

        public static LocalDeclarationStatementSyntax LocalDeclarationStatement(SyntaxTokenList modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
            => LocalDeclarationStatement(default(SyntaxToken), default(SyntaxToken), declaration, semicolonToken);
    }
}
