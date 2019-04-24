// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Syntax
{
    public abstract partial class TypeDeclarationSyntax
    {
        public int Arity
        {
            get
            {
                return this.TypeParameterList == null ? 0 : this.TypeParameterList.Parameters.Count;
            }
        }
    }
}

namespace StarkPlatform.CodeAnalysis.Stark
{
    public static partial class SyntaxFactory
    {
        internal static SyntaxKind GetTypeDeclarationKeywordKind(DeclarationKind kind)
        {
            switch (kind)
            {
                case DeclarationKind.Class:
                    return SyntaxKind.ClassKeyword;
                case DeclarationKind.Struct:
                    return SyntaxKind.StructKeyword;
                case DeclarationKind.Interface:
                    return SyntaxKind.InterfaceKeyword;
                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }
        }

        private static SyntaxKind GetTypeDeclarationKeywordKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ClassDeclaration:
                    return SyntaxKind.ClassKeyword;
                case SyntaxKind.StructDeclaration:
                    return SyntaxKind.StructKeyword;
                case SyntaxKind.InterfaceDeclaration:
                    return SyntaxKind.InterfaceKeyword;
                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }
        }

        public static TypeDeclarationSyntax TypeDeclaration(SyntaxKind kind, SyntaxToken identifier)
        {
            return TypeDeclaration(
                kind,
                default(SyntaxList<AttributeSyntax>),
                default(SyntaxTokenList),
                SyntaxFactory.Token(GetTypeDeclarationKeywordKind(kind)),
                identifier,
                default(TypeParameterListSyntax),
                default(ExtendListSyntax),
                default(ImplementListSyntax),
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                default(SyntaxList<MemberDeclarationSyntax>),
                SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
                default(SyntaxToken));
        }

        public static TypeDeclarationSyntax TypeDeclaration(SyntaxKind kind, string identifier)
        {
            return SyntaxFactory.TypeDeclaration(kind, SyntaxFactory.Identifier(identifier));
        }

        public static TypeDeclarationSyntax TypeDeclaration(SyntaxKind kind, SyntaxList<AttributeSyntax> attributes, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax typeParameterList, ExtendListSyntax extendList, ImplementListSyntax implementList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            switch (kind)
            {
                case SyntaxKind.ClassDeclaration:
                    return SyntaxFactory.ClassDeclaration(attributes, modifiers, keyword, identifier, typeParameterList, extendList, implementList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                case SyntaxKind.StructDeclaration:
                    return SyntaxFactory.StructDeclaration(attributes, modifiers, keyword, identifier, typeParameterList, extendList, implementList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                case SyntaxKind.InterfaceDeclaration:
                    return SyntaxFactory.InterfaceDeclaration(attributes, modifiers, keyword, identifier, typeParameterList, extendList, implementList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                default:
                    throw new ArgumentException("kind");
            }
        }
    }
}
