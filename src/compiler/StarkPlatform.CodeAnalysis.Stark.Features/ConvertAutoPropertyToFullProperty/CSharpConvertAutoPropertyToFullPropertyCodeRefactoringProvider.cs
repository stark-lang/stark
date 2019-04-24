// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StarkPlatform.CodeAnalysis.CodeRefactorings;
using StarkPlatform.CodeAnalysis.CodeStyle;
using StarkPlatform.CodeAnalysis.ConvertAutoPropertyToFullProperty;
using StarkPlatform.CodeAnalysis.Stark.CodeStyle;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Diagnostics.Analyzers.NamingStyles;
using StarkPlatform.CodeAnalysis.Editing;
using StarkPlatform.CodeAnalysis.Formatting;
using StarkPlatform.CodeAnalysis.Options;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.Shared.Naming;
using StarkPlatform.CodeAnalysis.Shared.Utilities;
using static StarkPlatform.CodeAnalysis.Diagnostics.Analyzers.NamingStyles.SymbolSpecification;

namespace StarkPlatform.CodeAnalysis.Stark.ConvertAutoPropertyToFullProperty
{
    [ExportCodeRefactoringProvider(LanguageNames.Stark, Name = nameof(CSharpConvertAutoPropertyToFullPropertyCodeRefactoringProvider)), Shared]
    internal class CSharpConvertAutoPropertyToFullPropertyCodeRefactoringProvider : AbstractConvertAutoPropertyToFullPropertyCodeRefactoringProvider
    {
        internal override SyntaxNode GetProperty(SyntaxToken token)
        {
            var containingProperty = token.Parent.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            if (containingProperty == null)
            {
                return null;
            }

            if (!(containingProperty.Parent is TypeDeclarationSyntax))
            {
                return null;
            }

            var start = containingProperty.AttributeLists.Count > 0
                ? containingProperty.AttributeLists.Last().GetLastToken().GetNextToken().SpanStart
                : containingProperty.SpanStart;

            // Offer this refactoring anywhere in the signature of the property
            var position = token.SpanStart;
            if (position < start || position > containingProperty.Identifier.Span.End)
            {
                return null;
            }

            return containingProperty;
        }

        internal override async Task<string> GetFieldNameAsync(Document document, IPropertySymbol propertySymbol, CancellationToken cancellationToken)
        {
            var rules = await document.GetNamingRulesAsync(FallbackNamingRules.RefactoringMatchLookupRules, cancellationToken).ConfigureAwait(false);
            return GenerateFieldName(propertySymbol, rules);
        }

        private string GenerateFieldName(IPropertySymbol property, ImmutableArray<NamingRule> rules)
        {
            var propertyName = property.Name;
            var fieldName = "";
            foreach (var rule in rules)
            {
                if (rule.SymbolSpecification.AppliesTo(
                    new SymbolKindOrTypeKind(SymbolKind.Field),
                    property.IsStatic ? DeclarationModifiers.Static : DeclarationModifiers.None,
                    Accessibility.Private))
                {
                    fieldName = rule.NamingStyle.MakeCompliant(propertyName).First();
                    break;
                }
            }

            return NameGenerator.GenerateUniqueName(fieldName, n => !property.ContainingType.GetMembers(n).Any());
        }

        internal override (SyntaxNode newGetAccessor, SyntaxNode newSetAccessor) GetNewAccessors(
            DocumentOptionSet options, SyntaxNode property,
            string fieldName, SyntaxGenerator generator)
        {
            // C# might have trivia with the accessors that needs to be preserved.  
            // so we will update the existing accessors instead of creating new ones
            var accessorListSyntax = ((PropertyDeclarationSyntax)property).AccessorList;
            var existingAccessors = GetExistingAccessors(accessorListSyntax);

            var getAccessorStatement = generator.ReturnStatement(generator.IdentifierName(fieldName));
            var newGetter = GetUpdatedAccessor(
                options, existingAccessors.getAccessor,
                getAccessorStatement, generator);

            SyntaxNode newSetter = null;
            if (existingAccessors.setAccessor != null)
            {
                var setAccessorStatement = generator.ExpressionStatement(generator.AssignmentStatement(
                    generator.IdentifierName(fieldName),
                    generator.IdentifierName("value")));
                newSetter = GetUpdatedAccessor(
                    options, existingAccessors.setAccessor,
                    setAccessorStatement, generator);
            }

            return (newGetAccessor: newGetter, newSetAccessor: newSetter);
        }

