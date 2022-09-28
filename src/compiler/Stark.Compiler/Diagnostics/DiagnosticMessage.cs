// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace Stark.Compiler.Diagnostics;

public record DiagnosticMessage(DiagnosticId Id, string Text);