// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Navigation
{
    [ExportWorkspaceServiceFactory(typeof(ISymbolNavigationService), ServiceLayer.Default), Shared]
    internal class DefaultSymbolNavigationServiceFactory : IWorkspaceServiceFactory
    {
        private ISymbolNavigationService _singleton;

        public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices)
        {
            if (_singleton == null)
            {
                _singleton = new DefaultSymbolNavigationService();
            }

            return _singleton;
        }
    }
}
