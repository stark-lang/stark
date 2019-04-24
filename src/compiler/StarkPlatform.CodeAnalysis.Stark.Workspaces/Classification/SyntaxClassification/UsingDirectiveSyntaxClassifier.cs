// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using StarkPlatform.CodeAnalysis.Classification;
using StarkPlatform.CodeAnalysis.Classification.Classifiers;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.PooledObjects;

namespace StarkPlatform.CodeAnalysis.Stark.Classification.Classifiers
{
    internal class ImportDirectiveSyntaxClassifier : AbstractSyntaxClassifier
    {
        public override void AddClassifications(
            Workspace workspace,
            SyntaxNode syntax,
            SemanticModel semanticModel,
            ArrayBuilder<ClassifiedSpan> result,
            CancellationToken cancellationToken)
        {
            if (syntax is ImportDirectiveSyntax usingDirective)
            {
                ClassifyImportDirectiveSyntax(usingDirective, semanticModel, result, cancellationToken);
            }
        }

        public override ImmutableArray<Type> SyntaxNodeTypes { get; } = ImmutableArray.Create(typeof(ImportDirectiveSyntax));

        private void ClassifyImportDirectiveSyntax(
            ImportDirectiveSyntax usingDirective,
            SemanticModel semanticModel,
            ArrayBuilder<ClassifiedSpan> result,
            CancellationToken cancellationToken)
        {
            // For using aliases, we bind the target on the right of the equals and use that
            // binding to classify the alias.
            if (usingDirective.Alias != null)
            {
                var token = usingDirective.Alias.Name;

                var symbolInfo = semanticModel.GetSymbolInfo(usingDirective.Name, cancellationToken);
                if (symbolInfo.Symbol is ITypeSymbol typeSymbol)
                {
                    var classification = GetClassificationForType(typeSymbol);
                    if (classification != null)
                    {
                        result.Add(new ClassifiedSpan(token.Span, classification));
                    }
                }
                else if (symbolInfo.Symbol?.Kind == SymbolKind.Namespace)
                {
                    result.Add(new ClassifiedSpan(token.Span, ClassificationTypeNames.NamespaceName));
                }
            }
        }
    }
}
