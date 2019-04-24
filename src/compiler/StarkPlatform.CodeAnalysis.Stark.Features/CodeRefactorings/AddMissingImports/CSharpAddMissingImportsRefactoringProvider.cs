// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.AddMissingImports;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.PasteTracking;

namespace StarkPlatform.CodeAnalysis.Stark.CodeRefactorings.AddMissingImports
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = PredefinedCodeRefactoringProviderNames.AddMissingImports), Shared]
    internal class CSharpAddMissingImportsRefactoringProvider : AbstractAddMissingImportsRefactoringProvider
    {
        protected override string CodeActionTitle => CSharpFeaturesResources.Add_missing_usings;

        [ImportingConstructor]
        public CSharpAddMissingImportsRefactoringProvider(IPasteTrackingService pasteTrackingService)
            : base(pasteTrackingService)
        {
        }
    }
}
