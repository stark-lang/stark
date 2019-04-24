// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.ConvertNumericLiteral;

namespace StarkPlatform.CodeAnalysis.Stark.ConvertNumericLiteral
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = nameof(CSharpConvertNumericLiteralCodeRefactoringProvider)), Shared]
    internal sealed class CSharpConvertNumericLiteralCodeRefactoringProvider : AbstractConvertNumericLiteralCodeRefactoringProvider
    {
        protected override (string hexPrefix, string binaryPrefix) GetNumericLiteralPrefixes() => (hexPrefix: "0x", binaryPrefix: "0b");
    }
}
