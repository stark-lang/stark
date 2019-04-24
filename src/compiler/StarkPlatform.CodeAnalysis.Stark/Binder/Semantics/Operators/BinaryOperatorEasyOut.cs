// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal sealed partial class OverloadResolution
    {
        internal static class BinopEasyOut
        {
            private const BinaryOperatorKind ERR = BinaryOperatorKind.Error;
            private const BinaryOperatorKind OBJ = BinaryOperatorKind.Object;
            private const BinaryOperatorKind STR = BinaryOperatorKind.String;
            private const BinaryOperatorKind OSC = BinaryOperatorKind.ObjectAndString;
            private const BinaryOperatorKind SOC = BinaryOperatorKind.StringAndObject;
            private const BinaryOperatorKind INT = BinaryOperatorKind.Int;
            private const BinaryOperatorKind UNT = BinaryOperatorKind.UInt;
            private const BinaryOperatorKind I32 = BinaryOperatorKind.Int32;
            private const BinaryOperatorKind U32 = BinaryOperatorKind.UInt32;
            private const BinaryOperatorKind I64 = BinaryOperatorKind.Int64;
            private const BinaryOperatorKind U64 = BinaryOperatorKind.UInt64;
            private const BinaryOperatorKind F32 = BinaryOperatorKind.Float32;
            private const BinaryOperatorKind F64 = BinaryOperatorKind.Float64;
            private const BinaryOperatorKind DEC = BinaryOperatorKind.Decimal;
            private const BinaryOperatorKind BOL = BinaryOperatorKind.Bool;
            private const BinaryOperatorKind LIT = BinaryOperatorKind.Lifted | BinaryOperatorKind.Int;
            private const BinaryOperatorKind LUT = BinaryOperatorKind.Lifted | BinaryOperatorKind.UInt;
            private const BinaryOperatorKind LI2 = BinaryOperatorKind.Lifted | BinaryOperatorKind.Int32;
            private const BinaryOperatorKind LU2 = BinaryOperatorKind.Lifted | BinaryOperatorKind.UInt32;
            private const BinaryOperatorKind LI4 = BinaryOperatorKind.Lifted | BinaryOperatorKind.Int64;
            private const BinaryOperatorKind LU4 = BinaryOperatorKind.Lifted | BinaryOperatorKind.UInt64;
            private const BinaryOperatorKind LF2 = BinaryOperatorKind.Lifted | BinaryOperatorKind.Float32;
            private const BinaryOperatorKind LF4 = BinaryOperatorKind.Lifted | BinaryOperatorKind.Float64;
            private const BinaryOperatorKind LDC = BinaryOperatorKind.Lifted | BinaryOperatorKind.Decimal;
            private const BinaryOperatorKind LBL = BinaryOperatorKind.Lifted | BinaryOperatorKind.Bool;

            // UNDONE: The lifted bits make these tables very redundant. We could make them smaller (and slower)
            // UNDONE: by having just the unlifted table and doing type manipulation. 

            // UNDONE: We could also make these tables smaller by special-casing string and object.

            // Overload resolution for Y * / - % < > <= >= X
            private static readonly BinaryOperatorKind[,] s_arithmetic =
            {
                //                ----------------regular-------------------                                 ----------------nullable-------------------
                //      obj  str  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt
                /*  obj */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  str */                                                                              
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* bool */                                                                                                                                              
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  chr */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, U32, U64, F32, F64, DEC, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  i08 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, I64, ERR, F32, F64, DEC, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /*  i16 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, I64, ERR, F32, F64, DEC, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /*  i32 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, I64, ERR, F32, F64, DEC, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /*  i64 */
                      { ERR, ERR, ERR, I64, I64, I64, I64, I64, I64, I64, I64, ERR, F32, F64, DEC, ERR, ERR, ERR, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, LF2, LF4, LDC, ERR, ERR },
                /*  u08 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, U32, U64, F32, F64, DEC, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  u16 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, U32, U64, F32, F64, DEC, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  u32 */
                      { ERR, ERR, ERR, U32, I64, I64, I64, I64, U32, U32, U32, U64, F32, F64, DEC, INT, UNT, ERR, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  u64 */
                      { ERR, ERR, ERR, U64, ERR, ERR, ERR, ERR, U64, U64, U64, U64, F32, F64, DEC, INT, UNT, ERR, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  r32 */
                      { ERR, ERR, ERR, F32, F32, F32, F32, F32, F32, F32, F32, F32, F32, F64, ERR, ERR, ERR, ERR, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF4, ERR, ERR, ERR },
                /*  r64 */
                      { ERR, ERR, ERR, F64, F64, F64, F64, F64, F64, F64, F64, F64, F64, F64, ERR, ERR, ERR, ERR, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, ERR, ERR, ERR },
                /*  dec */
                      { ERR, ERR, ERR, DEC, DEC, DEC, DEC, DEC, DEC, DEC, DEC, DEC, ERR, ERR, DEC, ERR, ERR, ERR, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, ERR, ERR, LDC, ERR, ERR },
                /*  int */
                      { ERR, ERR, ERR, INT, INT, INT, INT, ERR, INT, INT, INT, ERR, ERR, ERR, ERR, INT, UNT, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT },
                /*  unt */
                      { ERR, ERR, ERR, UNT, UNT, UNT, UNT, ERR, UNT, UNT, UNT, ERR, ERR, ERR, ERR, UNT, UNT, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LIT, LUT },
                /*nbool */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* nchr */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /* ni08 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /* ni16 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /* ni32 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /* ni64 */
                      { ERR, ERR, ERR, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, LF2, LF4, LDC, ERR, ERR, ERR, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /* nu08 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /* nu16 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /* nu32 */
                      { ERR, ERR, ERR, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, LF2, LF4, LDC, LIT, LUT, ERR, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /* nu64 */
                      { ERR, ERR, ERR, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, LF2, LF4, LDC, ERR, ERR, ERR, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, LF2, LF4, LDC, ERR, ERR },
                /* nr32 */
                      { ERR, ERR, ERR, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF4, ERR, ERR, ERR, ERR, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF4, ERR, ERR, ERR },
                /* nr64 */
                      { ERR, ERR, ERR, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, ERR, ERR, ERR, ERR, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, ERR, ERR, ERR },
                /* ndec */
                      { ERR, ERR, ERR, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, ERR, ERR, LDC, ERR, ERR, ERR, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, ERR, ERR, LDC, ERR, ERR },
                /* nint */
                      { ERR, ERR, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT },
                /* nunt */
                      { ERR, ERR, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LUT, LUT, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LIT, LUT },
            };

            // Overload resolution for Y + X
            private static readonly BinaryOperatorKind[,] s_addition =
            {
                //                ----------------regular-------------------                                 ----------------nullable-------------------
                //      obj  str  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt
                /*  obj */
                      { ERR, OSC, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  str */
                      { SOC, STR, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC, SOC },
                /* bool */
                      { ERR, OSC, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  chr */
                      { ERR, OSC, ERR, I32, I32, I32, I32, I64, I32, I32, U32, U64, ERR, ERR, DEC, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, LDC, LIT, LIT},
                /*  i08 */
                      { ERR, OSC, ERR, I32, I32, I32, I32, I64, I32, I32, I64, ERR, ERR, ERR, DEC, INT, INT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, LDC, LIT, LIT },
                /*  i16 */
                      { ERR, OSC, ERR, I32, I32, I32, I32, I64, I32, I32, I64, ERR, ERR, ERR, DEC, INT, INT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, LDC, LIT, LIT },
                /*  i32 */
                      { ERR, OSC, ERR, I32, I32, I32, I32, I64, I32, I32, I64, ERR, ERR, ERR, DEC, INT, INT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, LDC, LIT, LIT },
                /*  i64 */
                      { ERR, OSC, ERR, I64, I64, I64, I64, I64, I64, I64, I64, ERR, ERR, ERR, DEC, INT, INT, ERR, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, ERR, ERR, LDC, LIT, LIT },
                /*  u08 */
                      { ERR, OSC, ERR, I32, I32, I32, I32, I64, I32, I32, U32, U64, ERR, ERR, DEC, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, LDC, LIT, LUT },
                /*  u16 */
                      { ERR, OSC, ERR, I32, I32, I32, I32, I64, I32, I32, U32, U64, ERR, ERR, DEC, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, LDC, LIT, LUT },
                /*  u32 */
                      { ERR, OSC, ERR, U32, I64, I64, I64, I64, U32, U32, U32, U64, ERR, ERR, DEC, INT, UNT, ERR, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, ERR, ERR, LDC, LIT, LUT },
                /*  u64 */
                      { ERR, OSC, ERR, ERR, ERR, ERR, ERR, ERR, U64, U64, U64, U64, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  r32 */
                      { ERR, OSC, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, F32, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LF2, ERR, ERR, ERR, ERR },
                /*  r64 */
                      { ERR, OSC, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, F64, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LF4, ERR, ERR, ERR },
                /*  dec */
                      { ERR, OSC, ERR, DEC, DEC, DEC, DEC, DEC, DEC, DEC, DEC, DEC, ERR, ERR, DEC, ERR, ERR, ERR, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, ERR, ERR, LDC, ERR, ERR },
                /*  int */
                      { ERR, ERR, ERR, INT, INT, INT, INT, ERR, INT, INT, ERR, ERR, ERR, ERR, ERR, INT, UNT, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT },
                /*  unt */
                      { ERR, ERR, ERR, UNT, UNT, UNT, UNT, ERR, UNT, UNT, UNT, ERR, ERR, ERR, ERR, UNT, UNT, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LIT, LUT },
                /*nbool */
                      { ERR, OSC, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* nchr */
                      { ERR, OSC, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, LDC, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, LDC, LIT, LUT },
                /* ni08 */
                      { ERR, OSC, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, LDC, LIT, LIT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, LDC, LIT, LIT },
                /* ni16 */
                      { ERR, OSC, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, LDC, LIT, LIT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, LDC, LIT, LIT },
                /* ni32 */
                      { ERR, OSC, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, LDC, LIT, LIT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, LDC, LIT, LIT },
                /* ni64 */
                      { ERR, OSC, ERR, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, ERR, ERR, LDC, ERR, ERR, ERR, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, ERR, ERR, LDC, ERR, ERR },
                /* nu08 */
                      { ERR, OSC, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, LDC, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, LDC, LIT, LUT },
                /* nu16 */
                      { ERR, OSC, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, LDC, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, LDC, LIT, LUT },
                /* nu32 */
                      { ERR, OSC, ERR, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, ERR, ERR, LDC, LIT, LUT, ERR, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, ERR, ERR, LDC, LIT, LUT },
                /* nu64 */
                      { ERR, OSC, ERR, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, ERR, ERR, LDC, LIT, LUT, ERR, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, ERR, ERR, LDC, LIT, LUT },
                /* nr32 */
                      { ERR, OSC, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LF2, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LF2, ERR, ERR, ERR, ERR },
                /* nr64 */
                      { ERR, OSC, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LF4, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LF4, ERR, ERR, ERR },
                /* ndec */
                      { ERR, OSC, ERR, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, ERR, ERR, LDC, ERR, ERR, ERR, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, ERR, ERR, LDC, ERR, ERR },
                /* nint */
                      { ERR, ERR, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT },
                /* nunt */
                      { ERR, ERR, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LUT, LUT, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LIT, LUT },
            };

            // Overload resolution for Y << >> X
            private static readonly BinaryOperatorKind[,] s_shift =
            {
                //                ----------------regular-------------------                                 ----------------nullable-------------------
                //      obj  str  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt
                /*  obj */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  str */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* bool */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  chr */
                      { ERR, ERR, ERR, I32, I32, I32, I32, ERR, I32, I32, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  i08 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, ERR, I32, I32, ERR, ERR, ERR, ERR, ERR, I32, I32, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2},
                /*  i16 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, ERR, I32, I32, ERR, ERR, ERR, ERR, ERR, I32, I32, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /*  i32 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, ERR, I32, I32, ERR, ERR, ERR, ERR, ERR, I32, I32, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /*  i64 */
                      { ERR, ERR, ERR, I64, I64, I64, I64, ERR, I64, I64, ERR, ERR, ERR, ERR, ERR, I64, I64, ERR, LI4, LI4, LI4, LI4, ERR, LI4, LI4, ERR, ERR, ERR, ERR, ERR, LI4, LI4 },
                /*  u08 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, ERR, I32, I32, ERR, ERR, ERR, ERR, ERR, I32, I32, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /*  u16 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, ERR, I32, I32, ERR, ERR, ERR, ERR, ERR, I32, I32, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /*  u32 */
                      { ERR, ERR, ERR, U32, U32, U32, U32, ERR, U32, U32, ERR, ERR, ERR, ERR, ERR, U32, U32, ERR, LU2, LU2, LU2, LU2, ERR, LU2, LU2, ERR, ERR, ERR, ERR, ERR, LU2, LU2 },
                /*  u64 */
                      { ERR, ERR, ERR, U64, U64, U64, U64, ERR, U64, U64, ERR, ERR, ERR, ERR, ERR, U64, U64, ERR, LU4, LU4, LU4, LU4, ERR, LU4, LU4, ERR, ERR, ERR, ERR, ERR, LU4, LU4 },
                /*  r32 */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  r64 */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  dec */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  int */
                      { ERR, ERR, ERR, INT, INT, INT, INT, ERR, INT, INT, INT, ERR, ERR, ERR, ERR, INT, INT, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LIT },
                /*  unt */
                      { ERR, ERR, ERR, UNT, UNT, UNT, UNT, ERR, UNT, UNT, UNT, ERR, ERR, ERR, ERR, UNT, UNT, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LUT, LUT },
                /*nbool */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* nchr */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /* ni08 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /* ni16 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /* ni32 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /* ni64 */
                      { ERR, ERR, ERR, LI4, LI4, LI4, LI4, ERR, LI4, LI4, ERR, ERR, ERR, ERR, ERR, LI4, LI4, ERR, LI4, LI4, LI4, LI4, ERR, LI4, LI4, ERR, ERR, ERR, ERR, ERR, LI4, LI4 },
                /* nu08 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /* nu16 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2, ERR, LI2, LI2, LI2, LI2, ERR, LI2, LI2, ERR, ERR, ERR, ERR, ERR, LI2, LI2 },
                /* nu32 */
                      { ERR, ERR, ERR, LU2, LU2, LU2, LU2, ERR, LU2, LU2, ERR, ERR, ERR, ERR, ERR, LU2, LU2, ERR, LU2, LU2, LU2, LU2, ERR, LU2, LU2, ERR, ERR, ERR, ERR, ERR, LU2, LU2 },
                /* nu64 */
                      { ERR, ERR, ERR, LU4, LU4, LU4, LU4, ERR, LU4, LU4, ERR, ERR, ERR, ERR, ERR, LU4, LU4, ERR, LU4, LU4, LU4, LU4, ERR, LU4, LU4, ERR, ERR, ERR, ERR, ERR, LU4, LU4 },
                /* nr32 */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* nr64 */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* ndec */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* nint */
                      { ERR, ERR, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT },
                /* nunt */
                      { ERR, ERR, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LUT, LUT, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LUT, LUT },
            };

            // Overload resolution for Y == != X
            // Note that these are the overload resolution rules; overload resolution might pick an invalid operator.
            // For example, overload resolution on object == decimal chooses the object/object overload, which then
            // is not legal because decimal must be a reference type. But we don't know to give that error *until*
            // overload resolution has chosen the reference equality operator.
            private static readonly BinaryOperatorKind[,] s_equality =
            {
                //                ----------------regular-------------------                                 ----------------nullable-------------------
                //      obj  str  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt
                /*  obj */
                      { OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ },
                /*  str */
                      { OBJ, STR, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ },
                /* bool */
                      { OBJ, OBJ, BOL, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, LBL, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ },
                /*  chr */
                      { OBJ, OBJ, OBJ, I32, I32, I32, I32, I64, I32, I32, U32, U64, F32, F64, DEC, INT, UNT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  i08 */
                      { OBJ, OBJ, OBJ, I32, I32, I32, I32, I64, I32, I32, I64, ERR, F32, F64, DEC, INT, UNT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /*  i16 */
                      { OBJ, OBJ, OBJ, I32, I32, I32, I32, I64, I32, I32, I64, ERR, F32, F64, DEC, INT, UNT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /*  i32 */
                      { OBJ, OBJ, OBJ, I32, I32, I32, I32, I64, I32, I32, I64, ERR, F32, F64, DEC, INT, UNT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /*  i64 */
                      { OBJ, OBJ, OBJ, I64, I64, I64, I64, I64, I64, I64, I64, ERR, F32, F64, DEC, ERR, ERR, OBJ, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, LF2, LF4, LDC, ERR, ERR },
                /*  u08 */
                      { OBJ, OBJ, OBJ, I32, I32, I32, I32, I64, I32, I32, U32, U64, F32, F64, DEC, INT, UNT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  u16 */
                      { OBJ, OBJ, OBJ, I32, I32, I32, I32, I64, I32, I32, U32, U64, F32, F64, DEC, INT, UNT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  u32 */
                      { OBJ, OBJ, OBJ, U32, I64, I64, I64, I64, U32, U32, U32, U64, F32, F64, DEC, INT, UNT, OBJ, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  u64 */
                      { OBJ, OBJ, OBJ, U64, ERR, ERR, ERR, ERR, U64, U64, U64, U64, F32, F64, DEC, INT, UNT, OBJ, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, LF2, LF4, LDC, LIT, LUT },
                /*  r32 */
                      { OBJ, OBJ, OBJ, F32, F32, F32, F32, F32, F32, F32, F32, F32, F32, F64, OBJ, ERR, ERR, OBJ, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF4, OBJ, ERR, ERR },
                /*  r64 */
                      { OBJ, OBJ, OBJ, F64, F64, F64, F64, F64, F64, F64, F64, F64, F64, F64, OBJ, ERR, ERR, OBJ, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, OBJ, ERR, ERR },
                /*  dec */
                      { OBJ, OBJ, OBJ, DEC, DEC, DEC, DEC, DEC, DEC, DEC, DEC, DEC, OBJ, OBJ, DEC, ERR, ERR, OBJ, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, OBJ, OBJ, LDC, ERR, ERR },
                /*nbool */
                      { OBJ, OBJ, LBL, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, LBL, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ, OBJ },
                /* nchr */
                      { OBJ, OBJ, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /* ni08 */
                      { OBJ, OBJ, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /* ni16 */
                      { OBJ, OBJ, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /* ni32 */
                      { OBJ, OBJ, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, LF2, LF4, LDC, LIT, LUT },
                /* ni64 */
                      { OBJ, OBJ, OBJ, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, LF2, LF4, LDC, ERR, ERR, OBJ, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, LF2, LF4, LDC, ERR, ERR },
                /* nu08 */
                      { OBJ, OBJ, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /* nu16 */
                      { OBJ, OBJ, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT, OBJ, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /* nu32 */
                      { OBJ, OBJ, OBJ, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, LF2, LF4, LDC, LIT, LUT, OBJ, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, LF2, LF4, LDC, LIT, LUT },
                /* nu64 */
                      { OBJ, OBJ, OBJ, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, LF2, LF4, LDC, ERR, ERR, OBJ, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, LF2, LF4, LDC, ERR, ERR },
                /* nr32 */
                      { OBJ, OBJ, OBJ, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF4, OBJ, ERR, ERR, OBJ, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF2, LF4, OBJ, ERR, ERR },
                /* nr64 */
                      { OBJ, OBJ, OBJ, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, OBJ, ERR, ERR, OBJ, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, LF4, OBJ, ERR, ERR },
                /* ndec */
                      { OBJ, OBJ, OBJ, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, OBJ, OBJ, LDC, ERR, ERR, OBJ, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, LDC, OBJ, OBJ, LDC, ERR, ERR },
                /* nint */
                      { ERR, ERR, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT },
                /* nunt */
                      { ERR, ERR, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LUT, LUT, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LUT, LUT },
            };

            // Overload resolution for Y | & ^ || && X
            private static readonly BinaryOperatorKind[,] s_logical =
            {
                //                ----------------regular-------------------                                 ----------------nullable-------------------
                //      obj  str  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt  bool chr  i08  i16  i32  i64  u08  u16  u32  u64  r32  r64  dec  int  unt
                /*  obj */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  str */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* bool */
                      { ERR, ERR, BOL, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LBL, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  chr */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, U32, U64, ERR, ERR, ERR, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, ERR, LIT, LUT },
                /*  i08 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, I64, ERR, ERR, ERR, ERR, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, ERR, LIT, LUT },
                /*  i16 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, I64, ERR, ERR, ERR, ERR, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, ERR, LIT, LUT },
                /*  i32 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, I64, ERR, ERR, ERR, ERR, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, ERR, LIT, LUT },
                /*  i64 */
                      { ERR, ERR, ERR, I64, I64, I64, I64, I64, I64, I64, I64, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  u08 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, U32, U64, ERR, ERR, ERR, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, ERR, LIT, LUT },
                /*  u16 */
                      { ERR, ERR, ERR, I32, I32, I32, I32, I64, I32, I32, U32, U64, ERR, ERR, ERR, INT, UNT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, ERR, LIT, LUT },
                /*  u32 */
                      { ERR, ERR, ERR, U32, I64, I64, I64, I64, U32, U32, U32, U64, ERR, ERR, ERR, INT, UNT, ERR, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, ERR, ERR, ERR, LIT, LUT },
                /*  u64 */
                      { ERR, ERR, ERR, U64, ERR, ERR, ERR, ERR, U64, U64, U64, U64, ERR, ERR, ERR, ERR, ERR, ERR, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, ERR, ERR, ERR, ERR, ERR },
                /*  r32 */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  r64 */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*  dec */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /*nbool */
                      { ERR, ERR, LBL, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LBL, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* nchr */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, ERR, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, ERR, LIT, LUT },
                /* ni08 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, ERR, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, ERR, LIT, LUT },
                /* ni16 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, ERR, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, ERR, LIT, LUT },
                /* ni32 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, ERR, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LI4, ERR, ERR, ERR, ERR, LIT, LUT },
                /* ni64 */
                      { ERR, ERR, ERR, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, ERR, ERR, ERR, ERR, ERR, ERR, LI4, LI4, LI4, LI4, LI4, LI4, LI4, LI4, ERR, ERR, ERR, ERR, ERR, ERR },
                /* nu08 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, ERR, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, ERR, LIT, LUT },
                /* nu16 */
                      { ERR, ERR, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, ERR, LIT, LUT, ERR, LI2, LI2, LI2, LI2, LI4, LI2, LI2, LU2, LU4, ERR, ERR, ERR, LIT, LUT },
                /* nu32 */
                      { ERR, ERR, ERR, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, ERR, ERR, ERR, LIT, LUT, ERR, LU2, LI4, LI4, LI4, LI4, LU2, LU2, LU2, LU4, ERR, ERR, ERR, LIT, LUT },
                /* nu64 */
                      { ERR, ERR, ERR, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, ERR, ERR, ERR, ERR, ERR, ERR, LU4, ERR, ERR, ERR, ERR, LU4, LU4, LU4, LU4, ERR, ERR, ERR, ERR, ERR },
                /* nr32 */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* nr64 */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* ndec */
                      { ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR, ERR },
                /* nint */
                      { ERR, ERR, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT, ERR, LIT, LIT, LIT, LIT, ERR, LIT, LIT, LIT, ERR, ERR, ERR, ERR, LIT, LUT },
                /* nunt */
                      { ERR, ERR, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LUT, LUT, ERR, LUT, LUT, LUT, LUT, ERR, LUT, LUT, LUT, ERR, ERR, ERR, ERR, LUT, LUT },
            };

            private static readonly BinaryOperatorKind[][,] s_opkind =
            {
                /* *  */ s_arithmetic,
                /* +  */ s_addition,
                /* -  */ s_arithmetic,
                /* /  */ s_arithmetic,
                /* %  */ s_arithmetic,
                /* >> */ s_shift,
                /* << */ s_shift,
                /* == */ s_equality,
                /* != */ s_equality,
                /* >  */ s_arithmetic,
                /* <  */ s_arithmetic,
                /* >= */ s_arithmetic,
                /* <= */ s_arithmetic,
                /* &  */ s_logical,
                /* |  */ s_logical,
                /* ^  */ s_logical,
            };

            private static int? TypeToIndex(TypeSymbol type)
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

                    default: return null;
                }
            }

            public static BinaryOperatorKind OpKind(BinaryOperatorKind kind, TypeSymbol left, TypeSymbol right)
            {
                int? leftIndex = TypeToIndex(left);
                if (leftIndex == null)
                {
                    return BinaryOperatorKind.Error;
                }
                int? rightIndex = TypeToIndex(right);
                if (rightIndex == null)
                {
                    return BinaryOperatorKind.Error;
                }

                var result = BinaryOperatorKind.Error;

                // kind.OperatorIndex() collapses '&' and '&&' (and '|' and '||').  To correct
                // this problem, we handle kinds satisfying IsLogical() separately.  Fortunately,
                // such operators only work on boolean types, so there's no need to write out
                // a whole new table.
                //
                // Example: int & int is legal, but int && int is not, so we can't use the same
                // table for both operators.
                if (!kind.IsLogical() || (leftIndex == (int)BinaryOperatorKind.Bool && rightIndex == (int)BinaryOperatorKind.Bool))
                {
                    result = s_opkind[kind.OperatorIndex()][leftIndex.Value, rightIndex.Value];
                }

                return result == BinaryOperatorKind.Error ? result : result | kind;
            }
        }

        private void BinaryOperatorEasyOut(BinaryOperatorKind kind, BoundExpression left, BoundExpression right, BinaryOperatorOverloadResolutionResult result)
        {
            var leftType = left.Type;
            if ((object)leftType == null)
            {
                return;
            }

            var rightType = right.Type;
            if ((object)rightType == null)
            {
                return;
            }

            if (PossiblyUnusualConstantOperation(left, right))
            {
                return;
            }

            var easyOut = BinopEasyOut.OpKind(kind, leftType, rightType);

            if (easyOut == BinaryOperatorKind.Error)
            {
                return;
            }

            BinaryOperatorSignature signature = this.Compilation.builtInOperators.GetSignature(easyOut);

            Conversion leftConversion = Conversions.FastClassifyConversion(left.Type, signature.LeftType);
            Conversion rightConversion = Conversions.FastClassifyConversion(right.Type, signature.RightType);

            Debug.Assert(leftConversion.Exists && leftConversion.IsImplicit);
            Debug.Assert(rightConversion.Exists && rightConversion.IsImplicit);

            result.Results.Add(BinaryOperatorAnalysisResult.Applicable(signature, leftConversion, rightConversion));
        }

        private static bool PossiblyUnusualConstantOperation(BoundExpression left, BoundExpression right)
        {
            Debug.Assert(left != null);
            Debug.Assert((object)left.Type != null);
            Debug.Assert(right != null);
            Debug.Assert((object)right.Type != null);

            // If there are "special" conversions available on either expression
            // then the early out is not accurate. For example, "myuint + myint" 
            // would normally be determined by the easy out as "long + long". But
            // "myuint + 1" does not choose that overload because there is a special
            // conversion from 1 to uint. 

            // If we have one or more constants, then both operands have to be 
            // int, both have to be bool, or both have to be string. Otherwise
            // we skip the easy out and go for the slow path.

            if (left.ConstantValue == null && right.ConstantValue == null)
            {
                // Neither is constant. Go for the easy out.
                return false;
            }

            // One or both operands are constants. See if they are both int, bool or string.

            if (left.Type.SpecialType != right.Type.SpecialType)
            {
                // They are unequal types. Go for the slow path.
                return true;
            }

            if (left.Type.SpecialType == SpecialType.System_Int32 ||
                left.Type.SpecialType == SpecialType.System_Boolean ||
                left.Type.SpecialType == SpecialType.System_String)
            {
                // They are both int, both bool, or both string. Go for the fast path.
                return false;
            }

            // We don't know what's going on. Go for the slow path.
            return true;
        }
    }
}
