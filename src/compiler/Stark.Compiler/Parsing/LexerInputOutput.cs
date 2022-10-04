// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Text;
using Stark.Compiler.Diagnostics;
using Stark.Compiler.Helpers;
using Stark.Compiler.Syntax;
using Varena;

namespace Stark.Compiler.Parsing;

/// <summary>
/// The Lexer Input/Output. Contains all generates tokens, spans and values as well as temporary buffer used by the lexer.
/// </summary>
public class LexerInputOutput
{
    private readonly Dictionary<Utf8InternalString, Utf8StringHandle> _stringHandles;

    public LexerInputOutput(VirtualArenaManager manager)
    {
        Tokens = manager.CreateArray<TokenKind>("Tokens", 1 << 30);
        TokenSpans = manager.CreateArray<TokenSpan>("TokenSpans", 1 << 30);
        TokenValues = manager.CreateArray<TokenValue>("TokenValues", 1 << 30);
        InputBuffer = manager.CreateBuffer("InputBuffer", 1 << 30);
        TempBuffer = manager.CreateBuffer("LexerTempBuffer", 1 << 30);
        StringBuffer = manager.CreateBuffer("StringBuffer", 1 << 30);
        _stringHandles = new Dictionary<Utf8InternalString, Utf8StringHandle>(4096);
        Diagnostics = new DiagnosticBag();
    }

    public DiagnosticBag Diagnostics { get; }

    public VirtualArray<TokenKind> Tokens { get; }

    public VirtualArray<TokenSpan> TokenSpans { get; }

    public VirtualArray<TokenValue> TokenValues { get; }

    public VirtualBuffer InputBuffer { get; }

    public VirtualBuffer TempBuffer { get; }

    public VirtualBuffer StringBuffer { get; }

    public Utf8StringHandle GetStringHandle(ReadOnlySpan<byte> data, int hash)
    {
        unsafe
        {
            fixed (byte* ptr = data)
            {
                // Use the input temporarily
                var key = new Utf8InternalString((IntPtr)ptr, hash, (uint)data.Length);
                if (_stringHandles.TryGetValue(key, out var handle))
                {
                    return handle;
                }

                var offset = (uint)StringBuffer.AllocatedBytes;
                var pointer = (byte*)StringBuffer.BaseAddress + offset;
                data.CopyTo(StringBuffer.AllocateRange(data.Length));
                key = new Utf8InternalString((IntPtr)pointer, hash, (uint)data.Length);
                handle = new Utf8StringHandle(offset, (uint)data.Length);
                _stringHandles.Add(key, handle);
                return handle;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8String GetString(Utf8StringHandle stringHandle)
    {
        return stringHandle.Length == 0 ? default : new Utf8String(IntPtr.Add(StringBuffer.BaseAddress, (int)stringHandle.Offset), stringHandle.Length);
    }

    public void Reset()
    {
        Reset(VirtualArenaResetKind.KeepMinimalCommitted);
    }

    public void Reset(VirtualArenaResetKind kind)
    {
        Diagnostics.Clear();
        Tokens.Reset(kind);
        TokenSpans.Reset(kind);
        TokenValues.Reset(kind);
        InputBuffer.Reset(kind);
        TempBuffer.Reset(kind);
    }

    public void AddToken(TokenKind kind, TokenSpan span)
    {
        Tokens.Allocate() = kind;
        TokenSpans.Allocate() = span;
        TokenValues.Allocate() = default;
    }

    public void AddToken(TokenKind kind, TokenSpan span, TokenValue value)
    {
        Tokens.Allocate() = kind;
        TokenSpans.Allocate() = span;
        TokenValues.Allocate() = value;
    }

    public void AddToken(TokenKind kind, TokenSpan span, ReadOnlySpan<byte> value)
    {
        AddToken(kind, span, value, HashHelper.Hash(value));
    }

    public void AddToken(TokenKind kind, TokenSpan span, ReadOnlySpan<byte> value, int hash)
    {
        Tokens.Allocate() = kind;
        TokenSpans.Allocate() = span;
        TokenValues.Allocate() = new TokenValue(GetStringHandle(value, hash));
    }

    public void ResetInputBuffer()
    {
        InputBuffer.Reset(VirtualArenaResetKind.KeepMinimalCommitted);
    }

    public void ResetTempBuffer()
    {
        TempBuffer.Reset(VirtualArenaResetKind.KeepMinimalCommitted);
    }

    readonly struct Utf8InternalString : IEquatable<Utf8InternalString>
    {
        public Utf8InternalString(IntPtr pointer, int hashCode, uint length)
        {
            Pointer = pointer;
            HashCode = hashCode;
            Length = length;
        }

        public IntPtr Pointer { get; }

        public int HashCode { get; }

        public uint Length { get; }

        public override bool Equals(object? obj) => obj is Utf8InternalString str && Equals(str);

        public bool Equals(Utf8InternalString str)
        {
            if (str.Length != this.Length) return false;
            if (str.Pointer == this.Pointer) return true;
            return AsSpan().SequenceEqual(str.AsSpan());
        }

        public override int GetHashCode() => HashCode;

        public ReadOnlySpan<byte> AsSpan()
        {
            unsafe
            {
                return new ReadOnlySpan<byte>((byte*)Pointer, (int)Length);
            }
        }

        public override string? ToString()
        {
            unsafe
            {
                if (Pointer == IntPtr.Zero) return null;
                return Encoding.UTF8.GetString((byte*)Pointer, (int)Length);
            }
        }
    }
}