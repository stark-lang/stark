// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.ConflictMarkerResolution;

namespace StarkPlatform.CodeAnalysis.Stark.ConflictMarkerResolution
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal class CSharpResolveConflictMarkerCodeFixProvider : AbstractResolveConflictMarkerCodeFixProvider
    {
        private const string CS8300 = nameof(CS8300); // Merge conflict marker encountered

        public CSharpResolveConflictMarkerCodeFixProvider()
            : base(CS8300)
        {
        }

        protected override bool IsConflictMarker(SyntaxTrivia trivia)
            => trivia.Kind() == SyntaxKind.ConflictMarkerTrivia;

        protected override bool IsDisabledText(SyntaxTrivia trivia)
            => trivia.Kind() == SyntaxKind.DisabledTextTrivia;

        protected override bool IsEndOfLine(SyntaxTrivia trivia)
            => trivia.Kind() == SyntaxKind.EndOfLineTrivia;
    }
}
