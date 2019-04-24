// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Editor.CSharp.LanguageServices
{
    internal partial class CSharpSymbolDisplayService : AbstractSymbolDisplayService
    {
        public CSharpSymbolDisplayService(HostLanguageServices provider)
            : base(provider.GetService<IAnonymousTypeDisplayService>())
        {
        }

        public override ImmutableArray<SymbolDisplayPart> ToDisplayParts(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            return StarkPlatform.CodeAnalysis.Stark.SymbolDisplay.ToDisplayParts(symbol, format);
        }

        public override ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(SemanticModel semanticModel, int position, ISymbol symbol, SymbolDisplayFormat format)
        {
            return symbol.ToMinimalDisplayParts(semanticModel, position, format);
        }

        protected override AbstractSymbolDescriptionBuilder CreateDescriptionBuilder(Workspace workspace, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            return new SymbolDescriptionBuilder(this, semanticModel, position, workspace, this.AnonymousTypeDisplayService, cancellationToken);
        }
    }
}
