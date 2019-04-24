// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StarkPlatform.CodeAnalysis;
using StarkPlatform.CodeAnalysis.CodeGeneration;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using Roslyn.Utilities;

using static StarkPlatform.CodeAnalysis.CodeGeneration.CodeGenerationHelpers;
using static StarkPlatform.CodeAnalysis.Stark.CodeGeneration.CSharpCodeGenerationHelpers;

namespace StarkPlatform.CodeAnalysis.Stark.CodeGeneration
{
    internal static class AttributeGenerator
    {
        public static SyntaxList<AttributeSyntax> GenerateAttributeLists(
            ImmutableArray<AttributeData> attributes,
            CodeGenerationOptions options,
            SyntaxToken? target = null)
        {
            if (options.MergeAttributes)
            {
                var attributeNodes = attributes.OrderBy(a => a.AttributeClass.Name).Select(a => GenerateAttribute(target.HasValue ? SyntaxFactory.AttributeTargetSpecifier(target.Value) : default, a, options)).WhereNotNull().ToList();
                return attributeNodes.Count == 0
                    ? default
                    : SyntaxFactory.List(SyntaxFactory.SeparatedList(attributeNodes));
            }
            else
            {
                var attributeDeclarations = attributes.OrderBy(a => a.AttributeClass.Name).Select(a => GenerateAttributeDeclaration(a, target, options)).WhereNotNull().ToList();
                return attributeDeclarations.Count == 0
                    ? default
                    : SyntaxFactory.List<AttributeSyntax>(attributeDeclarations);
            }
        }

        private static AttributeSyntax GenerateAttributeDeclaration(
            AttributeData attribute, SyntaxToken? target, CodeGenerationOptions options)
        {
            return  GenerateAttribute(target.HasValue ? SyntaxFactory.AttributeTargetSpecifier(target.Value) : default, attribute, options);
        }

        private static AttributeSyntax GenerateAttribute(AttributeTargetSpecifierSyntax target, AttributeData attribute, CodeGenerationOptions options)
        {
            if (!options.MergeAttributes)
            {
                var reusableSyntax = GetReuseableSyntaxNodeForAttribute<AttributeSyntax>(attribute, options);
                if (reusableSyntax != null)
                {
                    return reusableSyntax;
                }
            }

            var attributeArguments = GenerateAttributeArgumentList(attribute);
            var nameSyntax = attribute.AttributeClass.GenerateTypeSyntax() as NameSyntax;
            return nameSyntax == null ? null : SyntaxFactory.Attribute(target, nameSyntax, attributeArguments);
        }

        private static AttributeArgumentListSyntax GenerateAttributeArgumentList(AttributeData attribute)
        {
            if (attribute.ConstructorArguments.Length == 0 && attribute.NamedArguments.Length == 0)
            {
                return null;
            }

            var arguments = new List<AttributeArgumentSyntax>();
            arguments.AddRange(attribute.ConstructorArguments.Select(c =>
                SyntaxFactory.AttributeArgument(ExpressionGenerator.GenerateExpression(c))));

            arguments.AddRange(attribute.NamedArguments.Select(kvp =>
                SyntaxFactory.AttributeArgument(
                    SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(kvp.Key)), null,
                    ExpressionGenerator.GenerateExpression(kvp.Value))));

            return SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(arguments));
        }
    }
}
