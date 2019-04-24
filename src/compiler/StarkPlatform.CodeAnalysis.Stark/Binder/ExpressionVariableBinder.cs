// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;
using System.Collections.Generic;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal sealed class ExpressionVariableBinder : LocalScopeBinder
    {
        internal override SyntaxNode ScopeDesignator { get; }

        internal ExpressionVariableBinder(SyntaxNode scopeDesignator, Binder next) : base(next)
        {
            this.ScopeDesignator = scopeDesignator;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            var builder = ArrayBuilder<LocalSymbol>.GetInstance();
            ExpressionVariableFinder.FindExpressionVariables(this, builder, (CSharpSyntaxNode)ScopeDesignator,
                                                             GetBinder((CSharpSyntaxNode)ScopeDesignator));
            return builder.ToImmutableAndFree();
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            if (ScopeDesignator == scopeDesignator)
            {
                return this.Locals;
            }

            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
