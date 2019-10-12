// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Features.RQName.SimpleTree;

namespace StarkPlatform.Compiler.Features.RQName.Nodes
{
    internal class RQArrayType : RQArrayOrPointerType
    {
        public RQArrayType(RQType elementType)
            : base(elementType)
        {
        }

        public override SimpleTreeNode ToSimpleTree()
        {
            return new SimpleGroupNode(RQNameStrings.Array,ElementType.ToSimpleTree());
        }
    }
}
