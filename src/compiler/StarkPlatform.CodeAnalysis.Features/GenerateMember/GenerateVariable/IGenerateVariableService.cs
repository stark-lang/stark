// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeActions;
using StarkPlatform.CodeAnalysis.Host;

namespace StarkPlatform.CodeAnalysis.GenerateMember.GenerateVariable
{
    internal interface IGenerateVariableService : ILanguageService
    {
        Task<ImmutableArray<CodeAction>> GenerateVariableAsync(Document document, SyntaxNode node, CancellationToken cancellationToken);
    }
}
