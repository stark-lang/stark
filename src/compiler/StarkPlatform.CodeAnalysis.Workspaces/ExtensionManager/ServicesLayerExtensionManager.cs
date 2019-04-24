// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Extensions
{
    [ExportWorkspaceServiceFactory(typeof(IExtensionManager), ServiceLayer.Default), Shared]
    internal class ServicesLayerExtensionManager : IWorkspaceServiceFactory
    {
        public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices)
        {
            return new ExtensionManager();
        }

        private class ExtensionManager : AbstractExtensionManager
        {
        }
    }
}
