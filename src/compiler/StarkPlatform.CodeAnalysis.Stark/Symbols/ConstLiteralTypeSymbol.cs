using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Roslyn.Utilities;
using StarkPlatform.CodeAnalysis.PooledObjects;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal sealed partial class ConstLiteralTypeSymbol : TypeSymbol
    {
        private readonly TypeSymbolWithAnnotations _elementType;
        private readonly object _value;

        public ConstLiteralTypeSymbol(TypeSymbolWithAnnotations elementType, object value)
        {
            _elementType = elementType;
            _value = value;
        }

        public override SymbolKind Kind => SymbolKind.ConstLiteralType;

        public TypeSymbolWithAnnotations UnderlyingType => _elementType;

        public object Value => _value;

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            if (t2 is ConstLiteralTypeSymbol t2Extended)
            {
                return _elementType.TypeSymbol.Equals(t2Extended.UnderlyingType.TypeSymbol, comparison) && Value.Equals(t2Extended.Value);
            }
            return _elementType.TypeSymbol.Equals(t2, comparison);
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        public override Symbol ContainingSymbol => null;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitConstLiteralType(this);
        }

        public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitConstLiteralType(this);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitConstLiteralType(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitConstLiteralType(this);
        }

        public override bool IsStatic => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool IsConstLiteral => true;

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument a)
        {
            return visitor.VisitConstLiteralType(this, a);
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        // const literal don't have base types
        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => null;

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved = null)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override bool IsReferenceType => _elementType.IsReferenceType;

        public override bool IsValueType => _elementType.IsValueType;

        public override TypeKind TypeKind => TypeKind.ConstLiteral;

        internal override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            return _elementType.TypeSymbol.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes);
        }

        internal override ManagedKind ManagedKind => ManagedKind.Unmanaged;

        internal override void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
            throw new NotSupportedException();
        }

        internal override bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeSymbol result)
        {
            throw new NotSupportedException();
        }

        internal override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeSymbolWithAnnotations, TypeSymbolWithAnnotations> transform)
        {
            throw new NotSupportedException();
        }

        internal override TypeSymbol MergeNullability(TypeSymbol other, VarianceKind variance, out bool hadNullabilityMismatch)
        {
            throw new NotSupportedException();
        }

        public override bool IsRefLikeType => false;
    }
}
