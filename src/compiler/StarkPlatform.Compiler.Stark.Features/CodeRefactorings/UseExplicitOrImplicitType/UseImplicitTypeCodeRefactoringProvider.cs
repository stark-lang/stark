// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.Compiler.CodeRefactorings;
using StarkPlatform.Compiler.Stark.CodeRefactorings.UseType;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Stark.TypeStyle;
using StarkPlatform.Compiler.Stark.Utilities;
using StarkPlatform.Compiler.Editing;
using StarkPlatform.Compiler.Options;

namespace StarkPlatform.Compiler.Stark.CodeRefactorings.UseImplicitType
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
