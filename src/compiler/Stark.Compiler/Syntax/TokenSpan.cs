// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using Stark.Compiler.Helpers;

namespace Stark.Compiler.Syntax;

public record struct TokenSpan(uint Offset, uint Length, uint Line, uint Column)
{
    public uint Offset = Offset;

    public uint Length = Length;

    public uint Line = Line;

    public uint Column = Column;
}

public record struct TokenValue(ulong Data)
{
    public TokenValue(Utf8StringHandle utf8StringHandle) : this((ulong)utf8StringHandle.Offset << 32 | utf8StringHandle.Length)
    {
    }

    public TokenValue(int value) : this((ulong)(value))
    {
    }

    public TokenValue(double value) : this(Unsafe.As<double, ulong>(ref value))
    {
    }

    public Utf8StringHandle AsStringHandle() => new Utf8StringHandle((uint)(Data >> 32), (uint)Data);
}