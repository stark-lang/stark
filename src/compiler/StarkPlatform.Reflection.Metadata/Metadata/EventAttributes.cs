using System;

namespace StarkPlatform.Reflection.Metadata
{
    /// <summary>Specifies the attributes of an event.</summary>
    [Flags]
    public enum EventAttributes
    {
        /// <summary>Specifies that the event has no attributes.</summary>
        /// <returns></returns>
        None = 0,
        /// <summary>Specifies a reserved flag for common language runtime use only.</summary>
        /// <returns></returns>
        ReservedMask = 1024, // 0x00000400
        /// <summary>Specifies that the common language runtime should check name encoding.</summary>
        /// <returns></returns>
        RTSpecialName = ReservedMask, // 0x00000400
        /// <summary>Specifies that the event is special in a way described by the name.</summary>
        /// <returns></returns>
        SpecialName = 512, // 0x00000200
    }
}