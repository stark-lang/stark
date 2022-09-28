// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Diagnostics;

public sealed class Diagnostic
{
    public Diagnostic(DiagnosticKind kind, DiagnosticLocation location, DiagnosticMessage message)
    {
        Kind = kind;
        Location = location;
        Message = message;
    }

    public DiagnosticLocation Location { get; }

    public DiagnosticKind Kind { get; }
    
    public DiagnosticMessage Message { get; }

    public override string ToString()
    {
        // test
        // C:\code\stark\stark\src\Stark.Compiler\Stark.Compiler\Diagnostics\DiagnosticBag.cs(44,22,44,22): error CS1002: ; expected
        return $"{Location}: {SeverityToString(Kind)} SK{(int)Message.Id:0000}: {Message.Text}";
    }

    private static string SeverityToString(DiagnosticKind kind) => kind switch
    {
        DiagnosticKind.Internal => "internal",
        DiagnosticKind.Info => "info",
        DiagnosticKind.Warning => "warning",
        DiagnosticKind.Error => "error",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };
}