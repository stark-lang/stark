using System;

namespace StarkPlatform.Reflection.Metadata
{
    [Flags]
    public enum TypeAccessModifiers
    {
        None = 0, // mutable, non ref, not transient

        Ref = 1 << 0,

        ReadOnlyRef = Ref | 1 << 1,

        RetainableRef = Ref | 1 << 2,

        ReadOnly = 1 << 3,

        Transient = 1 << 4,

        Immutable = 1 << 5,

        Isolated = 1 << 6,

        Const = 1 << 7,

        Unsafe = 1 << 8,
    }
}