// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StarkPlatform.CodeAnalysis;
using StarkPlatform.CodeAnalysis.Stark;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class ITypeParameterSymbolExtensions
    {
        public static SyntaxList<TypeParameterConstraintClauseSyntax> GenerateConstraintClauses(
            this ImmutableArray<ITypeParameterSymbol> typeParameters)
        {
            return typeParameters.AsEnumerable().GenerateConstraintClauses();
        }

        public static SyntaxList<TypeParameterConstraintClauseSyntax> GenerateConstraintClauses(
            this IEnumerable<ITypeParameterSymbol> typeParameters)
        {
            var clauses = new List<TypeParameterConstraintClauseSyntax>();

            foreach (var typeParameter in typeParameters)
            {
                AddConstraintClauses(clauses, typeParameter);
            }

            return clauses.Count == 0 ? default : clauses.ToSyntaxList();
        }

        private static void AddConstraintClauses(
            List<TypeParameterConstraintClauseSyntax> clauses,
            ITypeParameterSymbol typeParameter)
        {
            var constraints = new List<TypeParameterConstraintSyntax>();

            if (typeParameter.HasReferenceTypeConstraint)
            {
                constraints.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint));
            }
            else if (typeParameter.HasUnmanagedTypeConstraint)
            {
                throw new NotImplementedException("constraints.Add(SyntaxFactory.TypeConstraint(SyntaxFactory.IdentifierName(unmanaged)));");
                //constraints.Add(SyntaxFactory.TypeConstraint(SyntaxFactory.IdentifierName("unmanaged")));
            }
            else if (typeParameter.HasValueTypeConstraint)
            {
                constraints.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.StructConstraint));
            }

            var extendsTypes = typeParameter.ConstraintTypes.Where(t => t.TypeKind == TypeKind.Class);
            var implementsTypes = typeParameter.ConstraintTypes.Where(t => t.TypeKind == TypeKind.Interface);

            foreach (var type in extendsTypes)
            {
                if (type.SpecialType != SpecialType.System_Object)
                {
                    constraints.Add(SyntaxFactory.ExtendsOrImplementsTypeConstraint(SyntaxKind.ExtendsTypeConstraint, type.GenerateTypeSyntax()));
                }
            }
            foreach (var type in implementsTypes)
            {
                if (type.SpecialType != SpecialType.System_Object)
                {
                    constraints.Add(SyntaxFactory.ExtendsOrImplementsTypeConstraint(SyntaxKind.ImplementsTypeConstraint, type.GenerateTypeSyntax()));
                }
            }

            if (typeParameter.HasConstructorConstraint)
            {
                constraints.Add(SyntaxFactory.ConstructorConstraint());
            }

            if (constraints.Count == 0)
            {
                return;
            }

            clauses.Add(SyntaxFactory.TypeParameterConstraintClause(
                typeParameter.Name.ToIdentifierName(),
                SyntaxFactory.SeparatedList(constraints)));
        }
    }
}
