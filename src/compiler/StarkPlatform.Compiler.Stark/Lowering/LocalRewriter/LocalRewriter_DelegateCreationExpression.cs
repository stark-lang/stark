// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Stark
{
    internal partial class LocalRewriter
    {
        public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            if (node.Argument.Kind == BoundKind.MethodGroup)
            {
                var mg = (BoundMethodGroup)node.Argument;
                var method = node.MethodOpt;
                var oldSyntax = _factory.Syntax;
                _factory.Syntax = (mg.ReceiverOpt ?? mg).Syntax;
                var receiver = (method.IsStatic && !node.IsExtensionMethod) ? _factory.Type(method.ContainingType) : VisitExpression(mg.ReceiverOpt);
                _factory.Syntax = oldSyntax;
                return node.Update(receiver, method, node.IsExtensionMethod, node.Type);
            }

            return base.VisitDelegateCreationExpression(node);
        }
    }
}
