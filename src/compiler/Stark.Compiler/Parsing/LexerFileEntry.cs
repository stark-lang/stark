// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Parsing;

/// <summary>
/// A file entry used by the lexer.
/// </summary>
/// <param name="Path">The path to the file.</param>
/// <param name="Length">The length of the file.</param>
/// <param name="BeginTokenIndex">The first token index.</param>
/// <param name="EndTokenIndex">The end token index (inclusive).</param>
public record struct LexerFileEntry(string Path, uint Length, uint BeginTokenIndex, uint EndTokenIndex);