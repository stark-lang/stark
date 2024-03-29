﻿// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Diagnostics;

// When modifying this file, the Stark.Compiler.SourceGenerator will generate
// the DiagnosticMessages class with methods having the same name as the enum item and taking
// arguments specified in the comment.
// The following comment is then used as the string that will be displayed for the end user

public enum DiagnosticId
{
    ERR_InvalidInputExpectedEof = 90, // Invalid input. Unexpected EOF character (\x03) found before the end of the source code.

    ERR_InvalidHexNumberInString1 = 101, // (string c) => Unexpected hex number `{c}` following `\\u`. Expecting `\\u0000` to `\\uffff`.
    ERR_InvalidHexNumberInString2 = 102, // (string c) => Unexpected hex number `{c}` following `\\x`. Expecting `\\x0` to `\\xffff`.
    ERR_InvalidHexNumberInString3 = 103, // (string c) => Invalid hex number `{c}` following `\\U00HHHHHH`. Expecting 8 hex numbers from `\\U00000000` to `\\U0010FFFF`.
    ERR_UnexpectedEscapeCharacter = 104, // (string c) => Unsupported character `{c}` used as an escape sequence.
    ERR_UnexpectedEndOfString = 105, // Unexpected end of string without a terminating \".
    ERR_InvalidUtf8InString = 106, // (int c) => Invalid Unicode found in string `\\U{c:x8}` must be between `\\U00000000` to `\\U0010FFFF`.
    ERR_UnexpectedEndOfFileForInterpolatedString = 107, // (string c) => Unexpected end of file while parsing interpolated string. Missing a closing `{c}`.
    ERR_InvalidRawStringExpectingEmptyFirstLine = 108, // Invalid raw string literal. The first line of a multiline raw string literal must be empty.
    ERR_InvalidRawStringExpectingEmptyLastLine = 109, // Invalid raw string literal. The last line of a multiline raw string literal must be empty.
    ERR_InvalidRawStringExpectingSpaceToMatchClosing = 110, // Invalid raw string literal. Line does not start with the same whitespace as the closing line of the raw string literal
    ERR_InvalidRawStringExpectingEnoughQuotes = 111, // Invalid raw string literal. The raw string literal does not start with enough quote characters to allow this many consecutive quote characters as content
    ERR_InvalidRawStringUnexpectedEndOfString = 112, // (string c) => Invalid raw string literal. Expecting the terminating quotes `{c}`.
    ERR_InvalidRawStringExpectingAtLeastOneLine = 113, // Multi-line raw string literals must contain at least one line of content.
    ERR_InvalidRawStringUnexpectedMixSpaces = 114, // (string c, string d) => Invalid raw string literal. Line contains different whitespace `{c}` than previous lines (`{d}`).
    ERR_InvalidUtf8InRune = 115, // (int c) => Invalid Unicode found in rune `\\U{c:x8}` must be between `\\U00000000` to `\\U0010FFFF`.
    ERR_InvalidRuneTooManyCharacters = 116, // Invalid rune. Too many characters. Expecting a single rune.
    ERR_InvalidRuneCannotBeEmpty = 117, // Invalid rune. A rune cannot be empty.
    ERR_UnexpectedEndOfRune = 118, // Unexpected end of rune without a terminating \'.

    ERR_UnexpectedUnderscoreAfterDigit = 120, // Unexpected underscore found after digit. They can only be enclosed by digits.
    ERR_UnexpectedCharacterAfterDot = 121, // (string c) => Unexpected character `{c}` found after a dot while parsing a float. Expecting a digit 0-9.
    ERR_UnexpectedCharacterForExponent = 122, // (string c) => Unexpected character `{c}` found while parsing the exponent of a float. Expecting a digit 0-9.
    ERR_NumberOverflow = 123, // The number is overflowing 64 bit.
    ERR_InvalidHexNumberExpectingDigit = 124, // Invalid hexadecimal number. Expecting at least one [0-9a-fA-F] digit.
    ERR_InvalidOctalNumberExpectingDigit = 125, // Invalid octal number. Expecting at least one [0-7] digit.
    ERR_InvalidBinaryNumberExpectingDigit = 126, // Invalid binary number. Expecting at least one [0-1] digit.

    ERR_UnexpectedEndOfFileForMultiLineComment = 130, // (int c) => Unexpected end of file found while parsing a multi-line comment. Expecting {c} `*/` to close the comment.
}
