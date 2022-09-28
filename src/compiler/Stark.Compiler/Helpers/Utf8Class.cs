// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Helpers;

internal enum Utf8Class : byte
{
    Eof, // End of file
    Sof, // Start of file
    CarriageReturn, // \r
    LineFeed, // \n
    Space, // " "
    Tab,   // \t
    Digit,     // Digit
    Letter,     // Letter
    ExclamationMark,       // !
    DoubleQuote,           // "
    NumberSign,            // #
    DollarSign,            // $
    PercentSign,           // %
    Ampersand,             // &
    SingleQuote,           // '
    LeftParenthesis,       // (
    RightParenthesis,      // )
    Asterisk,              // *
    PlusSign,              // +
    Comma,                 // ,
    MinusSign,             // -
    Period,                // .
    Slash,                 // /
    Colon,                 // :
    SemiColon,             // ;
    LessThanSign,          // <
    EqualSign,             // =
    GreaterThanSign,       // >
    QuestionMark,          // ?
    CommercialAtSign,      // @
    LeftSquareBracket,     // [
    Backslash,             // \
    RightSquareBracket,    // ]
    CircumflexAccent,      // ^
    Underscore,            // _
    GraveAccent,           // `
    LeftBrace,             // {
    VerticalBar,           // |
    RightBrace,            // }
    TildeAccent,           // ~
    Invalid,               // Special characters
    Utf8Value, // 80-BF
    Utf8Head2, // C0-DF
    Utf8Head3, // E0-EF
    Utf8Head4  // F0-F7
}