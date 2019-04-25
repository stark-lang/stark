// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using StarkPlatform.Compiler.Classification;
using StarkPlatform.Compiler.Classification.Classifiers;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Classification
{
    internal class OperatorOverloadSyntaxClassifier : AbstractSyntaxClassifier
    {
        public override ImmutableArray<Type> SyntaxNodeTypes { get; } = ImmutableArray.Create(
            typeof(AssignmentExpressionSyntax),
            typeof(BinaryExpressionSyntax),
            typeof(PrefixUnaryExpressionSyntax),
            typeof(PostfixUnaryExpressionSyntax));

        public override void AddClassifications(
            Workspace workspace,
            SyntaxNode syntax,
            SemanticModel semanticModel,
            ArrayBuilder<ClassifiedSpan> result,
            CancellationToken cancellationToken)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(syntax, cancellationToken);
            if (symbolInfo.Symbol is IMethodSymbol methodSymbol
                && methodSymbol.MethodKind == MethodKind.UserDefinedOperator)
            {
                var operatorSpan = GetOperatorTokenSpan(syntax);
                if (!operatorSpan.IsEmpty)
                {
                    result.Add(new ClassifiedSpan(operatorSpan, ClassificationTypeNames.OperatorOverloaded));
                }
            }
        }

        private static TextSpan GetOperatorTokenSpan(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case AssignmentExpressionSyntax assignmentExpression:
                    return assignmentExpression.OperatorToken.Span;
                case BinaryExpressionSyntax binaryExpression:
                    return binaryExpression.OperatorToken.Span;
                case PrefixUnaryExpressionSyntax prefixUnaryExpression:
                    return prefixUnaryExpression.OperatorToken.Span;
                case PostfixUnaryExpressionSyntax postfixUnaryExpression:
                    return postfixUnaryExpression.OperatorToken.Span;
            }

            return default;
        }
    }
}
