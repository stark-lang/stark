// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using StarkPlatform.Compiler.Stark;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.LanguageServices;
using StarkPlatform.Compiler.Shared.Extensions;

namespace StarkPlatform.Compiler.Editor.CSharp.LanguageServices
{
    [ExportLanguageService(typeof(IAnonymousTypeDisplayService), LanguageNames.Stark), Shared]
    internal class CSharpAnonymousTypeDisplayService : AbstractAnonymousTypeDisplayService
    {
        public override IEnumerable<SymbolDisplayPart> GetAnonymousTypeParts(
            INamedTypeSymbol anonymousType, SemanticModel semanticModel, int position, ISymbolDisplayService displayService)
        {
            var members = new List<SymbolDisplayPart>();

            members.Add(Keyword(SyntaxFacts.GetText(SyntaxKind.NewKeyword)));
            members.AddRange(Space());
            members.Add(Punctuation(SyntaxFacts.GetText(SyntaxKind.OpenBraceToken)));
            members.AddRange(Space());

            bool first = true;
            foreach (var property in anonymousType.GetValidAnonymousTypeProperties())
            {
                if (!first)
                {
                    members.Add(Punctuation(SyntaxFacts.GetText(SyntaxKind.CommaToken)));
                    members.AddRange(Space());
                }

                first = false;
                members.AddRange(displayService.ToMinimalDisplayParts(semanticModel, position, property.Type).Select(p => p.MassageErrorTypeNames("?")));
                members.AddRange(Space());
                members.Add(new SymbolDisplayPart(SymbolDisplayPartKind.PropertyName, property, property.Name));
            }

            members.AddRange(Space());
            members.Add(Punctuation(SyntaxFacts.GetText(SyntaxKind.CloseBraceToken)));

            return members;
        }
    }
}
