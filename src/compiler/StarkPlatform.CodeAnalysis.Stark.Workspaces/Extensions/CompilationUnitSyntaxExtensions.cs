// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StarkPlatform.CodeAnalysis;
using StarkPlatform.CodeAnalysis.AddImports;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Stark.Utilities;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class CompilationUnitSyntaxExtensions
    {
        public static bool CanAddUsingDirectives(this SyntaxNode contextNode, CancellationToken cancellationToken)
        {
            var usingDirectiveAncestor = contextNode.GetAncestor<ImportDirectiveSyntax>();
            if ((usingDirectiveAncestor != null) && (usingDirectiveAncestor.GetAncestor<NamespaceDeclarationSyntax>() == null))
            {
                // We are inside a top level using directive (i.e. one that's directly in the compilation unit).
                return false;
            }

            if (contextNode.SyntaxTree.HasHiddenRegions())
            {
                var namespaceDeclaration = contextNode.GetInnermostNamespaceDeclarationWithUsings();
                var root = contextNode.GetAncestorOrThis<CompilationUnitSyntax>();
                var span = GetUsingsSpan(root, namespaceDeclaration);

                if (contextNode.SyntaxTree.OverlapsHiddenPosition(span, cancellationToken))
                {
                    return false;
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            return true;
        }

        private static TextSpan GetUsingsSpan(CompilationUnitSyntax root, NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (namespaceDeclaration != null)
            {
                var usings = namespaceDeclaration.Usings;
                var start = usings.First().SpanStart;
                var end = usings.Last().Span.End;
                return TextSpan.FromBounds(start, end);
            }
            else
            {
                var rootUsings = root.Usings;
                if (rootUsings.Any())
                {
                    var start = rootUsings.First().SpanStart;
                    var end = rootUsings.Last().Span.End;
                    return TextSpan.FromBounds(start, end);
                }
                else
                {
                    var start = 0;
                    var end = root.Members.Any()
                        ? root.Members.First().GetFirstToken().Span.End
                        : root.Span.End;
                    return TextSpan.FromBounds(start, end);
                }
            }
        }

        public static CompilationUnitSyntax AddUsingDirective(
            this CompilationUnitSyntax root,
            ImportDirectiveSyntax usingDirective,
            SyntaxNode contextNode,
            bool placeSystemNamespaceFirst,
            params SyntaxAnnotation[] annotations)
        {
            return root.AddUsingDirectives(new[] { usingDirective }, contextNode, placeSystemNamespaceFirst, annotations);
        }

        public static CompilationUnitSyntax AddUsingDirectives(
            this CompilationUnitSyntax root,
            IList<ImportDirectiveSyntax> usingDirectives,
            SyntaxNode contextNode,
            bool placeSystemNamespaceFirst,
            params SyntaxAnnotation[] annotations)
        {
            if (!usingDirectives.Any())
            {
                return root;
            }

            var firstOuterNamespaceWithUsings = contextNode.GetInnermostNamespaceDeclarationWithUsings();

            if (firstOuterNamespaceWithUsings == null)
            {
                return root.AddUsingDirectives(usingDirectives, placeSystemNamespaceFirst, annotations);
            }
            else
            {
                var newNamespace = firstOuterNamespaceWithUsings.AddUsingDirectives(usingDirectives, placeSystemNamespaceFirst, annotations);
                return root.ReplaceNode(firstOuterNamespaceWithUsings, newNamespace);
            }
        }

        public static CompilationUnitSyntax AddUsingDirectives(
            this CompilationUnitSyntax root,
            IList<ImportDirectiveSyntax> usingDirectives,
            bool placeSystemNamespaceFirst,
            params SyntaxAnnotation[] annotations)
        {
            if (usingDirectives.Count == 0)
            {
                return root;
            }

            var usings = AddUsingDirectives(root, usingDirectives);

            // Keep usings sorted if they were originally sorted.
            usings.SortUsingDirectives(root.Usings, placeSystemNamespaceFirst);

            if (root.Externs.Count == 0)
            {
                root = AddImportHelpers.MoveTrivia(
                    CSharpSyntaxFactsService.Instance, root, root.Usings, usings);
            }

            return root.WithUsings(
                usings.Select(u => u.WithAdditionalAnnotations(annotations)).ToSyntaxList());
        }

        private static bool IsDocCommentOrElastic(SyntaxTrivia t)
        {
            return t.IsDocComment() || t.IsElastic();
        }

        private static List<ImportDirectiveSyntax> AddUsingDirectives(
            CompilationUnitSyntax root, IList<ImportDirectiveSyntax> usingDirectives)
        {
            // We need to try and not place the using inside of a directive if possible.
            var usings = new List<ImportDirectiveSyntax>();
            var endOfList = root.Usings.Count - 1;
            var startOfLastDirective = -1;
            var endOfLastDirective = -1;
            for (var i = 0; i < root.Usings.Count; i++)
            {
                if (root.Usings[i].GetLeadingTrivia().Any(trivia => trivia.IsKind(SyntaxKind.IfDirectiveTrivia)))
                {
                    startOfLastDirective = i;
                }

                if (root.Usings[i].GetLeadingTrivia().Any(trivia => trivia.IsKind(SyntaxKind.EndIfDirectiveTrivia)))
                {
                    endOfLastDirective = i;
                }
            }

            // if the entire using is in a directive or there is a using list at the end outside of the directive add the using at the end, 
            // else place it before the last directive.
            usings.AddRange(root.Usings);
            if ((startOfLastDirective == 0 && (endOfLastDirective == endOfList || endOfLastDirective == -1)) ||
                (startOfLastDirective == -1 && endOfLastDirective == -1) ||
                (endOfLastDirective != endOfList && endOfLastDirective != -1))
            {
                usings.AddRange(usingDirectives);
            }
            else
            {
                usings.InsertRange(startOfLastDirective, usingDirectives);
            }

            return usings;
        }
    }
}
