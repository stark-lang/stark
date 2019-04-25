// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler
{
    /// <summary>
    /// Describes the kind of the namespace extent.
    /// </summary>
    public enum NamespaceKind
    {
        Module = 1,
        Assembly = 2,
        Compilation = 3
    }
}
