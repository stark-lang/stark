// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.GenerateFromMembers;
using StarkPlatform.CodeAnalysis.GenerateMember.GenerateDefaultConstructors;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.LanguageServices;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.GenerateMember.GenerateDefaultConstructors
{
    [ExportLanguageService(typeof(IGenerateDefaultConstructorsService), LanguageNames.Stark), Shared]
    internal class CSharpGenerateDefaultConstructorsService : AbstractGenerateDefaultConstructorsService<CSharpGenerateDefaultConstructorsService>
    {
        protected override bool TryInitializeState(
            SemanticDocument semanticDocument, TextSpan textSpan, CancellationToken cancellationToken,
            out INamedTypeSymbol classType)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Offer the feature if we're on the header for the class/struct, or if we're on the 
            // first base-type of a class.

            var syntaxFacts = semanticDocument.Document.GetLanguageService<ISyntaxFactsService>();
            if (syntaxFacts.IsOnTypeHeader(semanticDocument.Root, textSpan.Start))
            {
                classType = AbstractGenerateFromMembersCodeRefactoringProvider.GetEnclosingNamedType(
                    semanticDocument.SemanticModel, semanticDocument.Root, textSpan.Start, cancellationToken);
                return classType?.TypeKind == TypeKind.Class;
            }

            var syntaxTree = semanticDocument.SyntaxTree;
            var node = semanticDocument.Root.FindToken(textSpan.Start).GetAncestor<TypeSyntax>();
            if (node != null)
            {
                if (node.Parent is BaseTypeSyntax && node.Parent.IsParentKind(SyntaxKind.ImplementList))
                {
                    var baseList = (ImplementListSyntax)node.Parent.Parent;
                    if (baseList.Types.Count > 0 &&
                        baseList.Types[0].Type == node &&
                        baseList.IsParentKind(SyntaxKind.ClassDeclaration))
                    {
                        var semanticModel = semanticDocument.SemanticModel;
                        classType = semanticModel.GetDeclaredSymbol(baseList.Parent, cancellationToken) as INamedTypeSymbol;
                        return classType != null;
                    }
                }
            }

            classType = null;
            return false;
        }
    }
}
