// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class VariableDeclarationExtensions
    {
        public static TypeSyntax GetVariableType(this VariableDeclarationSyntax declarator)
        {
            if (declarator.Parent is VariableDeclarationSyntax variableDeclaration)
            {
                return variableDeclaration.Type;
            }

            return null;
        }

        public static bool IsTypeInferred(this VariableDeclarationSyntax variable, SemanticModel semanticModel)
        {
            var variableTypeName = variable.GetVariableType();
            if (variableTypeName == null)
            {
                return false;
            }

            return variableTypeName.IsTypeInferred(semanticModel);
        }
    }
}
