// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;
using System.Diagnostics;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal sealed class ForLoopBinder : LoopBinder
    {
        private readonly SyntaxNode _syntax;

        public ForLoopBinder(Binder enclosing, SyntaxNode syntax)
            : base(enclosing)
        {
            Debug.Assert(syntax != null);
            _syntax = syntax;
        }

        override protected ImmutableArray<LocalSymbol> BuildLocals()
        {
            var locals = ArrayBuilder<LocalSymbol>.GetInstance();

            //// Declaration and Initializers are mutually exclusive.
            //if (_syntax.Declaration != null)
            //{
            //    {
            //        var localSymbol = MakeLocal(_syntax.Declaration, LocalDeclarationKind.RegularVariable);
            //        locals.Add(localSymbol);

            //        // also gather expression-declared variables from the bracketed argument lists and the initializers
            //        ExpressionVariableFinder.FindExpressionVariables(this, locals, _syntax.Declaration);
            //    }
            //}
            //else
            //{
            //    ExpressionVariableFinder.FindExpressionVariables(this, locals, _syntax.Initializers);
            //}

            return locals.ToImmutableAndFree();
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            if (_syntax == scopeDesignator)
            {
                return this.Locals;
            }

            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override SyntaxNode ScopeDesignator
        {
            get
            {
                return _syntax;
            }
        }
    }
}
