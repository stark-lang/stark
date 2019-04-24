// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.MoveDeclarationNearReference;

namespace StarkPlatform.CodeAnalysis.Stark.MoveDeclarationNearReference
{
    [ExportLanguageService(typeof(IMoveDeclarationNearReferenceService), LanguageNames.Stark), Shared]
    internal partial class CSharpMoveDeclarationNearReferenceService :
        AbstractMoveDeclarationNearReferenceService<
            CSharpMoveDeclarationNearReferenceService,
            StatementSyntax,
            LocalDeclarationStatementSyntax,
            VariableDeclarationSyntax>
    {
        protected override bool IsMeaningfulBlock(SyntaxNode node)
        {
            return node is AnonymousFunctionExpressionSyntax ||
                   node is LocalFunctionStatementSyntax ||
                   node is ForStatementSyntax ||
                   node is WhileStatementSyntax ||
                   node is DoStatementSyntax ||
                   node is CheckedStatementSyntax;
        }

        protected override SyntaxNode GetVariableDeclarationSymbolNode(VariableDeclarationSyntax variableDeclarator)
            => variableDeclarator;

        protected override bool IsValidVariableDeclaration(VariableDeclarationSyntax variableDeclarator)
            => true;

        protected override SyntaxToken GetIdentifierOfVariableDeclaration(VariableDeclarationSyntax variableDeclarator)
            => variableDeclarator.Identifier;

        protected override async Task<bool> TypesAreCompatibleAsync(
            Document document, ILocalSymbol localSymbol,
            LocalDeclarationStatementSyntax declarationStatement,
            SyntaxNode right, CancellationToken cancellationToken)
        {
            var type = declarationStatement.Declaration.Type;
            if (type.IsNullWithNoType())
            {
                // Type inference.  Only merge if types match.
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                var rightType = semanticModel.GetTypeInfo(right, cancellationToken);
                return Equals(localSymbol.Type, rightType.Type);
            }

            return true;
        }

        protected override bool CanMoveToBlock(ILocalSymbol localSymbol, SyntaxNode currentBlock, SyntaxNode destinationBlock)
            => localSymbol.CanSafelyMoveLocalToBlock(currentBlock, destinationBlock);
    }
}
