// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.EditAndContinue;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Debugging
{
    [ExportWorkspaceServiceFactory(typeof(IDebuggingWorkspaceService), ServiceLayer.Host), Shared]
    internal sealed class DebuggingWorkspaceServiceFactory : IWorkspaceServiceFactory
    {
        private readonly IEditAndContinueService _editAndContinueServiceOpt;

        [ImportingConstructor]
        public DebuggingWorkspaceServiceFactory([Import(AllowDefault = true)]IEditAndContinueService editAndContinueServiceOpt)
        {
            _editAndContinueServiceOpt = editAndContinueServiceOpt;
        }

        public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices)
            => new DebuggingWorkspaceService(_editAndContinueServiceOpt);
    }
}
