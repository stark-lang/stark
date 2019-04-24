// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.ConvertToInterpolatedString;

namespace StarkPlatform.CodeAnalysis.Stark.ConvertToInterpolatedString
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = PredefinedCodeRefactoringProviderNames.ConvertToInterpolatedString), Shared]
    internal class CSharpConvertConcatenationToInterpolatedStringRefactoringProvider :
        AbstractConvertConcatenationToInterpolatedStringRefactoringProvider
    {
        private const string InterpolatedVerbatimText = "$@\"";

        protected override SyntaxToken CreateInterpolatedStringStartToken(bool isVerbatim)
        {
            return isVerbatim
                ? SyntaxFactory.Token(default, SyntaxKind.InterpolatedVerbatimStringStartToken, InterpolatedVerbatimText, InterpolatedVerbatimText, default)
                : SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken);
        }

        protected override SyntaxToken CreateInterpolatedStringEndToken()
            => SyntaxFactory.Token(SyntaxKind.InterpolatedStringEndToken);

        protected override string GetTextWithoutQuotes(string text, bool isVerbatim)
            => isVerbatim
                ? text.Substring("@'".Length, text.Length - "@''".Length)
                : text.Substring("'".Length, text.Length - "''".Length);
    }
}
