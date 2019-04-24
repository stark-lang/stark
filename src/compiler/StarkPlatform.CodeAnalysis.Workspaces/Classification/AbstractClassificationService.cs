// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.Extensions;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Classification
{
    internal abstract class AbstractClassificationService : IClassificationService
    {
        public abstract void AddLexicalClassifications(SourceText text, TextSpan textSpan, List<ClassifiedSpan> result, CancellationToken cancellationToken);
        public abstract ClassifiedSpan AdjustStaleClassification(SourceText text, ClassifiedSpan classifiedSpan);

        public async Task AddSemanticClassificationsAsync(Document document, TextSpan textSpan, List<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            var classificationService = document.GetLanguageService<ISyntaxClassificationService>();

            var extensionManager = document.Project.Solution.Workspace.Services.GetService<IExtensionManager>();
            var classifiers = classificationService.GetDefaultSyntaxClassifiers();

            var getNodeClassifiers = extensionManager.CreateNodeExtensionGetter(classifiers, c => c.SyntaxNodeTypes);
            var getTokenClassifiers = extensionManager.CreateTokenExtensionGetter(classifiers, c => c.SyntaxTokenKinds);

            var temp = ArrayBuilder<ClassifiedSpan>.GetInstance();
            await classificationService.AddSemanticClassificationsAsync(document, textSpan, getNodeClassifiers, getTokenClassifiers, temp, cancellationToken).ConfigureAwait(false);
            AddRange(temp, result);
            temp.Free();
        }

        public async Task AddSyntacticClassificationsAsync(Document document, TextSpan textSpan, List<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            var classificationService = document.GetLanguageService<ISyntaxClassificationService>();
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);

            var temp = ArrayBuilder<ClassifiedSpan>.GetInstance();
            classificationService.AddSyntacticClassifications(syntaxTree, textSpan, temp, cancellationToken);
            AddRange(temp, result);
            temp.Free();
        }

        protected void AddRange(ArrayBuilder<ClassifiedSpan> temp, List<ClassifiedSpan> result)
        {
            foreach (var span in temp)
            {
                result.Add(span);
            }
        }
    }
}
