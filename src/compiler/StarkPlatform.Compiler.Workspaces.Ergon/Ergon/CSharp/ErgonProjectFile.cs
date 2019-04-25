// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Ergon.Core;
using StarkPlatform.Compiler.Ergon.Build;
using StarkPlatform.Compiler.Ergon.Constants;
using StarkPlatform.Compiler.Ergon.Logging;

namespace StarkPlatform.Compiler.Ergon.CSharp
{
    internal class ErgonProjectFile : ProjectFile.ProjectFile
    {
        public ErgonProjectFile(ErgonProjectFileLoader loader, ErgonProject project, ProjectBuildManager buildManager, DiagnosticLog log)
            : base(loader, project, buildManager, log)
        {
        }

        protected override SourceCodeKind GetSourceCodeKind(string documentFileName)
            => SourceCodeKind.Regular;

        public override string GetDocumentExtension(SourceCodeKind sourceCodeKind)
            => ".sk";

        protected override IEnumerable<ITaskItem> GetCompilerCommandLineArgs(ErgonProjectInstance executedProject)
            => executedProject.GetItems(ItemNames.StarkcCommandLineArgs);

        protected override ImmutableArray<string> ReadCommandLineArgs(ErgonProjectInstance project)
            => ErgonCommandLineArgumentReader.Read(project);
    }
}
