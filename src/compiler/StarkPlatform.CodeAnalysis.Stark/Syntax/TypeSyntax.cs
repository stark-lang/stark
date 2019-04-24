// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace StarkPlatform.CodeAnalysis.Stark.Syntax
{
    public abstract partial class TypeSyntax
    {
        public bool IsUnmanaged => ((InternalSyntax.TypeSyntax)this.Green).IsUnmanaged;
    }

    public static class TypeSyntaxExtensions
    {
        public static bool IsNullWithNoType(this TypeSyntax type)
        {
            //return ((InternalSyntax.TypeSyntax)type.Green).IsVar;
            return type == null;
        }
    }
}
