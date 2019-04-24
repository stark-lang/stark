// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.Options.Providers;

namespace StarkPlatform.CodeAnalysis.CodeStyle
{
    [ExportOptionProvider, Shared]
    internal sealed class CodeStyleOptionsProvider : IOptionProvider
    {
        public ImmutableArray<IOption> Options { get; } = CodeStyleOptions.AllOptions;
    }
}
