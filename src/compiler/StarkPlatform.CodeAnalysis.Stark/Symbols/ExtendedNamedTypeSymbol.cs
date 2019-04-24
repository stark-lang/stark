using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.Cci;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal sealed partial class ExtendedNamedTypeSymbol : WrappedNamedTypeSymbol
    {
        private readonly TypeSymbolWithAnnotations _elementType;
        private readonly TypeAccessModifiers _accessModifiers;

        public ExtendedNamedTypeSymbol(TypeSymbolWithAnnotations elementType, TypeAccessModifiers accessModifiers) : base((NamedTypeSymbol)elementType.TypeSymbol)
        {
            _elementType = elementType;
            _accessModifiers = accessModifiers;
        }

        public override NamedTypeSymbol GetWithoutModifiers()
        {
            return UnderlyingNamedType;
        }

        public override TypeAccessModifiers AccessModifiers => _accessModifiers;

        public override Symbol ContainingSymbol => _underlyingType.ContainingSymbol;

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => _underlyingType.BaseTypeNoUseSiteDiagnostics;

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved = null)
        {
            return _underlyingType.InterfacesNoUseSiteDiagnostics(basesBeingResolved);
        }

        public override NamedTypeSymbol OriginalDefinition => this;

        internal override bool IsComImport => _underlyingType.IsComImport;

        public override bool IsRefLikeType => IsTransient;

        public override bool IsReadOnly => (_accessModifiers & TypeAccessModifiers.ReadOnly) != 0 || base.IsReadOnly;

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            if (base.Equals(t2, comparison) && t2 is ExtendedNamedTypeSymbol t2Extended)
            {
                return _accessModifiers == t2Extended._accessModifiers;
            }

            return false;
        }

        internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
        {
            return _underlyingType.GetFieldsToEmit();
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            return _underlyingType.GetInterfacesToEmit();
        }

        public override IEnumerable<string> MemberNames => _underlyingType.MemberNames;

        public override ImmutableArray<Symbol> GetMembers()
        {
            return _underlyingType.GetMembers();
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return _underlyingType.GetMembers(name);
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return _underlyingType.GetTypeMembers();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return _underlyingType.GetTypeMembers(name);
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return _underlyingType.GetTypeMembers(name, arity);
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers()
        {
            return _underlyingType.GetEarlyAttributeDecodingMembers();
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name)
        {
            return _underlyingType.GetEarlyAttributeDecodingMembers(name);
        }

        internal override NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved)
        {
            return _underlyingType.GetDeclaredBaseType(basesBeingResolved);
        }

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved)
        {
            return _underlyingType.GetDeclaredInterfaces(basesBeingResolved);
        }

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => _underlyingType.TypeParameters;

        internal override ImmutableArray<TypeSymbolWithAnnotations> TypeArgumentsNoUseSiteDiagnostics => _underlyingType.TypeArgumentsNoUseSiteDiagnostics;

        public override NamedTypeSymbol ConstructedFrom => _underlyingType.ConstructedFrom;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitExtendedType(this);
        }

        internal override string GetDebuggerDisplay()
        {
            return "Extended" + base.GetDebuggerDisplay();
        }      
    }


    internal static class ExtendedTypeSymbol
    {
        public static TypeSymbolWithAnnotations CreateExtendedTypeSymbol(CSharpSyntaxNode syntax, TypeSymbolWithAnnotations baseSymbol, TypeAccessModifiers accessModifiers, DiagnosticBag diagnostics)
        {
            if (baseSymbol.Kind == SymbolKind.NamedType)
            {
                return TypeSymbolWithAnnotations.Create(new ExtendedNamedTypeSymbol(baseSymbol, accessModifiers));
            }

            if (baseSymbol.Kind == SymbolKind.ArrayType)
            {
                return TypeSymbolWithAnnotations.Create(new ExtendedArrayTypeSymbol(baseSymbol, (ArrayTypeSymbol)baseSymbol.TypeSymbol, accessModifiers));
            }

            if (baseSymbol.Kind == SymbolKind.TypeParameter)
            {
                return TypeSymbolWithAnnotations.Create(new ExtendedTypeParameterSymbol((TypeParameterSymbol)baseSymbol.TypeSymbol, accessModifiers));
            }
            throw new NotSupportedException($"The syntax `{syntax}` is not supported");
        }
    }
}
