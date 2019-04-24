// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.Options.Providers;

namespace StarkPlatform.CodeAnalysis.ValidateFormatString
{
    internal class ValidateFormatStringOption
    {
        public static PerLanguageOption<bool> ReportInvalidPlaceholdersInStringDotFormatCalls =
            new PerLanguageOption<bool>(
                nameof(ValidateFormatStringOption),
                nameof(ReportInvalidPlaceholdersInStringDotFormatCalls),
                defaultValue: true,
                storageLocations: new RoamingProfileStorageLocation("TextEditor.%LANGUAGE%.Specific.WarnOnInvalidStringDotFormatCalls"));
    }

    [ExportOptionProvider, Shared]
    internal class ValidateFormatStringOptionProvider : IOptionProvider
    {
        public ImmutableArray<IOption> Options { get; } = ImmutableArray.Create<IOption>(
            ValidateFormatStringOption.ReportInvalidPlaceholdersInStringDotFormatCalls);
    }
}
