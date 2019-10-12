// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    /// <summary>
    /// Structure containing all semantic information about a for each statement.
    /// </summary>
    public struct ForEachStatementInfo : IEquatable<ForEachStatementInfo>
    {
        public IMethodSymbol IterateBegin { get; }

        public IMethodSymbol IterateHasCurrent { get; }

        public IMethodSymbol IterateCurrent { get; }

        public IMethodSymbol IterateNext { get; }

        public IMethodSymbol IterateEnd { get; }

        /// <summary>
        /// The intermediate type to which the output of the <see cref="IterateNext"/> is converted
        /// before being converted to the iteration variable type.
        /// </summary>
        /// <remarks>
        /// As you might hope, for an array, it is the element type of the array.
        /// </remarks>
        public ITypeSymbol ElementType { get; }

        /// <summary>
        /// The intermediate type to which the output of the <see cref="IterateNext"/> is converted
        /// before being converted to the iteration variable type.
        /// </summary>
        /// <remarks>
        /// As you might hope, for an array, it is the element type of the array.
        /// </remarks>
        public ITypeSymbol IteratorType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForEachStatementInfo" /> structure.
        /// </summary>
        internal ForEachStatementInfo(IMethodSymbol iterateBegin,
                                      IMethodSymbol iterateHasCurrent,
                                      IMethodSymbol iterateCurrent,
                                      IMethodSymbol iterateNext,
                                      IMethodSymbol iterateEnd,
                                      ITypeSymbol elementType,
                                      ITypeSymbol iteratorType
                                      )
        {
            this.IterateBegin = iterateBegin;
            this.IterateHasCurrent = iterateHasCurrent;
            this.IterateCurrent = iterateCurrent;
            this.IterateNext = iterateNext;
            this.IterateEnd = iterateEnd;
            this.ElementType = elementType;
            this.IteratorType = iteratorType;
        }

        public override bool Equals(object obj)
        {
            return obj is ForEachStatementInfo && Equals((ForEachStatementInfo)obj);
        }

        public bool Equals(ForEachStatementInfo other)
        {
            return object.Equals(this.IterateBegin, other.IterateBegin)
                   && object.Equals(this.IterateHasCurrent, other.IterateHasCurrent)
                   && object.Equals(this.IterateCurrent, other.IterateCurrent)
                   && object.Equals(this.IterateNext, other.IterateNext)
                   && object.Equals(this.IterateEnd, other.IterateEnd)
                   && object.Equals(this.ElementType, other.ElementType)
                   && object.Equals(this.IteratorType, other.IteratorType);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(IterateBegin,
                Hash.Combine(IterateHasCurrent,
                    Hash.Combine(IterateCurrent,
                        Hash.Combine(IterateNext,
                            Hash.Combine(IterateEnd,
                                Hash.Combine(ElementType, IteratorType.GetHashCode()))))));
        }
    }
}
