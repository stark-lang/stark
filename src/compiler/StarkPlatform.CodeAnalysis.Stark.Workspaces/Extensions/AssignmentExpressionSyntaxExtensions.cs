// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class AssignmentExpressionSyntaxExtensions
    {
        internal static bool IsDeconstruction(this AssignmentExpressionSyntax assignment)
        {
            var left = assignment.Left;
            return assignment.Kind() == SyntaxKind.SimpleAssignmentExpression &&
                   assignment.OperatorToken.Kind() == SyntaxKind.EqualsToken &&
                   (left.Kind() == SyntaxKind.TupleExpression || left.Kind() == SyntaxKind.DeclarationExpression);
        }
    }
}
