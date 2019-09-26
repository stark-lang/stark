using System;

namespace StarkPlatform.Reflection.Metadata
{
    [Flags]
    public enum TypeAccessModifiers
    {
        None = 0, // mutable, non ref, not transient

        Ref = 1 << 0,

        ReadOnly = 1 << 1,

        Transient = 1 << 2,

        Immutable = 1 << 3,

        Isolated = 1 << 4,
    }
}