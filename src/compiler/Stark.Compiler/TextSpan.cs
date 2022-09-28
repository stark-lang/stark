// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Data.Common;

namespace Stark.Compiler;

public record struct TextSpan(TextLocation Start, TextLocation End)
{
    public TextSpan(TextLocation location) : this(location, location)
    {
    }

    public string ToText() => $"({Start.ToText()}, {End.ToText()})";
}

public record struct TextLocation(uint Offset, uint Line, uint Column)
{
    public string ToText() => $"{Line}, {Column}";
}
