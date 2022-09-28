// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Text;

namespace Stark.Compiler.Helpers;

public readonly ref struct Utf8String
{
    public Utf8String(IntPtr pointer, uint length)
    {
        Pointer = pointer;
        Length = length;
    }

    public IntPtr Pointer { get; }

    public uint Length { get; }

    public override bool Equals(object? obj)
    {
        return false;
    }

    public bool Equals(Utf8String str)
    {
        if (str.Length != this.Length) return false;
        if (str.Pointer == this.Pointer) return true;
        return AsSpan().SequenceEqual(str.AsSpan());
    }

    public override int GetHashCode()
    {
        if (Pointer == IntPtr.Zero) return 0;
        unsafe
        {
            var hashCode = new HashCode();
            var span = new Span<byte>((byte*)Pointer, (int)Length);
            hashCode.AddBytes(span);
            return hashCode.ToHashCode();
        }
    }

    public Span<byte> AsSpan()
    {
        unsafe
        {
            return new Span<byte>((byte*)Pointer, (int)Length);
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

    public static bool operator ==(Utf8String left, Utf8String right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Utf8String left, Utf8String right)
    {
        return !left.Equals(right);
    }
}

public record struct Utf8StringHandle(uint Offset, uint Length);
