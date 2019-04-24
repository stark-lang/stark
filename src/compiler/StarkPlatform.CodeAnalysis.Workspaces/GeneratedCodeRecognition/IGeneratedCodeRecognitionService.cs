// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Host;

namespace StarkPlatform.CodeAnalysis.GeneratedCodeRecognition
{
    internal interface IGeneratedCodeRecognitionService : ILanguageService
    {
        bool IsGeneratedCode(Document document, CancellationToken cancellationToken);
    }
}
