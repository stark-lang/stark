// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Stark.Compiler.Helpers;

internal static class Utf8Helper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetWidth(Rune rune) => Wcwidth.UnicodeCalculator.GetWidth(rune);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDigit(byte b) => (uint)(b - '0') <= ('9' - '0');

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLetter(byte b) => (uint)((b - 'A') & ~0x20) <= ('Z' - 'A');
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLetterOrUnderscore(byte b) => IsLetter(b) || b == (byte)'_';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLetterContinuationForIdentifier(byte b) => IsLetter(b) || b == (byte)'_' || IsDigit(b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsHex(byte b) => IsDigit(b) || (uint)((b - 'A') & ~0x20) <= ('F' - 'A');

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HexToValue(byte b) => IsDigit(b) ? b - (byte)'0' : b >= (byte)'a' && b <= (byte)'f' ? b - (byte)'a' + 10 : b - (byte)'A' + 10;

    public static string ByteToSafeString(byte b) => b < ' ' || b >= 127 ? $"\\x{b:x2}" : ((char)b).ToString(CultureInfo.InvariantCulture);

    // UTF8 Characters
    // 1 byte: 00-7F
    // 2 byte: C0-DF 80-BF
    // 3 byte: E0-EF 80-BF 80-BF
    // 4 byte: F0-F7 80-BF 80-BF 80-BF

    public static Utf8Class GetClassFromByte(byte b)
    {
        // Equivalent to ByteToClass[b] but removes the bounds check because we know that we have only 256 values
        //
        // Generates the following code:
        //
        // Stark.Compiler.Parser.Utf8ClassHelper.GetClassFromByte2(Byte)
        //     L0000: mov eax, 0x1a0c0eb0
        //     L0005: movzx edx, cl
        //     L0008: movzx eax, byte ptr [eax+edx]
        //     L000c: ret
        //
        // Instead of:
        //
        // Stark.Compiler.Parser.Utf8ClassHelper.GetClassFromByte(Byte)
        //     L0000: movzx eax, cl
        //     L0003: cmp eax, 0x100
        //     L0008: jae short L0012
        //     L000a: movzx eax, byte ptr [eax+0x1a0c0eb0]
        //     L0011: ret
        //     L0012: call 0x71ff6250
        //     L0017: int3
        return Unsafe.AddByteOffset(ref Unsafe.AsRef(MemoryMarshal.AsRef<Utf8Class>(MemoryMarshal.AsBytes(ByteToClass))), b);
    }

    private static ReadOnlySpan<Utf8Class> ByteToClass => new(new Utf8Class[256]
    {
        Utf8Class.Invalid,                      //  0x00,  , 0
        Utf8Class.Invalid,                      //  0x01,  , 1
        Utf8Class.Sof,                          //  0x02,  , 2
        Utf8Class.Eof,                          //  0x03,  , 3
        Utf8Class.Invalid,                      //  0x04,  , 4
        Utf8Class.Invalid,                      //  0x05,  , 5
        Utf8Class.Invalid,                      //  0x06,  , 6
        Utf8Class.Invalid,                      //  0x07,  , 7
        Utf8Class.Invalid,                      //  0x08,  , 8
        Utf8Class.Tab,                          //  0x09,  , 9
        Utf8Class.LineFeed,                     //  0x0A,  , 10
        Utf8Class.Invalid,                      //  0x0B,  , 11
        Utf8Class.Invalid,                      //  0x0C,  , 12
        Utf8Class.CarriageReturn,               //  0x0D,  , 13
        Utf8Class.Invalid,                      //  0x0E,  , 14
        Utf8Class.Invalid,                      //  0x0F,  , 15
        Utf8Class.Invalid,                      //  0x10,  , 16
        Utf8Class.Invalid,                      //  0x11,  , 17
        Utf8Class.Invalid,                      //  0x12,  , 18
        Utf8Class.Invalid,                      //  0x13,  , 19
        Utf8Class.Invalid,                      //  0x14,  , 20
        Utf8Class.Invalid,                      //  0x15,  , 21
        Utf8Class.Invalid,                      //  0x16,  , 22
        Utf8Class.Invalid,                      //  0x17,  , 23
        Utf8Class.Invalid,                      //  0x18,  , 24
        Utf8Class.Invalid,                      //  0x19,  , 25
        Utf8Class.Invalid,                      //  0x1A,  , 26
        Utf8Class.Invalid,                      //  0x1B,  , 27
        Utf8Class.Invalid,                      //  0x1C,  , 28
        Utf8Class.Invalid,                      //  0x1D,  , 29
        Utf8Class.Invalid,                      //  0x1E,  , 30
        Utf8Class.Invalid,                      //  0x1F,  , 31
        Utf8Class.Space,                        //  0x20,  , 32
        Utf8Class.ExclamationMark,              //  0x21, !, 33
        Utf8Class.DoubleQuote,                  //  0x22, ", 34
        Utf8Class.NumberSign,                   //  0x23, #, 35
        Utf8Class.DollarSign,                   //  0x24, $, 36
        Utf8Class.PercentSign,                  //  0x25, %, 37
        Utf8Class.Ampersand,                    //  0x26, &, 38
        Utf8Class.SingleQuote,                  //  0x27, ', 39
        Utf8Class.LeftParenthesis,              //  0x28, (, 40
        Utf8Class.RightParenthesis,             //  0x29, ), 41
        Utf8Class.Asterisk,                     //  0x2A, *, 42
        Utf8Class.PlusSign,                     //  0x2B, +, 43
        Utf8Class.Comma,                        //  0x2C, ,, 44
        Utf8Class.MinusSign,                    //  0x2D, -, 45
        Utf8Class.Period,                       //  0x2E, ., 46
        Utf8Class.Slash,                        //  0x2F, /, 47
        Utf8Class.Digit,                        //  0x30, 0, 48
        Utf8Class.Digit,                        //  0x31, 1, 49
        Utf8Class.Digit,                        //  0x32, 2, 50
        Utf8Class.Digit,                        //  0x33, 3, 51
        Utf8Class.Digit,                        //  0x34, 4, 52
        Utf8Class.Digit,                        //  0x35, 5, 53
        Utf8Class.Digit,                        //  0x36, 6, 54
        Utf8Class.Digit,                        //  0x37, 7, 55
        Utf8Class.Digit,                        //  0x38, 8, 56
        Utf8Class.Digit,                        //  0x39, 9, 57
        Utf8Class.Colon,                        //  0x3A, :, 58
        Utf8Class.SemiColon,                    //  0x3B, ;, 59
        Utf8Class.LessThanSign,                 //  0x3C, <, 60
        Utf8Class.EqualSign,                    //  0x3D, =, 61
        Utf8Class.GreaterThanSign,              //  0x3E, >, 62
        Utf8Class.QuestionMark,                 //  0x3F, ?, 63
        Utf8Class.CommercialAtSign,             //  0x40, @, 64
        Utf8Class.Letter,                       //  0x41, A, 65
        Utf8Class.Letter,                       //  0x42, B, 66
        Utf8Class.Letter,                       //  0x43, C, 67
        Utf8Class.Letter,                       //  0x44, D, 68
        Utf8Class.Letter,                       //  0x45, E, 69
        Utf8Class.Letter,                       //  0x46, F, 70
        Utf8Class.Letter,                       //  0x47, G, 71
        Utf8Class.Letter,                       //  0x48, H, 72
        Utf8Class.Letter,                       //  0x49, I, 73
        Utf8Class.Letter,                       //  0x4A, J, 74
        Utf8Class.Letter,                       //  0x4B, K, 75
        Utf8Class.Letter,                       //  0x4C, L, 76
        Utf8Class.Letter,                       //  0x4D, M, 77
        Utf8Class.Letter,                       //  0x4E, N, 78
        Utf8Class.Letter,                       //  0x4F, O, 79
        Utf8Class.Letter,                       //  0x50, P, 80
        Utf8Class.Letter,                       //  0x51, Q, 81
        Utf8Class.Letter,                       //  0x52, R, 82
        Utf8Class.Letter,                       //  0x53, S, 83
        Utf8Class.Letter,                       //  0x54, T, 84
        Utf8Class.Letter,                       //  0x55, U, 85
        Utf8Class.Letter,                       //  0x56, V, 86
        Utf8Class.Letter,                       //  0x57, W, 87
        Utf8Class.Letter,                       //  0x58, X, 88
        Utf8Class.Letter,                       //  0x59, Y, 89
        Utf8Class.Letter,                       //  0x5A, Z, 90
        Utf8Class.LeftSquareBracket,            //  0x5B, [, 91
        Utf8Class.Backslash,                    //  0x5C, \, 92
        Utf8Class.RightSquareBracket,           //  0x5D, ], 93
        Utf8Class.CircumflexAccent,             //  0x5E, ^, 94
        Utf8Class.Underscore,                   //  0x5F, _, 95
        Utf8Class.GraveAccent,                  //  0x60, `, 96
        Utf8Class.Letter,                       //  0x61, a, 97
        Utf8Class.Letter,                       //  0x62, b, 98
        Utf8Class.Letter,                       //  0x63, c, 99
        Utf8Class.Letter,                       //  0x64, d, 100
        Utf8Class.Letter,                       //  0x65, e, 101
        Utf8Class.Letter,                       //  0x66, f, 102
        Utf8Class.Letter,                       //  0x67, g, 103
        Utf8Class.Letter,                       //  0x68, h, 104
        Utf8Class.Letter,                       //  0x69, i, 105
        Utf8Class.Letter,                       //  0x6A, j, 106
        Utf8Class.Letter,                       //  0x6B, k, 107
        Utf8Class.Letter,                       //  0x6C, l, 108
        Utf8Class.Letter,                       //  0x6D, m, 109
        Utf8Class.Letter,                       //  0x6E, n, 110
        Utf8Class.Letter,                       //  0x6F, o, 111
        Utf8Class.Letter,                       //  0x70, p, 112
        Utf8Class.Letter,                       //  0x71, q, 113
        Utf8Class.Letter,                       //  0x72, r, 114
        Utf8Class.Letter,                       //  0x73, s, 115
        Utf8Class.Letter,                       //  0x74, t, 116
        Utf8Class.Letter,                       //  0x75, u, 117
        Utf8Class.Letter,                       //  0x76, v, 118
        Utf8Class.Letter,                       //  0x77, w, 119
        Utf8Class.Letter,                       //  0x78, x, 120
        Utf8Class.Letter,                       //  0x79, y, 121
        Utf8Class.Letter,                       //  0x7A, z, 122
        Utf8Class.LeftBrace,                    //  0x7B, {, 123
        Utf8Class.VerticalBar,                  //  0x7C, |, 124
        Utf8Class.RightBrace,                   //  0x7D, }, 125
        Utf8Class.TildeAccent,                  //  0x7E, ~, 126
        Utf8Class.Invalid,                      //  0x7F, , 127
        Utf8Class.Utf8Value,                    //  0x80, €, 128
        Utf8Class.Utf8Value,                    //  0x81, , 129
        Utf8Class.Utf8Value,                    //  0x82, ‚, 130
        Utf8Class.Utf8Value,                    //  0x83, ƒ, 131
        Utf8Class.Utf8Value,                    //  0x84, „, 132
        Utf8Class.Utf8Value,                    //  0x85, …, 133
        Utf8Class.Utf8Value,                    //  0x86, †, 134
        Utf8Class.Utf8Value,                    //  0x87, ‡, 135
        Utf8Class.Utf8Value,                    //  0x88, ˆ, 136
        Utf8Class.Utf8Value,                    //  0x89, ‰, 137
        Utf8Class.Utf8Value,                    //  0x8A, Š, 138
        Utf8Class.Utf8Value,                    //  0x8B, ‹, 139
        Utf8Class.Utf8Value,                    //  0x8C, Œ, 140
        Utf8Class.Utf8Value,                    //  0x8D, , 141
        Utf8Class.Utf8Value,                    //  0x8E, Ž, 142
        Utf8Class.Utf8Value,                    //  0x8F, , 143
        Utf8Class.Utf8Value,                    //  0x90, , 144
        Utf8Class.Utf8Value,                    //  0x91, ‘, 145
        Utf8Class.Utf8Value,                    //  0x92, ’, 146
        Utf8Class.Utf8Value,                    //  0x93, “, 147
        Utf8Class.Utf8Value,                    //  0x94, ”, 148
        Utf8Class.Utf8Value,                    //  0x95, •, 149
        Utf8Class.Utf8Value,                    //  0x96, –, 150
        Utf8Class.Utf8Value,                    //  0x97, —, 151
        Utf8Class.Utf8Value,                    //  0x98, ˜, 152
        Utf8Class.Utf8Value,                    //  0x99, ™, 153
        Utf8Class.Utf8Value,                    //  0x9A, š, 154
        Utf8Class.Utf8Value,                    //  0x9B, ›, 155
        Utf8Class.Utf8Value,                    //  0x9C, œ, 156
        Utf8Class.Utf8Value,                    //  0x9D, , 157
        Utf8Class.Utf8Value,                    //  0x9E, ž, 158
        Utf8Class.Utf8Value,                    //  0x9F, Ÿ, 159
        Utf8Class.Utf8Value,                    //  0xA0,  , 160
        Utf8Class.Utf8Value,                    //  0xA1, ¡, 161
        Utf8Class.Utf8Value,                    //  0xA2, ¢, 162
        Utf8Class.Utf8Value,                    //  0xA3, £, 163
        Utf8Class.Utf8Value,                    //  0xA4, ¤, 164
        Utf8Class.Utf8Value,                    //  0xA5, ¥, 165
        Utf8Class.Utf8Value,                    //  0xA6, ¦, 166
        Utf8Class.Utf8Value,                    //  0xA7, §, 167
        Utf8Class.Utf8Value,                    //  0xA8, ¨, 168
        Utf8Class.Utf8Value,                    //  0xA9, ©, 169
        Utf8Class.Utf8Value,                    //  0xAA, ª, 170
        Utf8Class.Utf8Value,                    //  0xAB, «, 171
        Utf8Class.Utf8Value,                    //  0xAC, ¬, 172
        Utf8Class.Utf8Value,                    //  0xAD, ­, 173
        Utf8Class.Utf8Value,                    //  0xAE, ®, 174
        Utf8Class.Utf8Value,                    //  0xAF, ¯, 175
        Utf8Class.Utf8Value,                    //  0xB0, °, 176
        Utf8Class.Utf8Value,                    //  0xB1, ±, 177
        Utf8Class.Utf8Value,                    //  0xB2, ², 178
        Utf8Class.Utf8Value,                    //  0xB3, ³, 179
        Utf8Class.Utf8Value,                    //  0xB4, ´, 180
        Utf8Class.Utf8Value,                    //  0xB5, µ, 181
        Utf8Class.Utf8Value,                    //  0xB6, ¶, 182
        Utf8Class.Utf8Value,                    //  0xB7, ·, 183
        Utf8Class.Utf8Value,                    //  0xB8, ¸, 184
        Utf8Class.Utf8Value,                    //  0xB9, ¹, 185
        Utf8Class.Utf8Value,                    //  0xBA, º, 186
        Utf8Class.Utf8Value,                    //  0xBB, », 187
        Utf8Class.Utf8Value,                    //  0xBC, ¼, 188
        Utf8Class.Utf8Value,                    //  0xBD, ½, 189
        Utf8Class.Utf8Value,                    //  0xBE, ¾, 190
        Utf8Class.Utf8Value,                    //  0xBF, ¿, 191
        Utf8Class.Utf8Head2,                    //  0xC0, À, 192
        Utf8Class.Utf8Head2,                    //  0xC1, Á, 193
        Utf8Class.Utf8Head2,                    //  0xC2, Â, 194
        Utf8Class.Utf8Head2,                    //  0xC3, Ã, 195
        Utf8Class.Utf8Head2,                    //  0xC4, Ä, 196
        Utf8Class.Utf8Head2,                    //  0xC5, Å, 197
        Utf8Class.Utf8Head2,                    //  0xC6, Æ, 198
        Utf8Class.Utf8Head2,                    //  0xC7, Ç, 199
        Utf8Class.Utf8Head2,                    //  0xC8, È, 200
        Utf8Class.Utf8Head2,                    //  0xC9, É, 201
        Utf8Class.Utf8Head2,                    //  0xCA, Ê, 202
        Utf8Class.Utf8Head2,                    //  0xCB, Ë, 203
        Utf8Class.Utf8Head2,                    //  0xCC, Ì, 204
        Utf8Class.Utf8Head2,                    //  0xCD, Í, 205
        Utf8Class.Utf8Head2,                    //  0xCE, Î, 206
        Utf8Class.Utf8Head2,                    //  0xCF, Ï, 207
        Utf8Class.Utf8Head2,                    //  0xD0, Ð, 208
        Utf8Class.Utf8Head2,                    //  0xD1, Ñ, 209
        Utf8Class.Utf8Head2,                    //  0xD2, Ò, 210
        Utf8Class.Utf8Head2,                    //  0xD3, Ó, 211
        Utf8Class.Utf8Head2,                    //  0xD4, Ô, 212
        Utf8Class.Utf8Head2,                    //  0xD5, Õ, 213
        Utf8Class.Utf8Head2,                    //  0xD6, Ö, 214
        Utf8Class.Utf8Head2,                    //  0xD7, ×, 215
        Utf8Class.Utf8Head2,                    //  0xD8, Ø, 216
        Utf8Class.Utf8Head2,                    //  0xD9, Ù, 217
        Utf8Class.Utf8Head2,                    //  0xDA, Ú, 218
        Utf8Class.Utf8Head2,                    //  0xDB, Û, 219
        Utf8Class.Utf8Head2,                    //  0xDC, Ü, 220
        Utf8Class.Utf8Head2,                    //  0xDD, Ý, 221
        Utf8Class.Utf8Head2,                    //  0xDE, Þ, 222
        Utf8Class.Utf8Head2,                    //  0xDF, ß, 223
        Utf8Class.Utf8Head3,                    //  0xE0, à, 224
        Utf8Class.Utf8Head3,                    //  0xE1, á, 225
        Utf8Class.Utf8Head3,                    //  0xE2, â, 226
        Utf8Class.Utf8Head3,                    //  0xE3, ã, 227
        Utf8Class.Utf8Head3,                    //  0xE4, ä, 228
        Utf8Class.Utf8Head3,                    //  0xE5, å, 229
        Utf8Class.Utf8Head3,                    //  0xE6, æ, 230
        Utf8Class.Utf8Head3,                    //  0xE7, ç, 231
        Utf8Class.Utf8Head3,                    //  0xE8, è, 232
        Utf8Class.Utf8Head3,                    //  0xE9, é, 233
        Utf8Class.Utf8Head3,                    //  0xEA, ê, 234
        Utf8Class.Utf8Head3,                    //  0xEB, ë, 235
        Utf8Class.Utf8Head3,                    //  0xEC, ì, 236
        Utf8Class.Utf8Head3,                    //  0xED, í, 237
        Utf8Class.Utf8Head3,                    //  0xEE, î, 238
        Utf8Class.Utf8Head3,                    //  0xEF, ï, 239
        Utf8Class.Utf8Head4,                    //  0xF0, ð, 240
        Utf8Class.Utf8Head4,                    //  0xF1, ñ, 241
        Utf8Class.Utf8Head4,                    //  0xF2, ò, 242
        Utf8Class.Utf8Head4,                    //  0xF3, ó, 243
        Utf8Class.Utf8Head4,                    //  0xF4, ô, 244
        Utf8Class.Utf8Head4,                    //  0xF5, õ, 245
        Utf8Class.Utf8Head4,                    //  0xF6, ö, 246
        Utf8Class.Utf8Head4,                    //  0xF7, ÷, 247
        Utf8Class.Invalid,                      //  0xF8, ø, 248
        Utf8Class.Invalid,                      //  0xF9, ù, 249
        Utf8Class.Invalid,                      //  0xFA, ú, 250
        Utf8Class.Invalid,                      //  0xFB, û, 251
        Utf8Class.Invalid,                      //  0xFC, ü, 252
        Utf8Class.Invalid,                      //  0xFD, ý, 253
        Utf8Class.Invalid,                      //  0xFE, þ, 254
        Utf8Class.Invalid,                      //  0xFF, ÿ, 255
    });
}