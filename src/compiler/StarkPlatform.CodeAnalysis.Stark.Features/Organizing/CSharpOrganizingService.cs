// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.Organizing;
using StarkPlatform.CodeAnalysis.Organizing.Organizers;

namespace StarkPlatform.CodeAnalysis.Stark.Organizing
{
    [ExportLanguageService(typeof(IOrganizingService), LanguageNames.Stark), Shared]
    internal partial class CSharpOrganizingService : AbstractOrganizingService
    {
        [ImportingConstructor]
        public CSharpOrganizingService(
            [ImportMany]IEnumerable<Lazy<ISyntaxOrganizer, LanguageMetadata>> organizers)
            : base(organizers.Where(o => o.Metadata.Language == LanguageNames.Stark).Select(o => o.Value))
        {
        }

        protected override async Task<Document> ProcessAsync(Document document, IEnumerable<ISyntaxOrganizer> organizers, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var rewriter = new Rewriter(this, organizers, await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false), cancellationToken);
            return document.WithSyntaxRoot(rewriter.Visit(root));
        }
    }
}
