// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeRefactorings.MoveType;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.CodeRefactorings.MoveType
{
    [ExportLanguageService(typeof(IMoveTypeService), LanguageNames.Stark), Shared]
    internal class CSharpMoveTypeService :
        AbstractMoveTypeService<CSharpMoveTypeService, BaseTypeDeclarationSyntax, NamespaceDeclarationSyntax, MemberDeclarationSyntax, CompilationUnitSyntax>
    {
    }
}
