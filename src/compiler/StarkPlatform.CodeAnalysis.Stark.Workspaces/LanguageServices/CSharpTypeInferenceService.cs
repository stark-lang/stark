// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.LanguageServices.TypeInferenceService;

namespace StarkPlatform.CodeAnalysis.Stark
{
    [ExportLanguageService(typeof(ITypeInferenceService), LanguageNames.Stark), Shared]
    internal partial class CSharpTypeInferenceService : AbstractTypeInferenceService
    {
        public static readonly CSharpTypeInferenceService Instance = new CSharpTypeInferenceService();

        protected override AbstractTypeInferrer CreateTypeInferrer(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return new TypeInferrer(semanticModel, cancellationToken);
        }
    }
}
