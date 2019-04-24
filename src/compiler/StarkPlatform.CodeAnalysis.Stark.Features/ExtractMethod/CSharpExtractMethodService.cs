// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.ExtractMethod;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.ExtractMethod
{
    [Export(typeof(IExtractMethodService)), Shared]
    [ExportLanguageService(typeof(IExtractMethodService), LanguageNames.Stark)]
    internal class CSharpExtractMethodService : AbstractExtractMethodService<CSharpSelectionValidator, CSharpMethodExtractor, CSharpSelectionResult>
    {
        [ImportingConstructor]
        public CSharpExtractMethodService()
        {
        }

        protected override CSharpSelectionValidator CreateSelectionValidator(SemanticDocument document, TextSpan textSpan, OptionSet options)
        {
            return new CSharpSelectionValidator(document, textSpan, options);
        }

        protected override CSharpMethodExtractor CreateMethodExtractor(CSharpSelectionResult selectionResult)
        {
            return new CSharpMethodExtractor(selectionResult);
        }
    }
}
