namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>Specifies flags for the attributes of a method implementation.</summary>
    public enum MethodImplAttributes
    {
        /// <summary>Specifies that the method implementation is in Microsoft intermediate language (MSIL).</summary>
        /// <returns></returns>
        IL = 0,
        /// <summary>Specifies that the method is implemented in managed code.</summary>
        /// <returns></returns>
        Managed = 0,
        /// <summary>Specifies that the method implementation is native.</summary>
        /// <returns></returns>
        Native = 1,
        /// <summary>Specifies that the method implementation is in Optimized Intermediate Language (OPTIL).</summary>
        /// <returns></returns>
        OPTIL = 2,
        /// <summary>Specifies flags about code type.</summary>
        /// <returns></returns>
        CodeTypeMask = 3,
        /// <summary>Specifies that the method implementation is provided by the runtime.</summary>
        /// <returns></returns>
        Runtime = 3,
        /// <summary>Specifies whether the method is implemented in managed or unmanaged code.</summary>
        /// <returns></returns>
        ManagedMask = 4,
        /// <summary>Specifies that the method is implemented in unmanaged code.</summary>
        /// <returns></returns>
        Unmanaged = 4,
        /// <summary>Specifies that the method cannot be inlined.</summary>
        /// <returns></returns>
        NoInlining = 8,
        /// <summary>Specifies that the method is not defined.</summary>
        /// <returns></returns>
        ForwardRef = 16, // 0x00000010
                         /// <summary>Specifies that the method is single-threaded through the body. Static methods (Shared in Visual Basic) lock on the type, whereas instance methods lock on the instance. You can also use the C# <see cref="~/docs/csharp/language-reference/keywords/lock-statement.md">lock statement</see> or the Visual Basic <see cref="~/docs/visual-basic/language-reference/statements/synclock-statement.md">SyncLock statement</see> for this purpose.</summary>
                         /// <returns></returns>
        Synchronized = 32, // 0x00000020
                           /// <summary>Specifies that the method is not optimized by the just-in-time (JIT) compiler or by native code generation (see <see cref="~/docs/framework/tools/ngen-exe-native-image-generator.md">Ngen.exe</see>) when debugging possible code generation problems.</summary>
                           /// <returns></returns>
        NoOptimization = 64, // 0x00000040
                             /// <summary>Specifies that the method signature is exported exactly as declared.</summary>
                             /// <returns></returns>
        PreserveSig = 128, // 0x00000080
                           /// <summary>Specifies that the method should be inlined wherever possible.</summary>
                           /// <returns></returns>
        AggressiveInlining = 256, // 0x00000100
                                  /// <summary>Specifies an internal call.</summary>
                                  /// <returns></returns>
        InternalCall = 4096, // 0x00001000
                             /// <summary>Specifies a range check value.</summary>
                             /// <returns></returns>
        MaxMethodImplVal = 65535, // 0x0000FFFF
    }
}
