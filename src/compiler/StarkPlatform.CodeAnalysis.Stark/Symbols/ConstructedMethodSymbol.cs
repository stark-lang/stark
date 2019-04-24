// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal sealed class ConstructedMethodSymbol : SubstitutedMethodSymbol
    {
        private readonly ImmutableArray<TypeSymbolWithAnnotations> _typeArguments;

        internal ConstructedMethodSymbol(MethodSymbol constructedFrom, ImmutableArray<TypeSymbolWithAnnotations> typeArguments)
            : base(containingSymbol: constructedFrom.ContainingType,
                   map: new TypeMap(constructedFrom.ContainingType, ((MethodSymbol)constructedFrom.OriginalDefinition).TypeParameters, typeArguments),
                   originalDefinition: (MethodSymbol)constructedFrom.OriginalDefinition,
                   constructedFrom: constructedFrom)
        {
            _typeArguments = typeArguments;
        }

        public override ImmutableArray<TypeSymbolWithAnnotations> TypeArguments
        {
            get
            {
                return _typeArguments;
            }
        }

        public override bool IsTupleMethod
        {
            get
            {
                return ConstructedFrom.IsTupleMethod;
            }
        }

        public override MethodSymbol TupleUnderlyingMethod
        {
            get
            {
                return ConstructedFrom.TupleUnderlyingMethod?.Construct(_typeArguments);
            }
        }
    }
}
