// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.RemoveUnnecessaryImports;

namespace StarkPlatform.CodeAnalysis.Stark.RemoveUnnecessaryImports
{
    [ExportLanguageService(typeof(IRemoveUnnecessaryImportsService), LanguageNames.Stark), Shared]
    internal partial class CSharpRemoveUnnecessaryImportsService : AbstractCSharpRemoveUnnecessaryImportsService
    {
    }
}
