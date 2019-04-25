// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace StarkPlatform.Compiler.Stark
{
    internal sealed class NameofBinder : Binder
    {
        private readonly SyntaxNode _nameofArgument;

        public NameofBinder(SyntaxNode nameofArgument, Binder next) : base(next)
        {
            _nameofArgument = nameofArgument;
        }

        protected override SyntaxNode EnclosingNameofArgument => _nameofArgument;
    }
}
