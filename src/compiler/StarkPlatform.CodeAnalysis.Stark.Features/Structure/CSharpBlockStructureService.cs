// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.Structure;

namespace StarkPlatform.CodeAnalysis.Stark.Structure
{
    [ExportLanguageServiceFactory(typeof(BlockStructureService), LanguageNames.Stark), Shared]
    internal class CSharpBlockStructureServiceFactory : ILanguageServiceFactory
    {
        public ILanguageService CreateLanguageService(HostLanguageServices languageServices)
        {
            return new CSharpBlockStructureService(languageServices.WorkspaceServices.Workspace);
        }
    }

    internal class CSharpBlockStructureService : BlockStructureServiceWithProviders
    {
        public CSharpBlockStructureService(Workspace workspace) : base(workspace)
        {
        }

        protected override ImmutableArray<BlockStructureProvider> GetBuiltInProviders()
        {
            return ImmutableArray.Create<BlockStructureProvider>(
                new CSharpBlockStructureProvider());
        }

        public override string Language => LanguageNames.Stark;
    }
}
