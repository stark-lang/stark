// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using StarkPlatform.CodeAnalysis.Host;

namespace StarkPlatform.CodeAnalysis.Execution
{
    /// <summary>
    /// This lets consumer to get to inner temporary storage that references use
    /// as its shadow copy storage
    /// </summary>
    internal interface ISupportTemporaryStorage
    {
        IEnumerable<ITemporaryStreamStorage> GetStorages();
    }
}
