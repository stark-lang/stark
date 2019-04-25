// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.LanguageServices;
using StarkPlatform.Compiler.Notification;
using StarkPlatform.Compiler.ProjectManagement;

namespace StarkPlatform.Compiler.GenerateType
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
