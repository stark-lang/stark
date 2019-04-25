// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Host;

namespace StarkPlatform.Compiler.CodeActions.WorkspaceServices
{
    internal interface ISymbolRenamedCodeActionOperationFactoryWorkspaceService : IWorkspaceService
    {
        CodeActionOperation CreateSymbolRenamedOperation(ISymbol symbol, string newName, Solution startingSolution, Solution updatedSolution);
    }
}
