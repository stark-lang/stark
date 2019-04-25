// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.CodeStyle;
using StarkPlatform.Compiler.Stark.Extensions;
using StarkPlatform.Compiler.Stark.Simplification;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Diagnostics;
using StarkPlatform.Compiler.Options;
using StarkPlatform.Compiler.Text;
using StarkPlatform.Compiler.UseInferredMemberName;

namespace StarkPlatform.Compiler.Stark.UseInferredMemberName
{
    [DiagnosticAnalyzer(LanguageNames.Stark)]
    internal sealed class CSharpUseInferredMemberNameDiagnosticAnalyzer : AbstractUseInferredMemberNameDiagnosticAnalyzer
    {
        protected override void InitializeWorker(AnalysisContext context)
            => context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.NameColon, SyntaxKind.NameEquals);

        override protected void LanguageSpecificAnalyzeSyntax(SyntaxNodeAnalysisContext context, SyntaxTree syntaxTree, OptionSet optionSet)
        {
            var parseOptions = (CSharpParseOptions)syntaxTree.Options;
            switch (context.Node.Kind())
            {
                case SyntaxKind.NameColon:
                    ReportDiagnosticsIfNeeded((NameColonSyntax)context.Node, context, optionSet, syntaxTree);
                    break;
                case SyntaxKind.NameEquals:
                    ReportDiagnosticsIfNeeded((NameEqualsSyntax)context.Node, context, optionSet, syntaxTree);
                    break;
            }
        }

        private void ReportDiagnosticsIfNeeded(NameColonSyntax nameColon, SyntaxNodeAnalysisContext context, OptionSet optionSet, SyntaxTree syntaxTree)
        {
            if (!nameColon.IsParentKind(SyntaxKind.Argument))
            {
                return;
            }

            var argument = (ArgumentSyntax)nameColon.Parent;
            var parseOptions = (CSharpParseOptions)syntaxTree.Options;
            if (!optionSet.GetOption(CodeStyleOptions.PreferInferredTupleNames, context.Compilation.Language).Value ||
                !CSharpInferredMemberNameReducer.CanSimplifyTupleElementName(argument, parseOptions))
            {
                return;
            }

            // Create a normal diagnostic
            context.ReportDiagnostic(
                DiagnosticHelper.Create(
                    Descriptor,
                    nameColon.GetLocation(),
                    optionSet.GetOption(CodeStyleOptions.PreferInferredTupleNames, context.Compilation.Language).Notification.Severity,
                    additionalLocations: null,
                    properties: null));

            // Also fade out the part of the name-colon syntax
            var fadeSpan = TextSpan.FromBounds(nameColon.Name.SpanStart, nameColon.ColonToken.Span.End);
            context.ReportDiagnostic(
                Diagnostic.Create(
                    UnnecessaryWithoutSuggestionDescriptor,
                    syntaxTree.GetLocation(fadeSpan)));
        }

        private void ReportDiagnosticsIfNeeded(NameEqualsSyntax nameEquals, SyntaxNodeAnalysisContext context, OptionSet optionSet, SyntaxTree syntaxTree)
        {
            if (!nameEquals.IsParentKind(SyntaxKind.AnonymousObjectMemberDeclarator))
            {
                return;
            }

            var anonCtor = (AnonymousObjectMemberDeclaratorSyntax)nameEquals.Parent;
            if (!optionSet.GetOption(CodeStyleOptions.PreferInferredAnonymousTypeMemberNames, context.Compilation.Language).Value ||
                !CSharpInferredMemberNameReducer.CanSimplifyAnonymousTypeMemberName(anonCtor))
            {
                return;
            }

            // Create a normal diagnostic
            context.ReportDiagnostic(
                DiagnosticHelper.Create(
                    Descriptor,
                    nameEquals.GetLocation(),
                    optionSet.GetOption(CodeStyleOptions.PreferInferredAnonymousTypeMemberNames, context.Compilation.Language).Notification.Severity,
                    additionalLocations: null,
                    properties: null));

            // Also fade out the part of the name-equals syntax
            var fadeSpan = TextSpan.FromBounds(nameEquals.Name.SpanStart, nameEquals.EqualsToken.Span.End);
            context.ReportDiagnostic(
                Diagnostic.Create(
                    UnnecessaryWithoutSuggestionDescriptor,
                    syntaxTree.GetLocation(fadeSpan)));
        }
    }
}
