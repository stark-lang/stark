// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;

namespace StarkPlatform.Compiler.CodeLens
{
    [ExportWorkspaceServiceFactory(typeof(ICodeLensReferencesService)), Shared]
    internal sealed class CodeLensReferencesServiceFactory : IWorkspaceServiceFactory
    {
        public static readonly ICodeLensReferencesService Instance = new CodeLensReferencesService();

        public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices)
        {
            return Instance;
        }
    }
}
