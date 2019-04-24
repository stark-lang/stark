// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.CodeRefactorings.AddAwait;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.CodeRefactorings.AddAwait
{
    /// <summary>
    /// This refactoring complements the AddAwait fixer. It allows adding `await` and `await ... .ConfigureAwait(false)` even there is no compiler error to trigger the fixer.
    /// </summary>
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = PredefinedCodeRefactoringProviderNames.AddAwait), Shared]
    internal partial class CSharpAddAwaitCodeRefactoringProvider : AbstractAddAwaitCodeRefactoringProvider<ExpressionSyntax, InvocationExpressionSyntax>
    {
        protected override string GetTitle()
            => CSharpFeaturesResources.Add_await;

        protected override string GetTitleWithConfigureAwait()
            => CSharpFeaturesResources.Add_Await_and_ConfigureAwaitFalse;
    }
}
