// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.Options.Providers;

namespace StarkPlatform.Compiler.Formatting
{
    [ExportOptionProvider, Shared]
    internal sealed class FormattingOptionsProvider : IOptionProvider
    {
        public ImmutableArray<IOption> Options { get; } = FormattingOptions.AllOptions;
    }
}
