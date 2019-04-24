// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Diagnostics;
using StarkPlatform.CodeAnalysis.AddAccessibilityModifiers;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.AddAccessibilityModifiers
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal class CSharpAddAccessibilityModifiersCodeFixProvider : AbstractAddAccessibilityModifiersCodeFixProvider
    {
        protected override SyntaxNode MapToDeclarator(SyntaxNode node)
        {
            switch (node)
            {
                case FieldDeclarationSyntax field:
                    return field.Declaration;

                case EventFieldDeclarationSyntax eventField:
                    return eventField.Declaration;

                default:
                    return node;
            }
        }
    }
}
