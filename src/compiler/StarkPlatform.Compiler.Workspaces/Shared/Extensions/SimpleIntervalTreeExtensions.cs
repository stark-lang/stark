// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Shared.Collections;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Shared.Extensions
{
    internal static class SimpleIntervalTreeExtensions
    {
        /// <summary>
        /// check whether the given span is intersects with the tree
        /// </summary>
        public static bool HasIntervalThatIntersectsWith(this SimpleIntervalTree<TextSpan> tree, TextSpan span)
            => tree.HasIntervalThatIntersectsWith(span.Start, span.Length);
    }
}
