// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.GenerateEqualsAndGetHashCodeFromMembers;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.GenerateEqualsAndGetHashCodeFromMembers
{
    [ExportLanguageService(typeof(IGenerateEqualsAndGetHashCodeService), LanguageNames.Stark), Shared]
    internal class CSharpGenerateEqualsAndGetHashCodeService : AbstractGenerateEqualsAndGetHashCodeService
    {
        protected override bool TryWrapWithUnchecked(ImmutableArray<SyntaxNode> statements, out ImmutableArray<SyntaxNode> wrappedStatements)
        {
            wrappedStatements = ImmutableArray.Create<SyntaxNode>(
                SyntaxFactory.CheckedStatement(SyntaxKind.UncheckedStatement,
                    SyntaxFactory.Block(statements.OfType<StatementSyntax>())));
            return true;
        }
    }
}
