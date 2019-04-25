// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Organizing.Organizers;

namespace StarkPlatform.Compiler.Stark.Organizing.Organizers
{
    [ExportSyntaxNodeOrganizer(LanguageNames.Stark), Shared]
    internal class PropertyDeclarationOrganizer : AbstractSyntaxNodeOrganizer<PropertyDeclarationSyntax>
    {
        protected override PropertyDeclarationSyntax Organize(
            PropertyDeclarationSyntax syntax,
            CancellationToken cancellationToken)
        {
            return syntax.WithModifiers(ModifiersOrganizer.Organize(syntax.Modifiers));
        }
    }
}
