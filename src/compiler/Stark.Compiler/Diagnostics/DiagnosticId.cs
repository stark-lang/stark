// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Diagnostics;

// When modifying this file, the Stark.Compiler.SourceGenerator will generate
// the DiagnosticMessages class with methods having the same name as the enum item and taking
// arguments specified in the comment.
// The following comment is then used as the string that will be displayed for the end user

public enum DiagnosticId
{
    ERR_UnexpectedHexNumberInString1 = 101, // (string c) => Unexpected hex number `{c}` following `\\u`. Expecting `\\u0000` to `\\uffff`.
    ERR_UnexpectedHexNumberInString2 = 102, // (string c) => Unexpected hex number `{c}` following `\\x`. Expecting `\\x0` to `\\xffff`.
    ERR_InvalidHexNumberInString3 = 103, // (string c) => Invalid hex number `{c}` following `\\U00HHHHHH`. Expecting 8 hex numbers from `\\U00000000` to `\\U0010FFFF`.
    ERR_UnexpectedEscapeCharacter = 104, // (string c) => Unsupported character `{c}` used as an escape sequence.
    ERR_UnexpectedEndOfString = 105, // Unexpected end of string without a terminating \".
    ERR_InvalidUtf8InString = 106, // (int c) => Invalid Unicode `\\U{c:x8}` must be between `\\U00000000` to `\\U0010FFFF`.


    ERR_UnexpectedUnderscoreAfterDigit = 110, // Unexpected underscore found after digit. They can only be enclosed by digits.
    ERR_UnexpectedCharacterAfterDot = 111, // (string c) => Unexpected character `{c}` found after a dot while parsing a float. Expecting a digit 0-9.
    ERR_UnexpectedCharacterForExponent = 112, // (string c) => Unexpected character `{c}` found while parsing the exponent of a float. Expecting a digit 0-9.
    ERR_NumberOverflow = 113, // The number is overflowing 64 bit.
    ERR_InvalidHexNumberExpectingDigit = 114, // Invalid hexadecimal number. Expecting at least one [0-9a-fA-F] digit.
    ERR_InvalidOctalNumberExpectingDigit = 115, // Invalid octal number. Expecting at least one [0-7] digit.
    ERR_InvalidBinaryNumberExpectingDigit = 116, // Invalid binary number. Expecting at least one [0-1] digit.

    ERR_UnexpectedEndOfFileForMultiLineComment = 120, // (int c) => Unexpected end of file found while parsing a multi-line comment. Expecting {c} `*/` to close the comment.
}
