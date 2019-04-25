// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Text;
using System.Collections.Immutable;
using StarkPlatform.Compiler.Emit;
using System.Diagnostics;
using System.Linq;

namespace StarkPlatform.Compiler.Stark.Emit
{
    /// <summary>
    /// Represents a reference to a generic type instantiation.
    /// Subclasses represent nested and namespace types.
    /// </summary>
    internal abstract class GenericTypeInstanceReference : NamedTypeReference, Cci.IGenericTypeInstanceReference
    {
        public GenericTypeInstanceReference(NamedTypeSymbol underlyingNamedType)
            : base(underlyingNamedType)
        {
            Debug.Assert(underlyingNamedType.IsDefinition);
            // Definition doesn't have custom modifiers on type arguments
            Debug.Assert(!underlyingNamedType.TypeArgumentsNoUseSiteDiagnostics.Any(a => a.CustomModifiers.Any()));
        }

        public sealed override void Dispatch(Cci.MetadataVisitor visitor)
        {
            visitor.Visit((Cci.IGenericTypeInstanceReference)this);
        }

        ImmutableArray<Cci.ITypeReference> Cci.IGenericTypeInstanceReference.GetGenericArguments(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            var builder = ArrayBuilder<Cci.ITypeReference>.GetInstance();
            foreach (TypeSymbolWithAnnotations type in UnderlyingNamedType.TypeArgumentsNoUseSiteDiagnostics)
            {
                builder.Add(moduleBeingBuilt.Translate(type.TypeSymbol, syntaxNodeOpt: (CSharpSyntaxNode)context.SyntaxNodeOpt, diagnostics: context.Diagnostics));
            }

            return builder.ToImmutableAndFree();
        }

        Cci.INamedTypeReference Cci.IGenericTypeInstanceReference.GetGenericType(EmitContext context)
        {
            System.Diagnostics.Debug.Assert(UnderlyingNamedType.OriginalDefinition.IsDefinition);
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            return moduleBeingBuilt.Translate(UnderlyingNamedType.OriginalDefinition, syntaxNodeOpt: (CSharpSyntaxNode)context.SyntaxNodeOpt,
                                              diagnostics: context.Diagnostics, needDeclaration: true);
        }
    }
}
