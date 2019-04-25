// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Syntax
{
    public enum XmlNameAttributeElementKind : byte
    {
        Parameter = 0,
        ParameterReference = 1,
        TypeParameter = 2,
        TypeParameterReference = 3,
    }
}
