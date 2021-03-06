﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace StarkPlatform.Compiler.EditAndContinue
{
    internal sealed class EncDebuggingSessionInfo
    {
        public readonly List<EncEditSessionInfo> EditSessions = new List<EncEditSessionInfo>();
        public int EmptyEditSessions { get; private set; }

        internal void EndEditSession(EncEditSessionInfo encEditSessionInfo)
        {
            if (encEditSessionInfo.IsEmpty())
            {
                EmptyEditSessions++;
            }
            else
            {
                EditSessions.Add(encEditSessionInfo);
            }
        }
    }
}
