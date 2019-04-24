// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.NameTupleElement;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.NameTupleElement
{
    [ExtensionOrder(After = PredefinedCodeRefactoringProviderNames.IntroduceVariable)]
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = nameof(CSharpNameTupleElementCodeRefactoringProvider)), Shared]
    internal class CSharpNameTupleElementCodeRefactoringProvider : AbstractNameTupleElementCodeRefactoringProvider<ArgumentSyntax, TupleExpressionSyntax>
    {
        protected override bool IsCloseParenOrComma(SyntaxToken token)
            => token.IsKind(SyntaxKind.CloseParenToken, SyntaxKind.CommaToken);

        protected override ArgumentSyntax WithName(ArgumentSyntax argument, string argumentName)
            => argument.WithNameColon(SyntaxFactory.NameColon(argumentName.ToIdentifierName()));
    }
}
