// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.RemoveUnusedMembers;

namespace StarkPlatform.CodeAnalysis.Stark.RemoveUnusedMembers
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpRemoveUnusedMembersDiagnosticAnalyzer
        : AbstractRemoveUnusedMembersDiagnosticAnalyzer<DocumentationCommentTriviaSyntax, IdentifierNameSyntax>
    {
    }
}
