﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler;
using StarkPlatform.Compiler.Stark;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Extensions
{
    internal static class SymbolDisplayPartExtensions
    {
        public static SymbolDisplayPart MassageErrorTypeNames(this SymbolDisplayPart part, string replacement = null)
        {
            if (part.Kind == SymbolDisplayPartKind.ErrorTypeName)
            {
                var text = part.ToString();
                if (text == string.Empty)
                {
                    return replacement == null
                        ? new SymbolDisplayPart(SymbolDisplayPartKind.Keyword, null, "object")
                        : new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, replacement);
                }

                if (SyntaxFacts.GetKeywordKind(text) != SyntaxKind.None)
                {
                    return new SymbolDisplayPart(SymbolDisplayPartKind.ErrorTypeName, null, string.Format("@{0}", text));
                }
            }

            return part;
        }
    }
}
