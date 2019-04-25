// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using StarkPlatform.Compiler.Stark;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Extensions
{
    internal static class DocumentationCommentExtensions
    {
        public static bool IsMultilineDocComment(this DocumentationCommentTriviaSyntax documentationComment)
        {
            if (documentationComment == null)
            {
                return false;
            }
            return documentationComment.ToFullString().StartsWith("/**", StringComparison.Ordinal);
        }
    }
}
