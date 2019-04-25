// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.Compiler.Packaging;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.AddImport
{
    internal interface IRemoteAddImportFeatureService
    {
        Task<IList<AddImportFixData>> GetFixesAsync(
            DocumentId documentId, TextSpan span, string diagnosticId, int maxResults, bool placeSystemNamespaceFirst,
            bool searchReferenceAssemblies, IList<PackageSource> packageSources, CancellationToken cancellationToken);
    }
}
