// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Host.Mef;
using StarkPlatform.Compiler.ImplementAbstractClass;
using StarkPlatform.Compiler.Shared.Extensions;

namespace StarkPlatform.Compiler.Stark.ImplementAbstractClass
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
