// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Formatting.Rules
{
    internal interface IHostDependentFormattingRuleFactoryService : IWorkspaceService
    {
        bool ShouldNotFormatOrCommitOnPaste(Document document);
        bool ShouldUseBaseIndentation(Document document);
        IFormattingRule CreateRule(Document document, int position);
        IEnumerable<TextChange> FilterFormattedChanges(Document document, TextSpan span, IList<TextChange> changes);
    }
}
