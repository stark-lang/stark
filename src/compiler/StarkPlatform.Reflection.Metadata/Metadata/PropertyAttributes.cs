using System;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>Defines the attributes that can be associated with a property. These attribute values are defined in corhdr.h.</summary>
    [Flags]
    public enum PropertyAttributes
    {
        /// <summary>Specifies that the property has a default value.</summary>
        /// <returns></returns>
        HasDefault = 4096, // 0x00001000
        /// <summary>Specifies that no attributes are associated with a property.</summary>
        /// <returns></returns>
        None = 0,
        /// <summary>Reserved.</summary>
        /// <returns></returns>
        Reserved2 = 8192, // 0x00002000
        /// <summary>Reserved.</summary>
        /// <returns></returns>
        Reserved3 = 16384, // 0x00004000
        /// <summary>Reserved.</summary>
        /// <returns></returns>
        Reserved4 = 32768, // 0x00008000
        /// <summary>Specifies a flag reserved for runtime use only.</summary>
        /// <returns></returns>
        ReservedMask = 62464, // 0x0000F400
        /// <summary>Specifies that the metadata internal APIs check the name encoding.</summary>
        /// <returns></returns>
        RTSpecialName = 1024, // 0x00000400
        /// <summary>Specifies that the property is special, with the name describing how the property is special.</summary>
        /// <returns></returns>
        SpecialName = 512, // 0x00000200
    }
}