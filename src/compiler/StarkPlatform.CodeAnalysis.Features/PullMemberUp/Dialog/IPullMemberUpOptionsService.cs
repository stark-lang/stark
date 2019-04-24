// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.  

using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.PullMemberUp;

namespace StarkPlatform.CodeAnalysis.CodeRefactorings.PullMemberUp.Dialog
{
    internal interface IPullMemberUpOptionsService : IWorkspaceService
    {
        PullMembersUpOptions GetPullMemberUpOptions(Document document, ISymbol selectedNodeSymbol);
    }
}
