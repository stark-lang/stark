// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.Stark.CodeRefactorings.UseType;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Stark.TypeStyle;
using StarkPlatform.CodeAnalysis.Stark.Utilities;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.Options;

namespace StarkPlatform.CodeAnalysis.Stark.CodeRefactorings.UseImplicitType
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = PredefinedCodeRefactoringProviderNames.UseImplicitType), Shared]
    internal partial class UseImplicitTypeCodeRefactoringProvider : AbstractUseTypeCodeRefactoringProvider
    {
        protected override string Title
            => CSharpFeaturesResources.Use_implicit_type;

        protected override TypeSyntax FindAnalyzableType(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
            => CSharpUseImplicitTypeHelper.Instance.FindAnalyzableType(node, semanticModel, cancellationToken);

        protected override TypeStyleResult AnalyzeTypeName(TypeSyntax typeName, SemanticModel semanticModel, OptionSet optionSet, CancellationToken cancellationToken)
            => CSharpUseImplicitTypeHelper.Instance.AnalyzeTypeName(typeName, semanticModel, optionSet, cancellationToken);

        protected override Task HandleDeclarationAsync(Document document, SyntaxEditor editor, SyntaxNode node, CancellationToken cancellationToken)
        {
            UseImplicitTypeCodeFixProvider.ReplaceTypeWithVar(editor, node);
            return Task.CompletedTask;
        }
    }
}
