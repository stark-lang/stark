// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Syntax.InternalSyntax
{
    internal readonly struct BlendedNode
    {
        internal readonly Stark.CSharpSyntaxNode Node;
        internal readonly SyntaxToken Token;
        internal readonly Blender Blender;

        internal BlendedNode(Stark.CSharpSyntaxNode node, SyntaxToken token, Blender blender)
        {
            this.Node = node;
            this.Token = token;
            this.Blender = blender;
        }
    }
}
