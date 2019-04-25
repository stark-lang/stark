// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.CodeRefactorings.MoveType;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Host.Mef;

namespace StarkPlatform.Compiler.Stark.CodeRefactorings.MoveType
{
    [ExportLanguageService(typeof(IMoveTypeService), LanguageNames.Stark), Shared]
    internal class CSharpMoveTypeService :
        AbstractMoveTypeService<CSharpMoveTypeService, BaseTypeDeclarationSyntax, NamespaceDeclarationSyntax, MemberDeclarationSyntax, CompilationUnitSyntax>
    {
    }
}
