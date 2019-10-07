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

            if (constantValue.IsDateTime)
            {
                // C# does not support DateTime constants but VB does; we might have obtained a 
                // DateTime constant by calling a method with an optional parameter with a DateTime
                // for its default value.
                Debug.Assert((object)type != null);
                Debug.Assert(type.SpecialType == SpecialType.System_DateTime);
                return MakeDateTimeLiteral(syntax, constantValue);
            }
            else if (oldNodeOpt != null)
            {
                return oldNodeOpt.Update(constantValue, type);
            }
            else
            {
                return new BoundLiteral(syntax, constantValue, type, hasErrors: constantValue.IsBad);
            }
        }
       
        private BoundExpression MakeDateTimeLiteral(SyntaxNode syntax, ConstantValue constantValue)
        {
            Debug.Assert(constantValue != null);
            Debug.Assert(constantValue.IsDateTime);

            var arguments = new ArrayBuilder<BoundExpression>();
            arguments.Add(new BoundLiteral(syntax, ConstantValue.Create(constantValue.DateTimeValue.Ticks), _compilation.GetSpecialType(SpecialType.System_Int64)));

            var ctor = (MethodSymbol)_compilation.Assembly.GetSpecialTypeMember(SpecialMember.System_DateTime__CtorInt64);
            Debug.Assert((object)ctor != null);
            Debug.Assert(ctor.ContainingType.SpecialType == SpecialType.System_DateTime);

            // This is not a constant from C#'s perspective, so do not mark it as one.
            return new BoundObjectCreationExpression(
                syntax, ctor, arguments.ToImmutableAndFree(),
                default(ImmutableArray<string>), default(ImmutableArray<RefKind>), false, default(ImmutableArray<int>),
                ConstantValue.NotAvailable, null, null, ctor.ContainingType);
        }
    }
}
