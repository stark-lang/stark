// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler
{
    /// <summary>
    /// Represents an array.
    /// </summary>
    /// <remarks>
    /// This interface is reserved for implementation by its associated APIs. We reserve the right to
    /// change it in the future.
    /// </remarks>
    public interface IArrayTypeSymbol : ITypeWithElementTypeSymbol
    {
        /// <summary>
        /// Custom modifiers associated with the array type, or an empty array if there are none.
        /// </summary>
        ImmutableArray<CustomModifier> CustomModifiers { get; }

        bool Equals(IArrayTypeSymbol other);
    }
}
