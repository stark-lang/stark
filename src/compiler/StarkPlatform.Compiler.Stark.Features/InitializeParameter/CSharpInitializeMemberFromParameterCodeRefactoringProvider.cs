﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.Compiler.CodeRefactorings;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Editing;
using StarkPlatform.Compiler.InitializeParameter;
using StarkPlatform.Compiler.Operations;

namespace StarkPlatform.Compiler.Stark.InitializeParameter
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = nameof(CSharpInitializeMemberFromParameterCodeRefactoringProvider)), Shared]
    [ExtensionOrder(Before = nameof(CSharpAddParameterCheckCodeRefactoringProvider))]
    [ExtensionOrder(Before = PredefinedCodeRefactoringProviderNames.Wrapping)]
    internal class CSharpInitializeMemberFromParameterCodeRefactoringProvider :
        AbstractInitializeMemberFromParameterCodeRefactoringProvider<
            ParameterSyntax,
            StatementSyntax,
            ExpressionSyntax>
    {
        protected override bool IsFunctionDeclaration(SyntaxNode node)
            => InitializeParameterHelpers.IsFunctionDeclaration(node);

        protected override SyntaxNode GetTypeBlock(SyntaxNode node)
            => node;

        protected override SyntaxNode TryGetLastStatement(IBlockOperation blockStatementOpt)
            => InitializeParameterHelpers.TryGetLastStatement(blockStatementOpt);

        protected override void InsertStatement(SyntaxEditor editor, SyntaxNode functionDeclaration, IMethodSymbol method, SyntaxNode statementToAddAfterOpt, StatementSyntax statement)
            => InitializeParameterHelpers.InsertStatement(editor, functionDeclaration, method, statementToAddAfterOpt, statement);

        protected override bool IsImplicitConversion(Compilation compilation, ITypeSymbol source, ITypeSymbol destination)
            => InitializeParameterHelpers.IsImplicitConversion(compilation, source, destination);

        // Fields are always private by default in C#.
        protected override Accessibility DetermineDefaultFieldAccessibility(INamedTypeSymbol containingType)
            => Accessibility.Private;

        // Properties are always private by default in C#.
        protected override Accessibility DetermineDefaultPropertyAccessibility()
            => Accessibility.Private;

        protected override SyntaxNode GetBody(SyntaxNode functionDeclaration)
            => InitializeParameterHelpers.GetBody(functionDeclaration);
    }
}