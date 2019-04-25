// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Composition;
using System.Threading;
using StarkPlatform.Compiler.Classification;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark.Classification
{
    [ExportLanguageService(typeof(IClassificationService), LanguageNames.Stark), Shared]
    internal class CSharpEditorClassificationService : AbstractClassificationService
    {
        public override void AddLexicalClassifications(SourceText text, TextSpan textSpan, List<ClassifiedSpan> result, CancellationToken cancellationToken)
        {
            var temp = ArrayBuilder<ClassifiedSpan>.GetInstance();
            ClassificationHelpers.AddLexicalClassifications(text, textSpan, temp, cancellationToken);
            AddRange(temp, result);
            temp.Free();
        }

        public override ClassifiedSpan AdjustStaleClassification(SourceText text, ClassifiedSpan classifiedSpan)
            => ClassificationHelpers.AdjustStaleClassification(text, classifiedSpan);
    }
}
