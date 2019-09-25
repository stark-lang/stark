using System;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>Specifies flags for method attributes. These flags are defined in the corhdr.h file.</summary>
    [Flags]
    public enum MethodAttributes
    {
        /// <summary>Indicates that the class does not provide an implementation of this method.</summary>
        /// <returns></returns>
        Abstract = 1024, // 0x00000400
                         /// <summary>Indicates that the method is accessible to any class of this assembly.</summary>
                         /// <returns></returns>
        Assembly = 3,
        /// <summary>Indicates that the method can only be overridden when it is also accessible.</summary>
        /// <returns></returns>
        CheckAccessOnOverride = 512, // 0x00000200
                                     /// <summary>Indicates that the method is accessible to members of this type and its derived types that are in this assembly only.</summary>
                                     /// <returns></returns>
        FamANDAssem = 2,
        /// <summary>Indicates that the method is accessible only to members of this class and its derived classes.</summary>
        /// <returns></returns>
        Family = 4,
        /// <summary>Indicates that the method is accessible to derived classes anywhere, as well as to any class in the assembly.</summary>
        /// <returns></returns>
        FamORAssem = 5,
        /// <summary>Indicates that the method cannot be overridden.</summary>
        /// <returns></returns>
        Final = 32, // 0x00000020
                    /// <summary>Indicates that the method has security associated with it. Reserved flag for runtime use only.</summary>
                    /// <returns></returns>
        HasSecurity = 16384, // 0x00004000
                             /// <summary>Indicates that the method hides by name and signature; otherwise, by name only.</summary>
                             /// <returns></returns>
        HideBySig = 128, // 0x00000080
                         /// <summary>Retrieves accessibility information.</summary>
                         /// <returns></returns>
        MemberAccessMask = FamORAssem | FamANDAssem, // 0x00000007
                                                     /// <summary>Indicates that the method always gets a new slot in the vtable.</summary>
                                                     /// <returns></returns>
        NewSlot = 256, // 0x00000100
                       /// <summary>Indicates that the method implementation is forwarded through PInvoke (Platform Invocation Services).</summary>
                       /// <returns></returns>
        PinvokeImpl = 8192, // 0x00002000
                            /// <summary>Indicates that the method is accessible only to the current class.</summary>
                            /// <returns></returns>
        Private = 1,
        /// <summary>Indicates that the member cannot be referenced.</summary>
        /// <returns></returns>
        PrivateScope = 0,
        /// <summary>Indicates that the method is accessible to any object for which this object is in scope.</summary>
        /// <returns></returns>
        Public = Family | FamANDAssem, // 0x00000006
                                       /// <summary>Indicates that the method calls another method containing security code. Reserved flag for runtime use only.</summary>
                                       /// <returns></returns>
        RequireSecObject = 32768, // 0x00008000
                                  /// <summary>Indicates a reserved flag for runtime use only.</summary>
                                  /// <returns></returns>
        ReservedMask = 53248, // 0x0000D000
                              /// <summary>Indicates that the method will reuse an existing slot in the vtable. This is the default behavior.</summary>
                              /// <returns></returns>
        ReuseSlot = 0,
        /// <summary>Indicates that the common language runtime checks the name encoding.</summary>
        /// <returns></returns>
        RTSpecialName = 4096, // 0x00001000
                              /// <summary>Indicates that the method is special. The name describes how this method is special.</summary>
                              /// <returns></returns>
        SpecialName = 2048, // 0x00000800
                            /// <summary>Indicates that the method is defined on the type; otherwise, it is defined per instance.</summary>
                            /// <returns></returns>
        Static = 16, // 0x00000010
                     /// <summary>Indicates that the managed method is exported by thunk to unmanaged code.</summary>
                     /// <returns></returns>
        UnmanagedExport = 8,
        /// <summary>Indicates that the method is virtual.</summary>
        /// <returns></returns>
        Virtual = 64, // 0x00000040
                      /// <summary>Retrieves vtable attributes.</summary>
                      /// <returns></returns>
        VtableLayoutMask = NewSlot, // 0x00000100

        ReadOnly = 0x0004000,
    }
}
