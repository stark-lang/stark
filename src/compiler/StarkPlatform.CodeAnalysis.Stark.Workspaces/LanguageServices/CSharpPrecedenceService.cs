// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.LanguageServices;

namespace StarkPlatform.CodeAnalysis.Stark
{
    [ExportLanguageService(typeof(IPrecedenceService), LanguageNames.Stark), Shared]
    internal class CSharpPrecedenceService : AbstractPrecedenceService<ExpressionSyntax, OperatorPrecedence>
    {
        public static readonly CSharpPrecedenceService Instance = new CSharpPrecedenceService();

        public override OperatorPrecedence GetOperatorPrecedence(ExpressionSyntax expression)
            => expression.GetOperatorPrecedence();

        public override PrecedenceKind GetPrecedenceKind(OperatorPrecedence precedence)
        {
            switch (precedence)
            {
                case OperatorPrecedence.NullCoalescing: return PrecedenceKind.Coalesce;
                case OperatorPrecedence.ConditionalOr:
                case OperatorPrecedence.ConditionalAnd: return PrecedenceKind.Logical;
                case OperatorPrecedence.LogicalOr:
                case OperatorPrecedence.LogicalXor:
                case OperatorPrecedence.LogicalAnd: return PrecedenceKind.Bitwise;
                case OperatorPrecedence.Equality: return PrecedenceKind.Equality;
                case OperatorPrecedence.RelationalAndTypeTesting: return PrecedenceKind.Relational;
                case OperatorPrecedence.Shift: return PrecedenceKind.Shift;
                case OperatorPrecedence.Additive:
                case OperatorPrecedence.Multiplicative: return PrecedenceKind.Arithmetic;
                default: return PrecedenceKind.Other;
            }
        }
    }
}
