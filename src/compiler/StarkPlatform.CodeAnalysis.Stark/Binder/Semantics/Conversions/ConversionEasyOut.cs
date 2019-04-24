// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal abstract partial class ConversionsBase
    {
        private static class ConversionEasyOut
        {
            // There are situations in which we know that there is no unusual conversion going on
            // (such as a conversion involving constants, enumerated types, and so on.) In those
            // situations we can classify conversions via a simple table lookup:

            // PERF: Use byte instead of ConversionKind so the compiler can use array literal initialization.
            //       The most natural type choice, Enum arrays, are not blittable due to a CLR limitation.
            private static readonly byte[,] s_convkind;

            static ConversionEasyOut()
            {
                const byte IDN = (byte)ConversionKind.Identity;
                const byte IRF = (byte)ConversionKind.ImplicitReference;
                const byte XRF = (byte)ConversionKind.ExplicitReference;
                const byte XNM = (byte)ConversionKind.ExplicitNumeric;
                const byte NOC = (byte)ConversionKind.NoConversion;
                const byte NUM = (byte)ConversionKind.ImplicitNumeric;
                const byte NUL = (byte)ConversionKind.ImplicitNullable;
                const byte XNL = (byte)ConversionKind.ExplicitNullable;

                s_convkind = new byte[,] {
                    // Converting Y to X:
                    //                -----------------regular-------------------                                ----------------nullable-------------------
                    //      obj  str  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt
                    /*  obj */
                          { IDN, XRF, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC },
                    /*  str */
                          { IRF, IDN, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC },
                    /* bool */
                          { NOC, NOC, IDN, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NUL, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC },
                    /*  chr */
                          { NOC, NOC, NOC, IDN, XNM, XNM, NUM, NUM, XNM, NUM, NUM, NUM, XNM, XNM, NUM, XNM, XNM, NOC, NUL, XNL, XNL, NUL, NUL, XNL, NUL, NUL, NUL, XNL, XNL, NUL, NUL, NUL },
                    /*  i08 */
                          { NOC, NOC, NOC, XNM, IDN, NUM, NUM, NUM, XNM, XNM, XNM, XNM, XNM, XNM, NUM, NUM, XNM, NOC, XNL, NUL, NUL, NUL, NUL, XNL, XNL, XNL, XNL, XNL, XNL, NUL, NUL, XNL },
                    /*  i16 */
                          { NOC, NOC, NOC, XNM, XNM, IDN, NUM, NUM, XNM, XNM, XNM, XNM, XNM, XNM, NUM, NUM, XNM, NOC, XNL, XNL, NUL, NUL, NUL, XNL, XNL, XNL, XNL, XNL, XNL, NUL, NUL, XNL },
                    /*  i32 */
                          { NOC, NOC, NOC, XNM, XNM, XNM, IDN, NUM, XNM, XNM, XNM, XNM, XNM, XNM, NUM, NUM, XNM, NOC, XNL, XNL, XNL, NUL, NUL, XNL, XNL, XNL, XNL, XNL, XNL, NUL, NUL, XNL },
                    /*  i64 */
                          { NOC, NOC, NOC, XNM, XNM, XNM, XNM, IDN, XNM, XNM, XNM, XNM, XNM, XNM, NUM, NUM, XNM, NOC, XNL, XNL, XNL, XNL, NUL, XNL, XNL, XNL, XNL, XNL, XNL, NUL, NUL, XNL },
                    /*  u08 */
                          { NOC, NOC, NOC, XNM, XNM, NUM, NUM, NUM, IDN, NUM, NUM, NUM, XNM, XNM, NUM, XNM, NUM, NOC, XNL, XNL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, XNL, XNL, NUL, XNL, NUL },
                    /*  u16 */
                          { NOC, NOC, NOC, XNM, XNM, XNM, NUM, NUM, XNM, IDN, NUM, NUM, XNM, XNM, NUM, XNM, NUM, NOC, XNL, XNL, XNL, NUL, NUL, XNL, NUL, NUL, NUL, XNL, XNL, NUL, XNL, NUL },
                    /*  u32 */
                          { NOC, NOC, NOC, XNM, XNM, XNM, XNM, NUM, XNM, XNM, IDN, NUM, XNM, XNM, NUM, XNM, NUM, NOC, XNL, XNL, XNL, XNL, NUL, XNL, XNL, NUL, NUL, XNL, XNL, NUL, XNL, NUL },
                    /*  u64 */
                          { NOC, NOC, NOC, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, IDN, XNM, XNM, NUM, XNM, XNM, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NUL, XNL, XNL, NUL, XNL, XNL },
                    /*  r32 */
                          { NOC, NOC, NOC, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, IDN, XNM, XNM, XNM, XNM, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NOC },
                    /*  r64 */
                          { NOC, NOC, NOC, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, IDN, XNM, XNM, XNM, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NOC },
                    /*  dec */
                          { NOC, NOC, NOC, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, IDN, NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NUL, NOC, NOC },
                    /*  int */
                          { NOC, NOC, NOC, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, NOC, IDN, XNM, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NUL, XNL },
                    /* uint */
                          { NOC, NOC, NOC, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, XNM, NOC, XNM, IDN, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, XNL, NUL },
                    /*nbool */
                          { NOC, NOC, XNL, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, IDN, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC, NOC },
                    /* nchr */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NUM, XNM, NOC, IDN, XNL, XNL, NUL, NUL, XNL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, XNL },
                    /* ni08 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, XNL, IDN, NUL, NUL, NUL, XNL, XNL, XNL, XNL, NUL, NUL, NUL, XNL, XNL },
                    /* ni16 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, IDN, NUL, NUL, XNL, XNL, XNL, XNL, NUL, NUL, NUL, XNL, XNL },
                    /* ni32 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, XNL, XNL, XNL, IDN, NUL, XNL, XNL, XNL, XNL, NUL, NUL, NUL, XNL, XNL },
                    /* ni64 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, XNL, XNL, XNL, XNL, IDN, XNL, XNL, XNL, XNL, NUL, NUL, NUL, XNL, XNL },
                    /* nu08 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, XNL, XNL, NUL, NUL, NUL, IDN, NUL, NUL, NUL, NUL, NUL, NUL, XNL, XNL },
                    /* nu16 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, XNL, XNL, XNL, NUL, NUL, XNL, IDN, NUL, NUL, NUL, NUL, NUL, XNL, XNL },
                    /* nu32 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, XNL, XNL, XNL, XNL, NUL, XNL, XNL, IDN, NUL, NUL, NUL, NUL, XNL, XNL },
                    /* nu64 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, IDN, NUL, NUL, NUL, XNL, XNL },
                    /* nr32 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NOC, XNL, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, IDN, NUL, XNL, NOC, NOC },
                    /* nr64 */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NOC, XNL, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, IDN, XNL, NOC, NOC },
                    /* ndec */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NOC, XNL, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, IDN, NOC, NOC },
                    /* nint */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NOC, NOC, XNL, XNL, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NOC, NOC, XNL, XNL },
                    /* uint */
                          { NOC, NOC, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NOC, NOC, XNL, XNL, NOC, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, XNL, NOC, NOC, NOC, XNL, XNL },
                };
            }

            private static int TypeToIndex(TypeSymbol type)
            {
                switch (type.GetSpecialTypeSafe())
                {
                    case SpecialType.System_Object: return 0;
                    case SpecialType.System_String: return 1;
                    case SpecialType.System_Boolean: return 2;
                    case SpecialType.System_Char: return 3;
                    case SpecialType.System_Int8: return 4;
                    case SpecialType.System_Int16: return 5;
                    case SpecialType.System_Int32: return 6;
                    case SpecialType.System_Int64: return 7;
                    case SpecialType.System_UInt8: return 8;
                    case SpecialType.System_UInt16: return 9;
                    case SpecialType.System_UInt32: return 10;
                    case SpecialType.System_UInt64: return 11;
                    case SpecialType.System_Float32: return 12;
                    case SpecialType.System_Float64: return 13;
                    case SpecialType.System_Decimal: return 14;
                    case SpecialType.System_Int: return 15;
                    case SpecialType.System_UInt: return 16;

                    case SpecialType.None:
                        if ((object)type != null && type.IsNullableType())
                        {
                            TypeSymbol underlyingType = type.GetNullableUnderlyingType();

                            switch (underlyingType.GetSpecialTypeSafe())
                            {
                                case SpecialType.System_Boolean: return 17;
                                case SpecialType.System_Char: return 18;
                                case SpecialType.System_Int8: return 19;
                                case SpecialType.System_Int16: return 20;
                                case SpecialType.System_Int32: return 21;
                                case SpecialType.System_Int64: return 22;
                                case SpecialType.System_UInt8: return 23;
                                case SpecialType.System_UInt16: return 24;
                                case SpecialType.System_UInt32: return 25;
                                case SpecialType.System_UInt64: return 26;
                                case SpecialType.System_Float32: return 27;
                                case SpecialType.System_Float64: return 28;
                                case SpecialType.System_Decimal: return 29;
                                case SpecialType.System_Int: return 30;
                                case SpecialType.System_UInt: return 31;
                            }
                        }

                        // fall through
                        goto default;

                    default:
                        return -1;
                }
            }

            public static ConversionKind ClassifyConversion(TypeSymbol source, TypeSymbol target)
            {
                int sourceIndex = TypeToIndex(source);
                if (sourceIndex < 0)
                {
                    return ConversionKind.NoConversion;
                }
                int targetIndex = TypeToIndex(target);
                if (targetIndex < 0)
                {
                    return ConversionKind.NoConversion;
                }
                return (ConversionKind)s_convkind[sourceIndex, targetIndex];
            }
        }
    }
}
