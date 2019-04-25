// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.GeneratedCodeRecognition;
using StarkPlatform.Compiler.Host.Mef;

namespace StarkPlatform.Compiler.Stark.GeneratedCodeRecognition
{
    [ExportLanguageService(typeof(IGeneratedCodeRecognitionService), LanguageNames.Stark), Shared]
    internal class CSharpGeneratedCodeRecognitionService : AbstractGeneratedCodeRecognitionService
    {
    }
}
