﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace StarkPlatform.Compiler
{
    /// <summary>
    /// Specifies the options for how generics are displayed in the description of a symbol.
    /// </summary>
    [Flags]
    public enum SymbolDisplayGenericsOptions
    {
        /// <summary>
        /// Omits the type parameter list entirely.
        /// </summary>
        None = 0,

        /// <summary>
        /// Includes the type parameters. 
        /// For example, "Goo&lt;T&gt;" in C# or "Goo(Of T)" in Visual Basic.
        /// </summary>
        IncludeTypeParameters = 1 << 0,

        /// <summary>
        /// Includes type parameters and constraints.
        /// For example, "where T : new()" in C# or "Of T as New" in Visual Basic.
        /// </summary>
        IncludeTypeConstraints = 1 << 1,

        /// <summary>
        /// Includes <c>in</c> or <c>out</c> keywords before variant type parameters.
        /// For example, "Goo&lt;out T&gt;" in C# or (Goo Of Out T" in Visual Basic.
        /// </summary>
        IncludeVariance = 1 << 2,
    }
}
