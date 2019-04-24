// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.GeneratedCodeRecognition;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.GeneratedCodeRecognition
{
    [ExportLanguageService(typeof(IGeneratedCodeRecognitionService), LanguageNames.Stark), Shared]
    internal class CSharpGeneratedCodeRecognitionService : AbstractGeneratedCodeRecognitionService
    {
    }
}
