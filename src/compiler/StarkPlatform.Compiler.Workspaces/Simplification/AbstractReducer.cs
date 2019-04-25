// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.PooledObjects;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Simplification
{
    internal abstract partial class AbstractReducer
    {
        private readonly ObjectPool<IReductionRewriter> _pool;

        protected AbstractReducer(ObjectPool<IReductionRewriter> pool)
            => _pool = pool;

        public IReductionRewriter GetOrCreateRewriter()
            => _pool.Allocate();
    }
}
