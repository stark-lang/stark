// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Stark;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class ForEachStatementSyntaxExtensions
    {
        public static bool IsTypeInferred(this ForStatementSyntax forEachStatement, SemanticModel semanticModel)
        {
            switch (forEachStatement.Kind())
            {
                case SyntaxKind.ForStatement:
                    return (((ForStatementSyntax)forEachStatement).Variable as DeclarationExpressionSyntax)?.Type
                        .IsTypeInferred(semanticModel) == true;
                default:
                    return false;
            }
        }
    }
}
