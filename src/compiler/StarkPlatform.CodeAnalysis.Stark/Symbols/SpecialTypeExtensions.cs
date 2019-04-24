// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal static class SpecialTypeExtensions
    {
        /// <summary>
        /// Checks if a type is considered a "built-in integral" by CLR.
        /// </summary>
        public static bool IsIntegralType(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_UInt:
                case SpecialType.System_Int:
                case SpecialType.System_UInt8:
                case SpecialType.System_Int8:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSignedIntegralType(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Int:
                case SpecialType.System_Int8:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanBeConst(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_Int:
                case SpecialType.System_Int8:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt:
                case SpecialType.System_UInt8:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Float32:
                case SpecialType.System_Float64:
                case SpecialType.System_Decimal:
                case SpecialType.System_String:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// The type is one of the simple types defined in Dev10 C#, see "predeftype.h"/simple
        /// </summary>
        public static bool IsIntrinsicType(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_Int:
                case SpecialType.System_Int8:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt:
                case SpecialType.System_UInt8:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Float32:
                case SpecialType.System_Float64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsValidVolatileFieldType(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_UInt8:
                case SpecialType.System_Int8:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Char:
                case SpecialType.System_Float32:
                case SpecialType.System_Boolean:
                case SpecialType.System_Int:
                case SpecialType.System_UInt:
                    return true;
                default:
                    return false;
            }
        }

        public static int FixedBufferElementSizeInBytes(this SpecialType specialType)
        {
            // SizeInBytes() handles decimal (contrary to the language spec).  But decimal is not allowed
            // as a fixed buffer element type.
            return specialType == SpecialType.System_Decimal ? 0 : specialType.SizeInBytes();
        }
    }
}
