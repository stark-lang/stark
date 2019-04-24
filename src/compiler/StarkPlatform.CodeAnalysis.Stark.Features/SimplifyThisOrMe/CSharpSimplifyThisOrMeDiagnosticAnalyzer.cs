// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.SimplifyThisOrMe;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.SimplifyThisOrMe
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal sealed class CSharpSimplifyThisOrMeDiagnosticAnalyzer
        : AbstractSimplifyThisOrMeDiagnosticAnalyzer<
            SyntaxKind,
            ExpressionSyntax,
            ThisExpressionSyntax,
            MemberAccessExpressionSyntax>
    {
        public CSharpSimplifyThisOrMeDiagnosticAnalyzer()
            : base(ImmutableArray.Create(SyntaxKind.SimpleMemberAccessExpression))
        {
        }

        protected override string GetLanguageName()
            => LanguageNames.Stark;

        protected override ISyntaxFactsService GetSyntaxFactsService()
            => CSharpSyntaxFactsService.Instance;

        protected override bool CanSimplifyTypeNameExpression(
            SemanticModel model, MemberAccessExpressionSyntax node, OptionSet optionSet,
            out TextSpan issueSpan, CancellationToken cancellationToken)
        {
            return node.TryReduceOrSimplifyExplicitName(model, out _, out issueSpan, optionSet, cancellationToken);
        }
    }
}
