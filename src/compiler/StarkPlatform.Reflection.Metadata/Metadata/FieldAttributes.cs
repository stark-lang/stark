using System;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>Specifies flags that describe the attributes of a field.</summary>
    [Flags]
    public enum FieldAttributes
    {
        /// <summary>Specifies that the field is accessible throughout the assembly.</summary>
        /// <returns></returns>
        Assembly = 3,
        /// <summary>Specifies that the field is accessible only by subtypes in this assembly.</summary>
        /// <returns></returns>
        FamANDAssem = 2,
        /// <summary>Specifies that the field is accessible only by type and subtypes.</summary>
        /// <returns></returns>
        Family = 4,
        /// <summary>Specifies that the field is accessible by subtypes anywhere, as well as throughout this assembly.</summary>
        /// <returns></returns>
        FamORAssem = 5,
        /// <summary>Specifies the access level of a given field.</summary>
        /// <returns></returns>
        FieldAccessMask = FamORAssem | FamANDAssem, // 0x00000007
                                                    /// <summary>Specifies that the field has a default value.</summary>
                                                    /// <returns></returns>
        HasDefault = 32768, // 0x00008000
                            /// <summary>Specifies that the field has marshaling information.</summary>
                            /// <returns></returns>
        HasFieldMarshal = 4096, // 0x00001000
                                /// <summary>Specifies that the field has a relative virtual address (RVA). The RVA is the location of the method body in the current image, as an address relative to the start of the image file in which it is located.</summary>
                                /// <returns></returns>
        HasFieldRVA = 256, // 0x00000100
                           /// <summary>Specifies that the field is initialized only, and can be set only in the body of a constructor.</summary>
                           /// <returns></returns>
        InitOnly = 32, // 0x00000020
                       /// <summary>Specifies that the field's value is a compile-time (static or early bound) constant. Any attempt to set it throws a <see cref="T:System.FieldAccessException"></see>.</summary>
                       /// <returns></returns>
        Literal = 64, // 0x00000040
                      /// <summary>Specifies that the field does not have to be serialized when the type is remoted.</summary>
                      /// <returns></returns>
        NotSerialized = 128, // 0x00000080
                             /// <summary>Reserved for future use.</summary>
                             /// <returns></returns>
        PinvokeImpl = 8192, // 0x00002000
                            /// <summary>Specifies that the field is accessible only by the parent type.</summary>
                            /// <returns></returns>
        Private = 1,
        /// <summary>Specifies that the field cannot be referenced.</summary>
        /// <returns></returns>
        PrivateScope = 0,
        /// <summary>Specifies that the field is accessible by any member for whom this scope is visible.</summary>
        /// <returns></returns>
        Public = Family | FamANDAssem, // 0x00000006
                                       /// <summary>Reserved.</summary>
                                       /// <returns></returns>
        ReservedMask = 38144, // 0x00009500
                              /// <summary>Specifies that the common language runtime (metadata internal APIs) should check the name encoding.</summary>
                              /// <returns></returns>
        RTSpecialName = 1024, // 0x00000400
                              /// <summary>Specifies a special method, with the name describing how the method is special.</summary>
                              /// <returns></returns>
        SpecialName = 512, // 0x00000200
                           /// <summary>Specifies that the field represents the defined type, or else it is per-instance.</summary>
                           /// <returns></returns>
        Static = 16, // 0x00000010
    }
}
