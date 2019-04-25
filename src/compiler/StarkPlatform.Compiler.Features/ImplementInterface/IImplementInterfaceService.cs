// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.Compiler.CodeActions;
using StarkPlatform.Compiler.Host;

namespace StarkPlatform.Compiler.ImplementInterface
{
    internal interface IImplementInterfaceService : ILanguageService
    {
        Task<Document> ImplementInterfaceAsync(Document document, SyntaxNode node, CancellationToken cancellationToken);
        IEnumerable<CodeAction> GetCodeActions(Document document, SemanticModel model, SyntaxNode node, CancellationToken cancellationToken);
    }
}
