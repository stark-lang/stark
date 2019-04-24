// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.ImplementAbstractClass;
using StarkPlatform.CodeAnalysis.Shared.Extensions;

namespace StarkPlatform.CodeAnalysis.Stark.ImplementAbstractClass
{
    [ExportLanguageService(typeof(IImplementAbstractClassService), LanguageNames.Stark), Shared]
    internal class CSharpImplementAbstractClassService :
        AbstractImplementAbstractClassService<ClassDeclarationSyntax>
    {
        protected override bool TryInitializeState(
            Document document, SemanticModel model, ClassDeclarationSyntax classNode, CancellationToken cancellationToken,
            out INamedTypeSymbol classType, out INamedTypeSymbol abstractClassType)
        {
            classType = model.GetDeclaredSymbol(classNode);
            abstractClassType = classType?.BaseType;

            return classType != null && abstractClassType != null && abstractClassType.IsAbstractClass();
        }
    }
}
