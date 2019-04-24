// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Simplification;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Simplification
{
    internal abstract partial class AbstractCSharpReducer : AbstractReducer
    {
        protected AbstractCSharpReducer(ObjectPool<IReductionRewriter> pool) : base(pool)
        {
        }
    }
}
