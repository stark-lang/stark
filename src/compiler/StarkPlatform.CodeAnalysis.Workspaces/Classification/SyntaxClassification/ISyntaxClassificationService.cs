// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Classification.Classifiers;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Classification
{
    internal interface ISyntaxClassificationService : ILanguageService
    {
        ImmutableArray<ISyntaxClassifier> GetDefaultSyntaxClassifiers();

        void AddLexicalClassifications(SourceText text,
            TextSpan textSpan,
            ArrayBuilder<ClassifiedSpan> result,
            CancellationToken cancellationToken);

        void AddSyntacticClassifications(SyntaxTree syntaxTree,
            TextSpan textSpan,
            ArrayBuilder<ClassifiedSpan> result,
            CancellationToken cancellationToken);

        Task AddSemanticClassificationsAsync(Document document,
            TextSpan textSpan,
            Func<SyntaxNode, ImmutableArray<ISyntaxClassifier>> getNodeClassifiers,
            Func<SyntaxToken, ImmutableArray<ISyntaxClassifier>> getTokenClassifiers,
            ArrayBuilder<ClassifiedSpan> result,
            CancellationToken cancellationToken);

        void AddSemanticClassifications(
            SemanticModel semanticModel,
            TextSpan textSpan,
            Workspace workspace,
            Func<SyntaxNode, ImmutableArray<ISyntaxClassifier>> getNodeClassifiers,
            Func<SyntaxToken, ImmutableArray<ISyntaxClassifier>> getTokenClassifiers,
            ArrayBuilder<ClassifiedSpan> result,
            CancellationToken cancellationToken);

        ClassifiedSpan FixClassification(SourceText text, ClassifiedSpan classifiedSpan);
    }
}
