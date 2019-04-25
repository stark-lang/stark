// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Syntax.InternalSyntax
{
    internal partial class IdentifierNameSyntax
    {
        public override string ToString()
        {
            return this.Identifier.Text;
        }
    }
}
