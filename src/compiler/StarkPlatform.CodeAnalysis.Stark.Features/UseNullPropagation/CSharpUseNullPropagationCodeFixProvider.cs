// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.UseNullPropagation;

namespace StarkPlatform.CodeAnalysis.Stark.UseNullPropagation
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal class CSharpUseNullPropagationCodeFixProvider : AbstractUseNullPropagationCodeFixProvider<
            SyntaxKind,
            ExpressionSyntax,
            IfExpressionSyntax,
            BinaryExpressionSyntax,
            InvocationExpressionSyntax,
            MemberAccessExpressionSyntax,
            ConditionalAccessExpressionSyntax,
            ElementAccessExpressionSyntax>
    {
    }
}
