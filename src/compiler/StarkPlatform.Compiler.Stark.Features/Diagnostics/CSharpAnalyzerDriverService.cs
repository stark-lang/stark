// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Diagnostics
{
    [ExportLanguageService(typeof(IAnalyzerDriverService), LanguageNames.Stark), Shared]
    internal sealed class CSharpAnalyzerDriverService : IAnalyzerDriverService
    {
        public void ComputeDeclarationsInSpan(
            SemanticModel model,
            TextSpan span,
            bool getSymbol,
            ArrayBuilder<DeclarationInfo> builder,
            CancellationToken cancellationToken)
        {
            CSharpDeclarationComputer.ComputeDeclarationsInSpan(model, span, getSymbol, builder, cancellationToken);
        }
    }
}
