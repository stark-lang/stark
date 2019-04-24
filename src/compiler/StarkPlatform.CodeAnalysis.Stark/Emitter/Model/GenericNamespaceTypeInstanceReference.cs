// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Emit
{
    /// <summary>
    /// Represents a reference to a generic type instantiation that is not nested.
    /// e.g. MyNamespace.A{int}
    /// </summary>
    internal sealed class GenericNamespaceTypeInstanceReference : GenericTypeInstanceReference
    {
        public GenericNamespaceTypeInstanceReference(NamedTypeSymbol underlyingNamedType)
            : base(underlyingNamedType)
        {
        }

        public override StarkPlatform.Cci.IGenericTypeInstanceReference AsGenericTypeInstanceReference
        {
            get { return this; }
        }

        public override StarkPlatform.Cci.INamespaceTypeReference AsNamespaceTypeReference
        {
            get { return null; }
        }

        public override StarkPlatform.Cci.INestedTypeReference AsNestedTypeReference
        {
            get { return null; }
        }

        public override StarkPlatform.Cci.ISpecializedNestedTypeReference AsSpecializedNestedTypeReference
        {
            get { return null; }
        }
    }
}
