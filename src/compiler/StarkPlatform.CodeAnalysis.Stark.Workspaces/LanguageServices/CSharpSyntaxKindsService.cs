// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Stark.LanguageServices
{
    [ExportLanguageService(typeof(ISyntaxKindsService), LanguageNames.Stark), Shared]
    internal sealed class CSharpSyntaxKindsService : ISyntaxKindsService
    {
        public int IfKeyword => (int)SyntaxKind.IfKeyword;
        public int LogicalAndExpression => (int)SyntaxKind.LogicalAndExpression;
        public int LogicalOrExpression => (int)SyntaxKind.LogicalOrExpression;
    }
}
