// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarkPlatform.Compiler.Stark;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Extensions
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
