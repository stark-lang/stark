// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Organizing.Organizers;

namespace StarkPlatform.CodeAnalysis.Organizing
{
    /// <summary>
    /// internal interface used to use language specific service from common service layer
    /// </summary>
    internal interface IOrganizingService : ILanguageService
    {
        /// <summary>
        /// return default organizers
        /// </summary>
        IEnumerable<ISyntaxOrganizer> GetDefaultOrganizers();

        /// <summary>
        /// Organize document
        /// </summary>
        Task<Document> OrganizeAsync(Document document, IEnumerable<ISyntaxOrganizer> organizers, CancellationToken cancellationToken);
    }
}
