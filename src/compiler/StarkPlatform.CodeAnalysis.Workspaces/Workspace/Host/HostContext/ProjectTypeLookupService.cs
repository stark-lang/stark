// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Host.HostContext
{
    [ExportWorkspaceService(typeof(IProjectTypeLookupService), ServiceLayer.Default), Shared]
    internal class ProjectTypeLookupService : IProjectTypeLookupService
    {
        private const string StarkProjectType = "{0D82155C-3060-4DDC-9D49-06522FEDE816}";

        public string GetProjectType(Workspace workspace, ProjectId projectId)
        {
            if (workspace == null || projectId == null)
            {
                return string.Empty;
            }

            var project = workspace.CurrentSolution.GetProject(projectId);
            var language = project?.Language;

            switch (language)
            {
                case LanguageNames.Stark:
                    return StarkProjectType;
                default:
                    return string.Empty;
            }
        }
    }
}
