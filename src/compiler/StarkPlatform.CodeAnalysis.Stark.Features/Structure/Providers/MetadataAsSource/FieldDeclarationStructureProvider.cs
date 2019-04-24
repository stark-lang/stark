// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.PooledObjects;

namespace StarkPlatform.CodeAnalysis.Stark.Structure.MetadataAsSource
{
    internal class MetadataFieldDeclarationStructureProvider : AbstractMetadataAsSourceStructureProvider<FieldDeclarationSyntax>
    {
        protected override SyntaxToken GetEndToken(FieldDeclarationSyntax node)
        {
            return node.Modifiers.Count > 0
                    ? node.Modifiers.First()
                    : node.Declaration.GetFirstToken();
        }
    }
}
