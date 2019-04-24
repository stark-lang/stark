// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.Shared.TestHooks;

namespace StarkPlatform.CodeAnalysis.Notification
{
    [ExportWorkspaceServiceFactory(typeof(IGlobalOperationNotificationService), ServiceLayer.Default), Shared]
    internal class GlobalOperationNotificationServiceFactory : IWorkspaceServiceFactory
    {
        private readonly IAsynchronousOperationListener _listener;
        private readonly IGlobalOperationNotificationService _singleton;

        [ImportingConstructor]
        public GlobalOperationNotificationServiceFactory(IAsynchronousOperationListenerProvider listenerProvider)
        {
            _listener = listenerProvider.GetListener(FeatureAttribute.GlobalOperation);
            _singleton = new GlobalOperationNotificationService(_listener);
        }

        public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices)
        {
            return _singleton;
        }
    }
}
