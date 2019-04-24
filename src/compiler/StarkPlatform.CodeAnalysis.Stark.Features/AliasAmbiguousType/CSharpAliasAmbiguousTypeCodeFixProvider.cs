// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.AliasAmbiguousType;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.AliasAmbiguousType
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.AliasAmbiguousType), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.FullyQualify)]
    internal class CSharpAliasAmbiguousTypeCodeFixProvider : AbstractAliasAmbiguousTypeCodeFixProvider
    {
        /// <summary>
        /// 'reference' is an ambiguous reference between 'identifier' and 'identifier'
        /// </summary>
        private const string CS0104 = nameof(CS0104);

        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(CS0104);

        protected override string GetTextPreviewOfChange(string alias, ITypeSymbol typeSymbol)
            => $"using { alias } = { typeSymbol.ToNameDisplayString() };";
    }
}