        private (AccessorDeclarationSyntax getAccessor, AccessorDeclarationSyntax setAccessor)
            GetExistingAccessors(AccessorListSyntax accessorListSyntax)
            => (accessorListSyntax.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)),
                accessorListSyntax.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)));

        private SyntaxNode GetUpdatedAccessor(DocumentOptionSet options,
            SyntaxNode accessor, SyntaxNode statement, SyntaxGenerator generator)
        {
            var newAccessor = AddStatement(accessor, statement);
            var accessorDeclarationSyntax = (AccessorDeclarationSyntax)newAccessor;

            var preference = GetAccessorExpressionBodyPreference(options);
            if (preference == ExpressionBodyPreference.Never)
            {
                return accessorDeclarationSyntax.WithEosToken(default);
            }

            if (!accessorDeclarationSyntax.Body.TryConvertToArrowExpressionBody(
                    accessorDeclarationSyntax.Kind(), accessor.SyntaxTree.Options, preference,
                    out var arrowExpression, out var semicolonToken))
            {
                return accessorDeclarationSyntax.WithEosToken(default);
            };

            return accessorDeclarationSyntax
                .WithExpressionBody(arrowExpression)
                .WithBody(null)
                .WithEosToken(accessorDeclarationSyntax.EosToken)
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        internal SyntaxNode AddStatement(SyntaxNode accessor, SyntaxNode statement)
        {
            var blockSyntax = SyntaxFactory.Block(
                SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed),
                new SyntaxList<StatementSyntax>((StatementSyntax)statement),
                SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                    .WithTrailingTrivia(((AccessorDeclarationSyntax)accessor).EosToken.TrailingTrivia));

            return ((AccessorDeclarationSyntax)accessor).WithBody(blockSyntax);
        }

        internal override SyntaxNode ConvertPropertyToExpressionBodyIfDesired(
            DocumentOptionSet options, SyntaxNode property)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)property;

            var preference = GetPropertyExpressionBodyPreference(options);
            if (preference == ExpressionBodyPreference.Never)
            {
                return propertyDeclaration.WithEosToken(default);
            }

            // if there is a get accessor only, we can move the expression body to the property
            if (propertyDeclaration.AccessorList?.Accessors.Count == 1 &&
                propertyDeclaration.AccessorList.Accessors[0].Kind() == SyntaxKind.GetAccessorDeclaration)
            {
                var getAccessor = propertyDeclaration.AccessorList.Accessors[0];
                if (getAccessor.ExpressionBody != null)
                {
                    return propertyDeclaration.WithExpressionBody(getAccessor.ExpressionBody)
                        .WithEosToken(getAccessor.EosToken)
                        .WithAccessorList(null);
                }
            }

            return propertyDeclaration.WithEosToken(default);
        }

        internal ExpressionBodyPreference GetAccessorExpressionBodyPreference(DocumentOptionSet options)
            => options.GetOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors).Value;

        internal ExpressionBodyPreference GetPropertyExpressionBodyPreference(DocumentOptionSet options)
            => options.GetOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties).Value;


        internal override SyntaxNode GetTypeBlock(SyntaxNode syntaxNode)
            => syntaxNode;

        internal override SyntaxNode GetInitializerValue(SyntaxNode property)
            => ((PropertyDeclarationSyntax)property).Initializer?.Value;

        internal override SyntaxNode GetPropertyWithoutInitializer(SyntaxNode property)
            => ((PropertyDeclarationSyntax)property).WithInitializer(null);
    }
}
