// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Syntax
{
    public partial class MethodDeclarationSyntax
    {
        public int Arity
        {
            get
            {
                return this.TypeParameterList == null ? 0 : this.TypeParameterList.Parameters.Count;
            }
        }

        public TypeAccessModifiers GetAccessModifiers()
        {
            var modifiers = TypeAccessModifiers.None;
            foreach (var modifier in this.Modifiers)
            {
                var kind = modifier.Kind();
                if (kind == SyntaxKind.ReadOnlyKeyword) modifiers |= TypeAccessModifiers.ReadOnly;
                if (kind == SyntaxKind.TransientKeyword) modifiers |= TypeAccessModifiers.Transient;
            }

            return modifiers;
        }
    }
}

namespace StarkPlatform.CodeAnalysis.Stark
{
    public partial class SyntaxFactory
    {
        public static MethodDeclarationSyntax MethodDeclaration(
            SyntaxList<AttributeSyntax> attributeLists,
            SyntaxTokenList modifiers,
            SyntaxToken funcKeyword,
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier,
            SyntaxToken identifier,
            SyntaxToken minusGreaterThanToken,
            TypeSyntax returnType,
            TypeParameterListSyntax typeParameterList,
            ParameterListSyntax parameterList,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses,
            SyntaxList<ContractClauseSyntax> contractClauses,
            ThrowsListSyntax throwsList,
            BlockSyntax body,
            SyntaxToken semicolonToken)
        {
            return SyntaxFactory.MethodDeclaration(attributeLists, modifiers, funcKeyword, explicitInterfaceSpecifier, identifier, typeParameterList, parameterList, minusGreaterThanToken, returnType, constraintClauses, contractClauses, throwsList, body, null, semicolonToken);
        }
    }
}
