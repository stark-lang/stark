// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.NavigateTo;

namespace StarkPlatform.CodeAnalysis.Stark.NavigateTo
{
    [ExportLanguageService(typeof(INavigateToSearchService_RemoveInterfaceAboveAndRenameThisAfterInternalsVisibleToUsersUpdate), LanguageNames.Stark), Shared]
    internal class CSharpNavigateToSearchService : AbstractNavigateToSearchService
    {
    }
}
