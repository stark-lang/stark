// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Organizing.Organizers;

namespace StarkPlatform.CodeAnalysis.Stark.Organizing.Organizers
{
    [ExportSyntaxNodeOrganizer(LanguageNames.Stark), Shared]
    internal class EventDeclarationOrganizer : AbstractSyntaxNodeOrganizer<EventDeclarationSyntax>
    {
        protected override EventDeclarationSyntax Organize(
            EventDeclarationSyntax syntax,
            CancellationToken cancellationToken)
        {
            return syntax.Update(syntax.AttributeLists,
                ModifiersOrganizer.Organize(syntax.Modifiers),
                syntax.EventKeyword,
                syntax.Type,
                syntax.ExplicitInterfaceSpecifier,
                syntax.Identifier,
                syntax.ContractClauses,
                syntax.AccessorList);
        }
    }
}
