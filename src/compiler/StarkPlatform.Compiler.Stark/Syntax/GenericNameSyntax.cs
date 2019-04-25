// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Collections;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Syntax
{
    public partial class GenericNameSyntax
    {
        public bool IsUnboundGenericName
        {
            get
            {
                return this.TypeArgumentList.Arguments.Any(SyntaxKind.OmittedTypeArgument);
            }
        }

        internal override string ErrorDisplayName()
        {
            var pb = PooledStringBuilder.GetInstance();
            pb.Builder.Append(Identifier.ValueText).Append("<").Append(',', Arity - 1).Append(">");
            return pb.ToStringAndFree();
        }
    }
}
