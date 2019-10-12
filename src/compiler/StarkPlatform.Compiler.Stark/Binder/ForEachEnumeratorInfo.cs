// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;

namespace StarkPlatform.Compiler.Stark
{
    /// <summary>
    /// Information to be deduced while binding a foreach loop so that the loop can be lowered
    /// to a while over an enumerator.  Not applicable to the array or string forms.
    /// </summary>
    internal sealed class ForEachEnumeratorInfo
    {
        // Types identified by the algorithm in the spec (8.8.4).
        public readonly TypeSymbol IterableType;
        // public readonly TypeSymbol EnumeratorType; // redundant - return type of GetEnumeratorMethod
        public readonly TypeSymbolWithAnnotations ElementType;
        // Used to store the state
        public readonly TypeSymbol IteratorType;

        // Members required by the "pattern" based approach.  Also populated for other approaches.
        public readonly MethodSymbol IterateBegin;
        public readonly MethodSymbol IterateHasCurrent;
        public readonly MethodSymbol IterateCurrent;
        public readonly MethodSymbol IterateNext;
        public readonly MethodSymbol IterateEnd;

        // True if the enumerator needs disposal once used. 
        // Will be either IDisposable/IAsyncDisposable, or use DisposeMethod below if set
        // Computed during initial binding so that we can expose it in the semantic model.
        public readonly bool NeedsDisposal;
        public readonly MethodSymbol DisposeMethod;

        // When async and needs disposal, this stores the information to await the DisposeAsync() invocation
        public AwaitableInfo DisposeAwaitableInfo;

        public readonly BinderFlags Location;

        internal bool IsAsync
            => DisposeAwaitableInfo != null;

        private ForEachEnumeratorInfo(TypeSymbol iterableType,
            TypeSymbolWithAnnotations elementType,
            TypeSymbol iteratorType,
            MethodSymbol iterateBegin,
            MethodSymbol iterateHasCurrent,
            MethodSymbol iterateCurrent,
            MethodSymbol iterateNext,
            MethodSymbol iterateEnd,
            bool needsDisposal,
            AwaitableInfo disposeAwaitableInfo,
            BinderFlags location)
        {
            Debug.Assert((object)iterableType != null, "Field 'collectionType' cannot be null");
            Debug.Assert(!elementType.IsNull, "Field 'elementType' cannot be null");
            Debug.Assert((object)iteratorType != null, "Field 'stateType' cannot be null");
            Debug.Assert((object)iterateBegin != null, "Field 'iterateBegin' cannot be null");
            Debug.Assert((object)iterateNext != null, "Field 'iterateItem' cannot be null");
            Debug.Assert((object)iterateHasCurrent != null, "Field 'iterateNext' cannot be null");
            Debug.Assert((object)iterateEnd != null, "Field 'iterateEnd' cannot be null");

            this.IterableType = iterableType;
            this.ElementType = elementType;
            this.IteratorType = iteratorType;
            this.IterateBegin = iterateBegin;
            this.IterateHasCurrent = iterateHasCurrent;
            this.IterateCurrent = iterateCurrent;
            this.IterateNext = iterateNext;
            this.IterateEnd = iterateEnd;
            this.NeedsDisposal = needsDisposal;
            this.DisposeAwaitableInfo = disposeAwaitableInfo;
            this.Location = location;
        }

        // Mutable version of ForEachEnumeratorInfo.  Convert to immutable using Build.
        internal struct Builder
        {
            public TypeSymbol IterableType { get; set; }
            public TypeSymbolWithAnnotations ElementType;
            public TypeSymbol IteratorType;
            
            public MethodSymbol IterateBegin;
            public MethodSymbol IterateHasCurrent;
            public MethodSymbol IterateCurrent;
            public MethodSymbol IterateNext;
            public MethodSymbol IterateEnd;

            public bool NeedsDisposal;
            public AwaitableInfo DisposeAwaitableInfo;

            public ForEachEnumeratorInfo Build(BinderFlags location)
            {
                Debug.Assert((object)IterableType != null, "'IterableType' cannot be null");
                Debug.Assert((object)ElementType != null, "'ElementType' cannot be null");
                Debug.Assert((object)IteratorType != null, "'StateType' cannot be null");

                Debug.Assert(IterateBegin != null);
                Debug.Assert(IterateHasCurrent != null);
                Debug.Assert(IterateNext != null);
                Debug.Assert(IterateEnd != null);

                return new ForEachEnumeratorInfo(
                    IterableType,
                    ElementType,
                    IteratorType,
                    IterateBegin,
                    IterateHasCurrent,
                    IterateCurrent,
                    IterateNext,
                    IterateEnd,
                    NeedsDisposal,
                    DisposeAwaitableInfo,
                    location);
            }
        }
    }
}
