﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.Compiler.CodeFixes;
using StarkPlatform.Compiler.Stark.CodeStyle;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.OrderModifiers;

namespace StarkPlatform.Compiler.Stark.OrderModifiers
{
    [ExportCodeFixProvider(LanguageNames.Stark), Shared]
    internal class CSharpOrderModifiersCodeFixProvider : AbstractOrderModifiersCodeFixProvider
    {
        private const string CS0267 = nameof(CS0267); // The 'partial' modifier can only appear immediately before 'class', 'struct', 'interface', or 'void'

        public CSharpOrderModifiersCodeFixProvider()
            : base(CSharpSyntaxFactsService.Instance, CSharpCodeStyleOptions.PreferredModifierOrder, CSharpOrderModifiersHelper.Instance)
        {
        }

        protected override ImmutableArray<string> FixableCompilerErrorIds { get; } = ImmutableArray.Create(CS0267);
    }
}
