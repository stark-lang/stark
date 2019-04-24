// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Shared.Extensions
{
    internal static class SpecialTypeExtensions
    {
        public static PredefinedType ToPredefinedType(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Object:
                    return PredefinedType.Object;
                case SpecialType.System_Void:
                    return PredefinedType.Void;
                case SpecialType.System_Boolean:
                    return PredefinedType.Boolean;
                case SpecialType.System_Char:
                    return PredefinedType.Char;
                case SpecialType.System_Int8:
                    return PredefinedType.Int8;
                case SpecialType.System_UInt8:
                    return PredefinedType.UInt8;
                case SpecialType.System_Int16:
                    return PredefinedType.Int16;
                case SpecialType.System_UInt16:
                    return PredefinedType.UInt16;
                case SpecialType.System_Int32:
                    return PredefinedType.Int32;
                case SpecialType.System_UInt32:
                    return PredefinedType.UInt32;
                case SpecialType.System_Int64:
                    return PredefinedType.Int64;
                case SpecialType.System_UInt64:
                    return PredefinedType.UInt64;
                case SpecialType.System_Decimal:
                    return PredefinedType.Decimal;
                case SpecialType.System_Float32:
                    return PredefinedType.Float32;
                case SpecialType.System_Float64:
                    return PredefinedType.Float64;
                case SpecialType.System_String:
                    return PredefinedType.String;
                case SpecialType.System_DateTime:
                    return PredefinedType.DateTime;
                default:
                    return PredefinedType.None;
            }
        }
    }
}
