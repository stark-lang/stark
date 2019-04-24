// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class NamespaceDeclarationSyntaxExtensions
    {
        public static NamespaceDeclarationSyntax AddUsingDirectives(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            IList<ImportDirectiveSyntax> usingDirectives,
            bool placeSystemNamespaceFirst,
            params SyntaxAnnotation[] annotations)
        {
            if (usingDirectives.Count == 0)
            {
                return namespaceDeclaration;
            }

            var newUsings = new List<ImportDirectiveSyntax>();
            newUsings.AddRange(namespaceDeclaration.Usings);
            newUsings.AddRange(usingDirectives);

            newUsings.SortUsingDirectives(namespaceDeclaration.Usings, placeSystemNamespaceFirst);
            newUsings = newUsings.Select(u => u.WithAdditionalAnnotations(annotations)).ToList();

            var newNamespace = namespaceDeclaration.WithUsings(newUsings.ToSyntaxList());
            return newNamespace;
        }
    }
}
