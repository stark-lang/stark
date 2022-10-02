// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Diagnostics;
using System.Text;
using Stark.Compiler.Collections;
using Stark.Compiler.Diagnostics;
using Stark.Compiler.Helpers;
using Stark.Compiler.Syntax;
using Varena;
using static Stark.Compiler.Diagnostics.DiagnosticMessages;

namespace Stark.Compiler.Parsing;

public class Lexer
{
    private uint _line;
    private uint _column;
    private unsafe byte* _originalPtr;
    private int _length;
    private HashCode _hasher;
    private const byte StartUtf8 = 0xc0;
    public const byte Eof = 0x03;
    private const int PaddingBytes = 8;
    private readonly LexerInputOutput _lio;
    private InlineList<int> _stringMultiLineTokenIndices;
    private InlineList<StringMultiLineState> _stringMultilineStates;

    public Lexer(LexerInputOutput lio)
    {
        _lio = lio;
        _stringMultiLineTokenIndices = new InlineList<int>(4);
        _stringMultilineStates = new InlineList<StringMultiLineState>(4);
    }

    private int TokenCount => _lio.Tokens.Count;

    private void AddToken(TokenKind kind, TokenSpan span)
    {
        _lio.AddToken(kind, span);
    }

    private void AddToken(TokenKind kind, TokenSpan span, TokenValue value)
    {
        _lio.AddToken(kind, span, value);
    }

    private void AddToken(TokenKind kind, TokenSpan span, ReadOnlySpan<byte> value)
    {
        _lio.AddToken(kind, span, value);
    }

    private VirtualBuffer TempBuffer => _lio.TempBuffer;

    private void ResetTempBuffer()
    {
        _lio.ResetTempBuffer();
    }

    //private unsafe byte Peek(byte* ptr, int offset)
    //{
    //    ptr += offset;
    //    if (ptr < _originalPtr) return 0;
    //    if (ptr >= (_originalPtr + _length)) return Eof;
    //    return *ptr;
    //}

    //private unsafe Span<byte> GetSpan(byte* ptr)
    //{
    //    int offset = (int)(ptr - _originalPtr);
    //    int remainingLength = _length - offset;
    //    return new Span<byte>(ptr, remainingLength);
    //}

    private unsafe Span<byte> GetSpan(int offset, int length)
    {
        return new Span<byte>(_originalPtr + (int)offset, length);
    }

    public void Run(Stream stream)
    {
        if (stream.Length > int.MaxValue)
        {
            throw new InvalidOperationException($"Length of stream {stream.Length} must be less than {int.MaxValue}");
        }
        _lio.ResetInputBuffer();
        var inputBuffer = _lio.InputBuffer;
        var length = (int)stream.Length;
        var span = inputBuffer.AllocateRange(length + 1 + PaddingBytes);
        stream.ReadExactly(span.Slice(0, length));
        span[length] = Eof;
        span.Slice(length + 1).Fill(0);
        RunInternal(span);
    }

    public void Run(string text)
    {
        _lio.ResetInputBuffer();
        var inputBuffer = _lio.InputBuffer;
        var length = Encoding.UTF8.GetByteCount(text);
        var span = inputBuffer.AllocateRange(length + 1 + PaddingBytes);
        Encoding.UTF8.GetBytes(text, span);
        span[length] = Eof;
        span.Slice(length + 1).Fill(0);
        RunInternal(span);
    }

