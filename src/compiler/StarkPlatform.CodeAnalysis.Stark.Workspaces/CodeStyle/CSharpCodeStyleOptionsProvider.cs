// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.CodeStyle;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.Options.Providers;

namespace StarkPlatform.CodeAnalysis.Stark.CodeStyle
{
    [ExportOptionProvider, Shared]
    internal class CSharpCodeStyleOptionsProvider : IOptionProvider
    {
        public ImmutableArray<IOption> Options { get; } = CSharpCodeStyleOptions.AllOptions;
    }
}
