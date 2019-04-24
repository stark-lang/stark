// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.SolutionCrawler;

namespace StarkPlatform.CodeAnalysis.Stark.SolutionCrawler
{
    [ExportLanguageService(typeof(IDocumentDifferenceService), LanguageNames.Stark), Shared]
    internal class CSharpDocumentDifferenceService : AbstractDocumentDifferenceService
    {
    }
}
