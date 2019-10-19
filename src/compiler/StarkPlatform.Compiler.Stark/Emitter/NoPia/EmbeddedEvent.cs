// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Stark.Symbols;
using System.Collections.Generic;

namespace StarkPlatform.Compiler.Stark.Emit.NoPia
{
    internal sealed class EmbeddedEvent : EmbeddedTypesManager.CommonEmbeddedEvent
    {
        public EmbeddedEvent(EventSymbol underlyingEvent, EmbeddedMethod adder, EmbeddedMethod remover) :
            base(underlyingEvent, adder, remover, null)
        {
        }

        protected override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            return UnderlyingEvent.GetCustomAttributesToEmit(moduleBuilder);
        }

        protected override bool IsRuntimeSpecial
        {
            get
            {
                return UnderlyingEvent.HasRuntimeSpecialName;
            }
        }

        protected override bool IsSpecialName
        {
            get
            {
                return UnderlyingEvent.HasSpecialName;
            }
        }

        protected override Cci.ITypeReference GetType(PEModuleBuilder moduleBuilder, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            return moduleBuilder.Translate(UnderlyingEvent.Type.TypeSymbol, syntaxNodeOpt, diagnostics);
        }

        protected override EmbeddedType ContainingType
        {
            get { return AnAccessor.ContainingType; }
        }

        protected override Cci.TypeMemberVisibility Visibility
        {
            get
            {
                return PEModuleBuilder.MemberVisibility(UnderlyingEvent);
            }
        }

        protected override string Name
        {
            get
            {
                return UnderlyingEvent.MetadataName;
            }
        }
    }
}
