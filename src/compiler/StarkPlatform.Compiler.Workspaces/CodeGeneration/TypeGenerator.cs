// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Shared.Extensions;

namespace StarkPlatform.Compiler.CodeGeneration
{
    internal class TypeGenerator : ITypeGenerator
    {
        public TypeGenerator()
        {
        }

        public ITypeSymbol CreateArrayTypeSymbol(ITypeSymbol elementType)
        {
            return CodeGenerationSymbolFactory.CreateArrayTypeSymbol(elementType);
        }

        public ITypeSymbol CreatePointerTypeSymbol(ITypeSymbol pointedAtType)
        {
            return CodeGenerationSymbolFactory.CreatePointerTypeSymbol(pointedAtType);
        }

        public ITypeSymbol Construct(INamedTypeSymbol namedType, ITypeSymbol[] typeArguments)
        {
            return namedType.ToCodeGenerationSymbol().Construct(typeArguments);
        }
    }
}
