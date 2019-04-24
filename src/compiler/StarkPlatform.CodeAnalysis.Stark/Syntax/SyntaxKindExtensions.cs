// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal static partial class SyntaxKindExtensions
    {
        internal static SpecialType GetSpecialType(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.VoidKeyword:
                    return SpecialType.System_Void;
                case SyntaxKind.BoolKeyword:
                    return SpecialType.System_Boolean;
                case SyntaxKind.UInt8Keyword:
                    return SpecialType.System_UInt8;
                case SyntaxKind.Int8Keyword:
                    return SpecialType.System_Int8;
                case SyntaxKind.Int16Keyword:
                    return SpecialType.System_Int16;
                case SyntaxKind.UInt16Keyword:
                    return SpecialType.System_UInt16;
                case SyntaxKind.IntKeyword:
                    return SpecialType.System_Int;
                case SyntaxKind.UIntKeyword:
                    return SpecialType.System_UInt;
                case SyntaxKind.Int32Keyword:
                    return SpecialType.System_Int32;
                case SyntaxKind.UInt32Keyword:
                    return SpecialType.System_UInt32;
                case SyntaxKind.Int64Keyword:
                    return SpecialType.System_Int64;
                case SyntaxKind.UInt64Keyword:
                    return SpecialType.System_UInt64;
                case SyntaxKind.Float64Keyword:
                    return SpecialType.System_Float64;
                case SyntaxKind.Float32Keyword:
                    return SpecialType.System_Float32;
                case SyntaxKind.DecimalKeyword:
                    return SpecialType.System_Decimal;
                case SyntaxKind.StringKeyword:
                    return SpecialType.System_String;
                case SyntaxKind.CharKeyword:
                    return SpecialType.System_Char;
                case SyntaxKind.ObjectKeyword:
                    return SpecialType.System_Object;
                default:
                    // Note that "dynamic" is a contextual keyword, so it should never show up here.
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }
        }
    }
}
