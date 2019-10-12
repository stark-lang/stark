// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using StarkPlatform.Compiler.PooledObjects;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark.Symbols
{
    /// <summary>
    /// An ArrayTypeSymbol represents an array type, such as int[] or object[,].
    /// </summary>
    internal abstract partial class ArrayTypeSymbol : TypeSymbol, IArrayTypeSymbol
    {
        private readonly TypeSymbolWithAnnotations _elementType;
        private readonly NamedTypeSymbol _baseType;

        protected ArrayTypeSymbol(
            TypeSymbolWithAnnotations elementType,
            NamedTypeSymbol array)
        {
            Debug.Assert(!elementType.IsNull);
            Debug.Assert((object)array != null);

            _elementType = elementType;
            _baseType = array;
        }

        internal static ArrayTypeSymbol CreateArray(
            TypeSymbolWithAnnotations elementType,
            NamedTypeSymbol array,
            ImmutableArray<NamedTypeSymbol> constructedInterfaces)
        {
            return new DefaultArray(elementType, array, constructedInterfaces);
        }

        internal static ArrayTypeSymbol CreateArray(
            AssemblySymbol declaringAssembly,
            TypeSymbolWithAnnotations elementType)
        {
            var arrayType = declaringAssembly.GetSpecialType(SpecialType.core_Array_T).Construct(elementType.TypeSymbol);
            return CreateArray(elementType, arrayType, GetSZArrayInterfaces(elementType, arrayType, declaringAssembly));
        }

        internal ArrayTypeSymbol WithElementType(TypeSymbolWithAnnotations elementType)
        {
            return ElementType.IsSameAs(elementType) ? this : WithElementTypeCore(elementType);
        }

        protected internal abstract ArrayTypeSymbol WithElementTypeCore(TypeSymbolWithAnnotations elementType);

        private static ImmutableArray<NamedTypeSymbol> GetSZArrayInterfaces(
            TypeSymbolWithAnnotations elementType,
            NamedTypeSymbol arrayType,
            AssemblySymbol declaringAssembly)
        {
            var constructedInterfaces = ArrayBuilder<NamedTypeSymbol>.GetInstance();

            //There are cases where the platform does contain the interfaces.
            //So it is fine not to have them listed under the type
            var mutableIterableTItem_TState_TIterator = declaringAssembly.GetSpecialType(SpecialType.core_MutableIterable_T_TIterator);
            if (!mutableIterableTItem_TState_TIterator.IsErrorType())
            {
                var iteratorType = TypeSymbolWithAnnotations.Create(declaringAssembly.GetSpecialType(Compiler.SpecialType.System_Int));

                constructedInterfaces.Add(new ConstructedNamedTypeSymbol(mutableIterableTItem_TState_TIterator, ImmutableArray.Create(elementType, iteratorType)));
            }

            return constructedInterfaces.ToImmutableAndFree();
        }

        /// <summary>
        /// Gets the type of the elements stored in the array.
        /// </summary>
        public TypeSymbolWithAnnotations ElementType
        {
            get
            {
                return _elementType;
            }
        }

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => _baseType;

        public override bool IsReferenceType
        {
            get
            {
                return true;
            }
        }

        public override bool IsValueType
        {
            get
            {
                return false;
            }
        }

        internal sealed override ManagedKind ManagedKind => ManagedKind.Managed;

        public override bool IsRefLikeType
        {
            get
            {
                return false;
            }
        }

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData
        {
            get { return null; }
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

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override SymbolKind Kind
        {
            get
            {
                return SymbolKind.ArrayType;
            }
        }

        public override TypeKind TypeKind
        {
            get
            {
                return TypeKind.Array;
            }
        }

        public override Symbol ContainingSymbol
        {
            get
            {
                return null;
            }
        }

        public override ImmutableArray<Location> Locations
        {
            get
            {
                return ImmutableArray<Location>.Empty;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitArrayType(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitArrayType(this);
        }

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            return this.Equals(t2 as ArrayTypeSymbol, comparison);
        }

        internal bool Equals(ArrayTypeSymbol other)
        {
            return Equals(other, TypeCompareKind.ConsiderEverything);
        }

        private bool Equals(ArrayTypeSymbol other, TypeCompareKind comparison)
        {
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            TypeSymbol current = this;
            while (current.TypeKind == TypeKind.Array)
            {
                var cur = (ArrayTypeSymbol)current;
                current = cur.ElementType.TypeSymbol;
            }

            return Hash.Combine(current, hash);
        }

        internal override void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
            ElementType.AddNullableTransforms(transforms);
        }

        internal override bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeSymbol result)
        {
            TypeSymbolWithAnnotations oldElementType = ElementType;
            TypeSymbolWithAnnotations newElementType;

            if (!oldElementType.ApplyNullableTransforms(defaultTransformFlag, transforms, ref position, out newElementType))
            {
                result = this;
                return false;
            }

            result = WithElementType(newElementType);
            return true;
        }

        internal override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeSymbolWithAnnotations, TypeSymbolWithAnnotations> transform)
        {
            return WithElementType(transform(ElementType));
        }

        internal override TypeSymbol MergeNullability(TypeSymbol other, VarianceKind variance, out bool hadNullabilityMismatch)
        {
            Debug.Assert(this.Equals(other, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes));
            TypeSymbolWithAnnotations elementType = ElementType.MergeNullability(((ArrayTypeSymbol)other).ElementType, VarianceKind.None, out hadNullabilityMismatch);
            return WithElementType(elementType);
        }

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                return Accessibility.NotApplicable;
            }
        }

        public override bool IsStatic
        {
            get
            {
                return false;
            }
        }

        public override bool IsAbstract
        {
            get
            {
                return false;
            }
        }

        public override bool IsSealed
        {
            get
            {
                return false;
            }
        }

        #region Use-Site Diagnostics

        internal override DiagnosticInfo GetUseSiteDiagnostic()
        {
            DiagnosticInfo result = null;

            // check element type
            // check custom modifiers
            if (DeriveUseSiteDiagnosticFromType(ref result, this.ElementType))
            {
                return result;
            }

            return result;
        }

        internal override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            return _elementType.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes) ||
                   ((object)_baseType != null && _baseType.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes)) ||
                   GetUnificationUseSiteDiagnosticRecursive(ref result, this.InterfacesNoUseSiteDiagnostics(), owner, ref checkedTypes);
        }

        #endregion

        #region IArrayTypeSymbol Members

        ITypeSymbol ITypeWithElementTypeSymbol.ElementType
        {
            get { return this.ElementType.TypeSymbol; }
        }

        ImmutableArray<CustomModifier> IArrayTypeSymbol.CustomModifiers
        {
            get { return this.ElementType.CustomModifiers; }
        }

        bool IArrayTypeSymbol.Equals(IArrayTypeSymbol symbol)
        {
            return this.Equals(symbol as ArrayTypeSymbol);
        }

        #endregion

        #region ISymbol Members

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitArrayType(this);
        }

        #endregion

        /// <summary>
        /// Represents SZARRAY - zero-based one-dimensional array 
        /// </summary>
        private sealed class DefaultArray : ArrayTypeSymbol
        {
            private readonly ImmutableArray<NamedTypeSymbol> _interfaces;

            internal DefaultArray(
                TypeSymbolWithAnnotations elementType,
                NamedTypeSymbol array,
                ImmutableArray<NamedTypeSymbol> constructedInterfaces)
                : base(elementType, array)
            {
                Debug.Assert(constructedInterfaces.Length <= 2);
                _interfaces = constructedInterfaces;
            }

            protected internal override ArrayTypeSymbol WithElementTypeCore(TypeSymbolWithAnnotations newElementType)
            {
                var newInterfaces = _interfaces.SelectAsArray((i, t) => i.OriginalDefinition.Construct(t), newElementType.TypeSymbol);
                return new DefaultArray(newElementType, BaseTypeNoUseSiteDiagnostics, newInterfaces);
            }

            internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved = null)
            {
                return _interfaces;
            }
        }
    }
}
