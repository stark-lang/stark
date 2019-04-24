// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitThrowStatement(BoundThrowStatement node)
        {
            var result = (BoundStatement)base.VisitThrowStatement(node);
            if (this.Instrument && !node.WasCompilerGenerated)
            {
                result = _instrumenter.InstrumentThrowStatement(node, result);
            }

            return result;
        }
    }
}
