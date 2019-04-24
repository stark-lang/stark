// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Ergon.Core;
using StarkPlatform.CodeAnalysis.Ergon.Build;
using StarkPlatform.CodeAnalysis.Ergon.Logging;
using StarkPlatform.CodeAnalysis.Ergon.ProjectFile;

namespace StarkPlatform.CodeAnalysis.Ergon.CSharp
{
    internal partial class ErgonProjectFileLoader : ProjectFileLoader
    {
        public ErgonProjectFileLoader()
        {
        }

        public override string Language
        {
            get { return LanguageNames.Stark; }
        }

        protected override ProjectFile.ProjectFile CreateProjectFile(ErgonProject project, ProjectBuildManager buildManager, DiagnosticLog log)
        {
            return new ErgonProjectFile(this, project, buildManager, log);
        }
    }
}
