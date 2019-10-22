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

        Readable = 1 << 3,

        Transient = 1 << 4,

        Immutable = 1 << 5,

        Isolated = 1 << 6,

        Mutable = 1 << 7,

        Const = 1 << 8,

        Unsafe = 1 << 9,
    }
}