﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler;
using StarkPlatform.Compiler.Formatting;
using StarkPlatform.Compiler.Options;

namespace StarkPlatform.Compiler.Stark.Formatting
{
    internal partial class CSharpTriviaFormatter
    {
        private class DocumentationCommentExteriorCommentRewriter : CSharpSyntaxRewriter
        {
            private readonly bool _forceIndentation;
            private readonly int _indentation;
            private readonly int _indentationDelta;
            private readonly OptionSet _optionSet;

            public DocumentationCommentExteriorCommentRewriter(
                bool forceIndentation,
                int indentation,
                int indentationDelta,
                OptionSet optionSet,
                bool visitStructuredTrivia = true)
                : base(visitIntoStructuredTrivia: visitStructuredTrivia)
            {
                _forceIndentation = forceIndentation;
                _indentation = indentation;
                _indentationDelta = indentationDelta;
                _optionSet = optionSet;
            }

            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                if (trivia.Kind() == SyntaxKind.DocumentationCommentExteriorTrivia)
                {
                    if (IsBeginningOrEndOfDocumentComment(trivia))
                    {
                        return base.VisitTrivia(trivia);
                    }
                    else
                    {
                        var triviaText = trivia.ToFullString();

                        var newTriviaText = triviaText.AdjustIndentForXmlDocExteriorTrivia(
                                                _forceIndentation,
                                                _indentation,
                                                _indentationDelta,
                                                _optionSet.GetOption(FormattingOptions.UseTabs, LanguageNames.Stark),
                                                _optionSet.GetOption(FormattingOptions.TabSize, LanguageNames.Stark));

                        if (triviaText == newTriviaText)
                        {
                            return base.VisitTrivia(trivia);
                        }

                        var parsedNewTrivia = SyntaxFactory.DocumentationCommentExterior(newTriviaText);

                        return parsedNewTrivia;
                    }
                }

                return base.VisitTrivia(trivia);
            }

            private bool IsBeginningOrEndOfDocumentComment(SyntaxTrivia trivia)
            {
                var currentParent = trivia.Token.Parent;

                while (currentParent != null)
                {
                    if (currentParent.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia ||
                        currentParent.Kind() == SyntaxKind.MultiLineDocumentationCommentTrivia)
                    {
                        if (trivia.Span.End == currentParent.SpanStart ||
                            trivia.Span.End == currentParent.Span.End)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    currentParent = currentParent.Parent;
                }

                return false;
            }
        }
    }
}
