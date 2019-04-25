// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.LanguageServices;
using StarkPlatform.Compiler.Notification;

namespace StarkPlatform.Compiler.ExtractInterface
{
    internal interface IExtractInterfaceOptionsService : IWorkspaceService
    {
        Task<ExtractInterfaceOptionsResult> GetExtractInterfaceOptionsAsync(
            ISyntaxFactsService syntaxFactsService,
            INotificationService notificationService,
            List<ISymbol> extractableMembers,
            string defaultInterfaceName,
            List<string> conflictingTypeNames,
            string defaultNamespace,
            string generatedNameTypeParameterSuffix,
            string languageName);
    }
}
