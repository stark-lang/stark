// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.SolutionSize;
using StarkPlatform.Compiler.SQLite;

namespace StarkPlatform.Compiler.Storage
{
    [ExportWorkspaceServiceFactory(typeof(IPersistentStorageService), ServiceLayer.Desktop), Shared]
    internal class PersistenceStorageServiceFactory : IWorkspaceServiceFactory
    {
        private readonly object _gate = new object();
        private readonly ISolutionSizeTracker _solutionSizeTracker;

        [ImportingConstructor]
        public PersistenceStorageServiceFactory(ISolutionSizeTracker solutionSizeTracker)
        {
            _solutionSizeTracker = solutionSizeTracker;
        }

        public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices)
        {
            var optionService = workspaceServices.GetRequiredService<IOptionService>();
            var database = optionService.GetOption(StorageOptions.Database);
            switch (database)
            {
                case StorageDatabase.SQLite:
                    var locationService = workspaceServices.GetService<IPersistentStorageLocationService>();
                    if (locationService != null)
                    {
                        return new SQLitePersistentStorageService(optionService, locationService, _solutionSizeTracker);
                    }

                    break;
            }

            return NoOpPersistentStorageService.Instance;
        }
    }
}
