// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StarkPlatform.CodeAnalysis.Stark;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class SeparatedSyntaxListExtensions
    {
        private static Tuple<List<T>, List<SyntaxToken>> GetNodesAndSeparators<T>(this SeparatedSyntaxList<T> separatedList) where T : SyntaxNode
        {
            Debug.Assert(separatedList.Count == separatedList.SeparatorCount ||
                              separatedList.Count == separatedList.SeparatorCount + 1);

            var nodes = new List<T>(separatedList.Count);
            var separators = new List<SyntaxToken>(separatedList.SeparatorCount);

            for (int i = 0; i < separatedList.Count; i++)
            {
                nodes.Add(separatedList[i]);

                if (i < separatedList.SeparatorCount)
                {
                    separators.Add(separatedList.GetSeparator(i));
                }
            }

            return Tuple.Create(nodes, separators);
        }
    }
}
