﻿using System.Collections.Immutable;
using Roslyn.Utilities;
using StarkPlatform.Reflection.Metadata;

namespace StarkPlatform.Compiler.Stark.Symbols
{
    internal class ExtendedTypeParameterSymbol : WrappedTypeParameterSymbol, IExtendedTypeSymbol
    {
        public ExtendedTypeParameterSymbol(TypeParameterSymbol underlyingTypeParameter, TypeAccessModifiers modifiers) : base(underlyingTypeParameter)
        {
            AccessModifiers = modifiers;
        }

        public override TypeAccessModifiers AccessModifiers { get; }

        public override TypeParameterSymbol OriginalDefinition => this;

        public override Symbol ContainingSymbol => _underlyingTypeParameter.ContainingSymbol;

        internal override ImmutableArray<TypeSymbolWithAnnotations> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress, bool early)
        {
            return _underlyingTypeParameter.GetConstraintTypes(inProgress, early);
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress)
        {
            return _underlyingTypeParameter.GetInterfaces(inProgress);
        }

        internal override NamedTypeSymbol GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress)
        {
            return _underlyingTypeParameter.GetEffectiveBaseClass(inProgress);
        }

        internal override TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress)
        {
            return _underlyingTypeParameter.GetDeducedBaseType(inProgress);
        }

        public ITypeSymbol ElementType => _underlyingTypeParameter;


        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            if (t2 is ExtendedTypeParameterSymbol t2Extended && _underlyingTypeParameter.Equals(t2Extended._underlyingTypeParameter, comparison))
            {
                return AccessModifiers == t2Extended.AccessModifiers;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (base.GetHashCode() * 397) ^ (int)AccessModifiers;
        }
    }
}
