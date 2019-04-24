// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using StarkPlatform.CodeAnalysis.CodeGeneration;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.CodeGeneration
{
    internal static class StatementGenerator
    {
        internal static SyntaxList<StatementSyntax> GenerateStatements(IEnumerable<SyntaxNode> statements)
        {
            return statements.OfType<StatementSyntax>().ToSyntaxList();
        }

        internal static BlockSyntax GenerateBlock(IMethodSymbol method)
        {
            return SyntaxFactory.Block(
                StatementGenerator.GenerateStatements(CodeGenerationMethodInfo.GetStatements(method)));
        }
    }
}
