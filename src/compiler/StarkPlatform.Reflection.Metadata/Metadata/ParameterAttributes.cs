using System;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>Defines the attributes that can be associated with a parameter. These are defined in CorHdr.h.</summary>
    [Flags]
    public enum ParameterAttributes
    {
        /// <summary>Specifies that the parameter has a default value.</summary>
        /// <returns></returns>
        HasDefault = 4096, // 0x00001000
                           /// <summary>Specifies that the parameter has field marshaling information.</summary>
                           /// <returns></returns>
        HasFieldMarshal = 8192, // 0x00002000
                                /// <summary>Specifies that the parameter is an input parameter.</summary>
                                /// <returns></returns>
        In = 1,
        /// <summary>Specifies that the parameter is a locale identifier (lcid).</summary>
        /// <returns></returns>
        Lcid = 4,
        /// <summary>Specifies that there is no parameter attribute.</summary>
        /// <returns></returns>
        None = 0,
        /// <summary>Specifies that the parameter is optional.</summary>
        /// <returns></returns>
        Optional = 16, // 0x00000010
                       /// <summary>Specifies that the parameter is an output parameter.</summary>
                       /// <returns></returns>
        Out = 2,
        /// <summary>Reserved.</summary>
        /// <returns></returns>
        Reserved3 = 16384, // 0x00004000
                           /// <summary>Reserved.</summary>
                           /// <returns></returns>
        Reserved4 = 32768, // 0x00008000
                           /// <summary>Specifies that the parameter is reserved.</summary>
                           /// <returns></returns>
        ReservedMask = Reserved4 | Reserved3 | HasFieldMarshal | HasDefault, // 0x0000F000
                                                                             /// <summary>Specifies that the parameter is a return value.</summary>
                                                                             /// <returns></returns>
        Retval = 8,
    }
}
