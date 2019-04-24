// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace StarkPlatform.CodeAnalysis.Shared.Utilities
{
    internal interface IStreamingProgressTracker
    {
        Task AddItemsAsync(int count);
        Task ItemCompletedAsync();
    }
}
