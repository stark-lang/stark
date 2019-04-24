// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Notification;

namespace StarkPlatform.CodeAnalysis.ChangeSignature
{
    internal interface IChangeSignatureOptionsService : IWorkspaceService
    {
        ChangeSignatureOptionsResult GetChangeSignatureOptions(
            ISymbol symbol,
            ParameterConfiguration parameters,
            INotificationService notificationService);
    }
}
