// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace StarkPlatform.CodeAnalysis.Stark
{
    [Flags]
    internal enum DeclarationModifiers
    {
        None = 0,
        Abstract = 1 << 0,
        Sealed = 1 << 1,
        Static = 1 << 2,
        New = 1 << 3,
        Public = 1 << 4,
        Protected = 1 << 5,
        Internal = 1 << 6,
        ProtectedInternal = 1 << 7, // the two keywords together are treated as one modifier
        Private = 1 << 8,
        PrivateProtected = 1 << 9, // the two keywords together are treated as one modifier
        Let = 1 << 10,
        Transient = 1 << 11,
        Const = 1 << 12,
        Volatile = 1 << 13,

        Extern = 1 << 14,
        Partial = 1 << 15,
        Unsafe = 1 << 16,
        Fixed = 1 << 17,
        Virtual = 1 << 18, // used for method binding
        Override = 1 << 19, // used for method binding

        Indexer = 1 << 20, // not a real modifier, but used to record that indexer syntax was used. 

        Async = 1 << 21,
        Ref = 1 << 22, // used only for structs

        All = (1 << 23) - 1, // all modifiers
        Unset = 1 << 23, // used when a modifiers value hasn't yet been computed

        AccessibilityMask = PrivateProtected | Private | Protected | Internal | ProtectedInternal | Public,
    }
}
