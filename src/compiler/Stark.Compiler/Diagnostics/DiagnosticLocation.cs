// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Diagnostics;

/// <summary>
/// A diagnostic location. This class is abstract because a location could
/// be a specific line in a source file or a type/method from a precompiled library.
/// </summary>
public abstract class DiagnosticLocation
{
    public sealed override string ToString() => ToText();

    public abstract string ToText();
}