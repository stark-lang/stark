// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions.ContextQuery
{
    internal static partial class SyntaxTokenExtensions
    {
        public static bool IsLastTokenOfNode<T>(this SyntaxToken token)
            where T : SyntaxNode
        {
            var node = token.GetAncestor<T>();
            return node != null && token == node.GetLastToken(includeZeroWidth: true);
        }
    }
}
