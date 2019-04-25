// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using StarkPlatform.Compiler.Stark;
using StarkPlatform.Compiler.Stark.Recommendations;
using StarkPlatform.Compiler.LanguageServices;

namespace StarkPlatform.Compiler.Composition
{
#if false
    public class CSharpWorkspaceFeatures : FeaturePack
    {
        private CSharpWorkspaceFeatures()
        {
        }

        public static readonly FeaturePack Instance = new CSharpWorkspaceFeatures();

        internal override ExportSource ComposeExports(ExportSource root)
        {
            return new ExportList()
            {
                // case correction
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new StarkPlatform.Compiler.Stark.CaseCorrection.CSharpCaseCorrectionService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.CaseCorrection.ICaseCorrectionService), ServiceLayer.Default)),

                // code clean up
                new Lazy<ILanguageServiceFactory, LanguageServiceMetadata>(
                    () => new StarkPlatform.Compiler.Stark.CodeCleanup.CSharpCodeCleanerServiceFactory(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.CodeCleanup.ICodeCleanerService), ServiceLayer.Default)),

                // code generation
                new Lazy<ILanguageServiceFactory, LanguageServiceMetadata>(
                    () => new StarkPlatform.Compiler.Stark.CodeGeneration.CSharpCodeGenerationServiceFactory(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.CodeGeneration.ICodeGenerationService), ServiceLayer.Default)),

                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new StarkPlatform.Compiler.Stark.CodeGeneration.CSharpSyntaxFactory(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.CodeGeneration.ISyntaxFactoryService), ServiceLayer.Default)),

                // formatting service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingService(root),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.Formatting.IFormattingService), ServiceLayer.Default)),

                // formatting rules
                new Lazy<StarkPlatform.Compiler.Formatting.Rules.IFormattingRule, OrderableLanguageMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.AlignTokensFormattingRule(),
                    new OrderableLanguageMetadata(StarkPlatform.Compiler.Stark.Formatting.AlignTokensFormattingRule.Name, LanguageNames.CSharp)),

                new Lazy<StarkPlatform.Compiler.Formatting.Rules.IFormattingRule, OrderableLanguageMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.AnchorIndentationFormattingRule(),
                    new OrderableLanguageMetadata(StarkPlatform.Compiler.Stark.Formatting.AnchorIndentationFormattingRule.Name, LanguageNames.CSharp,
                        after: new string[] { StarkPlatform.Compiler.Stark.Formatting.SuppressFormattingRule.Name })),

                new Lazy<StarkPlatform.Compiler.Formatting.Rules.IFormattingRule, OrderableLanguageMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.ElasticTriviaFormattingRule(),
                    new OrderableLanguageMetadata(StarkPlatform.Compiler.Stark.Formatting.ElasticTriviaFormattingRule.Name, LanguageNames.CSharp)),

                new Lazy<StarkPlatform.Compiler.Formatting.Rules.IFormattingRule, OrderableLanguageMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.EndOfFileTokenFormattingRule(),
                    new OrderableLanguageMetadata(StarkPlatform.Compiler.Stark.Formatting.EndOfFileTokenFormattingRule.Name, LanguageNames.CSharp, 
                        after: new string[] { StarkPlatform.Compiler.Stark.Formatting.ElasticTriviaFormattingRule.Name })),

                new Lazy<StarkPlatform.Compiler.Formatting.Rules.IFormattingRule, OrderableLanguageMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.IndentBlockFormattingRule(),
                    new OrderableLanguageMetadata(StarkPlatform.Compiler.Stark.Formatting.IndentBlockFormattingRule.Name, LanguageNames.CSharp,
                        after: new string[] { StarkPlatform.Compiler.Stark.Formatting.StructuredTriviaFormattingRule.Name })),

                new Lazy<StarkPlatform.Compiler.Formatting.Rules.IFormattingRule, OrderableLanguageMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.QueryExpressionFormattingRule(),
                    new OrderableLanguageMetadata(StarkPlatform.Compiler.Stark.Formatting.QueryExpressionFormattingRule.Name, LanguageNames.CSharp, 
                        after: new string[] { StarkPlatform.Compiler.Stark.Formatting.AnchorIndentationFormattingRule.Name })),

                new Lazy<StarkPlatform.Compiler.Formatting.Rules.IFormattingRule, OrderableLanguageMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.StructuredTriviaFormattingRule(),
                    new OrderableLanguageMetadata(StarkPlatform.Compiler.Stark.Formatting.StructuredTriviaFormattingRule.Name, LanguageNames.CSharp, 
                        after: new string[] { StarkPlatform.Compiler.Stark.Formatting.EndOfFileTokenFormattingRule.Name })),

                new Lazy<StarkPlatform.Compiler.Formatting.Rules.IFormattingRule, OrderableLanguageMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.SuppressFormattingRule(),
                    new OrderableLanguageMetadata(StarkPlatform.Compiler.Stark.Formatting.SuppressFormattingRule.Name, LanguageNames.CSharp, 
                        after: new string[] { StarkPlatform.Compiler.Stark.Formatting.IndentBlockFormattingRule.Name })),

                new Lazy<StarkPlatform.Compiler.Formatting.Rules.IFormattingRule, OrderableLanguageMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Formatting.TokenBasedFormattingRule(),
                    new OrderableLanguageMetadata(StarkPlatform.Compiler.Stark.Formatting.TokenBasedFormattingRule.Name, LanguageNames.CSharp, 
                        after: new string[] { StarkPlatform.Compiler.Stark.Formatting.QueryExpressionFormattingRule.Name })),

                // formatting options
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.MethodDeclarationNameParenthesis),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.MethodDeclarationParenthesisArgumentList),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.MethodDeclarationEmptyArgument),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.MethodCallNameParenthesis),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.MethodCallArgumentList),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.MethodCallEmptyArgument),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OtherAfterControlFlowKeyword),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OtherBetweenParenthesisExpression),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OtherParenthesisTypeCast),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OtherParenControlFlow),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OtherParenAfterCast),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OtherSpacesDeclarationIgnore),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.SquareBracesBefore),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.SquareBracesEmpty),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.SquareBracesAndValue),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.DelimitersAfterColonInTypeDeclaration),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.DelimitersAfterCommaInParameterArgument),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.DelimitersAfterDotMemberAccessQualifiedName),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.DelimitersAfterSemiColonInForStatement),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.DelimitersBeforeColonInTypeDeclaration),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.DelimitersBeforeCommaInParameterArgument),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.DelimitersBeforeDotMemberAccessQualifiedName),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.DelimitersBeforeSemiColonInForStatement),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.SpacingAroundBinaryOperator),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OpenCloseBracesIndent),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.IndentBlock),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.IndentSwitchSection),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.IndentSwitchCaseSection),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.LabelPositioning),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.LeaveStatementMethodDeclarationSameLine),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OpenBracesInNewLineForTypes),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OpenBracesInNewLineForMethods),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OpenBracesInNewLineForAnonymousMethods),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OpenBracesInNewLineForControl),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OpenBracesInNewLineForAnonymousType),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OpenBracesInNewLineForObjectInitializers),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.OpenBracesInNewLineForLambda),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.NewLineForElse),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.NewLineForCatch),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.NewLineForFinally),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.NewLineForMembersInObjectInit),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.NewLineForMembersInAnonymousTypes),
                new Lazy<Options.IOption>(
                    () => StarkPlatform.Compiler.Stark.Formatting.CSharpFormattingOptions.NewLineForClausesInQuery),

                // Recommendation service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new CSharpRecommendationService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.Recommendations.IRecommendationService), ServiceLayer.Default)),

                // Command line arguments service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new CSharpCommandLineArgumentsFactoryService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.LanguageServices.ICommandLineArgumentsFactoryService), ServiceLayer.Default)),

                // Compilation factory service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new CSharpCompilationFactoryService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.LanguageServices.ICompilationFactoryService), ServiceLayer.Default)),

                // Project File Loader service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new CSharpProjectFileLoaderService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.Host.ProjectFileLoader.IProjectFileLoaderLanguageService), ServiceLayer.Default)),

                // Semantic Facts service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new CSharpSemanticFactsService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.LanguageServices.ISemanticFactsService), ServiceLayer.Default)),

                // Symbol Declaration service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new CSharpSymbolDeclarationService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.LanguageServices.ISymbolDeclarationService), ServiceLayer.Default)),

                // Syntax Facts service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new CSharpSyntaxFactsService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.LanguageServices.ISyntaxFactsService), ServiceLayer.Default)),

                // SyntaxTree Factory service
                new Lazy<ILanguageServiceFactory, LanguageServiceMetadata>(
                    () => new CSharpSyntaxTreeFactoryServiceFactory(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.LanguageServices.ISyntaxTreeFactoryService), ServiceLayer.Default)),

                // SyntaxVersion service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new CSharpSyntaxVersionService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.LanguageServices.ISyntaxVersionLanguageService), ServiceLayer.Default)),

                // Type Inference service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new CSharpTypeInferenceService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.LanguageServices.ITypeInferenceService), ServiceLayer.Default)),

                // Rename conflicts service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Rename.CSharpRenameConflictLanguageService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.Rename.IRenameRewriterLanguageService), ServiceLayer.Default)),

                // Simplification service
                new Lazy<ILanguageService, LanguageServiceMetadata>(
                    () => new StarkPlatform.Compiler.Stark.Simplification.CSharpSimplificationService(),
                    new LanguageServiceMetadata(LanguageNames.CSharp, typeof(StarkPlatform.Compiler.Simplification.ISimplificationService), ServiceLayer.Default))
            };
        }
    }
#endif
}
