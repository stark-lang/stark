// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.PreferFrameworkType;

namespace StarkPlatform.CodeAnalysis.Stark.Diagnostics.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal class CSharpPreferFrameworkTypeDiagnosticAnalyzer :
        PreferFrameworkTypeDiagnosticAnalyzerBase<SyntaxKind, ExpressionSyntax, PredefinedTypeSyntax>
    {
        protected override ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get; } =
            ImmutableArray.Create(SyntaxKind.PredefinedType);

        ///<remarks>
        /// every predefined type keyword except <c>void</c> can be replaced by its framework type in code.
        ///</remarks>
        protected override bool IsPredefinedTypeReplaceableWithFrameworkType(PredefinedTypeSyntax node)
            => node.Keyword.Kind() != SyntaxKind.VoidKeyword;

        protected override bool IsInMemberAccessOrCrefReferenceContext(ExpressionSyntax node)
            => node.IsInMemberAccessContext() || node.InsideCrefReference();

        protected override string GetLanguageName()
            => LanguageNames.Stark;
    }
}
