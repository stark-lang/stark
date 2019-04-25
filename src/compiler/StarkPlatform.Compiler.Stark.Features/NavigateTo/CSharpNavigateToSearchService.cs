// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.NavigateTo;

namespace StarkPlatform.Compiler.Stark.NavigateTo
{
    [ExportLanguageService(typeof(INavigateToSearchService_RemoveInterfaceAboveAndRenameThisAfterInternalsVisibleToUsersUpdate), LanguageNames.Stark), Shared]
    internal class CSharpNavigateToSearchService : AbstractNavigateToSearchService
    {
    }
}
