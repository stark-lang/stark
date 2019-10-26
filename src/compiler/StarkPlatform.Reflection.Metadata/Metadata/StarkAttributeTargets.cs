using System;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>Specifies the application elements on which it is valid to apply an attribute.</summary>
    [Flags]
    public enum StarkAttributeTargets
    {
        /// <summary>Attribute can be applied to an assembly.</summary>
        /// <returns></returns>
        Assembly = 1,

        /// <summary>Attribute can be applied to a class.</summary>
        /// <returns></returns>
        Class = 4,

        /// <summary>Attribute can be applied to a constructor.</summary>
        /// <returns></returns>
        Constructor = 32, // 0x00000020

        /// <summary>Attribute can be applied to a delegate.</summary>
        /// <returns></returns>
        Delegate = 4096, // 0x00001000

        /// <summary>Attribute can be applied to an enumeration.</summary>
        /// <returns></returns>
        Enum = 16, // 0x00000010

        /// <summary>Attribute can be applied to an event.</summary>
        /// <returns></returns>
        Event = 512, // 0x00000200

        /// <summary>Attribute can be applied to a field.</summary>
        /// <returns></returns>
        Field = 256, // 0x00000100

        /// <summary>Attribute can be applied to a generic parameter.</summary>
        /// <returns></returns>
        GenericParameter = 16384, // 0x00004000

        /// <summary>Attribute can be applied to an interface.</summary>
        /// <returns></returns>
        Interface = 1024, // 0x00000400

        /// <summary>Attribute can be applied to a method.</summary>
        /// <returns></returns>
        Func = 64, // 0x00000040

        /// <summary>Attribute can be applied to a module.</summary>
        /// <returns></returns>
        Module = 2,

        /// <summary>Attribute can be applied to a parameter.</summary>
        /// <returns></returns>
        Parameter = 2048, // 0x00000800

        /// <summary>Attribute can be applied to a property.</summary>
        /// <returns></returns>
        Property = 128, // 0x00000080

        /// <summary>Attribute can be applied to a return value.</summary>
        /// <returns></returns>
        ReturnValue = 8192, // 0x00002000

        /// <summary>Attribute can be applied to a structure; that is, a value type.</summary>
        /// <returns></returns>
        Struct = 8,

        /// <summary>Attribute can be applied to a static function only</summary>
        StaticFunc = 0x8000,

        /// <summary>Attribute can be applied to an extern static function only</summary>
        ExternStaticFunc = 0x10000,

        /// <summary>Attribute can be applied to an extern function only</summary>
        ExternFunc = 0x20000,
        
        /// <summary>Attribute can be applied to any application element.</summary>
        /// <returns></returns>
        All = 0x3FFFF
    }
}