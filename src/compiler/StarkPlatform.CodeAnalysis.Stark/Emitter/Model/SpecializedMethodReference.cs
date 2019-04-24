// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Emit
{
    /// <summary>
    /// Represents a method of a generic type instantiation.
    /// e.g. 
    /// A{int}.M()
    /// A.B{int}.C.M()
    /// </summary>
    internal class SpecializedMethodReference : MethodReference, Cci.ISpecializedMethodReference
    {
        public SpecializedMethodReference(MethodSymbol underlyingMethod)
            : base(underlyingMethod)
        {
        }

        public override void Dispatch(Cci.MetadataVisitor visitor)
        {
            visitor.Visit((Cci.ISpecializedMethodReference)this);
        }

        Cci.IMethodReference Cci.ISpecializedMethodReference.UnspecializedVersion
        {
            get
            {
                return UnderlyingMethod.OriginalDefinition;
            }
        }

        public override Cci.ISpecializedMethodReference AsSpecializedMethodReference
        {
            get
            {
                return this;
            }
        }
    }
}
