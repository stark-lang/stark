// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Remote
{
    /// <summary>
    /// Default implementation of IRemoteHostClientService
    /// </summary>
    [ExportWorkspaceServiceFactory(typeof(IRemoteHostClientService)), Shared]
    internal partial class DefaultRemoteHostClientServiceFactory : IWorkspaceServiceFactory
    {
        public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices)
        {
            return new RemoteHostClientService(workspaceServices.Workspace);
        }
    }
}
