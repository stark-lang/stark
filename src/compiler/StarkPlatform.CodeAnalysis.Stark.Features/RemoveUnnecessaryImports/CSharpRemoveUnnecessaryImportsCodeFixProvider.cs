// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.RemoveUnnecessaryImports;

namespace StarkPlatform.CodeAnalysis.Stark.RemoveUnnecessaryImports
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.RemoveUnnecessaryImports), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.AddMissingReference)]
    internal class CSharpRemoveUnnecessaryImportsCodeFixProvider : AbstractRemoveUnnecessaryImportsCodeFixProvider
    {
        protected override string GetTitle()
            => CSharpFeaturesResources.Remove_Unnecessary_Usings;
    }
}
