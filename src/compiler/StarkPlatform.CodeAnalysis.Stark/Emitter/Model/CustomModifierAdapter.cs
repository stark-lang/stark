// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Emit;
using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Emit;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal partial class CSharpCustomModifier : Cci.ICustomModifier
    {
        bool Cci.ICustomModifier.IsOptional
        {
            get { return this.IsOptional; }
        }

        Cci.ITypeReference Cci.ICustomModifier.GetModifier(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(this.Modifier, (CSharpSyntaxNode)context.SyntaxNodeOpt, context.Diagnostics);
        }
    }
}
