// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using StarkPlatform.CodeAnalysis.Collections;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.PooledObjects;
using StarkPlatform.CodeAnalysis.Stark.Syntax;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal class ControlFlowPass : AbstractFlowPass<ControlFlowPass.LocalState>
    {
        private readonly PooledDictionary<LabelSymbol, BoundBlock> _labelsDefined = PooledDictionary<LabelSymbol, BoundBlock>.GetInstance();
        private readonly PooledHashSet<LabelSymbol> _labelsUsed = PooledHashSet<LabelSymbol>.GetInstance();
        private readonly ArrayBuilder<BoundBlockWithExceptions> _blockWithExceptions = ArrayBuilder<BoundBlockWithExceptions>.GetInstance();

        protected bool _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = false; // By default, just let the original exception to bubble up.

        private readonly ArrayBuilder<(LocalSymbol symbol, BoundBlock block)> _usingDeclarations = ArrayBuilder<(LocalSymbol, BoundBlock)>.GetInstance();
        private BoundBlock _currentBlock = null;

        protected override void Free()
        {
            foreach (var blockWithException in _blockWithExceptions)
            {
                blockWithException.ThrowLocations.Free();
            }
            _blockWithExceptions.Free();
            _labelsDefined.Free();
            _labelsUsed.Free();
            _usingDeclarations.Free();
            base.Free();
        }

        internal ControlFlowPass(CSharpCompilation compilation, Symbol member, BoundNode node)
            : base(compilation, member, node)
        {
        }

        internal ControlFlowPass(CSharpCompilation compilation, Symbol member, BoundNode node, BoundNode firstInRegion, BoundNode lastInRegion)
            : base(compilation, member, node, firstInRegion, lastInRegion)
        {
        }

        internal struct LocalState : ILocalState
        {
            internal bool Alive;
            internal bool Reported; // reported unreachable statement

            internal LocalState(bool live, bool reported)
            {
                this.Alive = live;
                this.Reported = reported;
            }

            /// <summary>
            /// Produce a duplicate of this flow analysis state.
            /// </summary>
            /// <returns></returns>
            public LocalState Clone()
            {
                return this;
            }

            public bool Reachable
            {
                get { return Alive; }
            }
        }

        protected override void Meet(ref LocalState self, ref LocalState other)
        {
            self.Alive &= other.Alive;
            self.Reported &= other.Reported;
            Debug.Assert(!self.Alive || !self.Reported);
        }

        protected override bool Join(ref LocalState self, ref LocalState other)
        {
            var old = self;
            self.Alive |= other.Alive;
            self.Reported &= other.Reported;
            Debug.Assert(!self.Alive || !self.Reported);
            return self.Alive != old.Alive;
        }

        protected override string Dump(LocalState state)
        {
            return "[alive: " + state.Alive + "; reported: " + state.Reported + "]";
        }

        protected override LocalState TopState()
        {
            return new LocalState(true, false);
        }

        protected override LocalState UnreachableState()
        {
            return new LocalState(false, this.State.Reported);
        }

        protected override LocalState LabelState(LabelSymbol label)
        {
            LocalState result = base.LabelState(label);
            // We don't want errors reported in one pass to suppress errors in the next pass.
            result.Reported = false;
            return result;
        }

        //public override BoundNode Visit(BoundNode node)
        //{
        //    // there is no need to scan the contents of an expression, as expressions
        //    // do not contribute to reachability analysis (except for constants, which
        //    // are handled by the caller).
        //    if (!(node is BoundExpression))
        //    {
        //        return base.Visit(node);
        //    }
        //    else if (node is BoundTryExpression boundTry)
        //    {
        //        return VisitTryExpression(boundTry);
        //    }

        //    return null;
        //}

        protected override ImmutableArray<PendingBranch> Scan(ref bool badRegion)
        {
            this.Diagnostics.Clear();  // clear reported diagnostics
            var result = base.Scan(ref badRegion);
            foreach (var label in _labelsDefined.Keys)
            {
                if (!_labelsUsed.Contains(label))
                {
                    Diagnostics.Add(ErrorCode.WRN_UnreferencedLabel, label.Locations[0]);
                }
            }

            return result;
        }

        /// <summary>
        /// Perform control flow analysis, reporting all necessary diagnostics.  Returns true if the end of
        /// the body might be reachable...
        /// </summary>
        public static bool Analyze(CSharpCompilation compilation, Symbol member, BoundBlock block, DiagnosticBag diagnostics)
        {
            var walker = new ControlFlowPass(compilation, member, block);

            if (diagnostics != null)
            {
                walker._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = true;
            }

            try
            {
                bool badRegion = false;
                var result = walker.Analyze(ref badRegion, diagnostics);
                Debug.Assert(!badRegion);
                return result;
            }
            catch (BoundTreeVisitor.CancelledByStackGuardException ex) when (diagnostics != null)
            {
                ex.AddAnError(diagnostics);
                return true;
            }
            finally
            {
                walker.Free();
            }
        }

        protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
        {
            return _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;
        }

        /// <summary>
        /// Analyze the body, reporting all necessary diagnostics.  Returns true if the end of the
        /// body might be reachable.
        /// </summary>
        /// <returns></returns>
        protected bool Analyze(ref bool badRegion, DiagnosticBag diagnostics)
        {
            PushTryBlock((BoundBlock)methodMainNode);
            ImmutableArray<PendingBranch> returns = Analyze(ref badRegion);

            PopRootTryBlockAndVerify();
            if (diagnostics != null)
            {
                diagnostics.AddRange(this.Diagnostics);
            }

            // TODO: if in the body of a struct constructor, check that "this" is assigned at each return.
            return State.Alive;
        }

        protected override ImmutableArray<PendingBranch> RemoveReturns()
        {
            var result = base.RemoveReturns();
            foreach (var pending in result)
            {
                switch (pending.Branch.Kind)
                {
                    case BoundKind.GotoStatement:
                        {
                            var leave = pending.Branch;
                            var loc = new SourceLocation(leave.Syntax.GetFirstToken());
                            Diagnostics.Add(ErrorCode.ERR_LabelNotFound, loc, ((BoundGotoStatement)pending.Branch).Label.Name);
                            break;
                        }
                    case BoundKind.BreakStatement:
                    case BoundKind.ContinueStatement:
                        {
                            var leave = pending.Branch;
                            var loc = new SourceLocation(leave.Syntax.GetFirstToken());
                            Diagnostics.Add(ErrorCode.ERR_BadDelegateLeave, loc);
                            break;
                        }
                    case BoundKind.ReturnStatement:
                        break;
                    default:
                        break; // what else could it be?
                }
            }
            return result;
        }

        protected override void VisitStatement(BoundStatement statement)
        {
            switch (statement.Kind)
            {
                case BoundKind.NoOpStatement:
                case BoundKind.Block:
                case BoundKind.ThrowStatement:
                case BoundKind.LabeledStatement:
                case BoundKind.LocalFunctionStatement:
                    base.VisitStatement(statement);
                    break;
                case BoundKind.StatementList:
                    base.VisitStatementList((BoundStatementList)statement);
                    break;
                default:
                    CheckReachable(statement);
                    base.VisitStatement(statement);
                    break;
            }
        }

        public override BoundNode VisitTryExpression(BoundTryExpression node)
        {
            var call = (BoundCall)node.Expression;
            RecordThrows(call.Syntax, call.Method.ThrowsList);
            return base.VisitTryExpression(node);
        }

        private void CheckReachable(BoundStatement statement)
        {
            if (!this.State.Alive &&
                !this.State.Reported &&
                !statement.WasCompilerGenerated &&
                statement.Syntax.Span.Length != 0)
            {
                var firstToken = statement.Syntax.GetFirstToken();
                Diagnostics.Add(ErrorCode.WRN_UnreachableCode, new SourceLocation(firstToken));
                this.State.Reported = true;
            }
        }

        protected override void VisitTryBlock(BoundStatement tryBlock, BoundTryStatement node, ref LocalState tryState)
        {
            PushTryBlock(node);
            try
            {
                if (node.CatchBlocks.IsEmpty)
                {
                    base.VisitTryBlock(tryBlock, node, ref tryState);
                    return;
                }

                var oldPending = SavePending(); // we do not support branches into a try block
                base.VisitTryBlock(tryBlock, node, ref tryState);
                RestorePending(oldPending);
            } finally
            {
                PopTryBlock();
            }
        }

        protected override void VisitCatchBlock(BoundCatchBlock catchBlock, ref LocalState finallyState)
        {
            var oldPending = SavePending(); // we do not support branches into a catch block
            base.VisitCatchBlock(catchBlock, ref finallyState);
            RestorePending(oldPending);
        }

        protected override void VisitFinallyBlock(BoundStatement finallyBlock, ref LocalState endState)
        {
            var oldPending1 = SavePending(); // we do not support branches into a finally block
            var oldPending2 = SavePending(); // track only the branches out of the finally block
            base.VisitFinallyBlock(finallyBlock, ref endState);
            RestorePending(oldPending2); // resolve branches that remain within the finally block
            foreach (var branch in PendingBranches)
            {
                if (branch.Branch == null) continue; // a tracked exception
                var location = new SourceLocation(branch.Branch.Syntax.GetFirstToken());
                switch (branch.Branch.Kind)
                {
                    case BoundKind.YieldBreakStatement:
                    case BoundKind.YieldReturnStatement:
                        // ERR_BadYieldInFinally reported during initial binding
                        break;
                    default:
                        Diagnostics.Add(ErrorCode.ERR_BadFinallyLeave, location);
                        break;
                }
            }

            RestorePending(oldPending1);
        }

        // For purpose of control flow analysis, awaits do not create pending branches, so asynchronous usings and foreachs don't either
        public sealed override bool AwaitUsingAndForeachAddsPendingBranch => false;

        protected override void VisitLabel(BoundLabeledStatement node)
        {
            _labelsDefined[node.Label] = _currentBlock;
            base.VisitLabel(node);
        }

        public override BoundNode VisitLabeledStatement(BoundLabeledStatement node)
        {
            VisitLabel(node);
            CheckReachable(node);
            VisitStatement(node.Body);
            return null;
        }

        public override BoundNode VisitGotoStatement(BoundGotoStatement node)
        {
            _labelsUsed.Add(node.Label);

            // check for illegal jumps across using declarations
            var sourceLocation = node.Syntax.Location;
            var sourceStart = sourceLocation.SourceSpan.Start;
            var targetStart = node.Label.Locations[0].SourceSpan.Start;

            foreach (var usingDecl in _usingDeclarations)
            {
                var usingStart = usingDecl.symbol.Locations[0].SourceSpan.Start;
                if (sourceStart < usingStart && targetStart > usingStart)
                {
                    // No forward jumps
                    Diagnostics.Add(ErrorCode.ERR_GoToForwardJumpOverUsingVar, sourceLocation);
                    break;
                }
                else if (sourceStart > usingStart && targetStart < usingStart)
                {
                    // Backwards jump, so we must have already seen the label
                    Debug.Assert(_labelsDefined.ContainsKey(node.Label));

                    // Error if label and using are part of the same block
                    if (_labelsDefined[node.Label] == usingDecl.block)
                    {
                        Diagnostics.Add(ErrorCode.ERR_GoToBackwardJumpOverUsingVar, sourceLocation);
                        break;
                    }
                }
            }

            return base.VisitGotoStatement(node);
        }

        protected override void VisitSwitchSection(BoundSwitchSection node, bool isLastSection)
        {
            base.VisitSwitchSection(node, isLastSection);

            // Check for switch section fall through error
            if (this.State.Alive)
            {
                var syntax = node.SwitchLabels.Last().Pattern.Syntax;
                Diagnostics.Add(isLastSection ? ErrorCode.ERR_SwitchFallOut : ErrorCode.ERR_SwitchFallThrough,
                                new SourceLocation(syntax), syntax.ToString());
            }
        }

        public override BoundNode VisitBlock(BoundBlock node)
        {
            var parentBlock = _currentBlock;
            _currentBlock = node;
            var initialUsingCount = _usingDeclarations.Count;
            foreach (var local in node.Locals)
            {
                if (local.IsUsing)
                {
                    _usingDeclarations.Add((local, node));
                }
            }

            var result = base.VisitBlock(node);

            _usingDeclarations.Clip(initialUsingCount);
            _currentBlock = parentBlock;
            return result;
        }

        private void PushTryBlock(BoundStatement bound)
        {
            var block = new BoundBlockWithExceptions(bound);
            _blockWithExceptions.Add(block);
        }

        private BoundBlockWithExceptions CurrentBlockWithExceptions => _blockWithExceptions[_blockWithExceptions.Count - 1];

        private void PopTryBlock()
        {
            var lastBlock = CurrentBlockWithExceptions;
            _blockWithExceptions.RemoveLast();
            if (_blockWithExceptions.Count > 0)
            {
                CurrentBlockWithExceptions.MergeThrowLocations(lastBlock);
            }

            // If we are going out of the BoundTryStatement
            // Log an error for any catch block that will never be reached
            if (lastBlock.Block is BoundTryStatement boundTry)
            {
                foreach (var catchBlock in boundTry.CatchBlocks)
                {
                    if (!lastBlock.CatchBlockUsed.Contains(catchBlock))
                    {
                        Diagnostics.Add(ErrorCode.ERR_CatchBlockNotUsed, catchBlock.Syntax.Location, catchBlock.ExceptionTypeOpt?.ToDisplayString() ?? "(none)");
                    }
                }
            }

            lastBlock.Free();
        }

        public override BoundNode VisitThrowExpression(BoundThrowExpression node)
        {
            RecordThrow(node.Syntax, node.Expression.Type);
            return base.VisitThrowExpression(node);
        }

        public override BoundNode VisitThrowStatement(BoundThrowStatement node)
        {
            if (node.ExpressionOpt != null)
            {
                RecordThrow(node.Syntax, node.ExpressionOpt.Type);
            }
            return base.VisitThrowStatement(node);
        }

        private void RecordThrow(SyntaxNode syntax, TypeSymbol throwType)
        {
            var throwList = ArrayBuilder<TypeSymbol>.GetInstance();
            try
            {
                throwList.Add(throwType);
                RecordThrows(syntax, throwList);
            }
            finally
            {
                throwList.Free();
            }
        }

        private void RecordThrows<TThrows>(SyntaxNode syntax, TThrows throwsList) where TThrows : IEnumerable<TypeSymbol>
        {
            var currentBlockWithExceptions = CurrentBlockWithExceptions;

            var finalThrowsList = ArrayBuilder<TypeSymbol>.GetInstance();
            try
            {
                if (currentBlockWithExceptions.Block is BoundTryStatement boundTry)
                {
                    foreach (var throwType in throwsList)
                    {
                        bool isCatched = false;
                        foreach (var catchBlock in boundTry.CatchBlocks)
                        {
                            if (IsCatching(catchBlock, throwType))
                            {
                                currentBlockWithExceptions.CatchBlockUsed.Add(catchBlock);
                                isCatched = true;
                                break;
                            }

                            // We record block being used
                            if (IsCatchingPossible(catchBlock, throwType))
                            {
                                currentBlockWithExceptions.CatchBlockUsed.Add(catchBlock);
                            }
                        }

                        if (!isCatched)
                        {
                            finalThrowsList.Add(throwType);
                        }
                    }
                }
                else
                {
                    foreach (var throwType in throwsList)
                    {
                        finalThrowsList.Add(throwType);
                    }
                }

                foreach (var throwType in finalThrowsList)
                {
                    currentBlockWithExceptions.AddThrow(throwType, syntax);
                }
            }
            finally
            {
                finalThrowsList.Free();
            }
        }
        
        private void PopRootTryBlockAndVerify()
        {
            var topBlock = CurrentBlockWithExceptions;

            HashSet<DiagnosticInfo> diagnosticInfos = null;
            if (_symbol is SourceMethodSymbol methodSymbol)
            {
                if (topBlock.ThrowLocations.Count > 0)
                {
                    foreach (var throwTypePair in topBlock.ThrowLocations)
                    {
                        var throwType = throwTypePair.Key;
                        bool isDeclared = false;
                        foreach (var declaredType in methodSymbol.ThrowsList)
                        {
                            if (throwType.IsEqualToOrDerivedFrom(declaredType, TypeCompareKind.ConsiderEverything, ref diagnosticInfos))
                            {
                                isDeclared = true;
                                break;
                            }
                        }

                        if (!isDeclared)
                        {
                            foreach (var throwSyntax in throwTypePair.Value)
                            {
                                // TODO: Add locations
                                Diagnostics.Add(ErrorCode.ERR_ExceptionThrownButNotDeclared, throwSyntax.Location, throwType.ToDisplayString());
                            }
                        }
                    }
                }
                else if (methodSymbol.GetNonNullSyntaxNode() is MethodDeclarationSyntax methodDecl && methodSymbol.HasThrows)
                {
                    Diagnostics.Add(ErrorCode.ERR_ExpectingThrows, methodDecl.ThrowsList.ThrowsKeyword.GetLocation());
                }
            }

            // Pop top level block
            PopTryBlock();
        }

        private static bool IsCatching(BoundCatchBlock catchBlock, TypeSymbol exception)
        {
            // If it is catching all exceptions
            if ((object)catchBlock.ExceptionTypeOpt == null)
            {
                return true;
            }

            HashSet<DiagnosticInfo> useSiteDiagnostics = null;

            if (exception.IsEqualToOrDerivedFrom(catchBlock.ExceptionTypeOpt, TypeCompareKind.ConsiderEverything, ref useSiteDiagnostics))
            {
                return catchBlock.ExceptionFilterOpt == null;
            }

            return false;
        }

        private static bool IsCatchingPossible(BoundCatchBlock catchBlock, TypeSymbol exception)
        {
            // Because IsCatching is called before
            Debug.Assert((object)catchBlock.ExceptionTypeOpt != null);

            HashSet<DiagnosticInfo> useSiteDiagnostics = null;
            return catchBlock.ExceptionTypeOpt.IsEqualToOrDerivedFrom(exception, TypeCompareKind.ConsiderEverything, ref useSiteDiagnostics);
        }

        struct BoundBlockWithExceptions
        {
            public BoundBlockWithExceptions(BoundStatement block) : this()
            {
                Block = block;
                ThrowLocations = PooledDictionary<TypeSymbol, PooledHashSet<SyntaxNode>>.GetInstance();
                CatchBlockUsed = PooledHashSet<BoundCatchBlock>.GetInstance();
            }

            public readonly BoundStatement Block;

            public readonly PooledDictionary<TypeSymbol, PooledHashSet<SyntaxNode>> ThrowLocations;

            public readonly PooledHashSet<BoundCatchBlock> CatchBlockUsed;

            public void AddThrow(TypeSymbol type, SyntaxNode syntax)
            {
                PooledHashSet<SyntaxNode> nodes;
                if (!ThrowLocations.TryGetValue(type, out nodes))
                {
                    nodes = PooledHashSet<SyntaxNode>.GetInstance();
                    ThrowLocations.Add(type, nodes);
                }
                nodes.Add(syntax);
            }

            public void MergeThrowLocations(BoundBlockWithExceptions block)
            {
                foreach (var item in block.ThrowLocations)
                {
                    var throwType = item.Key;
                    PooledHashSet<SyntaxNode> nodes;
                    if (!ThrowLocations.TryGetValue(throwType, out nodes))
                    {
                        nodes = PooledHashSet<SyntaxNode>.GetInstance();
                        ThrowLocations.Add(throwType, nodes);
                    }

                    foreach (var syntax in item.Value)
                    {
                        nodes.Add(syntax);
                    }
                }
            }

            public void Free()
            {
                foreach (var item in ThrowLocations)
                {
                    item.Value.Free();
                }
                ThrowLocations.Free();
                CatchBlockUsed.Free();
            }
        }
    }
}
