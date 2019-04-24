// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Linq;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark.ConvertAnonymousTypeToClass
{
    [ExtensionOrder(Before = PredefinedCodeRefactoringProviderNames.IntroduceVariable)]
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = nameof(PredefinedCodeRefactoringProviderNames.ConvertAnonymousTypeToClass)), Shared]
    internal class CSharpConvertAnonymousTypeToClassCodeRefactoringProvider :
        AbstractConvertAnonymousTypeToClassCodeRefactoringProvider<
            ExpressionSyntax,
            NameSyntax,
            IdentifierNameSyntax,
            ObjectCreationExpressionSyntax,
            AnonymousObjectCreationExpressionSyntax,
            NamespaceDeclarationSyntax>
    {
        protected override ObjectCreationExpressionSyntax CreateObjectCreationExpression(
            NameSyntax nameNode, AnonymousObjectCreationExpressionSyntax anonymousObject)
        {
            return SyntaxFactory.ObjectCreationExpression(
                nameNode, CreateArgumentList(anonymousObject), initializer: default);
        }

        private ArgumentListSyntax CreateArgumentList(AnonymousObjectCreationExpressionSyntax anonymousObject)
            => SyntaxFactory.ArgumentList(
                SyntaxFactory.Token(SyntaxKind.OpenParenToken).WithTriviaFrom(anonymousObject.OpenBraceToken),
                CreateArguments(anonymousObject.Initializers),
                SyntaxFactory.Token(SyntaxKind.CloseParenToken).WithTriviaFrom(anonymousObject.CloseBraceToken));

        private SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> initializers)
            => SyntaxFactory.SeparatedList<ArgumentSyntax>(CreateArguments(initializers.GetWithSeparators()));

        private SyntaxNodeOrTokenList CreateArguments(SyntaxNodeOrTokenList list)
            => new SyntaxNodeOrTokenList(list.Select(CreateArgumentOrComma));

        private SyntaxNodeOrToken CreateArgumentOrComma(SyntaxNodeOrToken declOrComma)
            => declOrComma.IsToken
                ? declOrComma
                : CreateArgument((AnonymousObjectMemberDeclaratorSyntax)declOrComma);

        private ArgumentSyntax CreateArgument(AnonymousObjectMemberDeclaratorSyntax decl)
            => SyntaxFactory.Argument(decl.Expression);
    }
}
