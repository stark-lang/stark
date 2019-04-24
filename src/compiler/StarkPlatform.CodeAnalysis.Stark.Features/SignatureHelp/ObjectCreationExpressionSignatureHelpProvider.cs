// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.DocumentationComments;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.SignatureHelp;
using StarkPlatform.CodeAnalysis.Text;
using StarkPlatform.CodeAnalysis.Stark.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.SignatureHelp
{
    [ExportSignatureHelpProvider("ObjectCreationExpressionSignatureHelpProvider", LanguageNames.Stark), Shared]
    internal partial class ObjectCreationExpressionSignatureHelpProvider : AbstractCSharpSignatureHelpProvider
    {
        public override bool IsTriggerCharacter(char ch)
        {
            return ch == '(' || ch == ',';
        }

        public override bool IsRetriggerCharacter(char ch)
        {
            return ch == ')';
        }

        private bool TryGetObjectCreationExpression(SyntaxNode root, int position, ISyntaxFactsService syntaxFacts, SignatureHelpTriggerReason triggerReason, CancellationToken cancellationToken, out ObjectCreationExpressionSyntax expression)
        {
            if (!CommonSignatureHelpUtilities.TryGetSyntax(root, position, syntaxFacts, triggerReason, IsTriggerToken, IsArgumentListToken, cancellationToken, out expression))
            {
                return false;
            }

            return expression.ArgumentList != null;
        }

        private bool IsTriggerToken(SyntaxToken token)
        {
            return SignatureHelpUtilities.IsTriggerParenOrComma<ObjectCreationExpressionSyntax>(token, IsTriggerCharacter);
        }

        private static bool IsArgumentListToken(ObjectCreationExpressionSyntax expression, SyntaxToken token)
        {
            return expression.ArgumentList != null &&
                expression.ArgumentList.Span.Contains(token.SpanStart) &&
                token != expression.ArgumentList.CloseParenToken;
        }

        protected override async Task<SignatureHelpItems> GetItemsWorkerAsync(Document document, int position, SignatureHelpTriggerInfo triggerInfo, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (!TryGetObjectCreationExpression(root, position, document.GetLanguageService<ISyntaxFactsService>(), triggerInfo.TriggerReason, cancellationToken, out var objectCreationExpression))
            {
                return null;
            }

            var semanticModel = await document.GetSemanticModelForNodeAsync(objectCreationExpression, cancellationToken).ConfigureAwait(false);
            var type = semanticModel.GetTypeInfo(objectCreationExpression, cancellationToken).Type as INamedTypeSymbol;
            if (type == null)
            {
                return null;
            }

            var within = semanticModel.GetEnclosingNamedType(position, cancellationToken);
            if (within == null)
            {
                return null;
            }

            var symbolDisplayService = document.GetLanguageService<ISymbolDisplayService>();
            var anonymousTypeDisplayService = document.GetLanguageService<IAnonymousTypeDisplayService>();
            var documentationCommentFormattingService = document.GetLanguageService<IDocumentationCommentFormattingService>();
            var textSpan = SignatureHelpUtilities.GetSignatureHelpSpan(objectCreationExpression.ArgumentList);
            var syntaxFacts = document.GetLanguageService<ISyntaxFactsService>();

            var (items, selectedItem) = type.TypeKind == TypeKind.Delegate
                ? GetDelegateTypeConstructors(objectCreationExpression, semanticModel, symbolDisplayService, anonymousTypeDisplayService, type, within, cancellationToken)
                : GetNormalTypeConstructors(document, objectCreationExpression, semanticModel, symbolDisplayService, anonymousTypeDisplayService, documentationCommentFormattingService, type, within, cancellationToken);

            return CreateSignatureHelpItems(items, textSpan,
                GetCurrentArgumentState(root, position, syntaxFacts, textSpan, cancellationToken), selectedItem);
        }

        public override SignatureHelpState GetCurrentArgumentState(SyntaxNode root, int position, ISyntaxFactsService syntaxFacts, TextSpan currentSpan, CancellationToken cancellationToken)
        {
            if (TryGetObjectCreationExpression(
                    root,
                    position,
                    syntaxFacts,
                    SignatureHelpTriggerReason.InvokeSignatureHelpCommand,
                    cancellationToken,
                    out var expression) &&
                currentSpan.Start == SignatureHelpUtilities.GetSignatureHelpSpan(expression.ArgumentList).Start)
            {
                return SignatureHelpUtilities.GetSignatureHelpState(expression.ArgumentList, position);
            }

            return null;
        }
    }
}
