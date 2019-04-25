// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.Compiler.CodeActions;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.CodeRefactorings.MoveType
{
    internal interface IMoveTypeService : ILanguageService
    {
        Task<ImmutableArray<CodeAction>> GetRefactoringAsync(Document document, TextSpan textSpan, CancellationToken cancellationToken);
    }
}
