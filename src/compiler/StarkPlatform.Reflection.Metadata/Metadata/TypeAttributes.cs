using System;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>Specifies type attributes.</summary>
    [Flags]
    public enum TypeAttributes
    {
        /// <summary>Specifies that the type is abstract.</summary>
        /// <returns></returns>
        Abstract = 128, // 0x00000080

        /// <summary>LPTSTR is interpreted as ANSI.</summary>
        /// <returns></returns>
        AnsiClass = 0,

        /// <summary>LPTSTR is interpreted automatically.</summary>
        /// <returns></returns>
        AutoClass = 131072, // 0x00020000

        /// <summary>Specifies that class fields are automatically laid out by the common language runtime.</summary>
        /// <returns></returns>
        AutoLayout = 0,

        /// <summary>Specifies that calling static methods of the type does not force the system to initialize the type.</summary>
        /// <returns></returns>
        BeforeFieldInit = 1048576, // 0x00100000

        /// <summary>Specifies class semantics information; the current class is contextful (else agile).</summary>
        /// <returns></returns>
        ClassSemanticsMask = 0x60, // 0x00000060

        /// <summary>Specifies that the type is a class.</summary>
        /// <returns></returns>
        Class = 0,

        /// <summary>Specifies that the type is an interface.</summary>
        /// <returns></returns>
        Interface = 0x00000020, // 0x00000020

        /// <summary>Specifies that the type is an interface.</summary>
        /// <returns></returns>
        Struct = 0x00000040, // 0x00000040

        /// <summary>LPSTR is interpreted by some implementation-specific means, which includes the possibility of throwing a <see cref="T:System.NotSupportedException"></see>. Not used in the Microsoft implementation of the .NET Framework.</summary>
        /// <returns></returns>
        CustomFormatClass = 196608, // 0x00030000

        /// <summary>Used to retrieve non-standard encoding information for native interop. The meaning of the values of these 2 bits is unspecified. Not used in the Microsoft implementation of the .NET Framework.</summary>
        /// <returns></returns>
        CustomFormatMask = 12582912, // 0x00C00000

        /// <summary>Specifies that class fields are laid out at the specified offsets.</summary>
        /// <returns></returns>
        ExplicitLayout = 16, // 0x00000010

        /// <summary>Type has security associate with it.</summary>
        /// <returns></returns>
        HasSecurity = 262144, // 0x00040000

        /// <summary>Specifies that the class or interface is imported from another module.</summary>
        /// <returns></returns>
        // Import = 4096, // 0x00001000

        /// <summary>Specifies class layout information.</summary>
        /// <returns></returns>
        LayoutMask = 24, // 0x00000018

        /// <summary>Specifies that the class is nested with assembly visibility, and is thus accessible only by methods within its assembly.</summary>
        /// <returns></returns>
        NestedAssembly = 5,

        /// <summary>Specifies that the class is nested with assembly and family visibility, and is thus accessible only by methods lying in the intersection of its family and assembly.</summary>
        /// <returns></returns>
        NestedFamANDAssem = 6,

        /// <summary>Specifies that the class is nested with family visibility, and is thus accessible only by methods within its own type and any derived types.</summary>
        /// <returns></returns>
        NestedFamily = 4,

        /// <summary>Specifies that the class is nested with family or assembly visibility, and is thus accessible only by methods lying in the union of its family and assembly.</summary>
        /// <returns></returns>
        NestedFamORAssem = 7,

        /// <summary>Specifies that the class is nested with private visibility.</summary>
        /// <returns></returns>
        NestedPrivate = 3,

        /// <summary>Specifies that the class is nested with public visibility.</summary>
        /// <returns></returns>
        NestedPublic = 2,

        /// <summary>Specifies that the class is not public.</summary>
        /// <returns></returns>
        NotPublic = 0,

        /// <summary>Specifies that the class is public.</summary>
        /// <returns></returns>
        Public = 1,

        /// <summary>Attributes reserved for runtime use.</summary>
        /// <returns></returns>
        ReservedMask = 264192, // 0x00040800

        /// <summary>Runtime should check name encoding.</summary>
        /// <returns></returns>
        RTSpecialName = 2048, // 0x00000800

        /// <summary>Specifies that the class is concrete and cannot be extended.</summary>
        /// <returns></returns>
        Sealed = 256, // 0x00000100

        /// <summary>Specifies that class fields are laid out sequentially, in the order that the fields were emitted to the metadata.</summary>
        /// <returns></returns>
        SequentialLayout = 8,

        /// <summary>Specifies that the class can be serialized.</summary>
        /// <returns></returns>
        Serializable = 8192, // 0x00002000

        /// <summary>Specifies that the class is special in a way denoted by the name.</summary>
        /// <returns></returns>
        SpecialName = 1024, // 0x00000400

        /// <summary>Used to retrieve string information for native interoperability.</summary>
        /// <returns></returns>
        StringFormatMask = CustomFormatClass, // 0x00030000

        /// <summary>LPTSTR is interpreted as UNICODE.</summary>
        /// <returns></returns>
        UnicodeClass = 65536, // 0x00010000

        /// <summary>Specifies type visibility information.</summary>
        /// <returns></returns>
        VisibilityMask = Public | NestedPublic | NestedFamily, // 0x00000007

        /// <summary>Specifies a Windows Runtime type.</summary>
        /// <returns></returns>
        // WindowsRuntime = 16384, // 0x00004000
    }
}
