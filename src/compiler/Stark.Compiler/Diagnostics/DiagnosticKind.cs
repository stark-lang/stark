// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Diagnostics;

/// <summary>
/// Describes the kind of a diagnostic.
/// </summary>
public enum DiagnosticKind
{
    /// <summary>
    /// Used internally.
    /// </summary>
    Internal = 0,

    /// <summary>
    /// This is a diagnostic that provides an information.
    /// </summary>
    Info = 1,

    /// <summary>
    /// This is a warning about something suspicious but allowed.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Something is not allowed by the rules of the language or other authority.
    /// </summary>
    Error = 3,
}