// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.GenerateMember.GenerateEnumMember;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.GenerateMember.GenerateEnumMember
{
    [ExportLanguageService(typeof(IGenerateEnumMemberService), LanguageNames.Stark), Shared]
    internal partial class CSharpGenerateEnumMemberService :
        AbstractGenerateEnumMemberService<CSharpGenerateEnumMemberService, SimpleNameSyntax, ExpressionSyntax>
    {
        protected override bool IsIdentifierNameGeneration(SyntaxNode node)
        {
            return node is IdentifierNameSyntax;
        }

        protected override bool TryInitializeIdentifierNameState(
            SemanticDocument document, SimpleNameSyntax identifierName, CancellationToken cancellationToken,
            out SyntaxToken identifierToken, out ExpressionSyntax simpleNameOrMemberAccessExpression)
        {
            identifierToken = identifierName.Identifier;
            if (identifierToken.ValueText != string.Empty &&
                !identifierName.IsNullWithNoType())
            {
                var memberAccess = identifierName.Parent as MemberAccessExpressionSyntax;
                simpleNameOrMemberAccessExpression = memberAccess != null && memberAccess.Name == identifierName
                    ? (ExpressionSyntax)memberAccess
                    : identifierName;

                // If we're being invoked, then don't offer this, offer generate method instead.
                // Note: we could offer to generate a field with a delegate type.  However, that's
                // very esoteric and probably not what most users want.
                if (simpleNameOrMemberAccessExpression.IsParentKind(SyntaxKind.InvocationExpression) ||
                    simpleNameOrMemberAccessExpression.IsParentKind(SyntaxKind.ObjectCreationExpression) ||
                    simpleNameOrMemberAccessExpression.IsParentKind(SyntaxKind.GotoStatement) ||
                    simpleNameOrMemberAccessExpression.IsParentKind(SyntaxKind.AliasQualifiedName))
                {
                    return false;
                }

                return true;
            }

            identifierToken = default;
            simpleNameOrMemberAccessExpression = null;
            return false;
        }
    }
}
