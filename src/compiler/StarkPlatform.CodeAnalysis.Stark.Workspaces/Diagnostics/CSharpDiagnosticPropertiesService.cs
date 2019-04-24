// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.Diagnostics
{
    [ExportLanguageService(typeof(IDiagnosticPropertiesService), LanguageNames.Stark), Shared]
    internal class CSharpDiagnosticPropertiesService : AbstractDiagnosticPropertiesService
    {
        private static readonly Compilation s_compilation = CSharpCompilation.Create("empty");

        protected override Compilation GetCompilation() => s_compilation;
    }
}
