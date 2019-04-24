// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.Options.Providers;

namespace StarkPlatform.CodeAnalysis.Completion
{
    [ExportOptionProvider, Shared]
    internal class CompletionOptionsProvider : IOptionProvider
    {
        public ImmutableArray<IOption> Options { get; } = ImmutableArray.Create<IOption>(
            CompletionOptions.HideAdvancedMembers,
            CompletionOptions.TriggerOnTyping,
            CompletionOptions.TriggerOnTypingLetters,
            CompletionOptions.ShowCompletionItemFilters,
            CompletionOptions.HighlightMatchingPortionsOfCompletionListItems,
            CompletionOptions.EnterKeyBehavior,
            CompletionOptions.SnippetsBehavior);
    }
}
