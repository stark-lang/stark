// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal sealed class SimpleLocalScopeBinder : LocalScopeBinder
    {
        private readonly ImmutableArray<LocalSymbol> _locals;

        public SimpleLocalScopeBinder(ImmutableArray<LocalSymbol> locals, Binder next) :
            base(next)
        {
            _locals = locals;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            return _locals;
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
