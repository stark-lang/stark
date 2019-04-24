// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark
{
    /// <summary>
    /// This portion of the binder converts an AwaitExpressionSyntax into a BoundExpression
    /// </summary>
    internal partial class Binder
    {
        private BoundExpression BindTry(TryExpressionSyntax node, DiagnosticBag diagnostics)
        {
            BoundExpression expression = BindValue(node.Expression, diagnostics, BindValueKind.RValue);

            if (expression.Kind == BoundKind.Call)
            {
                var call = (BoundCall)expression;
                var methodCalled = call.Method;

                if (!methodCalled.HasThrows)
                {
                    diagnostics.Add(ErrorCode.ERR_ThrowsExpected, node.Expression.GetLocation());
                    return expression;
                }

                return new BoundTryExpression(node, expression, expression.Type);
            }
            else
            {
                diagnostics.Add(ErrorCode.ERR_TryNotSupportedByThisExpression, node.Expression.GetLocation());
            }

            return expression;
        }

        public static void VerifyMethodThrowsWithBase(MethodSymbol method, MethodSymbol baseMethod, DiagnosticBag diagnostics)
        {
            if (method.HasThrows && baseMethod != null)
            {
                var syntax = (MethodDeclarationSyntax)method.GetNonNullSyntaxNode();
                if (!baseMethod.HasThrows)
                {
                    diagnostics.Add(ErrorCode.ERR_MissingThrowsOnBaseMethod, syntax.Identifier.GetLocation());
                }
                else
                {
                    // TODO: Should we use it?
                    HashSet<DiagnosticInfo> unused = null;
                    int throwTypeIndex = 0;
                    foreach (var throwType in method.ThrowsList)
                    {
                        bool isDeclaredByBase = false;
                        foreach (var baseThrowType in baseMethod.ThrowsList)
                        {
                            if (throwType.IsEqualToOrDerivedFrom(baseThrowType, TypeCompareKind.ConsiderEverything, ref unused))
                            {
                                isDeclaredByBase = true;
                                break;
                            }
                        }

                        if (!isDeclaredByBase)
                        {
                            diagnostics.Add(ErrorCode.ERR_ThrowsNotDeclaredByBaseMethod, syntax.ThrowsList.Types[throwTypeIndex].GetLocation(), throwType.ToDisplayString());
                        }

                        throwTypeIndex++;
                    }
                }
            }
        }
    }
}
