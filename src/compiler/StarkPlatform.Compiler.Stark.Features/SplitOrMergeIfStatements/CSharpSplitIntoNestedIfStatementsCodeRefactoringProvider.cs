// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.CodeRefactorings;
using StarkPlatform.Compiler.SplitOrMergeIfStatements;

namespace StarkPlatform.Compiler.Stark.SplitOrMergeIfStatements
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = PredefinedCodeRefactoringProviderNames.SplitIntoNestedIfStatements), Shared]
    [ExtensionOrder(After = PredefinedCodeRefactoringProviderNames.InvertLogical, Before = PredefinedCodeRefactoringProviderNames.IntroduceVariable)]
    internal sealed class CSharpSplitIntoNestedIfStatementsCodeRefactoringProvider
        : AbstractSplitIntoNestedIfStatementsCodeRefactoringProvider
    {
    }
}
