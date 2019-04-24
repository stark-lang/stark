// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.Notification;
using StarkPlatform.CodeAnalysis.ProjectManagement;

namespace StarkPlatform.CodeAnalysis.GenerateType
{
    internal interface IGenerateTypeOptionsService : IWorkspaceService
    {
        GenerateTypeOptionsResult GetGenerateTypeOptions(
            string className,
            GenerateTypeDialogOptions generateTypeDialogOptions,
            Document document,
            INotificationService notificationService,
            IProjectManagementService projectManagementService,
            ISyntaxFactsService syntaxFactsService);
    }
}
