// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.EmbeddedLanguages.Common;

namespace StarkPlatform.Compiler.EmbeddedLanguages.RegularExpressions
{
    internal abstract class RegexNode : EmbeddedSyntaxNode<RegexKind, RegexNode>
    {
        protected RegexNode(RegexKind kind) : base(kind)
        {
        }

        public abstract void Accept(IRegexNodeVisitor visitor);
    }
}
