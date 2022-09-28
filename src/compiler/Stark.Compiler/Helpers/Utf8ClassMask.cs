// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Helpers;

[Flags]
internal enum Utf8ClassMask : long
{
    Eof = 1L << Utf8Class.Eof,
    Sof = 1L << Utf8Class.Sof,
    CarriageReturn = 1L << Utf8Class.CarriageReturn,
    LineFeed = 1L << Utf8Class.LineFeed,
    Space = 1L << Utf8Class.Space,
    Tab = 1L << Utf8Class.Tab,
    Digit = 1L << Utf8Class.Digit,
    Letter = 1L << Utf8Class.Letter,
    ExclamationMark = 1L << Utf8Class.ExclamationMark,
    DoubleQuote = 1L << Utf8Class.DoubleQuote,
    NumberSign = 1L << Utf8Class.NumberSign,
    DollarSign = 1L << Utf8Class.DollarSign,
    PercentSign = 1L << Utf8Class.PercentSign,
    Ampersand = 1L << Utf8Class.Ampersand,
    SingleQuote = 1L << Utf8Class.SingleQuote,
    LeftParenthesis = 1L << Utf8Class.LeftParenthesis,
    RightParenthesis = 1L << Utf8Class.RightParenthesis,
    Asterisk = 1L << Utf8Class.Asterisk,
    PlusSign = 1L << Utf8Class.PlusSign,
    Comma = 1L << Utf8Class.Comma,
    MinusSign = 1L << Utf8Class.MinusSign,
    Period = 1L << Utf8Class.Period,
    Slash = 1L << Utf8Class.Slash,
    Colon = 1L << Utf8Class.Colon,
    SemiColon = 1L << Utf8Class.SemiColon,
    LessThanSign = 1L << Utf8Class.LessThanSign,
    EqualSign = 1L << Utf8Class.EqualSign,
    GreaterThanSign = 1L << Utf8Class.GreaterThanSign,
    QuestionMark = 1L << Utf8Class.QuestionMark,
    CommercialAtSign = 1L << Utf8Class.CommercialAtSign,
    LeftSquareBracket = 1L << Utf8Class.LeftSquareBracket,
    Backslash = 1L << Utf8Class.Backslash,
    RightSquareBracket = 1L << Utf8Class.RightSquareBracket,
    CircumflexAccent = 1L << Utf8Class.CircumflexAccent,
    Underscore = 1L << Utf8Class.Underscore,
    GraveAccent = 1L << Utf8Class.GraveAccent,
    LeftBrace = 1L << Utf8Class.LeftBrace,
    VerticalBar = 1L << Utf8Class.VerticalBar,
    RightBrace = 1L << Utf8Class.RightBrace,
    TildeAccent = 1L << Utf8Class.TildeAccent,
    Invalid = 1L << Utf8Class.Invalid,
    Utf8Value = 1L << Utf8Class.Utf8Value,
    Utf8Head2 = 1L << Utf8Class.Utf8Head2,
    Utf8Head3 = 1L << Utf8Class.Utf8Head3,
    Utf8Head4 = 1L << Utf8Class.Utf8Head4
}