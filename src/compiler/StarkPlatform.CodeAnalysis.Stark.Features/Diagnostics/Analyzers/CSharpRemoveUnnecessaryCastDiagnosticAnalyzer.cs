// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Diagnostics.RemoveUnnecessaryCast;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Diagnostics.RemoveUnnecessaryCast
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal sealed class CSharpRemoveUnnecessaryCastDiagnosticAnalyzer
        : RemoveUnnecessaryCastDiagnosticAnalyzerBase<SyntaxKind, CastExpressionSyntax>
    {
        private static readonly ImmutableArray<SyntaxKind> s_kindsOfInterest = ImmutableArray.Create(SyntaxKind.CastExpression);

        protected override ImmutableArray<SyntaxKind> SyntaxKindsOfInterest => s_kindsOfInterest;

        protected override bool IsUnnecessaryCast(SemanticModel model, CastExpressionSyntax cast, CancellationToken cancellationToken)
            => cast.IsUnnecessaryCast(model, cancellationToken);

        protected override TextSpan GetFadeSpan(CastExpressionSyntax node)
            => TextSpan.FromBounds(node.AsKeyword.SpanStart, node.Type.Span.End);
    }
}
