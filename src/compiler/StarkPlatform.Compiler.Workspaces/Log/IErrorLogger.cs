﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using StarkPlatform.Compiler.Host;

namespace StarkPlatform.Compiler.ErrorLogger
{
    internal interface IErrorLoggerService : IWorkspaceService
    {
        void LogException(object source, Exception exception);
    }
}