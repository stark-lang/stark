// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.ChangeSignature
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = PredefinedCodeRefactoringProviderNames.ChangeSignature), Shared]
    internal class ChangeSignatureCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            if (context.Span.IsEmpty)
            {
                var service = context.Document.GetLanguageService<AbstractChangeSignatureService>();
                var actions = await service.GetChangeSignatureCodeActionAsync(context.Document, context.Span, context.CancellationToken).ConfigureAwait(false);
                context.RegisterRefactorings(actions);
            }
        }
    }
}
