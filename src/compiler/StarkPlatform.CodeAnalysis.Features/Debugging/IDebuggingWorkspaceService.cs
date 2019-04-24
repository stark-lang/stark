// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using StarkPlatform.CodeAnalysis.EditAndContinue;
using StarkPlatform.CodeAnalysis.Host;

namespace StarkPlatform.CodeAnalysis.Debugging
{
    internal interface IDebuggingWorkspaceService : IWorkspaceService
    {
        IEditAndContinueService EditAndContinueServiceOpt { get; }

        event EventHandler<DebuggingStateChangedEventArgs> BeforeDebuggingStateChanged;

        void OnBeforeDebuggingStateChanged(DebuggingState before, DebuggingState after);
    }
}
