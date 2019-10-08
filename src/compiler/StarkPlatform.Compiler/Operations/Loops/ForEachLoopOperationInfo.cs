// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;

namespace StarkPlatform.Compiler.Operations
{
    internal class ForEachLoopOperationInfo
    {
        /// <summary>
        /// Element type of the collection
        /// </summary>
        public readonly ITypeSymbol ElementType;

        public readonly ITypeSymbol IteratorType;

        public readonly IMethodSymbol IterateBegin;
        public readonly IMethodSymbol IterateHasNext;
        public readonly IMethodSymbol IterateNext;
        public readonly IMethodSymbol IterateEnd;

        public readonly ImmutableArray<IArgumentOperation> IterateBeginArguments;
        public readonly ImmutableArray<IArgumentOperation> IterateHasNextArguments;
        public readonly ImmutableArray<IArgumentOperation> IterateNextArguments;
        public readonly ImmutableArray<IArgumentOperation> IterateEndArguments;

        public ForEachLoopOperationInfo(
            ITypeSymbol elementType,
            ITypeSymbol iteratorType,
            IMethodSymbol iterateBegin,
            IMethodSymbol iterateHasNext,
            IMethodSymbol iterateNext,
            IMethodSymbol iterateEnd,
            ImmutableArray<IArgumentOperation> iterateBeginArguments = default,
            ImmutableArray<IArgumentOperation> iterateHasNextArguments = default,
            ImmutableArray<IArgumentOperation> iterateNextArguments = default,
            ImmutableArray<IArgumentOperation> iterateEndArguments = default)
        {
            ElementType = elementType;
            IteratorType = iteratorType;
            IterateBegin = iterateBegin;
            IterateHasNext = iterateHasNext;
            IterateNext = iterateNext;
            IterateEnd = iterateEnd;
            IterateBeginArguments = iterateBeginArguments;
            IterateHasNextArguments = iterateHasNextArguments;
            IterateNextArguments = iterateNextArguments;
            IterateEndArguments = iterateEndArguments;
        }
    }
}
