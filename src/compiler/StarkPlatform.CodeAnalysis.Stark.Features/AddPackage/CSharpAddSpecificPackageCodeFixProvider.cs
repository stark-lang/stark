// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.AddPackage;
using StarkPlatform.CodeAnalysis.CodeFixes;

namespace StarkPlatform.CodeAnalysis.Stark.AddPackage
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal class CSharpAddSpecificPackageCodeFixProvider : AbstractAddSpecificPackageCodeFixProvider
    {
        private const string CS8179 = nameof(CS8179); // Predefined type 'System.ValueTuple`2' is not defined or imported

        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(CS8179);

        protected override string GetAssemblyName(string id)
        {
            switch (id)
            {
                case CS8179: return "System.ValueTuple";
            }

            return null;
        }
    }
}
