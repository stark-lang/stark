// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using StarkPlatform.Compiler.CodeGen;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.PooledObjects;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    /// <summary>
    /// The class that represents a translated iterator method.
    /// </summary>
    internal sealed class IteratorStateMachine : StateMachineTypeSymbol
    {
        private readonly MethodSymbol _constructor;
        private readonly ImmutableArray<NamedTypeSymbol> _interfaces;

        internal readonly TypeSymbol ElementType;

        public IteratorStateMachine(VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, MethodSymbol iteratorMethod, int iteratorMethodOrdinal, bool isEnumerable, TypeSymbol elementType)
            : base(slotAllocatorOpt, compilationState, iteratorMethod, iteratorMethodOrdinal)
        {
            this.ElementType = TypeMap.SubstituteType(elementType).TypeSymbol;

            throw new NotImplementedException("TODO");

            var interfaces = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            // TODO: Add proper arguments
            interfaces.Add(ContainingAssembly.GetSpecialType(SpecialType.core_Iterable_T_TIterator).Construct(ElementType));

            _interfaces = interfaces.ToImmutableAndFree();

            _constructor = new IteratorConstructor(this);
        }

        public override TypeKind TypeKind
        {
            get { return TypeKind.Class; }
        }

        internal override MethodSymbol Constructor
        {
            get { return _constructor; }
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved)
        {
            return _interfaces;
        }

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => ContainingAssembly.GetSpecialType(SpecialType.System_Object);
    }
}
