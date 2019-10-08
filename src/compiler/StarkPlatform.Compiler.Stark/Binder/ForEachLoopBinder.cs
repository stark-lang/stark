// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.PooledObjects;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    /// <summary>
    /// A loop binder that (1) knows how to bind foreach loops and (2) has the foreach iteration variable in scope.
    /// </summary>
    /// <remarks>
    /// This binder produces BoundForEachStatements.  The lowering described in the spec is performed in ControlFlowRewriter.
    /// </remarks>
    internal sealed class ForEachLoopBinder : LoopBinder
    {
        private const string IteratorMethodName = WellKnownMemberNames.IteratorMethodName;
        private const string IterateBeginMethodName = "iterate_begin";
        private const string IterateNextMethodName = "iterate_next";
        private const string IterateItemMethodName = "iterate_item";
        private const string IterateEndMethodName = "iterate_end";

        private const string GetAsyncEnumeratorMethodName = WellKnownMemberNames.GetAsyncEnumeratorMethodName;
        private const string MoveNextAsyncMethodName = WellKnownMemberNames.MoveNextAsyncMethodName;

        private readonly ForStatementSyntax _syntax;

        private bool IsAsync
            => _syntax.AwaitKeyword != default;

        public ForEachLoopBinder(Binder enclosing, ForStatementSyntax syntax)
            : base(enclosing)
        {
            Debug.Assert(syntax != null);
            _syntax = syntax;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            switch (_syntax.Kind())
            {
                case SyntaxKind.ForStatement:
                    {
                        var syntax = (ForStatementSyntax)_syntax;
                        var locals = ArrayBuilder<LocalSymbol>.GetInstance();
                        CollectLocalsFromDeconstruction(
                            syntax.Variable,
                            LocalDeclarationKind.ForEachIterationVariable,
                            locals,
                            syntax);
                        return locals.ToImmutableAndFree();
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(_syntax.Kind());
            }
        }

        internal void CollectLocalsFromDeconstruction(
            ExpressionSyntax declaration,
            LocalDeclarationKind kind,
            ArrayBuilder<LocalSymbol> locals,
            SyntaxNode deconstructionStatement,
            Binder enclosingBinderOpt = null)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.TupleExpression:
                    {
                        var tuple = (TupleExpressionSyntax)declaration;
                        foreach (var arg in tuple.Arguments)
                        {
                            CollectLocalsFromDeconstruction(arg.Expression, kind, locals, deconstructionStatement, enclosingBinderOpt);
                        }
                        break;
                    }
                case SyntaxKind.DeclarationExpression:
                    {
                        var declarationExpression = (DeclarationExpressionSyntax)declaration;
                        CollectLocalsFromDeconstruction(
                            declarationExpression.Designation, declarationExpression.Type,
                            kind, locals, deconstructionStatement, enclosingBinderOpt);

                        break;
                    }
                case SyntaxKind.IdentifierName:
                    var syntax = (IdentifierNameSyntax)declaration;
                    var collection = ((ForStatementSyntax)deconstructionStatement).Expression;
                    var symbol = SourceLocalSymbol.MakeForeachLocal(
                        (MethodSymbol)this.ContainingMemberOrLambda,
                        this,
                        null,
                        syntax.Identifier,
                        collection);
                    locals.Add(symbol);
                    break;
                default:
                    // In broken code, we can have an arbitrary expression here. Collect its expression variables.
                    ExpressionVariableFinder.FindExpressionVariables(this, locals, declaration);
                    break;
            }
        }

        internal void CollectLocalsFromDeconstruction(
            VariableDesignationSyntax designation,
            TypeSyntax closestTypeSyntax,
            LocalDeclarationKind kind,
            ArrayBuilder<LocalSymbol> locals,
            SyntaxNode deconstructionStatement,
            Binder enclosingBinderOpt)
        {
            switch (designation.Kind())
            {
                case SyntaxKind.SingleVariableDesignation:
                    {
                        var single = (SingleVariableDesignationSyntax)designation;
                        SourceLocalSymbol localSymbol = SourceLocalSymbol.MakeDeconstructionLocal(
                                                                    this.ContainingMemberOrLambda,
                                                                    this,
                                                                    enclosingBinderOpt ?? this,
                                                                    closestTypeSyntax,
                                                                    single.Identifier,
                                                                    kind,
                                                                    deconstructionStatement);
                        locals.Add(localSymbol);
                        break;
                    }
                case SyntaxKind.ParenthesizedVariableDesignation:
                    {
                        var tuple = (ParenthesizedVariableDesignationSyntax)designation;
                        foreach (var d in tuple.Variables)
                        {
                            CollectLocalsFromDeconstruction(d, closestTypeSyntax, kind, locals, deconstructionStatement, enclosingBinderOpt);
                        }
                        break;
                    }
                case SyntaxKind.DiscardDesignation:
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(designation.Kind());
            }
        }

        /// <summary>
        /// Bind the ForEachVariableStatementSyntax at the root of this binder.
        /// </summary>
        internal override BoundStatement BindForEachParts(DiagnosticBag diagnostics, Binder originalBinder)
        {
            BoundForEachStatement result = BindForEachPartsWorker(diagnostics, originalBinder);
            return result;
        }

        /// <summary>
        /// Like BindForEachParts, but only bind the deconstruction part of the foreach, for purpose of inferring the types of the declared locals.
        /// </summary>
        internal override BoundStatement BindForEachDeconstruction(DiagnosticBag diagnostics, Binder originalBinder)
        {
            // Use the right binder to avoid seeing iteration variable
            BoundExpression collectionExpr = originalBinder.GetBinder(_syntax.Expression).BindValue(_syntax.Expression, diagnostics, BindValueKind.RValue);

            ForEachEnumeratorInfo.Builder builder = new ForEachEnumeratorInfo.Builder();
            TypeSymbolWithAnnotations inferredType;
            bool hasErrors = !GetEnumeratorInfoAndInferCollectionElementType(ref builder, ref collectionExpr, diagnostics, out inferredType);

            ExpressionSyntax variables = ((ForStatementSyntax)_syntax).Variable;

            // Tracking narrowest safe-to-escape scope by default, the proper val escape will be set when doing full binding of the foreach statement
            var valuePlaceholder = new BoundDeconstructValuePlaceholder(_syntax.Expression, this.LocalScopeDepth, inferredType.TypeSymbol ?? CreateErrorType("var"));

            DeclarationExpressionSyntax declaration = null;
            ExpressionSyntax expression = null;
            BoundDeconstructionAssignmentOperator deconstruction = BindDeconstruction(
                                                        variables,
                                                        variables,
                                                        right: _syntax.Expression,
                                                        diagnostics: diagnostics,
                                                        rightPlaceholder: valuePlaceholder,
                                                        declaration: ref declaration,
                                                        expression: ref expression);

            return new BoundExpressionStatement(_syntax, deconstruction);
        }

        private BoundForEachStatement BindForEachPartsWorker(DiagnosticBag diagnostics, Binder originalBinder)
        {
            // Use the right binder to avoid seeing iteration variable
            BoundExpression collectionExpr = originalBinder.GetBinder(_syntax.Expression).BindValue(_syntax.Expression, diagnostics, BindValueKind.RValue);

            ForEachEnumeratorInfo.Builder builder = new ForEachEnumeratorInfo.Builder();
            TypeSymbolWithAnnotations inferredType;
            bool hasErrors = !GetEnumeratorInfoAndInferCollectionElementType(ref builder, ref collectionExpr, diagnostics, out inferredType);

            AwaitableInfo awaitInfo = null;
            if (IsAsync)
            {
                throw new NotImplementedException("Async Foreach not implemented");
                var placeholder = new BoundAwaitableValuePlaceholder(_syntax.Expression, builder.IterateHasNext?.ReturnType.TypeSymbol ?? CreateErrorType());
                awaitInfo = BindAwaitInfo(placeholder, _syntax.Expression, _syntax.AwaitKeyword.GetLocation(), diagnostics, ref hasErrors);

                if (!hasErrors && awaitInfo.GetResult?.ReturnType.SpecialType != SpecialType.System_Boolean)
                {
                    diagnostics.Add(ErrorCode.ERR_BadGetAsyncEnumerator, _syntax.Expression.Location, collectionExpr.Type, collectionExpr);
                    hasErrors = true;
                }
            }

            TypeSymbol iterationVariableType;
            BoundTypeExpression boundIterationVariableType;
            bool hasNameConflicts = false;
            BoundForEachDeconstructStep deconstructStep = null;
            BoundExpression iterationErrorExpression = null;
            uint collectionEscape = GetValEscape(collectionExpr, this.LocalScopeDepth);
            switch (_syntax.Kind())
            {
                case SyntaxKind.ForStatement:
                    {
                        var node = (ForStatementSyntax)_syntax;
                        iterationVariableType = inferredType.TypeSymbol ?? CreateErrorType("var");

                        var variables = node.Variable;
                        if (variables.IsDeconstructionLeft())
                        {
                            var valuePlaceholder = new BoundDeconstructValuePlaceholder(_syntax.Expression, collectionEscape, iterationVariableType).MakeCompilerGenerated();
                            DeclarationExpressionSyntax declaration = null;
                            ExpressionSyntax expression = null;
                            BoundDeconstructionAssignmentOperator deconstruction = BindDeconstruction(
                                                                                    variables,
                                                                                    variables,
                                                                                    right: _syntax.Expression,
                                                                                    diagnostics: diagnostics,
                                                                                    rightPlaceholder: valuePlaceholder,
                                                                                    declaration: ref declaration,
                                                                                    expression: ref expression);

                            if (expression != null)
                            {
                                // error: must declare foreach loop iteration variables.
                                Error(diagnostics, ErrorCode.ERR_MustDeclareForeachIteration, variables);
                                hasErrors = true;
                            }

                            deconstructStep = new BoundForEachDeconstructStep(variables, deconstruction, valuePlaceholder).MakeCompilerGenerated();
                        }
                        else
                        {
                            var iterationVariable = (SourceLocalSymbol)this.Locals[0];
                            hasNameConflicts = originalBinder.ValidateDeclarationNameConflictsInScope(iterationVariable, diagnostics);

                            iterationVariable.SetType(TypeSymbolWithAnnotations.Create(iterationVariableType));
                            iterationVariable.SetValEscape(collectionEscape);

                            if (iterationVariable.RefKind != RefKind.None)
                            {
                                // The ref-escape of a ref-returning property is decided
                                // by the value escape of its receiverm, in this case the
                                // collection
                                iterationVariable.SetRefEscape(collectionEscape);

                                if (IsDirectlyInIterator)
                                {
                                    diagnostics.Add(ErrorCode.ERR_BadIteratorLocalType, iterationVariable.IdentifierToken.GetLocation());
                                    hasErrors = true;
                                }
                                else if (IsInAsyncMethod())
                                {
                                    diagnostics.Add(ErrorCode.ERR_BadAsyncLocalType, iterationVariable.IdentifierToken.GetLocation());
                                    hasErrors = true;
                                }
                            }

                            if (!hasErrors)
                            {
                                BindValueKind requiredCurrentKind;
                                switch (iterationVariable.RefKind)
                                {
                                    case RefKind.None:
                                        requiredCurrentKind = BindValueKind.RValue;
                                        break;
                                    case RefKind.Ref:
                                        requiredCurrentKind = BindValueKind.Assignable | BindValueKind.RefersToLocation;
                                        break;
                                    case RefKind.RefReadOnly:
                                        requiredCurrentKind = BindValueKind.RefersToLocation;
                                        break;
                                    default:
                                        throw ExceptionUtilities.UnexpectedValue(iterationVariable.RefKind);
                                }

                                hasErrors |= !CheckMethodReturnValueKind(
                                    builder.IterateNext,
                                    callSyntaxOpt: null,
                                    collectionExpr.Syntax,
                                    requiredCurrentKind,
                                    checkingReceiver: false,
                                    diagnostics);
                            }
                        }
                        boundIterationVariableType = new BoundTypeExpression(variables, aliasOpt: null, type: iterationVariableType).MakeCompilerGenerated();
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(_syntax.Kind());
            }

            BoundStatement body = originalBinder.BindPossibleEmbeddedStatement(_syntax.Statement, diagnostics);

            // NOTE: in error cases, binder may collect all kind of variables, not just formally declared iteration variables.
            //       As a matter of error recovery, we will treat such variables the same as the iteration variables.
            //       I.E. - they will be considered declared and assigned in each iteration step. 
            ImmutableArray<LocalSymbol> iterationVariables = this.Locals;

            Debug.Assert(hasErrors ||
                _syntax.HasErrors ||
                iterationVariables.All(local => local.DeclarationKind == LocalDeclarationKind.ForEachIterationVariable),
                "Should not have iteration variables that are not ForEachIterationVariable in valid code");

            hasErrors = hasErrors || boundIterationVariableType.HasErrors || iterationVariableType.IsErrorType();

            // Skip the conversion checks and array/enumerator differentiation if we know we have an error (except local name conflicts).
            if (hasErrors)
            {
                return new BoundForEachStatement(
                    _syntax,
                    enumeratorInfoOpt: null, // can't be sure that it's complete
                    elementConversion: default,
                    boundIterationVariableType,
                    iterationVariables,
                    iterationErrorExpression,
                    collectionExpr,
                    deconstructStep,
                    awaitInfo,
                    body,
                    CheckOverflowAtRuntime,
                    this.BreakLabel,
                    this.ContinueLabel,
                    hasErrors);
            }

            hasErrors |= hasNameConflicts;

            var foreachKeyword = _syntax.ForEachKeyword;
            ReportDiagnosticsIfObsolete(diagnostics, builder.IterateBegin, foreachKeyword, hasBaseReceiver: false);
            ReportDiagnosticsIfObsolete(diagnostics, builder.IterateHasNext, foreachKeyword, hasBaseReceiver: false);
            ReportDiagnosticsIfObsolete(diagnostics, builder.IterateNext, foreachKeyword, hasBaseReceiver: false);
            ReportDiagnosticsIfObsolete(diagnostics, builder.IterateEnd, foreachKeyword, hasBaseReceiver: false);

            // We want to convert from inferredType in the array/string case and builder.ElementType in the enumerator case,
            // but it turns out that these are equivalent (when both are available).

            HashSet<DiagnosticInfo> useSiteDiagnostics = null;
            Conversion elementConversion = this.Conversions.ClassifyConversionFromType(inferredType.TypeSymbol, iterationVariableType, ref useSiteDiagnostics, forCast: true);

            if (!elementConversion.IsValid)
            {
                ImmutableArray<MethodSymbol> originalUserDefinedConversions = elementConversion.OriginalUserDefinedConversions;
                if (originalUserDefinedConversions.Length > 1)
                {
                    diagnostics.Add(ErrorCode.ERR_AmbigUDConv, foreachKeyword.GetLocation(), originalUserDefinedConversions[0], originalUserDefinedConversions[1], inferredType.TypeSymbol, iterationVariableType);
                }
                else
                {
                    SymbolDistinguisher distinguisher = new SymbolDistinguisher(this.Compilation, inferredType.TypeSymbol, iterationVariableType);
                    diagnostics.Add(ErrorCode.ERR_NoExplicitConv, foreachKeyword.GetLocation(), distinguisher.First, distinguisher.Second);
                }
                hasErrors = true;
            }
            else
            {
                ReportDiagnosticsIfObsolete(diagnostics, elementConversion, _syntax.ForEachKeyword, hasBaseReceiver: false);
            }

            // Spec (§8.8.4):
            // If the type X of expression is dynamic then there is an implicit conversion from >>expression<< (not the type of the expression) 
            // to the System.Collections.IEnumerable interface (§6.1.8). 

            diagnostics.Add(_syntax.ForEachKeyword.GetLocation(), useSiteDiagnostics);

            if (builder.NeedsDisposal && IsAsync)
            {
                hasErrors |= GetAwaitDisposeAsyncInfo(ref builder, diagnostics);
            }

            return new BoundForEachStatement(
                _syntax,
                builder.
                Build(this.Flags),
                elementConversion,
                boundIterationVariableType,
                iterationVariables,
                iterationErrorExpression,
                collectionExpr,
                deconstructStep,
                awaitInfo,
                body,
                CheckOverflowAtRuntime,
                this.BreakLabel,
                this.ContinueLabel,
                hasErrors);
        }

        private bool GetAwaitDisposeAsyncInfo(ref ForEachEnumeratorInfo.Builder builder, DiagnosticBag diagnostics)
        {
            var awaitableType = builder.IterableType;

            var placeholder = new BoundAwaitableValuePlaceholder(_syntax.Expression, awaitableType);

            bool hasErrors = false;
            builder.DisposeAwaitableInfo = BindAwaitInfo(placeholder, _syntax.Expression, _syntax.AwaitKeyword.GetLocation(), diagnostics, ref hasErrors);
            return hasErrors;
        }

        internal TypeSymbolWithAnnotations InferCollectionElementType(DiagnosticBag diagnostics, ExpressionSyntax collectionSyntax)
        {
            // Use the right binder to avoid seeing iteration variable
            BoundExpression collectionExpr = this.GetBinder(collectionSyntax).BindValue(collectionSyntax, diagnostics, BindValueKind.RValue);

            ForEachEnumeratorInfo.Builder builder = new ForEachEnumeratorInfo.Builder();
            GetEnumeratorInfoAndInferCollectionElementType(ref builder, ref collectionExpr, diagnostics, out TypeSymbolWithAnnotations inferredType);
            return inferredType;
        }

        private bool GetEnumeratorInfoAndInferCollectionElementType(ref ForEachEnumeratorInfo.Builder builder, ref BoundExpression collectionExpr, DiagnosticBag diagnostics, out TypeSymbolWithAnnotations inferredType)
        {
            UnwrapCollectionExpressionIfNullable(ref collectionExpr, diagnostics);

            bool gotInfo = GetEnumeratorInfo(ref builder, collectionExpr, diagnostics);

            inferredType = !gotInfo ? default : builder.ElementType;

            return gotInfo;
        }

        private void UnwrapCollectionExpressionIfNullable(ref BoundExpression collectionExpr, DiagnosticBag diagnostics)
        {
            TypeSymbol collectionExprType = collectionExpr.Type;

            // If collectionExprType is a nullable type, then use the underlying type and take the value (i.e. .Value) of collectionExpr.
            // This behavior is not spec'd, but it's what Dev10 does.
            if ((object)collectionExprType != null && collectionExprType.IsNullableType())
            {
                SyntaxNode exprSyntax = collectionExpr.Syntax;

                MethodSymbol nullableValueGetter = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_Nullable_T_get_Value, diagnostics, exprSyntax);
                if ((object)nullableValueGetter != null)
                {
                    nullableValueGetter = nullableValueGetter.AsMember((NamedTypeSymbol)collectionExprType);

                    // Synthesized call, because we don't want to modify the type in the SemanticModel.
                    collectionExpr = BoundCall.Synthesized(
                        syntax: exprSyntax,
                        receiverOpt: collectionExpr,
                        method: nullableValueGetter);
                }
                else
                {
                    collectionExpr = new BoundBadExpression(
                        exprSyntax,
                        LookupResultKind.Empty,
                        ImmutableArray<Symbol>.Empty,
                        ImmutableArray.Create(collectionExpr),
                        collectionExprType.GetNullableUnderlyingType())
                    { WasCompilerGenerated = true }; // Don't affect the type in the SemanticModel.
                }
            }
        }

        /// <summary>
        /// The spec describes an algorithm for finding the following types:
        ///   1) Collection type
        ///   2) Enumerator type
        ///   3) Element type
        ///   
        /// The implementation details are a bit different.  If we're iterating over a string or an array, then we don't need to record anything
        /// but the inferredType (in case the iteration variable is implicitly typed).  If we're iterating over anything else, then we want the 
        /// inferred type plus a ForEachEnumeratorInfo.Builder with:
        ///   1) Collection type
        ///   2) Element type
        ///   3) GetEnumerator (or GetAsyncEnumerator) method of the collection type (return type will be the enumerator type from the spec)
        ///   4) Current property and MoveNext (or MoveNextAsync) method of the enumerator type
        ///   
        /// The caller will have to do some extra conversion checks before creating a ForEachEnumeratorInfo for the BoundForEachStatement.
        /// </summary>
        /// <param name="builder">Builder to fill in (partially, all but conversions).</param>
        /// <param name="collectionExpr">The expression over which to iterate.</param>
        /// <param name="diagnostics">Populated with binding diagnostics.</param>
        /// <returns>Partially populated (all but conversions) or null if there was an error.</returns>
        private bool GetEnumeratorInfo(ref ForEachEnumeratorInfo.Builder builder, BoundExpression collectionExpr, DiagnosticBag diagnostics)
        {
            bool isAsync = IsAsync;
            EnumeratorResult found = GetEnumeratorInfo(ref builder, collectionExpr, isAsync, diagnostics);
            switch (found)
            {
                case EnumeratorResult.Succeeded:
                    return true;
                case EnumeratorResult.FailedAndReported:
                    return false;
            }

            TypeSymbol collectionExprType = collectionExpr.Type;
            if (string.IsNullOrEmpty(collectionExprType.Name) && collectionExpr.HasErrors)
            {
                return false;
            }

            if (collectionExprType.IsErrorType())
            {
                return false;
            }

            // Retry with a different assumption about whether the foreach is async
            var ignoredBuilder = new ForEachEnumeratorInfo.Builder();
            var ignoredDiagnostics = DiagnosticBag.GetInstance();
            bool wrongAsync = GetEnumeratorInfo(ref ignoredBuilder, collectionExpr, !isAsync, ignoredDiagnostics) == EnumeratorResult.Succeeded;
            ignoredDiagnostics.Free();

            var errorCode = wrongAsync
                ? (isAsync ? ErrorCode.ERR_AwaitForEachMissingMemberWrongAsync : ErrorCode.ERR_ForEachMissingMemberWrongAsync)
                : (isAsync ? ErrorCode.ERR_AwaitForEachMissingMember : ErrorCode.ERR_ForEachMissingMember);

            diagnostics.Add(errorCode, _syntax.Expression.Location,
                collectionExprType, isAsync ? GetAsyncEnumeratorMethodName : IteratorMethodName);
            return false;
        }

        private enum EnumeratorResult
        {
            Succeeded,
            FailedNotReported,
            FailedAndReported
        }

        private EnumeratorResult GetEnumeratorInfo(ref ForEachEnumeratorInfo.Builder builder, BoundExpression collectionExpr, bool isAsync, DiagnosticBag diagnostics)
        {
            TypeSymbol collectionExprType = collectionExpr.Type;

            if (collectionExpr.ConstantValue != null)
            {
                if (collectionExpr.ConstantValue.IsNull)
                {
                    // Spec seems to refer to null literals, but Dev10 reports anything known to be null.
                    diagnostics.Add(ErrorCode.ERR_NullNotValid, _syntax.Expression.Location);
                    return EnumeratorResult.FailedAndReported;
                }
            }

            if ((object)collectionExprType == null) // There's no way to enumerate something without a type.
            {
                if (collectionExpr.Kind == BoundKind.DefaultExpression)
                {
                    diagnostics.Add(ErrorCode.ERR_DefaultLiteralNotValid, _syntax.Expression.Location);
                }
                else
                {
                    // The null and default literals were caught above, so anything else with a null type is a method group or anonymous function
                    diagnostics.Add(ErrorCode.ERR_AnonMethGrpInForEach, _syntax.Expression.Location, collectionExpr.Display);
                    // CONSIDER: dev10 also reports ERR_ForEachMissingMember (i.e. failed pattern match).
                }
                return EnumeratorResult.FailedAndReported;
            }

            if (collectionExpr.ResultKind == LookupResultKind.NotAValue)
            {
                // Short-circuiting to prevent strange behavior in the case where the collection
                // expression is a type expression and the type is enumerable.
                Debug.Assert(collectionExpr.HasAnyErrors); // should already have been reported
                return EnumeratorResult.FailedAndReported;
            }

            // The spec specifically lists the collection, enumerator, and element types for arrays and dynamic.
            if (collectionExprType.Kind == SymbolKind.ArrayType)
            {
                builder = GetDefaultEnumeratorInfo(builder, diagnostics, collectionExprType);
                return EnumeratorResult.Succeeded;
            }

            bool foundMultipleGenericIEnumerableInterfaces;

            //if (SatisfiesGetEnumeratorPattern(ref builder, collectionExprType, isAsync, diagnostics))
            //{
            //    Debug.Assert((object)builder.IteratorMethod != null);

            //    builder.CollectionType = collectionExprType;

            //    if (SatisfiesForEachPattern(ref builder, isAsync, diagnostics))
            //    {
            //        builder.ElementType = ((PropertySymbol)builder.IterateItem.AssociatedSymbol).Type;

            //        GetDisposalInfoForEnumerator(ref builder, collectionExpr, isAsync, diagnostics);

            //        return EnumeratorResult.Succeeded;
            //    }

            //    MethodSymbol getEnumeratorMethod = builder.IteratorMethod;
            //    diagnostics.Add(isAsync ? ErrorCode.ERR_BadGetAsyncEnumerator : ErrorCode.ERR_BadGetEnumerator, _syntax.Expression.Location, getEnumeratorMethod.ReturnType.TypeSymbol, getEnumeratorMethod);
            //    return EnumeratorResult.FailedAndReported;
            //}

            //if (isAsync && IsIAsyncEnumerable(collectionExprType))
            //{
            //    // This indicates a problem with the well-known IAsyncEnumerable type - it should have satisfied the GetAsyncEnumerator pattern.
            //    diagnostics.Add(ErrorCode.ERR_AwaitForEachMissingMember, _syntax.Expression.Location, collectionExprType, GetAsyncEnumeratorMethodName);
            //    return EnumeratorResult.FailedAndReported;
            //}

            if (AllInterfacesContainsIterable(ref builder, collectionExprType, isAsync, diagnostics, out foundMultipleGenericIEnumerableInterfaces))
            {
                CSharpSyntaxNode errorLocationSyntax = _syntax.Expression;

                if (foundMultipleGenericIEnumerableInterfaces)
                {
                    diagnostics.Add(isAsync ? ErrorCode.ERR_MultipleIAsyncEnumOfT : ErrorCode.ERR_MultipleIEnumOfT, errorLocationSyntax.Location, collectionExprType,
                        isAsync ?
                            this.Compilation.GetWellKnownType(WellKnownType.core_Collections_Generic_IAsyncEnumerable_T) :
                            this.Compilation.GetSpecialType(SpecialType.core_Iterable_T_TIterator));
                    return EnumeratorResult.FailedAndReported;
                }

                Debug.Assert((object)builder.IterableType != null);

                NamedTypeSymbol iterableType = (NamedTypeSymbol)builder.IterableType;
                Debug.Assert(iterableType.IsGenericType);

                // If the type is generic, we have to search for the methods
                builder.ElementType = iterableType.TypeArgumentsNoUseSiteDiagnostics[0];
                builder.IteratorType = iterableType.TypeArgumentsNoUseSiteDiagnostics[1].TypeSymbol;

                //MethodSymbol iteratorMethod = GetIteratorMethod(iterableType, diagnostics, isAsync, errorLocationSyntax);


                if (isAsync)
                {
                    throw new NotImplementedException("Add support for async iterator in stark");
                    Debug.Assert(builder.IterableType.OriginalDefinition.Equals(Compilation.GetWellKnownType(WellKnownType.core_Collections_Generic_IAsyncEnumerator_T)));

                    MethodSymbol moveNextAsync = (MethodSymbol)GetWellKnownTypeMember(Compilation, WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__MoveNextAsync,
                        diagnostics, errorLocationSyntax.Location, isOptional: false);

                    if ((object)moveNextAsync != null)
                    {
                        builder.IterateHasNext = moveNextAsync.AsMember((NamedTypeSymbol)builder.IterableType);
                    }
                }
                else
                {
                    builder.IterateBegin = ((MethodSymbol)GetSpecialTypeMember(SpecialMember.core_Iterable_T_TIterator__iterate_begin, diagnostics, _syntax)).AsMember((NamedTypeSymbol)builder.IterableType);
                    builder.IterateHasNext = ((MethodSymbol)GetSpecialTypeMember(SpecialMember.core_Iterable_T_TIterator__iterate_has_next, diagnostics, _syntax)).AsMember((NamedTypeSymbol)builder.IterableType);
                    builder.IterateNext = ((MethodSymbol)GetSpecialTypeMember(SpecialMember.core_Iterable_T_TIterator__iterate_next, diagnostics, _syntax)).AsMember((NamedTypeSymbol)builder.IterableType);
                    builder.IterateEnd = ((MethodSymbol)GetSpecialTypeMember(SpecialMember.core_Iterable_T_TIterator__iterate_end, diagnostics, _syntax)).AsMember((NamedTypeSymbol)builder.IterableType);
                }

                // We don't know the runtime type, so we will have to insert a runtime check for IDisposable (with a conditional call to IDisposable.Dispose).
                builder.NeedsDisposal = false;
                return EnumeratorResult.Succeeded;
            }

            return EnumeratorResult.FailedNotReported;
        }

        private void GetDisposalInfoForEnumerator(ref ForEachEnumeratorInfo.Builder builder, BoundExpression expr, bool isAsync, DiagnosticBag diagnostics)
        {
            //// NOTE: if IDisposable is not available at all, no diagnostics will be reported - we will just assume that
            //// the enumerator is not disposable.  If it has IDisposable in its interface list, there will be a diagnostic there.
            //// If IDisposable is available but its Dispose method is not, then diagnostics will be reported only if the enumerator
            //// is potentially disposable.

            //TypeSymbol enumeratorType = builder.IteratorMethod.ReturnType.TypeSymbol;
            //HashSet<DiagnosticInfo> useSiteDiagnostics = null;

            //// For async foreach, we don't do the runtime check
            //if ((!enumeratorType.IsSealed && !isAsync) ||
            //    this.Conversions.ClassifyImplicitConversionFromType(enumeratorType,
            //        isAsync ? this.Compilation.GetWellKnownType(WellKnownType.core_IAsyncDisposable) : this.Compilation.GetSpecialType(SpecialType.System_IDisposable),
            //        ref useSiteDiagnostics).IsImplicit)
            //{
            //    builder.NeedsDisposal = true;
            //}
            //else if (enumeratorType.IsRefLikeType || isAsync)
            //{
            //    // if it wasn't directly convertable to IDisposable, see if it is pattern-disposable
            //    // again, we throw away any binding diagnostics, and assume it's not disposable if we encounter errors
            //    var patternDisposeDiags = new DiagnosticBag();
            //    var receiver = new BoundDisposableValuePlaceholder(_syntax, enumeratorType);
            //    MethodSymbol disposeMethod = TryFindDisposePatternMethod(receiver, _syntax, isAsync, patternDisposeDiags);
            //    if (!(disposeMethod is null))
            //    {
            //        builder.NeedsDisposal = true;
            //        //builder.DisposeMethod = disposeMethod;
            //        throw new NotImplementedException("TODO: Dispose not implemented for Async in stark");
            //    }
            //    patternDisposeDiags.Free();
            //}

            //diagnostics.Add(_syntax, useSiteDiagnostics);
        }

        private ForEachEnumeratorInfo.Builder GetDefaultEnumeratorInfo(ForEachEnumeratorInfo.Builder builder, DiagnosticBag diagnostics, TypeSymbol collectionExprType)
        {
            // NOTE: for arrays, we won't actually use any of these members - they're just for the API.
            builder.ElementType = ((ArrayTypeSymbol)collectionExprType).ElementType;
            builder.IteratorType =  GetSpecialType(SpecialType.System_Int, diagnostics, _syntax);

            // Construct the collection type
            builder.IterableType = GetSpecialType(SpecialType.core_Iterable_T_TIterator, diagnostics, _syntax).Construct(builder.ElementType.TypeSymbol, builder.IteratorType, collectionExprType);

            // CONSIDER: 
            // For arrays and string none of these members will actually be emitted, so it seems strange to prevent compilation if they can't be found.
            // skip this work in the batch case?
            builder.IterateBegin = ((MethodSymbol)GetSpecialTypeMember(SpecialMember.core_Iterable_T_TIterator__iterate_begin, diagnostics, _syntax)).AsMember((NamedTypeSymbol)builder.IterableType);
            builder.IterateHasNext = ((MethodSymbol)GetSpecialTypeMember(SpecialMember.core_Iterable_T_TIterator__iterate_has_next, diagnostics, _syntax)).AsMember((NamedTypeSymbol)builder.IterableType);
            builder.IterateNext = ((MethodSymbol)GetSpecialTypeMember(SpecialMember.core_Iterable_T_TIterator__iterate_next, diagnostics, _syntax)).AsMember((NamedTypeSymbol)builder.IterableType);
            builder.IterateEnd = ((MethodSymbol)GetSpecialTypeMember(SpecialMember.core_Iterable_T_TIterator__iterate_end, diagnostics, _syntax)).AsMember((NamedTypeSymbol)builder.IterableType);

            //Debug.Assert((object)builder.GetEnumeratorMethod == null ||
            //    TypeSymbol.Equals(builder.GetEnumeratorMethod.ReturnType.TypeSymbol, this.Compilation.GetSpecialType(SpecialType.core_Iterator_TItem_TState), TypeCompareKind.ConsiderEverything2));

            // We don't know the runtime type, so we will have to insert a runtime check for IDisposable (with a conditional call to IDisposable.Dispose).
            builder.NeedsDisposal = true;
            return builder;
        }

        /// <summary>
        /// Perform a lookup for the specified method on the specified type.  Perform overload resolution
        /// on the lookup results.
        /// </summary>
        /// <param name="patternType">Type to search.</param>
        /// <param name="methodName">Method to search for.</param>
        /// <param name="lookupResult">Passed in for reusability.</param>
        /// <param name="warningsOnly">True if failures should result in warnings; false if they should result in errors.</param>
        /// <param name="diagnostics">Populated with binding diagnostics.</param>
        /// <returns>The desired method or null.</returns>
        private MethodSymbol FindForEachPatternMethod(TypeSymbol patternType, string methodName, int parameterCount, LookupResult lookupResult, bool warningsOnly, DiagnosticBag diagnostics, bool isAsync)
        {
            Debug.Assert(lookupResult.IsClear);

            // Not using LookupOptions.MustBeInvocableMember because we don't want the corresponding lookup error.
            // We filter out non-methods below.
            HashSet<DiagnosticInfo> useSiteDiagnostics = null;
            this.LookupMembersInType(
                lookupResult,
                patternType,
                methodName,
                arity: 0,
                basesBeingResolved: null,
                options: LookupOptions.Default,
                originalBinder: this,
                diagnose: false,
                useSiteDiagnostics: ref useSiteDiagnostics);

            diagnostics.Add(_syntax.Expression, useSiteDiagnostics);

            if (!lookupResult.IsMultiViable)
            {
                ReportPatternMemberLookupDiagnostics(lookupResult, patternType, methodName, warningsOnly, diagnostics);
                return null;
            }

            ArrayBuilder<MethodSymbol> candidateMethods = ArrayBuilder<MethodSymbol>.GetInstance();

            foreach (Symbol member in lookupResult.Symbols)
            {
                if (member.Kind != SymbolKind.Method)
                {
                    candidateMethods.Free();

                    if (warningsOnly)
                    {
                        ReportEnumerableWarning(diagnostics, patternType, member);
                    }
                    return null;
                }

                MethodSymbol method = (MethodSymbol)member;

                // SPEC VIOLATION: The spec says we should apply overload resolution, but Dev10 uses
                // some custom logic in ExpressionBinder.BindGrpToParams.  The biggest difference
                // we've found (so far) is that it only considers methods with expected number of parameters
                // (i.e. doesn't work with "params" or optional parameters).

                // Note: for pattern-based lookup for `await foreach` we accept `GetAsyncEnumerator` and
                // `MoveNextAsync` methods with optional/params parameters.
                if (method.ParameterCount == parameterCount || isAsync)
                {
                    candidateMethods.Add((MethodSymbol)member);
                }
            }

            MethodSymbol patternMethod = PerformForEachPatternOverloadResolution(patternType, candidateMethods, warningsOnly, diagnostics, isAsync);

            candidateMethods.Free();

            return patternMethod;
        }

        /// <summary>
        /// The overload resolution portion of FindForEachPatternMethod.
        /// If no arguments are passed in, then an empty argument list will be used.
        /// </summary>
        private MethodSymbol PerformForEachPatternOverloadResolution(TypeSymbol patternType, ArrayBuilder<MethodSymbol> candidateMethods, bool warningsOnly, DiagnosticBag diagnostics, bool isAsync)
        {
            var analyzedArguments = AnalyzedArguments.GetInstance();
            var typeArguments = ArrayBuilder<TypeSymbolWithAnnotations>.GetInstance();
            var overloadResolutionResult = OverloadResolutionResult<MethodSymbol>.GetInstance();

            HashSet<DiagnosticInfo> useSiteDiagnostics = null;
            // We create a dummy receiver of the invocation so MethodInvocationOverloadResolution knows it was invoked from an instance, not a type
            var dummyReceiver = new BoundImplicitReceiver(_syntax.Expression, patternType);
            this.OverloadResolution.MethodInvocationOverloadResolution(
                methods: candidateMethods,
                typeArguments: typeArguments,
                receiver: dummyReceiver,
                arguments: analyzedArguments,
                result: overloadResolutionResult,
                useSiteDiagnostics: ref useSiteDiagnostics);
            diagnostics.Add(_syntax.Expression, useSiteDiagnostics);

            MethodSymbol result = null;

            if (overloadResolutionResult.Succeeded)
            {
                result = overloadResolutionResult.ValidResult.Member;

                if (result.IsStatic || result.DeclaredAccessibility != Accessibility.Public)
                {
                    if (warningsOnly)
                    {
                        MessageID patternName = isAsync ? MessageID.IDS_FeatureAsyncStreams : MessageID.IDS_Collection;
                        diagnostics.Add(ErrorCode.WRN_PatternStaticOrInaccessible, _syntax.Expression.Location, patternType, patternName.Localize(), result);
                    }
                    result = null;
                }
                else if (result.CallsAreOmitted(_syntax.SyntaxTree))
                {
                    // Calls to this method are omitted in the current syntax tree, i.e it is either a partial method with no implementation part OR a conditional method whose condition is not true in this source file.
                    // We don't want to want to allow this case, see StatementBinder::bindPatternToMethod.
                    result = null;
                }
            }
            else if (overloadResolutionResult.Results.Length > 1)
            {
                if (warningsOnly)
                {
                    diagnostics.Add(ErrorCode.WRN_PatternIsAmbiguous, _syntax.Expression.Location, patternType, MessageID.IDS_Collection.Localize(),
                        overloadResolutionResult.Results[0].Member, overloadResolutionResult.Results[1].Member);
                }
            }

            overloadResolutionResult.Free();
            analyzedArguments.Free();
            typeArguments.Free();

            return result;
        }

        private void ReportEnumerableWarning(DiagnosticBag diagnostics, TypeSymbol enumeratorType, Symbol patternMemberCandidate)
        {
            HashSet<DiagnosticInfo> useSiteDiagnostics = null;
            if (this.IsAccessible(patternMemberCandidate, ref useSiteDiagnostics))
            {
                diagnostics.Add(ErrorCode.WRN_PatternBadSignature, _syntax.Expression.Location, enumeratorType, MessageID.IDS_Collection.Localize(), patternMemberCandidate);
            }

            diagnostics.Add(_syntax.Expression, useSiteDiagnostics);
        }

        private bool IsIAsyncEnumerable(TypeSymbol type)
        {
            return type.OriginalDefinition.Equals(Compilation.GetWellKnownType(WellKnownType.core_Collections_Generic_IAsyncEnumerable_T));
        }

        /// <summary>
        /// Checks if the given type implements (or extends, in the case of an interface),
        /// System.Collections.IEnumerable or System.Collections.Generic.IEnumerable&lt;T&gt;,
        /// (or System.Collections.Generic.IAsyncEnumerable&lt;T&gt;)
        /// for at least one T.
        /// </summary>
        /// <param name="builder">builder to fill in CollectionType.</param>
        /// <param name="type">Type to check.</param>
        /// <param name="diagnostics" />
        /// <param name="foundMultiple">True if multiple T's are found.</param>
        /// <returns>True if some IEnumerable is found (may still be ambiguous).</returns>
        private bool AllInterfacesContainsIterable(
            ref ForEachEnumeratorInfo.Builder builder,
            TypeSymbol type,
            bool isAsync,
            DiagnosticBag diagnostics,
            out bool foundMultiple)
        {
            NamedTypeSymbol implementedIEnumerable = null;
            foundMultiple = false;
            HashSet<DiagnosticInfo> useSiteDiagnostics = null;

            if (type.TypeKind == TypeKind.TypeParameter)
            {
                var typeParameter = (TypeParameterSymbol)type;
                GetIterableOfTItemTStateTIterator(typeParameter.EffectiveBaseClass(ref useSiteDiagnostics).AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteDiagnostics), isAsync, ref @implementedIEnumerable, ref foundMultiple);
                GetIterableOfTItemTStateTIterator(typeParameter.AllEffectiveInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteDiagnostics), isAsync, ref @implementedIEnumerable, ref foundMultiple);
            }
            else
            {
                GetIterableOfTItemTStateTIterator(type.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteDiagnostics), isAsync, ref @implementedIEnumerable, ref foundMultiple);
            }

            diagnostics.Add(_syntax.Expression, useSiteDiagnostics);

            builder.IterableType = implementedIEnumerable;
            return (object)implementedIEnumerable != null;
        }

        private void GetIterableOfTItemTStateTIterator(ImmutableArray<NamedTypeSymbol> interfaces, bool isAsync, ref NamedTypeSymbol result, ref bool foundMultiple)
        {
            if (foundMultiple)
            {
                return;
            }
            foreach (NamedTypeSymbol @interface in interfaces)
            {
                if (IsIterableOfTItemTStateTIterator(@interface.OriginalDefinition, isAsync))
                {
                    if ((object)result == null ||
                        TypeSymbol.Equals(@interface, result, TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                    {
                        result = @interface;
                    }
                    else
                    {
                        foundMultiple = true;
                        return;
                    }
                }
            }
        }

        private bool IsIterableOfTItemTStateTIterator(TypeSymbol type, bool isAsync)
        {
            if (isAsync)
            {
                return type.Equals(Compilation.GetWellKnownType(WellKnownType.core_Collections_Generic_IAsyncEnumerable_T));
            }
            else
            {
                return type.SpecialType == SpecialType.core_Iterable_T_TIterator;
            }
        }

        /// <summary>
        /// Report appropriate diagnostics when lookup of a pattern member (i.e. GetEnumerator, Current, or MoveNext) fails.
        /// </summary>
        /// <param name="lookupResult">Failed lookup result.</param>
        /// <param name="patternType">Type in which member was looked up.</param>
        /// <param name="memberName">Name of looked up member.</param>
        /// <param name="warningsOnly">True if failures should result in warnings; false if they should result in errors.</param>
        /// <param name="diagnostics">Populated appropriately.</param>
        private void ReportPatternMemberLookupDiagnostics(LookupResult lookupResult, TypeSymbol patternType, string memberName, bool warningsOnly, DiagnosticBag diagnostics)
        {
            if (lookupResult.Symbols.Any())
            {
                if (warningsOnly)
                {
                    ReportEnumerableWarning(diagnostics, patternType, lookupResult.Symbols.First());
                }
                else
                {
                    lookupResult.Clear();

                    HashSet<DiagnosticInfo> useSiteDiagnostics = null;
                    this.LookupMembersInType(
                        lookupResult,
                        patternType,
                        memberName,
                        arity: 0,
                        basesBeingResolved: null,
                        options: LookupOptions.Default,
                        originalBinder: this,
                        diagnose: true,
                        useSiteDiagnostics: ref useSiteDiagnostics);

                    diagnostics.Add(_syntax.Expression, useSiteDiagnostics);

                    if (lookupResult.Error != null)
                    {
                        diagnostics.Add(lookupResult.Error, _syntax.Expression.Location);
                    }
                }
            }
            else if (!warningsOnly)
            {
                diagnostics.Add(ErrorCode.ERR_NoSuchMember, _syntax.Expression.Location, patternType, memberName);
            }
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
