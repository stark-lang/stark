// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.OrganizeImports;

namespace StarkPlatform.CodeAnalysis.Stark.OrganizeImports
{
    [ExportLanguageService(typeof(IOrganizeImportsService), LanguageNames.Stark), Shared]
    internal partial class CSharpOrganizeImportsService : IOrganizeImportsService
    {
        public async Task<Document> OrganizeImportsAsync(Document document, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var options = await document.GetOptionsAsync(cancellationToken).ConfigureAwait(false);

            var placeSystemNamespaceFirst = options.GetOption(GenerationOptions.PlaceSystemNamespaceFirst);
            var blankLineBetweenGroups = options.GetOption(GenerationOptions.SeparateImportDirectiveGroups);

            var rewriter = new Rewriter(placeSystemNamespaceFirst, blankLineBetweenGroups);
            var newRoot = rewriter.Visit(root);

            return document.WithSyntaxRoot(newRoot);
        }

        public string SortAndRemoveUnusedImportsDisplayStringWithAccelerator =>
            CSharpFeaturesResources.Remove_and_Sort_Usings;
    }
}
