// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Host;

namespace StarkPlatform.Compiler.LanguageServices.ProjectInfoService
{
    internal interface IProjectInfoService : IWorkspaceService
    {
        bool GeneratedTypesMustBePublic(Project project);
    }
}
