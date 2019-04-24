// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using StarkPlatform.CodeAnalysis.Stark;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
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
