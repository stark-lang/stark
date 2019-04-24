// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Shared.Extensions
{
    internal static class PredefinedTypeExtensions
    {
        public static SpecialType ToSpecialType(this PredefinedType predefinedType)
        {
            switch (predefinedType)
            {
                case PredefinedType.Object:
                    return SpecialType.System_Object;
                case PredefinedType.Void:
                    return SpecialType.System_Void;
                case PredefinedType.Boolean:
                    return SpecialType.System_Boolean;
                case PredefinedType.Char:
                    return SpecialType.System_Char;
                case PredefinedType.Int8:
                    return SpecialType.System_Int8;
                case PredefinedType.UInt8:
                    return SpecialType.System_UInt8;
                case PredefinedType.Int16:
                    return SpecialType.System_Int16;
                case PredefinedType.UInt16:
                    return SpecialType.System_UInt16;
                case PredefinedType.Int32:
                    return SpecialType.System_Int32;
                case PredefinedType.UInt32:
                    return SpecialType.System_UInt32;
                case PredefinedType.Int64:
                    return SpecialType.System_Int64;
                case PredefinedType.UInt64:
                    return SpecialType.System_UInt64;
                case PredefinedType.Decimal:
                    return SpecialType.System_Decimal;
                case PredefinedType.Float32:
                    return SpecialType.System_Float32;
                case PredefinedType.Float64:
                    return SpecialType.System_Float64;
                case PredefinedType.String:
                    return SpecialType.System_String;
                case PredefinedType.DateTime:
                    return SpecialType.System_DateTime;
                default:
                    return SpecialType.None;
            }
        }
    }
}
