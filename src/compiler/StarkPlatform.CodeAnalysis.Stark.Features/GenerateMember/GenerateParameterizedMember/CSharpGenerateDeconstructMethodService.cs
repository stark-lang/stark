// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.GenerateMember.GenerateParameterizedMember;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.GenerateMember.GenerateParameterizedMember;
using StarkPlatform.CodeAnalysis.Host.Mef;

namespace StarkPlatform.CodeAnalysis.Stark.GenerateMember.GenerateMethod
{
    [ExportLanguageService(typeof(IGenerateDeconstructMemberService), LanguageNames.Stark), Shared]
    internal sealed class CSharpGenerateDeconstructMethodService :
        AbstractGenerateDeconstructMethodService<CSharpGenerateDeconstructMethodService, SimpleNameSyntax, ExpressionSyntax, InvocationExpressionSyntax>
    {
        protected override bool ContainingTypesOrSelfHasUnsafeKeyword(INamedTypeSymbol containingType)
            => containingType.ContainingTypesOrSelfHasUnsafeKeyword();

        protected override AbstractInvocationInfo CreateInvocationMethodInfo(SemanticDocument document, AbstractGenerateParameterizedMemberService<CSharpGenerateDeconstructMethodService, SimpleNameSyntax, ExpressionSyntax, InvocationExpressionSyntax>.State state)
            => new CSharpGenerateParameterizedMemberService<CSharpGenerateDeconstructMethodService>.InvocationExpressionInfo(document, state);

        protected override bool AreSpecialOptionsActive(SemanticModel semanticModel)
            => CSharpCommonGenerationServiceMethods.AreSpecialOptionsActive(semanticModel);

        protected override bool IsValidSymbol(ISymbol symbol, SemanticModel semanticModel)
            => CSharpCommonGenerationServiceMethods.IsValidSymbol(symbol, semanticModel);
    }
}
