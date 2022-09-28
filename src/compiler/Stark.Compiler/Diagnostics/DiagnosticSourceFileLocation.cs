// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Diagnostics;

/// <summary>
/// A location to a source file.
/// </summary>
public sealed class DiagnosticSourceFileLocation : DiagnosticLocation
{
    public DiagnosticSourceFileLocation(string filePath, TextSpan span)
    {
        FilePath = filePath;
        Span = span;
    }
    
    public string FilePath { get; }

    public TextSpan Span { get; }

    public override string ToText()
    {
        return $"{FilePath}{Span.ToText()}";
    }
}