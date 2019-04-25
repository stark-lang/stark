// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.RemoveUnusedMembers;

namespace StarkPlatform.Compiler.Stark.RemoveUnusedMembers
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpRemoveUnusedMembersDiagnosticAnalyzer
        : AbstractRemoveUnusedMembersDiagnosticAnalyzer<DocumentationCommentTriviaSyntax, IdentifierNameSyntax>
    {
    }
}
