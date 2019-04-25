// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.QuickInfo;

namespace StarkPlatform.Compiler.Stark.QuickInfo
{
    [ExportLanguageServiceFactory(typeof(QuickInfoService), LanguageNames.Stark), Shared]
    internal class CSharpQuickInfoServiceFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices languageServices)
        {
            return new CSharpQuickInfoService(languageServices.WorkspaceServices.Workspace);
        }
    }

    internal class CSharpQuickInfoService : QuickInfoServiceWithProviders
    {
        internal CSharpQuickInfoService(Workspace workspace)
            : base(workspace, LanguageNames.Stark)
        {
        }
    }
}

