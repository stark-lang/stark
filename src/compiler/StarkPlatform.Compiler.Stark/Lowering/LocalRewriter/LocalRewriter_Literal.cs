// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.PooledObjects;
using StarkPlatform.Compiler.Text;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    internal partial class LocalRewriter
    {
        public override BoundNode VisitLiteral(BoundLiteral node)
        {
            return MakeLiteral(node.Syntax, node.ConstantValue, node.Type, oldNodeOpt: node);
        }

        private BoundExpression MakeLiteral(SyntaxNode syntax, ConstantValue constantValue, TypeSymbol type, BoundLiteral oldNodeOpt = null)
        {
            Debug.Assert(constantValue != null);

            if (oldNodeOpt != null)
            {
                return oldNodeOpt.Update(constantValue, type);
            }
            else
            {
                return new BoundLiteral(syntax, constantValue, type, hasErrors: constantValue.IsBad);
            }
        }
    }
}
