// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Host;

namespace StarkPlatform.CodeAnalysis.DesignerAttributes
{
    internal interface IDesignerAttributeService : ILanguageService
    {
        Task<DesignerAttributeResult> ScanDesignerAttributesAsync(Document document, CancellationToken cancellationToken);
    }
}
