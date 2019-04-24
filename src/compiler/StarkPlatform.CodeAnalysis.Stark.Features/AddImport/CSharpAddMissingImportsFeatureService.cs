// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.AddMissingImports;
using StarkPlatform.CodeAnalysis.Stark.AddImport;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.AddMissingImports
{
    [ExportLanguageService(typeof(IAddMissingImportsFeatureService), LanguageNames.Stark), Shared]
    internal class CSharpAddMissingImportsFeatureService : AbstractAddMissingImportsFeatureService
    {
        protected sealed override ImmutableArray<string> FixableDiagnosticIds => AddImportDiagnosticIds.FixableDiagnosticIds;
    }
}
