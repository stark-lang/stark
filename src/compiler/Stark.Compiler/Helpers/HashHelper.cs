// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace Stark.Compiler.Helpers;

/// <summary>
/// FNV-1a hash for identifiers/strings. Might be not fast enough for large strings.
/// </summary>
internal static class HashHelper
{
    public static int Init() => unchecked((int)2166136261);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Hash(byte b, ref int hash)
    {
        hash ^= b;
        hash *= 16777619;
    }

    public static int Hash(ReadOnlySpan<byte> bytes)
    {
        var hash = Init();
        foreach (var b in bytes)
        {
            Hash(b, ref hash);
        }
        return hash;
    }
}