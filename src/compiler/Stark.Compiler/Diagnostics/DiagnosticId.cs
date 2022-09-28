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
    ERR_UnexpectedHexNumberInString2 = 102, // (string c) => Unexpected hex number `{c}` following `\\x`. Expecting `\\x00` to `\\xff`.
    ERR_UnexpectedEscapeCharacter = 103, // (string c) => Unsupported character `{c}` used as an escape sequence.
    ERR_UnexpectedEndOfString = 104, // Unexpected end of string without a terminating \".


    ERR_UnexpectedUnderscoreAfterDigit = 110, // Unexpected underscore found after digit. They can only be enclosed by digits.
    ERR_UnexpectedCharacterAfterDot = 111, // (string c) => Unexpected character `{c}` found after a dot while parsing a float. Expecting a digit 0-9.
    ERR_UnexpectedCharacterForExponent = 112, // (string c) => Unexpected character `{c}` found while parsing the exponent of a float. Expecting a digit 0-9.
    ERR_NumberOverflow = 113, // The number is overflowing 64 bit.
}
