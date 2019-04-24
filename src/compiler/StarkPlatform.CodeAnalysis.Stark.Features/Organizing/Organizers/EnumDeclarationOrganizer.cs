// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Organizing.Organizers;

namespace StarkPlatform.CodeAnalysis.Stark.Organizing.Organizers
{
    [ExportSyntaxNodeOrganizer(LanguageNames.Stark), Shared]
    internal class EnumDeclarationOrganizer : AbstractSyntaxNodeOrganizer<EnumDeclarationSyntax>
    {
        protected override EnumDeclarationSyntax Organize(
            EnumDeclarationSyntax syntax,
            CancellationToken cancellationToken)
        {
            return syntax.Update(
                syntax.AttributeLists,
                ModifiersOrganizer.Organize(syntax.Modifiers),
                syntax.EnumKeyword,
                syntax.Identifier,
                syntax.ExtendList,
                syntax.ImplementList,
                syntax.OpenBraceToken,
                syntax.Members,
                syntax.CloseBraceToken,
                syntax.EosToken);
        }
    }
}
