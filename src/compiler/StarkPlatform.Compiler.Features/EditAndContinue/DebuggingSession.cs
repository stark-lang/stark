﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;

namespace StarkPlatform.Compiler.EditAndContinue
{
    /// <summary>
    /// Represents a debugging session.
    /// </summary>
    internal sealed class DebuggingSession
    {
        public readonly Solution InitialSolution;

        internal DebuggingSession(Solution initialSolution)
        {
            Debug.Assert(initialSolution != null);
            InitialSolution = initialSolution;
        }
    }
}
