// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.MakeFieldReadonly;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.MakeFieldReadonly
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal class CSharpMakeFieldReadonlyCodeFixProvider : AbstractMakeFieldReadonlyCodeFixProvider<VariableDeclarationSyntax, FieldDeclarationSyntax>
    {
        protected override SyntaxNode GetInitializerNode(VariableDeclarationSyntax declaration)
            => declaration.Initializer?.Value;

        protected override VariableDeclarationSyntax GetVariableDeclarations(FieldDeclarationSyntax fieldDeclaration)
            => fieldDeclaration.Declaration;
    }
}
