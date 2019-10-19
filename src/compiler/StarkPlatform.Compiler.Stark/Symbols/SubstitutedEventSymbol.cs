// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;

namespace StarkPlatform.Compiler.Stark.Symbols
{
    internal sealed class SubstitutedEventSymbol : WrappedEventSymbol
    {
        private readonly SubstitutedNamedTypeSymbol _containingType;

        private TypeSymbolWithAnnotations.Builder _lazyType;

        internal SubstitutedEventSymbol(SubstitutedNamedTypeSymbol containingType, EventSymbol originalDefinition)
            : base(originalDefinition)
        {
            Debug.Assert(originalDefinition.IsDefinition);
            _containingType = containingType;
        }

        public override TypeSymbolWithAnnotations Type
        {
            get
            {
                if (_lazyType.IsNull)
                {
                    _lazyType.InterlockedInitialize(_containingType.TypeSubstitution.SubstituteTypeWithTupleUnification(OriginalDefinition.Type));
                }

                return _lazyType.ToType();
            }
        }

        public override Symbol ContainingSymbol
        {
            get
            {
                return _containingType;
            }
        }

        public override EventSymbol OriginalDefinition
        {
            get
            {
                return _underlyingEvent;
            }
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return OriginalDefinition.GetAttributes();
        }

        public override MethodSymbol AddMethod
        {
            get
            {
                MethodSymbol originalAddMethod = OriginalDefinition.AddMethod;
                return (object)originalAddMethod == null ? null : originalAddMethod.AsMember(_containingType);
            }
        }

        public override MethodSymbol RemoveMethod
        {
            get
            {
                MethodSymbol originalRemoveMethod = OriginalDefinition.RemoveMethod;
                return (object)originalRemoveMethod == null ? null : originalRemoveMethod.AsMember(_containingType);
            }
        }

        internal override FieldSymbol AssociatedField
        {
            get
            {
                FieldSymbol originalAssociatedField = OriginalDefinition.AssociatedField;
                return (object)originalAssociatedField == null ? null : originalAssociatedField.AsMember(_containingType);
            }
        }

        internal override bool IsExplicitInterfaceImplementation
        {
            get { return OriginalDefinition.IsExplicitInterfaceImplementation; }
        }

        //we want to compute this lazily since it may be expensive for the underlying symbol
        private ImmutableArray<EventSymbol> _lazyExplicitInterfaceImplementations;

        private OverriddenOrHiddenMembersResult _lazyOverriddenOrHiddenMembers;

        public override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations
        {
            get
            {
                if (_lazyExplicitInterfaceImplementations.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(
                        ref _lazyExplicitInterfaceImplementations,
                        ExplicitInterfaceHelpers.SubstituteExplicitInterfaceImplementations(OriginalDefinition.ExplicitInterfaceImplementations, _containingType.TypeSubstitution),
                        default(ImmutableArray<EventSymbol>));
                }
                return _lazyExplicitInterfaceImplementations;
            }
        }

        internal override bool MustCallMethodsDirectly
        {
            get { return OriginalDefinition.MustCallMethodsDirectly; }
        }

        internal override OverriddenOrHiddenMembersResult OverriddenOrHiddenMembers
        {
            get
            {
                if (_lazyOverriddenOrHiddenMembers == null)
                {
                    Interlocked.CompareExchange(ref _lazyOverriddenOrHiddenMembers, this.MakeOverriddenOrHiddenMembers(), null);
                }
                return _lazyOverriddenOrHiddenMembers;
            }
        }
    }
}
