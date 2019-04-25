// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.Compiler.CodeFixes;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.MakeFieldReadonly;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark.MakeFieldReadonly
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