    private void RunInternal(ReadOnlySpan<byte> buffer)
    {
        _line = 0;
        _column = 0;

        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                _originalPtr = ptr;
                _length = buffer.Length;
                var startPtr = ptr;
                // Skip UTF8 BOM
                // 0xEF,0xBB,0xBF
                if (buffer.Length >= 3 && *ptr == 0xEF && ptr[1] == 0xBB && ptr[2] == 0xBF)
                {
                    startPtr += 3;
                }
                RunImpl(startPtr);
            }
            _originalPtr = null;
            _length = 0;
        }
    }

    private unsafe void RunImpl(byte* ptr)
    {
        while (ptr != null)
        {
            var c = *ptr;
            var startPtr = ptr;
            ptr = ByteToParser[c](this, ptr, c);
            Debug.Assert(ptr != startPtr, $"Invalid state of current pointer after processing `{Utf8Helper.ByteToSafeString(c)}`");
        }
    }

    private static unsafe byte* ParseEof(Lexer lexer, byte* ptr, byte c)
    {
        var offset = (uint)(ptr - lexer._originalPtr);
        lexer.AddToken(TokenKind.Eof, new TokenSpan(offset, 0, lexer._line, lexer._column));
        return null;
    }

    private static unsafe byte* ParseNop(Lexer lexer, byte* ptr, byte c)
    {
        // Skip the character
        ptr++;
        return ptr;
    }
    
    private static unsafe byte* ParseNewLine(Lexer lexer, byte* ptr, byte c)
    {
        var offset = (uint)(ptr - lexer._originalPtr);
        var length = (c == (byte)'\r' && ptr[1] == (byte)'\n') ? 2u : 1;
        lexer.AddToken(TokenKind.NewLine, new TokenSpan(offset, length, lexer._line, lexer._column));
        lexer._line++;
        lexer._column = 0;
        ptr += length;
        return ptr;
    }

    private static unsafe byte* ParseString(Lexer lexer, byte* ptr, byte c)
    {
        return ParseInterpolatedString(lexer, ptr, ptr, c, 0);
    }

    private static unsafe byte* ParseInterpolatedString(Lexer lexer, byte* startPtr, byte* ptr, byte c, int interpolatedCount)
    {
        // """
        if (c == (byte)'"' && *(short*)(ptr + 1) == 0x2222)
        {
            return ParseMultiLineString(lexer, startPtr, ptr, c, interpolatedCount);
        }
        return ParseSingleLineString(lexer, startPtr, ptr, c, interpolatedCount);
    }

    private static unsafe byte* ParseMultiLineString(Lexer lexer, byte* startPtr, byte* ptr, byte c, int interpolatedCount)
    {
        if (interpolatedCount > 0)
        {
            var localOffset = (uint)(startPtr - lexer._originalPtr);
            var localLength = (uint)(ptr - startPtr);
            lexer.AddToken(TokenKind.StringInterpolatedMacro, new TokenSpan(localOffset, localLength, lexer._line, lexer._column));
            lexer._column += (uint)interpolatedCount;
            startPtr = ptr;
        }

        var line = lexer._line;
        var column = lexer._column;
        while (c == (byte)'"')
        {
            ptr++;
            column++;
            c = *ptr;
        }

        int startingDoubleQuoteCount = (int)(ptr - startPtr);
        lexer.AddToken(TokenKind.MultiLineStringBegin, new TokenSpan((uint)(startPtr - lexer._originalPtr), (uint)startingDoubleQuoteCount, lexer._line, lexer._column));
        lexer._column = column;

        startPtr = ptr;
        var startLine = line;
        var startColumn = column;

        ref var tokenPartIndices = ref lexer._stringMultiLineTokenIndices;
        var tokenPartIndicesCount = tokenPartIndices.Count;

        ref var lineStates = ref lexer._stringMultilineStates;
        var lineStateCount = lineStates.Count;

        // We start with a negative spaceCount
        uint lineCount = 0;
        uint leadingSpaceCountCurrentLine = 0;
        bool isCurrentLineSpaceOnly = true;
        bool hasErrors = false;
        byte* startPtrCurrentLine = null;
        bool isFirstLine = true;
        var closingDoubleQuoteCount = 0;

    continue_interpolated_string:

        bool processInterpolated = false;
        while (true)
        {
            c = *ptr;
            if (c == (byte)'"')
            {
                while (c == (byte)'"')
                {
                    closingDoubleQuoteCount++;
                    ptr++;
                    column++;
                    c = *ptr;
                }

                // Check if we need to close the multiline string
                if (closingDoubleQuoteCount >= startingDoubleQuoteCount)
                {
                    // In case of a multiline
                    if (lineCount > 0)
                    {
                        // If the last line is not empty, this is an error
                        if (!isCurrentLineSpaceOnly)
                        {
                            lexer.LogError(ERR_InvalidRawStringExpectingEmptyLastLine(), startPtrCurrentLine + leadingSpaceCountCurrentLine, 1, line, leadingSpaceCountCurrentLine, line, leadingSpaceCountCurrentLine);
                            hasErrors = true;
                        }
                        else
                        {
                            // Verify that each line start with the correct number of leading spaces
                            for (var i = lineStateCount; i < lineStates.Count; i++)
                            {
                                ref var previousLineState = ref lineStates.Items[i];
                                if (previousLineState.LeadingSpaceCount < leadingSpaceCountCurrentLine)
                                {
                                    lexer.LogError(ERR_InvalidRawStringExpectingSpaceToMatchClosing(), lexer._originalPtr + previousLineState.Offset + previousLineState.LeadingSpaceCount, 1, previousLineState.Line, previousLineState.LeadingSpaceCount, previousLineState.Line, previousLineState.LeadingSpaceCount);
                                    hasErrors = true;
                                }
                            }

                            if (lineStateCount == lineStates.Count)
                            {
                                var delta = (uint)closingDoubleQuoteCount;
                                lexer.LogError(ERR_InvalidRawStringExpectingAtLeastOneLine(), ptr - delta, 1, line, column - delta, line, column - delta);
                            }
                        }
                    }

                    // We must match the exact number starting/ending quotes, if not this is an error
                    if (closingDoubleQuoteCount > startingDoubleQuoteCount)
                    {
                        var delta = (uint)(closingDoubleQuoteCount - startingDoubleQuoteCount);
                        lexer.LogError(ERR_InvalidRawStringExpectingEnoughQuotes(), ptr - delta, 1, line, column - delta, line, column - delta);
                        hasErrors = true;
                    }
                    break;
                }

                // Reset if not matched
                isCurrentLineSpaceOnly = false;
                closingDoubleQuoteCount = 0;
            }
            else if (c == (byte)'\r' || c == (byte)'\n')
            {
                ptr++;
                line++;
                if (c == (byte)'\r' && *ptr == '\n')
                {
                    ptr++;
                }

                if (isFirstLine)
                {
                    if (!isCurrentLineSpaceOnly)
                    {
                        var errorColumn = startColumn + leadingSpaceCountCurrentLine;
                        lexer.LogError(ERR_InvalidRawStringExpectingEmptyFirstLine(), startPtr + leadingSpaceCountCurrentLine, 1, startLine, errorColumn, startLine, errorColumn);
                        hasErrors = true;
                    }
                    isFirstLine = false;
                }
                else
                {
                    // We record the beginning of the previous line (but not the first line that must be empty)
                    lineStates.Add(new StringMultiLineState((uint)(startPtrCurrentLine - lexer._originalPtr), (uint)leadingSpaceCountCurrentLine, line - 1));
                }

                column = 0;
                startPtrCurrentLine = ptr;
                leadingSpaceCountCurrentLine = 0;
                isCurrentLineSpaceOnly = true;
                lineCount++;
            }
            else if (c >= StartUtf8 && TryParseUtf8(ref ptr, out var rune))
            {
                // If we have a multibyte UTF8, try to parse it correctly and correct the column
                var width = Wcwidth.UnicodeCalculator.GetWidth(rune);
                var delta = width <= 0 ? 0 : (uint)width;
                column += delta;
                isCurrentLineSpaceOnly = false;
            }
            else if (c == Eof)
            {
                lexer.LogError(ERR_InvalidRawStringUnexpectedEndOfString(new string('"', startingDoubleQuoteCount)), ptr, 1, line, column, line, column);
                hasErrors = true;
                break;
            }
            else
            {
                if (c == (byte)'{' && interpolatedCount > 0)
                {
                    isCurrentLineSpaceOnly = false;

                    int countOpenBrace = 0;
                    while (true)
                    {
                        if (ptr[countOpenBrace] != (byte)'{')
                        {
                            break;
                        }
                        countOpenBrace++;
                    }

                    // We take the last interpolatedCount:
                    // {{{{{
                    //    ^^ 
                    //    interpolatedCount
                    if (countOpenBrace >= interpolatedCount)
                    {
                        var advance = countOpenBrace - interpolatedCount;
                        ptr += advance;
                        column += (uint)advance;
                        processInterpolated = true;
                        break;
                    }

                    ptr += countOpenBrace;
                    column += (uint)countOpenBrace;
                    continue;
                }

                if (isCurrentLineSpaceOnly)
                {
                    if (c == (byte)' ')
                    {
                        leadingSpaceCountCurrentLine++;
                    }
                    else
                    {
                        isCurrentLineSpaceOnly = false;
                    }
                }

                ptr++;
                column++;
            }
        }

        var offset = (uint)(startPtr - lexer._originalPtr);
        var length = (uint)(ptr - startPtr - closingDoubleQuoteCount);
        // Only emit a token if we have really some content for the token
        if (length > 0)
        {
            // Store the index of this token
            tokenPartIndices.Add(lexer.TokenCount);
            var kind = interpolatedCount > 0 ? TokenKind.MultiLineStringInterpolatedPart : TokenKind.MultiLineStringPart;
            // Empty string
            lexer.AddToken(kind, new TokenSpan(offset, length, lexer._line, lexer._column));
            lexer._line = line;
            lexer._column = (uint)(column - closingDoubleQuoteCount);
        }

        if (closingDoubleQuoteCount > 0)
        {
            offset = (uint)(ptr - lexer._originalPtr - closingDoubleQuoteCount);
            lexer.AddToken(TokenKind.MultiLineStringEnd, new TokenSpan(offset, (uint)closingDoubleQuoteCount, lexer._line, lexer._column));
            lexer._column += (uint)closingDoubleQuoteCount;

            // Generates the string content if we didn't have any errors
            if (!hasErrors)
            {
                ParseMultiLineStringContent(lexer, (int)tokenPartIndicesCount, leadingSpaceCountCurrentLine, lineCount);
            }
        }
        else if (processInterpolated)
        {
            ptr = ParseInterpolatedContent(lexer, ptr, interpolatedCount);
            if (ptr is not null)
            {
                startPtr = ptr;
                line = lexer._line;
                column = lexer._column;
                goto continue_interpolated_string;
            }
        }

        // Restore the count when entering this method
        tokenPartIndices.Count = tokenPartIndicesCount;
        lineStates.Count = lineStateCount;

        return ptr;
    }

    private static void ParseMultiLineStringContent(Lexer lexer, int startStringTokenIndex, uint trailingSpaceCount, uint expectedLineCount)
    {
        // First line
        var stringTokens = lexer._stringMultiLineTokenIndices;
        var lio = lexer._lio;
        var tokenSpan = lio.TokenSpans[stringTokens[(uint)startStringTokenIndex]];
        var span = lexer.GetSpan((int)tokenSpan.Offset, (int)tokenSpan.Length);

        var tempBuffer = lexer.TempBuffer;

        // Skip leading \r\n
        if (expectedLineCount > 0 && span.Length > 0)
        {
            for (int i = 0; i < span.Length; i++)
            {
                var c = span[i];
                if (c == (byte)'\r' || c == (byte)'\n')
                {
                    int length = 1;
                    if (c == (byte)'\r' && i + 1 < span.Length && span[i + 1] == (byte)'\n')
                    {
                        length++;
                    }
                    span = span.Slice(i + length);
                    break;
                }
            }
        }
        
        int tokenSpanIndirectIndex = startStringTokenIndex;
        bool startOfLine = expectedLineCount > 0;
        int currentLineIndex = 1;
        int leadingSpaceCount = 0;
        tempBuffer.Reset(VirtualArenaResetKind.KeepAllCommitted);

        // Proceed spans
        while (true)
        {
            // optimize this loop by appending ranges instead of character 1 by 1
            for (int i = 0; i < span.Length; i++)
            {
                var c = span[i];
                if (c == (byte)'\r' || c == (byte)'\n')
                {
                    if (c == (byte)'\r' && i + 1 < span.Length && span[i + 1] == '\n')
                    {
                        i++;
                    }

                    startOfLine = true;
                    leadingSpaceCount = 0;
                    currentLineIndex++;

                    if (currentLineIndex < expectedLineCount)
                    {
                        tempBuffer.Append((byte)'\n');
                    }
                }
                else
                {
                    if (c == (byte)' ')
                    {
                        if (startOfLine)
                        {
                            leadingSpaceCount++;
                            if (leadingSpaceCount <= trailingSpaceCount)
                            {
                                continue;
                            }
                        }
                    }
                    startOfLine = false;
                    tempBuffer.Append(c);
                }
            }

            if (tempBuffer.AllocatedBytes > 0)
            {
                var strHandle = lio.GetStringHandle(tempBuffer.AsSpan());
                var tokenSpanIndex = stringTokens[(uint)tokenSpanIndirectIndex];
                lio.TokenValues[tokenSpanIndex] = new TokenValue(strHandle);
                tempBuffer.Reset(VirtualArenaResetKind.KeepAllCommitted);
            }

            tokenSpanIndirectIndex++;
            if (tokenSpanIndirectIndex >= stringTokens.Count)
            {
                break;
            }
            
            tokenSpan = lexer._lio.TokenSpans[stringTokens[(uint)tokenSpanIndirectIndex]];
            span = lexer.GetSpan((int)tokenSpan.Offset, (int)tokenSpan.Length);
        }
    }
    
    private static unsafe byte* ParseSingleLineString(Lexer lexer, byte* startPtr, byte* ptr, byte c, int interpolatedCount)
    {
        var startChar = c;

        // Token for string starting with macro $
        if (interpolatedCount > 0)
        {
            var localOffset = (uint)(startPtr - lexer._originalPtr);
            var localLength = (uint)(ptr - startPtr);
            lexer.AddToken(TokenKind.StringInterpolatedMacro, new TokenSpan(localOffset, localLength, lexer._line, lexer._column));
            lexer._column += (uint)interpolatedCount;
            startPtr = ptr;
        }
        var column = lexer._column;

    continue_interpolated_string:
        // Use the temp buffer to create the string
        var tempBuffer = lexer.TempBuffer;
        lexer.ResetTempBuffer();

        bool processInterpolated = false;
        while (true)
        {
            ptr++;
            proceed_next_char:
            c = *ptr;
            if (c == (byte)'\\')
            {
                column++;
                ptr++; // Skip \
                c = *ptr;
                switch (c)
                {
                    case (byte)'0':
                        tempBuffer.Append(0);
                        column++;
                        continue;
                    case (byte)'\'':
                        tempBuffer.Append((byte)'\'');
                        column++;
                        continue;
                    case (byte)'"':
                        tempBuffer.Append((byte)'"');
                        column++;
                        continue;
                    case (byte)'\\':
                        tempBuffer.Append((byte)'\\');
                        column++;
                        continue;
                    case (byte)'b':
                        tempBuffer.Append((byte)'\b');
                        column++;
                        continue;
                    case (byte)'f':
                        tempBuffer.Append((byte)'\f');
                        column++;
                        continue;
                    case (byte)'n':
                        tempBuffer.Append((byte)'\n');
                        column++;
                        continue;
                    case (byte)'r':
                        tempBuffer.Append((byte)'\r');
                        column++;
                        continue;
                    case (byte)'t':
                        tempBuffer.Append((byte)'\t');
                        column++;
                        continue;
                    case (byte)'v':
                        tempBuffer.Append((byte)'\v');
                        column++;
                        continue;
                    case (byte)'U':
                        var startColumnUtf32 = column;
                        var startPtrUtf8 = ptr - 1;
                        column++;
                        int valueUtf32 = 0;
                        int countUtf32 = 0;
                        // \U	Unicode escape sequence (UTF-32)	\U00HHHHHH (range: 000000 - 10FFFF; example: \U0001F47D = "👽")
                        // Expecting 8 hex numbers
                        for (int i = 0; i < 8; i++)
                        {
                            ptr++;
                            c = *ptr;
                            if (Utf8Helper.IsHex(c))
                            {
                                column++;
                                valueUtf32 = (valueUtf32 << 4) | Utf8Helper.HexToValue(c);
                                countUtf32++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (countUtf32 == 8)
                        {
                            if (Rune.TryCreate(valueUtf32, out var rune))
                            {
                                var span = tempBuffer.AllocateRange(rune.Utf8SequenceLength); // append value
                                rune.EncodeToUtf8(span);
                            }
                            else
                            {
                                
                                lexer.LogError(ERR_InvalidUtf8InString(valueUtf32), startPtrUtf8, startColumnUtf32);
                            }
                            continue;
                        }

                        lexer.LogError(ERR_InvalidHexNumberInString3(Utf8Helper.ByteToSafeString(c)), ptr, column + 1);
                        goto proceed_next_char;
                    
                    case (byte)'u':
                        column++;

                        ptr++;
                        c = *ptr;
                        // \u	Unicode escape sequence (UTF-16)	\uHHHH (range: 0000 - FFFF; example: \u00E7 = "ç")

                        // Must be followed 4 hex numbers (0000-FFFF)
                        if (Utf8Helper.IsHex(c)) // 1
                        {
                            column++;
                            var valueUtf16 = Utf8Helper.HexToValue(c);
                            ptr++;
                            c = *ptr;
                            if (Utf8Helper.IsHex(c)) // 2
                            {
                                column++;
                                valueUtf16 = (valueUtf16 << 4) | Utf8Helper.HexToValue(c);
                                ptr++;
                                c = *ptr;
                                if (Utf8Helper.IsHex(c)) // 3
                                {
                                    column++;
                                    valueUtf16 = (valueUtf16 << 4) | Utf8Helper.HexToValue(c);
                                    ptr++;
                                    c = *ptr;
                                    if (Utf8Helper.IsHex(c)) // 4
                                    {
                                        column++;
                                        valueUtf16 = (valueUtf16 << 4) | Utf8Helper.HexToValue(c);
                                        Rune.TryCreate((char)valueUtf16, out var rune);
                                        var span = tempBuffer.AllocateRange(rune.Utf8SequenceLength); // append value
                                        rune.EncodeToUtf8(span);
                                        continue;
                                    }
                                }
                            }
                        }

                        lexer.LogError(ERR_InvalidHexNumberInString1(Utf8Helper.ByteToSafeString(c)), ptr, column + 1);
                        goto proceed_next_char;

                    case (byte)'x':
                        column++;

                        ptr++;
                        c = *ptr;
                        // \x	Unicode escape sequence similar to "\u" except with variable length	\xH[H][H][H] (range: 0 - FFFF; example: \x00E7 or \x0E7 or \xE7 = "ç")
                        // Must be followed 2 hex numbers (00-FF)
                        if (Utf8Helper.IsHex(c))
                        {
                            column++;
                            var valueUtf16 = Utf8Helper.HexToValue(c);
                            bool proceedToNextChar = false;
                            for (int i = 0; i < 3; i++)
                            {
                                ptr++;
                                c = *ptr;
                                if (Utf8Helper.IsHex(c))
                                {
                                    column++;
                                    valueUtf16 = (valueUtf16 << 4) | Utf8Helper.HexToValue(c);
                                }
                                else
                                {
                                    proceedToNextChar = true;
                                    break;
                                }
                            }

                            Rune.TryCreate((char)valueUtf16, out var rune);
                            var span = tempBuffer.AllocateRange(rune.Utf8SequenceLength); // append value
                            rune.EncodeToUtf8(span);

                            if (proceedToNextChar)
                            {
                                goto proceed_next_char;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        lexer.LogError(ERR_InvalidHexNumberInString2(Utf8Helper.ByteToSafeString(c)), ptr, column + 1);
                        goto proceed_next_char;

                    default:
                        lexer.LogError(ERR_UnexpectedEscapeCharacter(Utf8Helper.ByteToSafeString(c)), ptr, column + 1);
                        goto proceed_next_char;
                }
            }
            else if (c == startChar)
            {
                column++;

                ptr++; // We can skip the last " or '
                column++;
                break;
            }
            else if (c >= StartUtf8 && TryParseUtf8(ref ptr, out var rune))
            {
                var span = tempBuffer.AllocateRange(rune.Utf8SequenceLength); // append value
                rune.EncodeToUtf8(span);

                // If we have a multibyte UTF8, try to parse it correctly and correct the column
                var width = Wcwidth.UnicodeCalculator.GetWidth(rune);
                column += width <= 0 ? 0 : (uint)width;
                goto proceed_next_char;
            }
            else if (c == Eof || c == '\r' || c == '\n')
            {
                column++;
                lexer.LogError(ERR_UnexpectedEndOfString(), ptr, column);
                break;
            }
            else
            {
                if (c == '{' && interpolatedCount > 0)
                {
                    int countOpenBrace = 0;
                    while(true)
                    {
                        if (ptr[countOpenBrace] != '{')
                        {
                            break;
                        }
                        countOpenBrace++;
                    }

                    // We take the last interpolatedCount:
                    // {{{{{
                    //    ^^ 
                    //    interpolatedCount
                    if (countOpenBrace >= interpolatedCount)
                    {
                        var advance = countOpenBrace - interpolatedCount;
                        for (int i = 0; i < advance; i++)
                        {
                            tempBuffer.Append(c);
                        }
                        ptr += advance;
                        column += (uint)advance + 1;
                        processInterpolated = true;
                        break;
                    }
                    else
                    {
                        for (int i = 0; i < countOpenBrace; i++)
                        {
                            tempBuffer.Append(c);
                        }
                        ptr += countOpenBrace;
                        column += (uint)countOpenBrace;
                        goto proceed_next_char;
                    }
                }

                tempBuffer.Append(c); // append value
                column++;
            }
        }

        var offset = (uint)(startPtr - lexer._originalPtr);
        var length = (uint)(ptr - startPtr);
        var kind = interpolatedCount > 0 ? TokenKind.StringInterpolatedPart : TokenKind.String;
        if (tempBuffer.AllocatedBytes > 0)
        {
            lexer.AddToken(kind, new TokenSpan(offset, length, lexer._line, lexer._column), tempBuffer.AsSpan());
        }
        else
        {
            // Empty string
            lexer.AddToken(kind, new TokenSpan(offset, length, lexer._line, lexer._column));
        }
        lexer._column = column;

        // Parse interpolated string
        if (processInterpolated)
        {
            ptr = ParseInterpolatedContent(lexer, ptr, interpolatedCount);
            if (ptr is not null)
            {
                startPtr = ptr;
                column = lexer._column - 1;
                ptr--;
                goto continue_interpolated_string;
            }
        }

        return ptr;
    }

    private static unsafe byte* ParseInterpolatedContent(Lexer lexer, byte* ptr, int interpolatedCount)
    {
        // Start of interpolated
        var offset = (uint)(ptr - lexer._originalPtr);
        lexer.AddToken(TokenKind.StringInterpolatedBegin, new TokenSpan(offset, (uint)interpolatedCount, lexer._line, lexer._column));
        ptr += interpolatedCount;
        lexer._column += (uint)interpolatedCount;

        // Iterate on teh interpolated content
        var innerBrace = 0;
        byte* previousPtr = ptr;
        while (ptr != null)
        {
            var c = *ptr;
            if (c == '{')
            {
                innerBrace++;
            }
            else if (c == '}')
            {
                if (innerBrace == 0)
                {
                    bool processInterpolated = true;
                    for (int i = 1; i < interpolatedCount; i++)
                    {
                        if (ptr[i] != '}')
                        {
                            processInterpolated = false;
                            break;
                        }
                    }

                    if (processInterpolated)
                    {
                        break;
                    }
                }
                else
                {
                    innerBrace--;
                }
            }

            previousPtr = ptr;
            ptr = ByteToParser[c](lexer, ptr, c);
            Debug.Assert(ptr != previousPtr, $"Invalid state of current pointer after processing `{Utf8Helper.ByteToSafeString(c)}`");
        }

        // Unexpected end of file
        if (ptr == null)
        {
            lexer.LogError(ERR_UnexpectedEndOfFileForInterpolatedString(new string('}', interpolatedCount)), previousPtr, lexer._column);
        }
        else
        {
            // End of interpolated
            offset = (uint)(ptr - lexer._originalPtr);
            lexer.AddToken(TokenKind.StringInterpolatedEnd, new TokenSpan(offset, (uint)interpolatedCount, lexer._line, lexer._column));
            ptr += interpolatedCount;
            lexer._column += (uint)interpolatedCount;
        }

        return ptr;
    }

    private unsafe void LogError(DiagnosticMessage message, byte* ptr, uint column)
    {
        var textLocation = new TextLocation((uint)(ptr - _originalPtr), _line, column);
        _lio.Diagnostics.Add(new Diagnostic(DiagnosticKind.Error, new DiagnosticSourceFileLocation("TODO_FILE", new TextSpan(textLocation)), message));
    }

    private unsafe void LogError(DiagnosticMessage message, byte* ptr, int length, uint column)
    {
        var startLocation = new TextLocation((uint)(ptr - _originalPtr), _line, column);
        var endLocation = new TextLocation((uint)(ptr - _originalPtr + length - 1), _line, column + (uint)length - 1);
        _lio.Diagnostics.Add(new Diagnostic(DiagnosticKind.Error, new DiagnosticSourceFileLocation("TODO_FILE", new TextSpan(startLocation, endLocation)), message));
    }

    private unsafe void LogError(DiagnosticMessage message, byte* ptr, int length, uint startOfLine, uint columnStart, uint endOfLine, uint columnEnd)
    {
        var startLocation = new TextLocation((uint)(ptr - _originalPtr), startOfLine, columnStart);
        var endLocation = new TextLocation((uint)(ptr - _originalPtr + length - 1), endOfLine, columnEnd);
        _lio.Diagnostics.Add(new Diagnostic(DiagnosticKind.Error, new DiagnosticSourceFileLocation("TODO_FILE", new TextSpan(startLocation, endLocation)), message));
    }


    private static unsafe byte* ParseWhitespace(Lexer lexer, byte* ptr, byte c)
    {
        // We backtrack to optimize for leading spaces in a line that are usually a multiple of 4 bytes
        var startPtr = ptr;
        while (true)
        {
            var value = *(int*)ptr;
            if (value == 0x20202020)
            {
                ptr += 4;
            }
            else if ((byte)value == (byte)' ')
            {
                ptr++;
            }
            else
            {
                break;
            }
        }

        var offset = (uint)(startPtr - lexer._originalPtr);
        var length = (uint)(ptr - startPtr);
        lexer.AddToken(TokenKind.WhiteSpace, new TokenSpan(offset, length, lexer._line, lexer._column));
        lexer._column += length;

        return ptr;
    }

    private static unsafe byte* ParseNumber(Lexer lexer, byte* ptr, byte c)
    {
        if (c == (byte)'0')
        {
            switch (ptr[1])
            {
                case (byte)'x':
                    return ParseNumberHex(lexer, ptr);
                case (byte)'b':
                    return ParseNumberBinary(lexer, ptr);
                case (byte)'o':
                    return ParseNumberOctal(lexer, ptr);
            }
        }

        lexer.ResetTempBuffer();
        var tempBuffer = lexer.TempBuffer;
        tempBuffer.Append(c);

        var startPtr = ptr;

        var ptrToFirstUnderscore = (byte*)null;
        int lengthOfUnderscore = 0;
        while (true)
        {
            ptr++;
            c = *ptr;
            if (c == (byte)'_')
            {
                if (ptrToFirstUnderscore is null)
                {
                    ptrToFirstUnderscore = ptr;
                }
                lengthOfUnderscore++;
                continue;
            }

            if (!Utf8Helper.IsDigit(c))
            {
                break;
            }
            else
            {
                tempBuffer.Append(c);

                ptrToFirstUnderscore = null;
                lengthOfUnderscore = 0;
            }
        }

        // We cannot end a number with an underscore
        if (lengthOfUnderscore > 0)
        {
            lexer.LogError(ERR_UnexpectedUnderscoreAfterDigit(), ptrToFirstUnderscore, lengthOfUnderscore, lexer._column + (uint)(ptrToFirstUnderscore - startPtr));
        }

        // Switch to parsing a float if detected
        if ((c == (byte)'.' && Utf8Helper.IsDigit(ptr[1])) || c == (byte)'e' || c == 'E')
        {
            return ParseNumberFloat(lexer, startPtr, ptr, c);
        }

        var number = 0ul;
        var span = tempBuffer.AsSpan();
        bool hasOverflow = false;
        for (int i = 0; i < span.Length; i++)
        {
            var digit = (int)(span[i] - '0');
            var next = number * 10 + (ulong)digit;
            if (next < number)
            {
                hasOverflow = true;
            }
            number = next;
        }

        var offset = (uint)(startPtr - lexer._originalPtr);
        var length = (uint)(ptr - startPtr);

        if (hasOverflow)
        {
            // Generate an overflow error only once
            lexer.LogError(ERR_NumberOverflow(), startPtr, (int)length, lexer._column);
        }

        lexer.AddToken(TokenKind.Integer, new TokenSpan(offset, length, lexer._line, lexer._column), new TokenValue(number));
        lexer._column += length;

        return ptr;
    }

    private static unsafe byte* ParseNumberHex(Lexer lexer, byte* ptr)
    {
        ulong number = 0;
        var startPtr = ptr;
        ptr++; // Skip 0, next is skipping x
        var ptrToFirstUnderscore = (byte*)null;
        int lengthOfUnderscore = 0;
        bool hasOverflow = false;
        bool hasDigits = false;
        while (true)
        {
            ptr++;
            var c = *ptr;
            if (c == (byte)'_')
            {
                if (ptrToFirstUnderscore is null)
                {
                    ptrToFirstUnderscore = ptr;
                }
                lengthOfUnderscore++;
                continue;
            }

            if (!Utf8Helper.IsHex(c))
            {
                break;
            }
            else
            {
                if ((number >> 60) != 0)
                {
                    hasOverflow = true;
                }

                var hex = (ulong)(Utf8Helper.HexToValue(c));
                number = (number << 4) | hex;

                ptrToFirstUnderscore = null;
                lengthOfUnderscore = 0;
                hasDigits = true;
            }
        }
        

        // We cannot end a number with an underscore
        if (lengthOfUnderscore > 0)
        {
            lexer.LogError(ERR_UnexpectedUnderscoreAfterDigit(), ptrToFirstUnderscore, lengthOfUnderscore, lexer._column + (uint)(ptrToFirstUnderscore - startPtr));
        }

        var offset = (uint)(startPtr - lexer._originalPtr);
        var length = (uint)(ptr - startPtr);

        if (!hasDigits)
        {
            lexer.LogError(ERR_InvalidHexNumberExpectingDigit(), startPtr, (int)length, lexer._column);
        }
        
        if (hasOverflow)
        {
            // Generate an overflow error only once
            lexer.LogError(ERR_NumberOverflow(), startPtr, (int)length, lexer._column);
        }
        
        lexer.AddToken(TokenKind.Integer, new TokenSpan(offset, length, lexer._line, lexer._column), new TokenValue(number));
        lexer._column += length;

        return ptr;
    }

    private static unsafe byte* ParseNumberOctal(Lexer lexer, byte* ptr)
    {
        ulong number = 0;
        var startPtr = ptr;
        ptr++; // Skip 0, next is skipping o
        var ptrToFirstUnderscore = (byte*)null;
        int lengthOfUnderscore = 0;
        bool hasOverflow = false;
        bool hasDigits = false;
        while (true)
        {
            ptr++;
            var c = *ptr;
            if (c == (byte)'_')
            {
                if (ptrToFirstUnderscore is null)
                {
                    ptrToFirstUnderscore = ptr;
                }
                lengthOfUnderscore++;
                continue;
            }

            if (c < (byte)'0' || c > (byte)'7')
            {
                break;
            }
            else
            {
                if ((number >> 61) != 0)
                {
                    hasOverflow = true;
                }

                var hex = (ulong)(c - '0');
                number = (number << 3) | hex;

                ptrToFirstUnderscore = null;
                lengthOfUnderscore = 0;
                hasDigits = true;
            }
        }

        // We cannot end a number with an underscore
        if (lengthOfUnderscore > 0)
        {
            lexer.LogError(ERR_UnexpectedUnderscoreAfterDigit(), ptrToFirstUnderscore, lengthOfUnderscore, lexer._column + (uint)(ptrToFirstUnderscore - startPtr));
        }

        var offset = (uint)(startPtr - lexer._originalPtr);
        var length = (uint)(ptr - startPtr);

        if (!hasDigits)
        {
            lexer.LogError(ERR_InvalidOctalNumberExpectingDigit(), startPtr, (int)length, lexer._column);
        }

        if (hasOverflow)
        {
            // Generate an overflow error only once
            lexer.LogError(ERR_NumberOverflow(), startPtr, (int)length, lexer._column);
        }

        lexer.AddToken(TokenKind.Integer, new TokenSpan(offset, length, lexer._line, lexer._column), new TokenValue(number));
        lexer._column += length;

        return ptr;
    }
    
    private static unsafe byte* ParseNumberBinary(Lexer lexer, byte* ptr)
    {
        ulong number = 0;
        var startPtr = ptr;
        ptr++; // skip 0, next is skipping b
        var ptrToFirstUnderscore = (byte*)null;
        int lengthOfUnderscore = 0;
        bool hasOverflow = false;
        bool hasDigits = false;
        while (true)
        {
            ptr++;
            var c = *ptr;
            if (c == (byte)'_')
            {
                if (ptrToFirstUnderscore is null)
                {
                    ptrToFirstUnderscore = ptr;
                }
                lengthOfUnderscore++;
                continue;
            }

            if (c == (byte)'0' || c == (byte)'1')
            {
                if ((long)number < 0)
                {
                    hasOverflow = true;
                }

                var hex = (ulong)(c - '0');
                number = (number << 1) | hex;
                
                ptrToFirstUnderscore = null;
                lengthOfUnderscore = 0;
                hasDigits = true;
            }
            else
            {
                break;
            }
        }

        // We cannot end a number with an underscore
        if (lengthOfUnderscore > 0)
        {
            lexer.LogError(ERR_UnexpectedUnderscoreAfterDigit(), ptrToFirstUnderscore, lengthOfUnderscore, lexer._column + (uint)(ptrToFirstUnderscore - startPtr));
        }

        var offset = (uint)(startPtr - lexer._originalPtr);
        var length = (uint)(ptr - startPtr);
        
        if (!hasDigits)
        {
            lexer.LogError(ERR_InvalidHexNumberExpectingDigit(), startPtr, (int)length, lexer._column);
        }

        if (hasOverflow)
        {
            // Generate an overflow error only once
            lexer.LogError(ERR_NumberOverflow(), startPtr, (int)length, lexer._column);
        }

        lexer.AddToken(TokenKind.Integer, new TokenSpan(offset, length, lexer._line, lexer._column), new TokenValue(number));
        lexer._column += length;

        return ptr;
    }

    private static unsafe byte* ParseNumberFloat(Lexer lexer, byte* startPtr, byte* ptr, byte c)
    {
        var tempBuffer = lexer.TempBuffer;
       
        var ptrToFirstUnderscore = (byte*)null;
        int lengthOfUnderscore = 0;
        
        // c is `.` or `e` or `E`
        if (c == '.')
        {
            tempBuffer.Append(c);

            // When we are here, we have a guarantee that we will get at least 1 digit
            // Parse digit after the dot
            while (true)
            {
                ptr++;
                c = *ptr;
                if (c == (byte)'_')
                {
                    if (ptrToFirstUnderscore is null)
                    {
                        ptrToFirstUnderscore = ptr;
                    }

                    lengthOfUnderscore++;
                    continue;
                }

                if (!Utf8Helper.IsDigit(c))
                {
                    break;
                }
                else
                {
                    tempBuffer.Append(c);

                    ptrToFirstUnderscore = null;
                    lengthOfUnderscore = 0;
                }
            }

            // We cannot end a number with an underscore
            if (lengthOfUnderscore > 0)
            {
                lexer.LogError(ERR_UnexpectedUnderscoreAfterDigit(), ptrToFirstUnderscore, lengthOfUnderscore, lexer._column + (uint)(ptrToFirstUnderscore - startPtr));
            }
        }

        // Parse Exponent: e.g E+100, e-10
        if (c == 'E' || c == 'e')
        {
            tempBuffer.Append(c);
            ptr++;
            c = *ptr;
            if (c == '+' || c == '-')
            {
                tempBuffer.Append(c);
                ptr++;
                c = *ptr;
            }

            // Parse number 
            if (Utf8Helper.IsDigit(c))
            {
                tempBuffer.Append(c);

                ptrToFirstUnderscore = null;
                lengthOfUnderscore = 0;
                while (true)
                {
                    ptr++;
                    c = *ptr;
                    if (c == (byte)'_')
                    {
                        if (ptrToFirstUnderscore is null)
                        {
                            ptrToFirstUnderscore = ptr;
                        }
                        lengthOfUnderscore++;
                        continue;
                    }

                    if (!Utf8Helper.IsDigit(c))
                    {
                        break;
                    }
                    else
                    {
                        tempBuffer.Append(c);

                        ptrToFirstUnderscore = null;
                        lengthOfUnderscore = 0;
                    }
                }

                // We cannot end the exponent with an underscore
                if (lengthOfUnderscore > 0)
                {
                    lexer.LogError(ERR_UnexpectedUnderscoreAfterDigit(), ptrToFirstUnderscore, lengthOfUnderscore, lexer._column + (uint)(ptrToFirstUnderscore - startPtr));
                }
            }
            else
            {
                lexer.LogError(ERR_UnexpectedCharacterForExponent(Utf8Helper.ByteToSafeString(c)), ptr, 1, lexer._column + (uint)(ptr - startPtr));

                // log an error
                // Append a 0 to be able to parse the double
                tempBuffer.Append((byte)'0');
            }
        }

        var span = tempBuffer.AsSpan();
        // Should always succeed
        _ = csFastFloat.FastDoubleParser.TryParseDouble(span, out var result);
        var offset = (uint)(startPtr - lexer._originalPtr);
        var length = (uint)(ptr - startPtr);
        lexer.AddToken(TokenKind.Float, new TokenSpan(offset, length, lexer._line, lexer._column), new TokenValue(result));
        lexer._column += length;

        return ptr;
    }

    private static unsafe byte* ParseCommentOrSlash(Lexer lexer, byte* ptr, byte c)
    {
        c = ptr[1];
        switch (c)
        {
            case (byte)'/':
                return ParseCommentSingleLine(lexer, ptr);
            case (byte)'*':
                return ParseCommentMultiLine(lexer, ptr);
        }

        var offset = (uint)(ptr - lexer._originalPtr);
        var kind = TokenKind.Slash;
        var length = 1u;
        if (c == (byte)'=') // /=
        {
            kind = TokenKind.SlashEqual;
            length = 2;
        }

        lexer.AddToken(kind, new TokenSpan(offset, length, lexer._line, lexer._column));
        lexer._column += length;
        ptr += length;
        return ptr;
    }

    private static unsafe byte* ParseCommentSingleLine(Lexer lexer, byte* ptr)
    {
        var startPtr = ptr;
        var offset = (uint)(ptr - lexer._originalPtr);

        var kind = ptr[2] == (byte)'/' ? TokenKind.CommentDocumentationSingleLine : TokenKind.CommentSingleLine;
        var column = lexer._column + 1;
        while (true)
        {
            ptr++;
            proceed_next_char:
            var c = *ptr;
            if (c >= StartUtf8 && TryParseUtf8(ref ptr, out var rune))
            {
                // If we have a multibyte UTF8, try to parse it correctly and correct the column
                var width = Wcwidth.UnicodeCalculator.GetWidth(rune);
                column += width <= 0 ? 0 : (uint)width;
                goto proceed_next_char;
            }
            else if (c == Eof || c == '\r' || c == '\n')
            {
                break;
            }
            else
            {
                column++;
            }
        }

        var length = (uint)(ptr - startPtr);
        lexer.AddToken(kind, new TokenSpan(offset, length, lexer._line, lexer._column));
        lexer._column = column;

        return ptr;
    }

    private static unsafe byte* ParseCommentMultiLine(Lexer lexer, byte* ptr)
    {
        var startPtr = ptr;
        var offset = (uint)(ptr - lexer._originalPtr);
        int commentDepth = 1;

        ptr++; // Skip /
        int column = (int)lexer._column + 1;
        var line = lexer._line;
        while (true)
        {
            ptr++;
            proceed_next_char:
            var c = *ptr;
            // Match pending */
            if (c == (byte)'*')
            {
                column++;
                if (ptr[1] == (byte)'/')
                {
                    ptr++; // skip *
                    column++;
                    commentDepth--;
                    if (commentDepth == 0)
                    {
                        ptr++; // skip /
                        column++;
                        break;
                    }
                }
            }
            else if (c == (byte)'\r')
            {
                line++;
                if (ptr[1] == (byte)'\n')
                {
                    ptr++; // skip \r
                }
                column = -1;
            }
            else if (c == (byte)'\n')
            {
                // Count line
                line++;
                column = -1;
            }
            else if (c == (byte)'/') // Match nesting /*
            {
                column++;
                if (ptr[1] == (byte)'*')
                {
                    ptr++; // skip /
                    column++;
                    commentDepth++;
                }
            }
            else if (c >= StartUtf8 && TryParseUtf8(ref ptr, out var rune))
            {
                // If we have a multibyte UTF8, try to parse it correctly and correct the column
                var width = Wcwidth.UnicodeCalculator.GetWidth(rune);
                column += width <= 0 ? 0 : width;
                goto proceed_next_char;
            }
            else if (c == Eof)
            {
                // Break on Eof, this is an invalid multiline comment not terminating by a */
                column++;
                break;
            }
            else
            {
                column++;
            }
        }

        var length = (uint)(ptr - startPtr);

        // Eof without terminating the multi-line comment => this is an error
        if (commentDepth > 0)
        {
            lexer.LogError(ERR_UnexpectedEndOfFileForMultiLineComment(commentDepth), startPtr, (int)length, lexer._line, lexer._column, line, (uint)(column - 1));
        }

        lexer.AddToken(TokenKind.CommentMultiLine, new TokenSpan(offset, length, lexer._line, lexer._column));
        lexer._line = line;
        lexer._column = (uint)column;
        return ptr;
    }

    private static unsafe byte* ParseKeywordOrIdentifierOrUnderscore(Lexer lexer, byte* ptr, byte c)
    {
        var startPtr = ptr;
        var underscoreCount = c == (byte)'_' ? 1 : 0;
        while (true)
        {
            ptr++;
            c = *ptr;
            if (!Utf8Helper.IsLetterContinuationForIdentifier(c))
            {
                break;
            }
            underscoreCount += (c == (byte)'_') ? 1 : 0;
        }
        var offset = (uint)(startPtr - lexer._originalPtr);
        var length = (uint)(ptr - startPtr);

        // The underscore symbol generates an underscore symbol, not an identifier!
        if (underscoreCount == length)
        {
            lexer.AddToken(TokenKind.Underscore, new TokenSpan(offset, length, lexer._line, lexer._column));
        }
        else
        {
            var span = new ReadOnlySpan<byte>(startPtr, (int)length);
            var hash = lexer.GetHashCode(span);
            var kind = KeywordHelper.GetKeywordTokenKind(span, hash);
            if (kind == TokenKind.Identifier)
            {
                lexer.AddToken(kind, new TokenSpan(offset, length, lexer._line, lexer._column), span);
            }
            else
            {
                // For keyword, we don't generate
                lexer.AddToken(kind, new TokenSpan(offset, length, lexer._line, lexer._column));
            }
        }
        lexer._column += length;

        return ptr;
    }

    private static unsafe byte* ParseInvalid(Lexer lexer, byte* ptr, byte c)
    {
        var offset = (uint)(ptr - lexer._originalPtr);
        lexer.AddToken(TokenKind.Invalid, new TokenSpan(offset, 1, lexer._line, lexer._column));
        lexer._column++;
        ptr++;
        return ptr;
    }

    private static unsafe byte* ParseInvalidTab(Lexer lexer, byte* ptr, byte c)
    {
        var offset = (uint)(ptr - lexer._originalPtr);
        lexer.AddToken(TokenKind.InvalidTab, new TokenSpan(offset, 1, lexer._line, lexer._column));
        lexer._column++;
        ptr++;
        return ptr;
    }

    private static unsafe bool TryParseUtf8(ref byte* ptr, out Rune result)
    {
        var status = Rune.DecodeFromUtf8(new ReadOnlySpan<byte>(ptr, 4), out result, out var numberBytes) == OperationStatus.Done;
        ptr += status ? numberBytes : 0;
        return status;
    }

    private static unsafe byte* ParseUtf8(Lexer lexer, byte* ptr, byte c)
    {
        var startPtr = ptr;
        var offset = (uint)(ptr - lexer._originalPtr);
        if (TryParseUtf8(ref ptr, out var rune))
        {
            lexer.AddToken(TokenKind.InvalidUtf8, new TokenSpan(offset,(uint)(ptr - startPtr) , lexer._line, lexer._column));
            var width = Wcwidth.UnicodeCalculator.GetWidth(rune);
            lexer._column += width <= 0 ? 0 : (uint)width;
        }
        else
        {
            lexer.AddToken(TokenKind.Invalid, new TokenSpan(offset, 1, lexer._line, lexer._column));
            ptr++;
            lexer._column++;
        }
        return ptr;
    }
    
    private static unsafe byte* ParseDollar(Lexer lexer, byte* ptr, byte c)
    {
        // Probe for interpolated string
        var probePtr = ptr;
        while (c == (byte)'$')
        {
            probePtr++;
            c = *probePtr;
        }

        if (c == (byte)'"')
        {
            return ParseInterpolatedString(lexer, ptr, probePtr, c, (int)(probePtr - ptr));
        }

        var offset = (uint)(ptr - lexer._originalPtr);
        lexer.AddToken(TokenKind.Dollar, new TokenSpan(offset, 1, lexer._line, lexer._column));
        lexer._column++;
        ptr++;
        return ptr;
    }

    private static unsafe byte* ParseSymbol1Byte(Lexer lexer, byte* ptr, byte c)
    {
        var offset = (uint)(ptr - lexer._originalPtr);
        ptr++;
        var kind = Symbol1ByteToTokenKind[(byte)Utf8Helper.GetClassFromByte(c)];
        Debug.Assert(kind != TokenKind.Invalid);
        lexer.AddToken(kind, new TokenSpan(offset, 1, lexer._line, lexer._column));

        lexer._column++;
        return ptr;
    }

    private static unsafe byte* ParseSymbolMultiBytes(Lexer lexer, byte* ptr, byte c)
    {
        var offset = (uint)(ptr - lexer._originalPtr);

        // PercentSign,           // %
        // Ampersand,             // &
        // Asterisk,              // *
        // PlusSign,              // +
        // MinusSign,             // -
        // Period,                // .
        // Colon,                 // :
        // EqualSign,             // =
        // CircumflexAccent,      // ^
        // VerticalBar,           // |
        // TildeAccent,           // ~
        var nc = ptr[1];
        var kind = TokenKind.Invalid;
        var length = 1u;
        switch (c)
        {
            case (byte)'%':
                if (nc == (byte)'=')
                {
                    kind = TokenKind.PercentEqual;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.Percent;
                }
                break;
            case (byte)'&':
                if (nc == (byte)'=')
                {
                    kind = TokenKind.AmpersandEqual;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.Ampersand;
                }
                break;
            case (byte)'*':
                if (nc == (byte)'=')
                {
                    kind = TokenKind.StarEqual;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.Star;
                }
                break;
            case (byte)'+':
                if (nc == (byte)'=')
                {
                    kind = TokenKind.PlusEqual;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.Plus;
                }
                break;
            case (byte)'-':
                if (nc == (byte)'=')
                {
                    kind = TokenKind.MinusEqual;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.Minus;
                }
                break;
            case (byte)'.':
                if (nc == (byte)'.')
                {
                    if (ptr[2] == (byte)'<')
                    {
                        kind = TokenKind.DoubleDotLessThan;
                        length = 3;
                    }
                    else
                    {
                        kind = TokenKind.DoubleDot;
                        length = 2;
                    }
                }
                else
                {
                    kind = TokenKind.Dot;
                }
                break;
            case (byte)':':
                if (nc == (byte)':')
                {
                    kind = TokenKind.DoubleColon;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.Colon;
                }
                break;
            case (byte)'=':
                if (nc == (byte)'=')
                {
                    if (ptr[2] == (byte)'=')
                    {
                        kind = TokenKind.TripleEqual;
                        length = 3;
                    }
                    else
                    {
                        kind = TokenKind.DoubleEqual;
                        length = 2;
                    }
                }
                else if (nc == (byte)'>')
                {
                    kind = TokenKind.EqualGreaterThan;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.Equal;
                }
                break;
            case (byte)'^':
                if (nc == (byte)'=')
                {
                    kind = TokenKind.CircumflexEqual;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.Circumflex;
                }
                break;
            case (byte)'|':
                if (nc == (byte)'=')
                {
                    kind = TokenKind.VerticalBarEqual;
                    length = 2;
                }
                else if (nc == (byte)'>')
                {
                    kind = TokenKind.VerticalBarGreaterThan;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.VerticalBar;
                }
                break;
            case (byte)'~':
                if (nc == (byte)'=')
                {
                    kind = TokenKind.TildeEqual;
                    length = 2;
                }
                else
                {
                    kind = TokenKind.Tilde;
                }
                break;
            default:
                System.Diagnostics.Debug.Assert(false, "We should not be here");
                break;
        }

        lexer.AddToken(kind, new TokenSpan(offset, length, lexer._line, lexer._column));
        lexer._column += length;
        ptr += length;
        return ptr;
    }

    private int GetHashCode(ReadOnlySpan<byte> data)
    {
        _hasher = new HashCode();
        _hasher.AddBytes(data);
        return _hasher.ToHashCode();
    }

    private static readonly unsafe delegate*<Lexer, byte*, byte, byte*>[] ByteToParser;

    private static readonly unsafe delegate*<Lexer, byte*, byte, byte*>[] Utf8ClassToParser =
    {
        &ParseEof, // Eof, // End of file
        &ParseInvalid, // Sof, // Start of file
        &ParseNewLine, // CarriageReturn, // \r
        &ParseNewLine, // LineFeed, // \n
        &ParseWhitespace, // Space, // " "
        &ParseInvalidTab, // Tab,   // \t
        &ParseNumber, // Digit,     // Digit
        &ParseKeywordOrIdentifierOrUnderscore, // Letter,     // Letter
        &ParseSymbol1Byte, // ExclamationMark,       // !
        &ParseString, // DoubleQuote,           // "
        &ParseSymbol1Byte, // NumberSign,            // #
        &ParseDollar, // DollarSign,            // $
        &ParseSymbolMultiBytes, // PercentSign,           // %
        &ParseSymbolMultiBytes, // Ampersand,             // &
        &ParseNop, // SingleQuote,           // '
        &ParseSymbol1Byte, // LeftParenthesis,       // (
        &ParseSymbol1Byte, // RightParenthesis,      // )
        &ParseSymbolMultiBytes, // Asterisk,              // *
        &ParseSymbolMultiBytes, // PlusSign,              // +
        &ParseSymbol1Byte, // Comma,                 // ,
        &ParseSymbolMultiBytes, // MinusSign,             // -
        &ParseSymbolMultiBytes, // Period,                // .
        &ParseCommentOrSlash, // Slash,                 // /
        &ParseSymbolMultiBytes, // Colon,                 // :
        &ParseSymbol1Byte, // SemiColon,             // ;
        &ParseSymbol1Byte, // LessThanSign,          // <
        &ParseSymbolMultiBytes, // EqualSign,             // =
        &ParseSymbol1Byte, // GreaterThanSign,       // >
        &ParseSymbol1Byte, // QuestionMark,          // ?
        &ParseSymbol1Byte, // CommercialAtSign,      // @
        &ParseSymbol1Byte, // LeftSquareBracket,     // [
        &ParseSymbol1Byte, // Backslash,             // \
        &ParseSymbol1Byte, // RightSquareBracket,    // ]
        &ParseSymbolMultiBytes, // CircumflexAccent,      // ^
        &ParseKeywordOrIdentifierOrUnderscore, // Underscore,            // _
        &ParseSymbol1Byte, // GraveAccent,           // `
        &ParseSymbol1Byte, // LeftBrace,             // {
        &ParseSymbolMultiBytes, // VerticalBar,           // |
        &ParseSymbol1Byte, // RightBrace,            // }
        &ParseSymbolMultiBytes, // TildeAccent,           // ~
        &ParseInvalid, // Invalid,           // Special characters
        &ParseInvalid, // Utf8Value, // 80-BF
        &ParseUtf8, // Utf8Head2, // C0-DF
        &ParseUtf8, // Utf8Head3, // E0-EF
        &ParseUtf8, // Utf8Head4  // F0-F7
    };

    private static readonly unsafe TokenKind[] Symbol1ByteToTokenKind =
    {
        TokenKind.Invalid, // Eof, // End of file                                                Eof, // End of file   
        TokenKind.Invalid, // Sof, // Start of file                                              Sof, // Start of file
        TokenKind.Invalid, // CarriageReturn, // \r                                              CarriageReturn, // \r
        TokenKind.Invalid, // LineFeed, // \n                                                    LineFeed, // \n
        TokenKind.Invalid, // Space, // " "                                                      Space, // " "
        TokenKind.Invalid, // Tab,   // \t                                                       Tab,   // \t
        TokenKind.Invalid, // Digit,     // Digit                                                Digit,     // Digit
        TokenKind.Invalid, // Letter,     // Letter                                              Letter,     // Letter
        TokenKind.Exclamation, // ExclamationMark,       // !                          ExclamationMark,       // !
        TokenKind.Invalid, // DoubleQuote,           // "                                        DoubleQuote,           // "
        TokenKind.Number, // NumberSign,            // #                               NumberSign,            // #
        TokenKind.Invalid, // DollarSign,            // $                               DollarSign,            // $
        TokenKind.Invalid, // PercentSign,           // %                                        PercentSign,           // %
        TokenKind.Invalid, // Ampersand,             // &                                        Ampersand,             // &
        TokenKind.Invalid, // SingleQuote,           // '                                        SingleQuote,           // '
        TokenKind.LeftParent, // LeftParenthesis,       // (                          LeftParenthesis,       // (
        TokenKind.RightParent, // RightParenthesis,      // )                         RightParenthesis,      // )
        TokenKind.Invalid, // Asterisk,              // *                                        Asterisk,              // *
        TokenKind.Invalid, // PlusSign,              // +                                        PlusSign,              // +
        TokenKind.Comma, // Comma,                 // ,                                    Comma,                 // ,
        TokenKind.Invalid, // MinusSign,             // -                                        MinusSign,             // -
        TokenKind.Invalid, // Period,                // .                                        Period,                // .
        TokenKind.Invalid, // Slash,                 // /                                        Slash,                 // /
        TokenKind.Invalid, // Colon,                 // :                                        Colon,                 // :
        TokenKind.SemiColon, // SemiColon,             // ;                                SemiColon,             // ;
        TokenKind.LessThan, // LessThanSign,          // <                             LessThanSign,          // <
        TokenKind.Invalid, // EqualSign,             // =                                        EqualSign,             // =
        TokenKind.GreaterThan, // GreaterThanSign,       // >                          GreaterThanSign,       // >
        TokenKind.Question, // QuestionMark,          // ?                             QuestionMark,          // ?
        TokenKind.CommercialAt, // CommercialAtSign,      // @                         CommercialAtSign,      // @
        TokenKind.LeftBracket, // LeftSquareBracket,     // [                        LeftSquareBracket,     // [
        TokenKind.Backslash, // Backslash,             // \                                Backslash,             // \
        TokenKind.RightBracket, // RightSquareBracket,    // ]                       RightSquareBracket,    // ]
        TokenKind.Invalid, // CircumflexAccent,      // ^                                        CircumflexAccent,      // ^
        TokenKind.Invalid, // Underscore,            // _                                        Underscore,            // _
        TokenKind.Backtick, // GraveAccent,           // `                              GraveAccent,           // `
        TokenKind.LeftBrace, // LeftBrace,             // {                                LeftBrace,             // {
        TokenKind.Invalid, // VerticalBar,           // |                                        VerticalBar,           // |
        TokenKind.RightBrace, // RightBrace,            // }                               RightBrace,            // }
        TokenKind.Invalid, // TildeAccent,           // ~                                        TildeAccent,           // ~
        TokenKind.Invalid, // Invalid,           // Special characters                           Invalid,               // Special characters
        TokenKind.Invalid, // Utf8Value, // 80-BF                                                Utf8Value, // 80-BF
        TokenKind.Invalid, // Utf8Head2, // C0-DF                                                Utf8Head2, // C0-DF
        TokenKind.Invalid, // Utf8Head3, // E0-EF                                                Utf8Head3, // E0-EF
        TokenKind.Invalid, // Utf8Head4  // F0-F7                                                Utf8Head4  // F0-F7
    };                                                                                           

    static Lexer()
    {
        unsafe
        {
            Debug.Assert(Symbol1ByteToTokenKind.Length == (int)Utf8Class.Utf8Head4 + 1);
            // Initialize the ByteToParser table
            ByteToParser = new delegate*<Lexer, byte*, byte, byte*>[256];
            for (int i = 0; i < 256; i++)
            {
                ByteToParser[i] = Utf8ClassToParser[(byte)Utf8Helper.GetClassFromByte((byte)i)];
            }
        }
    }

    private record struct StringMultiLineState(uint Offset, uint LeadingSpaceCount, uint Line);
}