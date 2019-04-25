// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.AddFileBanner;
using StarkPlatform.Compiler.CodeRefactorings;

namespace StarkPlatform.Compiler.Stark.AddFileBanner
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark,
        Name = PredefinedCodeRefactoringProviderNames.AddFileBanner), Shared]
    internal class CSharpAddFileBannerCodeRefactoringProvider : AbstractAddFileBannerCodeRefactoringProvider
    {
        protected override bool IsCommentStartCharacter(char ch)
            => ch == '/';

        protected override SyntaxTrivia CreateTrivia(SyntaxTrivia trivia, string text)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    return SyntaxFactory.Comment(text);
            }

            return trivia;
        }
    }
}
