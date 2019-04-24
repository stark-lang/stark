// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.AddImports;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.AddImports
{
    [ExportLanguageService(typeof(IAddImportsService), LanguageNames.Stark), Shared]
    internal class CSharpAddImportsService : AbstractAddImportsService<
        CompilationUnitSyntax, NamespaceDeclarationSyntax, ImportDirectiveSyntax, ExternAliasDirectiveSyntax>
    {
        // C# doesn't have global imports.
        protected override ImmutableArray<SyntaxNode> GetGlobalImports(Compilation compilation)
            => ImmutableArray<SyntaxNode>.Empty;

        protected override SyntaxNode GetAlias(ImportDirectiveSyntax usingOrAlias)
            => usingOrAlias.Alias;

        protected override SyntaxNode Rewrite(
            ExternAliasDirectiveSyntax[] externAliases,
            ImportDirectiveSyntax[] usingDirectives,
            ImportDirectiveSyntax[] aliasDirectives,
            SyntaxNode externContainer,
            SyntaxNode usingContainer,
            SyntaxNode aliasContainer,
            bool placeSystemNamespaceFirst,
            SyntaxNode root)
        {
            var rewriter = new Rewriter(
                externAliases, usingDirectives, aliasDirectives,
                externContainer, usingContainer, aliasContainer,
                placeSystemNamespaceFirst);

            var newRoot = rewriter.Visit(root);
            return newRoot;
        }

        protected override SyntaxList<ImportDirectiveSyntax> GetUsingsAndAliases(SyntaxNode node)
        {
            switch (node)
            {
                case CompilationUnitSyntax c: return c.Usings;
                case NamespaceDeclarationSyntax n: return n.Usings;
                default: return default;
            }
        }

        protected override SyntaxList<ExternAliasDirectiveSyntax> GetExterns(SyntaxNode node)
        {
            switch (node)
            {
                case CompilationUnitSyntax c: return c.Externs;
                case NamespaceDeclarationSyntax n: return n.Externs;
                default: return default;
            }
        }

        private class Rewriter : CSharpSyntaxRewriter
        {
            private readonly bool _placeSystemNamespaceFirst;
            private readonly SyntaxNode _externContainer;
            private readonly SyntaxNode _usingContainer;
            private readonly SyntaxNode _aliasContainer;

            private readonly ImportDirectiveSyntax[] _aliasDirectives;
            private readonly ExternAliasDirectiveSyntax[] _externAliases;
            private readonly ImportDirectiveSyntax[] _usingDirectives;

            public Rewriter(
                ExternAliasDirectiveSyntax[] externAliases,
                ImportDirectiveSyntax[] usingDirectives,
                ImportDirectiveSyntax[] aliasDirectives,
                SyntaxNode externContainer,
                SyntaxNode usingContainer,
                SyntaxNode aliasContainer,
                bool placeSystemNamespaceFirst)
            {
                _externAliases = externAliases;
                _usingDirectives = usingDirectives;
                _aliasDirectives = aliasDirectives;
                _externContainer = externContainer;
                _usingContainer = usingContainer;
                _aliasContainer = aliasContainer;
                _placeSystemNamespaceFirst = placeSystemNamespaceFirst;
            }

            public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
            {
                // recurse downwards so we visit inner namespaces first.
                var rewritten = (NamespaceDeclarationSyntax)base.VisitNamespaceDeclaration(node);

                if (node == _aliasContainer)
                {
                    rewritten = rewritten.AddUsingDirectives(_aliasDirectives, _placeSystemNamespaceFirst);
                }

                if (node == _usingContainer)
                {
                    rewritten = rewritten.AddUsingDirectives(_usingDirectives, _placeSystemNamespaceFirst);
                }

                if (node == _externContainer)
                {
                    rewritten = rewritten.AddExterns(_externAliases);
                }

                return rewritten;
            }

            public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
            {
                // recurse downwards so we visit inner namespaces first.
                var rewritten = (CompilationUnitSyntax)base.VisitCompilationUnit(node);

                if (node == _aliasContainer)
                {
                    rewritten = rewritten.AddUsingDirectives(_aliasDirectives, _placeSystemNamespaceFirst);
                }

                if (node == _usingContainer)
                {
                    rewritten = rewritten.AddUsingDirectives(_usingDirectives, _placeSystemNamespaceFirst);
                }

                if (node == _externContainer)
                {
                    rewritten = rewritten.AddExterns(_externAliases);
                }

                return rewritten;
            }
        }
    }
}
