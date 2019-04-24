// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeFixes;
using StarkPlatform.CodeAnalysis.CodeFixes.ImplementAbstractClass;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.ImplementAbstractClass
{
    [ExportCodeFixProvider(LanguageNames.Stark, Name = PredefinedCodeFixProviderNames.ImplementAbstractClass), Shared]
    [ExtensionOrder(After = PredefinedCodeFixProviderNames.GenerateType)]
    internal class CSharpImplementAbstractClassCodeFixProvider :
        AbstractImplementAbstractClassCodeFixProvider<ClassDeclarationSyntax>
    {
        private const string CS0534 = nameof(CS0534); // 'Program' does not implement inherited abstract member 'Goo.bar()'

        public CSharpImplementAbstractClassCodeFixProvider()
            : base(CS0534)
        {
        }
    }
}
