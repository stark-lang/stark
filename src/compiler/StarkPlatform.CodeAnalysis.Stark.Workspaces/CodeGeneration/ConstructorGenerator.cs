// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StarkPlatform.CodeAnalysis.CodeGeneration;
using StarkPlatform.CodeAnalysis.Stark.CodeStyle;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using static StarkPlatform.CodeAnalysis.CodeGeneration.CodeGenerationHelpers;
using static StarkPlatform.CodeAnalysis.Stark.CodeGeneration.CSharpCodeGenerationHelpers;

namespace StarkPlatform.CodeAnalysis.Stark.CodeGeneration
{
    internal static class ConstructorGenerator
    {
        private static MemberDeclarationSyntax LastConstructorOrField(SyntaxList<MemberDeclarationSyntax> members)
        {
            return LastConstructor(members) ?? LastField(members);
        }

        internal static TypeDeclarationSyntax AddConstructorTo(
            TypeDeclarationSyntax destination,
            IMethodSymbol constructor,
            Workspace workspace,
            CodeGenerationOptions options,
            IList<bool> availableIndices)
        {
            var constructorDeclaration = GenerateConstructorDeclaration(
                constructor, GetDestination(destination), workspace, options,
                destination?.SyntaxTree.Options ?? options.ParseOptions);

            // Generate after the last constructor, or after the last field, or at the start of the
            // type.
            var members = Insert(destination.Members, constructorDeclaration, options,
                availableIndices, after: LastConstructorOrField, before: FirstMember);

            return AddMembersTo(destination, members);
        }

        internal static ConstructorDeclarationSyntax GenerateConstructorDeclaration(
            IMethodSymbol constructor, CodeGenerationDestination destination,
            Workspace workspace, CodeGenerationOptions options, ParseOptions parseOptions)
        {
            options = options ?? CodeGenerationOptions.Default;

            var reusableSyntax = GetReuseableSyntaxNodeForSymbol<ConstructorDeclarationSyntax>(constructor, options);
            if (reusableSyntax != null)
            {
                return reusableSyntax;
            }

            var hasNoBody = !options.GenerateMethodBodies;

            var declaration = SyntaxFactory.ConstructorDeclaration(
                attributeLists: AttributeGenerator.GenerateAttributeLists(constructor.GetAttributes(), options),
                modifiers: GenerateModifiers(constructor, options),
                parameterList: ParameterGenerator.GenerateParameterList(constructor.Parameters, isExplicit: false, options: options),
                initializer: GenerateConstructorInitializer(constructor),
                body: hasNoBody ? null : GenerateBlock(constructor),
                expressionBody: null,
                eosToken: hasNoBody ? SyntaxFactory.Token(SyntaxKind.SemicolonToken) : default);

            declaration = UseExpressionBodyIfDesired(workspace, declaration, parseOptions);

            return AddFormatterAndCodeGeneratorAnnotationsTo(
                ConditionallyAddDocumentationCommentTo(declaration, constructor, options));
        }

        private static ConstructorDeclarationSyntax UseExpressionBodyIfDesired(
            Workspace workspace, ConstructorDeclarationSyntax declaration, ParseOptions options)
        {
            if (declaration.ExpressionBody == null)
            {
                var expressionBodyPreference = workspace.Options.GetOption(CSharpCodeStyleOptions.PreferExpressionBodiedConstructors).Value;
                if (declaration.Body.TryConvertToArrowExpressionBody(
                        declaration.Kind(), options, expressionBodyPreference,
                        out var expressionBody, out var eosToken))
                {
                    return declaration.WithBody(null)
                                      .WithExpressionBody(expressionBody)
                                      .WithEosToken(eosToken);
                }
            }

            return declaration;
        }

        private static ConstructorInitializerSyntax GenerateConstructorInitializer(
            IMethodSymbol constructor)
        {
            var thisArguments = CodeGenerationConstructorInfo.GetThisConstructorArgumentsOpt(constructor);

            var arguments = !thisArguments.IsDefault ? thisArguments : CodeGenerationConstructorInfo.GetBaseConstructorArgumentsOpt(constructor);
            var kind = CodeGenerationConstructorInfo.GetThisConstructorArgumentsOpt(constructor) != null
                ? SyntaxKind.ThisConstructorInitializer
                : SyntaxKind.BaseConstructorInitializer;

            return arguments == null
                ? null
                : SyntaxFactory.ConstructorInitializer(kind).WithArgumentList(GenerateArgumentList(arguments));
        }

        private static ArgumentListSyntax GenerateArgumentList(ImmutableArray<SyntaxNode> arguments)
        {
            return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments.Select(ArgumentGenerator.GenerateArgument)));
        }

        private static BlockSyntax GenerateBlock(
            IMethodSymbol constructor)
        {
            var statements = CodeGenerationConstructorInfo.GetStatements(constructor) == null
                ? default
                : StatementGenerator.GenerateStatements(CodeGenerationConstructorInfo.GetStatements(constructor));

            return SyntaxFactory.Block(statements);
        }

        private static SyntaxTokenList GenerateModifiers(IMethodSymbol constructor, CodeGenerationOptions options)
        {
            var tokens = new List<SyntaxToken>();

            if (constructor.IsStatic)
            {
                tokens.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }
            else
            {
                AddAccessibilityModifiers(constructor.DeclaredAccessibility, tokens, options, Accessibility.Private);
            }

            return tokens.ToSyntaxTokenList();
        }
    }
}
