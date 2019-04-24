// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using StarkPlatform.CodeAnalysis.CaseCorrection;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.CaseCorrection
{
    [ExportLanguageService(typeof(ICaseCorrectionService), LanguageNames.Stark), Shared]
    internal class CSharpCaseCorrectionService : AbstractCaseCorrectionService
    {
        protected override void AddReplacements(
            SemanticModel semanticModel,
            SyntaxNode root,
            ImmutableArray<TextSpan> spans,
            Workspace workspace,
            ConcurrentDictionary<SyntaxToken, SyntaxToken> replacements,
            CancellationToken cancellationToken)
        {
            // C# doesn't support case correction since we are a case sensitive language.
            return;
        }
    }
}
