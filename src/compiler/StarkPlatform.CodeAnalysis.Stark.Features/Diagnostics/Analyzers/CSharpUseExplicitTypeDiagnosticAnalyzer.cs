// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.CodeStyle.TypeStyle;
using StarkPlatform.CodeAnalysis.Stark.Utilities;
using StarkPlatform.CodeAnalysis.Diagnostics;
using StarkPlatform.CodeAnalysis.Options;

namespace StarkPlatform.CodeAnalysis.Stark.Diagnostics.TypeStyle
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal sealed class CSharpUseExplicitTypeDiagnosticAnalyzer : CSharpTypeStyleDiagnosticAnalyzerBase
    {
        private static readonly LocalizableString s_Title =
            new LocalizableResourceString(nameof(CSharpFeaturesResources.Use_explicit_type), CSharpFeaturesResources.ResourceManager, typeof(CSharpFeaturesResources));

        private static readonly LocalizableString s_Message =
            new LocalizableResourceString(nameof(CSharpFeaturesResources.Use_explicit_type_instead_of_var), CSharpFeaturesResources.ResourceManager, typeof(CSharpFeaturesResources));

        protected override CSharpTypeStyleHelper Helper => CSharpUseExplicitTypeHelper.Instance;

        public CSharpUseExplicitTypeDiagnosticAnalyzer()
            : base(diagnosticId: IDEDiagnosticIds.UseExplicitTypeDiagnosticId,
                   title: s_Title,
                   message: s_Message)
        {
        }
    }
}
