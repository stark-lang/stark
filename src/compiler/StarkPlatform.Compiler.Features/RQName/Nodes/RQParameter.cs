﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Features.RQName.SimpleTree;

namespace StarkPlatform.Compiler.Features.RQName.Nodes
{
    internal abstract class RQParameter
    {
        public readonly RQType Type;
        public RQParameter(RQType type)
        {
            System.Diagnostics.Debug.Assert(type != null);
            this.Type = type;
        }

        public SimpleTreeNode ToSimpleTree()
        {
            return new SimpleGroupNode(RQNameStrings.Param, CreateSimpleTreeForType());
        }

        public abstract SimpleTreeNode CreateSimpleTreeForType();
    }
}
