// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.EncapsulateField
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = PredefinedCodeRefactoringProviderNames.EncapsulateField), Shared]
    internal class EncapsulateFieldRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var service = context.Document.GetLanguageService<AbstractEncapsulateFieldService>();
            var actions = await service.GetEncapsulateFieldCodeActionsAsync(context.Document, context.Span, context.CancellationToken).ConfigureAwait(false);
            context.RegisterRefactorings(actions);
        }
    }
}
