using System.Collections.Immutable;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal sealed class ExtendedArrayTypeSymbol : ArrayTypeSymbol, IExtendedTypeSymbol
    {
        private readonly TypeAccessModifiers _accessModifiers;
        private readonly ArrayTypeSymbol _underlyingArray;


        public ExtendedArrayTypeSymbol(TypeSymbolWithAnnotations arrayWithAnnotations, ArrayTypeSymbol underlyingArray, TypeAccessModifiers accessModifiers) : base(underlyingArray.ElementType, underlyingArray.BaseTypeNoUseSiteDiagnostics)
        {
            _underlyingArray = underlyingArray;
            _accessModifiers = accessModifiers;
        }


        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved = null)
        {
            return _underlyingArray.InterfacesNoUseSiteDiagnostics(basesBeingResolved);
        }

        protected internal override ArrayTypeSymbol WithElementTypeCore(TypeSymbolWithAnnotations elementType)
        {
            return _underlyingArray.WithElementTypeCore(elementType);
        }

        public override int Rank => _underlyingArray.Rank;

        public override bool IsSZArray => _underlyingArray.IsSZArray;

        public override bool IsRefLikeType => (_accessModifiers & TypeAccessModifiers.Transient) != 0;

        internal override bool HasDefaultSizesAndLowerBounds => _underlyingArray.HasDefaultSizesAndLowerBounds;

        public TypeAccessModifiers AccessModifiers => _accessModifiers;
    }
}
