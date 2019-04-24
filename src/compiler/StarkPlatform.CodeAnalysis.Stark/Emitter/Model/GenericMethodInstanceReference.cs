// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Emit;

namespace StarkPlatform.CodeAnalysis.Stark.Emit
{
    /// <summary>
    /// Represents a reference to a generic method instantiation, closed over type parameters,
    /// e.g. MyNamespace.Class.Method{T}()
    /// </summary>
    internal sealed class GenericMethodInstanceReference : MethodReference, Cci.IGenericMethodInstanceReference
    {
        public GenericMethodInstanceReference(MethodSymbol underlyingMethod)
            : base(underlyingMethod)
        {
        }

        public override void Dispatch(Cci.MetadataVisitor visitor)
        {
            visitor.Visit((Cci.IGenericMethodInstanceReference)this);
        }

        IEnumerable<Cci.ITypeReference> Cci.IGenericMethodInstanceReference.GetGenericArguments(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;

            foreach (var arg in UnderlyingMethod.TypeArguments)
            {
                Debug.Assert(arg.CustomModifiers.IsEmpty);
                yield return moduleBeingBuilt.Translate(arg.TypeSymbol, syntaxNodeOpt: (CSharpSyntaxNode)context.SyntaxNodeOpt, diagnostics: context.Diagnostics);
            }
        }

        Cci.IMethodReference Cci.IGenericMethodInstanceReference.GetGenericMethod(EmitContext context)
        {
            // NoPia method might come through here.
            return ((PEModuleBuilder)context.Module).Translate(
                UnderlyingMethod.OriginalDefinition,
                syntaxNodeOpt: (CSharpSyntaxNode)context.SyntaxNodeOpt,
                diagnostics: context.Diagnostics,
                needDeclaration: true);
        }

        public override Cci.IGenericMethodInstanceReference AsGenericMethodInstanceReference
        {
            get
            {
                return this;
            }
        }
    }
}
