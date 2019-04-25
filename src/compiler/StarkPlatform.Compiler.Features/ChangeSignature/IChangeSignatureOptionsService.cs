// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Notification;

namespace StarkPlatform.Compiler.ChangeSignature
{
    internal interface IChangeSignatureOptionsService : IWorkspaceService
    {
        ChangeSignatureOptionsResult GetChangeSignatureOptions(
            ISymbol symbol,
            ParameterConfiguration parameters,
            INotificationService notificationService);
    }
}
