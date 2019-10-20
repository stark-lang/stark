// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;

namespace StarkPlatform.Compiler.Stark
{
    /// <summary>
    /// This pass detects and reports diagnostics that do not affect lambda convertibility.
    /// This part of the partial class focuses on features that cannot be used in expression trees.
    /// CAVEAT: Errors may be produced for ObsoleteAttribute, but such errors don't affect lambda convertibility.
    /// </summary>
    internal sealed partial class DiagnosticsPass
    {
        private readonly DiagnosticBag _diagnostics;
        private readonly CSharpCompilation _compilation;
        private LocalFunctionSymbol _staticLocalFunction;
        private bool _reportedUnsafe;
        private readonly MethodSymbol _containingSymbol;

        public static void IssueDiagnostics(CSharpCompilation compilation, BoundNode node, DiagnosticBag diagnostics, MethodSymbol containingSymbol)
        {
            Debug.Assert(node != null);
            Debug.Assert((object)containingSymbol != null);

            try
            {
                var diagnosticPass = new DiagnosticsPass(compilation, diagnostics, containingSymbol);
                diagnosticPass.Visit(node);
            }
            catch (CancelledByStackGuardException ex)
            {
                ex.AddAnError(diagnostics);
            }
        }

        private DiagnosticsPass(CSharpCompilation compilation, DiagnosticBag diagnostics, MethodSymbol containingSymbol)
        {
            Debug.Assert(diagnostics != null);
            Debug.Assert((object)containingSymbol != null);

            _compilation = compilation;
            _diagnostics = diagnostics;
            _containingSymbol = containingSymbol;
        }

        private void Error(ErrorCode code, BoundNode node, params object[] args)
        {
            _diagnostics.Add(code, node.Syntax.Location, args);
        }

        public override BoundNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            var outerLocalFunction = _staticLocalFunction;
            if (node.Symbol.IsStaticLocalFunction)
            {
                _staticLocalFunction = node.Symbol;
            }
            var result = base.VisitLocalFunctionStatement(node);
            _staticLocalFunction = outerLocalFunction;
            return result;
        }

        public override BoundNode VisitThisReference(BoundThisReference node)
        {
            CheckReferenceToThisOrBase(node);
            return base.VisitThisReference(node);
        }

        public override BoundNode VisitBaseReference(BoundBaseReference node)
        {
            CheckReferenceToThisOrBase(node);
            return base.VisitBaseReference(node);
        }

        public override BoundNode VisitLocal(BoundLocal node)
        {
            CheckReferenceToVariable(node, node.LocalSymbol);
            return base.VisitLocal(node);
        }

        public override BoundNode VisitParameter(BoundParameter node)
        {
            CheckReferenceToVariable(node, node.ParameterSymbol);
            return base.VisitParameter(node);
        }

        private void CheckReferenceToThisOrBase(BoundExpression node)
        {
            if ((object)_staticLocalFunction != null)
            {
                Error(ErrorCode.ERR_StaticLocalFunctionCannotCaptureThis, node);
            }
        }

        private void CheckReferenceToVariable(BoundExpression node, Symbol symbol)
        {
            Debug.Assert(symbol.Kind == SymbolKind.Local || symbol.Kind == SymbolKind.Parameter);

            if ((object)_staticLocalFunction != null && !IsContainedIn(_staticLocalFunction, symbol))
            {
                Error(ErrorCode.ERR_StaticLocalFunctionCannotCaptureVariable, node, new FormattedSymbol(symbol, SymbolDisplayFormat.ShortFormat));
            }
        }

        private static bool IsContainedIn(LocalFunctionSymbol container, Symbol symbol)
        {
            Debug.Assert((object)container != null);
            Debug.Assert(container != symbol);
            while (true)
            {
                var containingSymbol = symbol.ContainingSymbol;
                if (containingSymbol is null)
                {
                    return false;
                }
                if (container == containingSymbol)
                {
                    return true;
                }
                symbol = containingSymbol;
            }
        }

        public override BoundNode VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            if (!node.HasAnyErrors)
            {
                CheckForDeconstructionAssignmentToSelf((BoundTupleLiteral)node.Left, node.Right);
            }

            return base.VisitDeconstructionAssignmentOperator(node);
        }

        public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            CheckForAssignmentToSelf(node);
            return base.VisitAssignmentOperator(node);
        }

        public override BoundNode VisitEventAccess(BoundEventAccess node)
        {
            // Don't bother reporting an obsolete diagnostic if the access is already wrong for other reasons
            // (specifically, we can't use it as a field here).
            if (node.IsUsableAsField)
            {
                bool hasBaseReceiver = node.ReceiverOpt != null && node.ReceiverOpt.Kind == BoundKind.BaseReference;
                Binder.ReportDiagnosticsIfObsolete(_diagnostics, node.EventSymbol.AssociatedField, node.Syntax, hasBaseReceiver, _containingSymbol, _containingSymbol.ContainingType, BinderFlags.None);
            }
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitEventAccess(node);
        }

        public override BoundNode VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            bool hasBaseReceiver = node.ReceiverOpt != null && node.ReceiverOpt.Kind == BoundKind.BaseReference;
            Binder.ReportDiagnosticsIfObsolete(_diagnostics, node.Event, ((AssignmentExpressionSyntax)node.Syntax).Left, hasBaseReceiver, _containingSymbol, _containingSymbol.ContainingType, BinderFlags.None);
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitEventAssignmentOperator(node);
        }

        public override BoundNode VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            CheckCompoundAssignmentOperator(node);

            return base.VisitCompoundAssignmentOperator(node);
        }

        private void VisitCall(
            MethodSymbol method,
            PropertySymbol propertyAccess,
            ImmutableArray<BoundExpression> arguments,
            ImmutableArray<RefKind> argumentRefKindsOpt,
            ImmutableArray<string> argumentNamesOpt,
            bool expanded,
            BoundNode node)
        {
            Debug.Assert((object)method != null);
            Debug.Assert(((object)propertyAccess == null) ||
                (method == propertyAccess.GetOwnOrInheritedGetMethod()) ||
                (method == propertyAccess.GetOwnOrInheritedSetMethod()) ||
                propertyAccess.MustCallMethodsDirectly);

            CheckArguments(argumentRefKindsOpt, arguments, method);
        }

        public override BoundNode VisitCall(BoundCall node)
        {
            VisitCall(node.Method, null, node.Arguments, node.ArgumentRefKindsOpt, node.ArgumentNamesOpt, node.Expanded, node);
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitCall(node);
        }

        public override BoundNode VisitCollectionElementInitializer(BoundCollectionElementInitializer node)
        {
            VisitCall(node.AddMethod, null, node.Arguments, default(ImmutableArray<RefKind>), default(ImmutableArray<string>), node.Expanded, node);
            return base.VisitCollectionElementInitializer(node);
        }

        public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            VisitCall(node.Constructor, null, node.Arguments, node.ArgumentRefKindsOpt, node.ArgumentNamesOpt, node.Expanded, node);
            return base.VisitObjectCreationExpression(node);
        }

        public override BoundNode VisitIndexerAccess(BoundIndexerAccess node)
        {
            var indexer = node.Indexer;
            var method = indexer.GetOwnOrInheritedGetMethod() ?? indexer.GetOwnOrInheritedSetMethod();
            if ((object)method != null)
            {
                VisitCall(method, indexer, node.Arguments, node.ArgumentRefKindsOpt, node.ArgumentNamesOpt, node.Expanded, node);
            }
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitIndexerAccess(node);
        }

        public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
        {
            var property = node.PropertySymbol;
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitPropertyAccess(node);
        }

        public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
        {
            // It is very common for bound trees to be left-heavy binary operators, eg,
            // a + b + c + d + ...
            // To avoid blowing the stack, do not recurse down the left hand side.

            // In order to avoid blowing the stack, we end up visiting right children
            // before left children; this should not be a problem in the diagnostics 
            // pass.

            BoundBinaryOperator current = node;
            while (true)
            {
                CheckBinaryOperator(current);

                Visit(current.Right);
                if (current.Left.Kind == BoundKind.BinaryOperator)
                {
                    current = (BoundBinaryOperator)current.Left;
                }
                else
                {
                    Visit(current.Left);
                    break;
                }
            }

            return null;
        }

        public override BoundNode VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node)
        {
            CheckLiftedUserDefinedConditionalLogicalOperator(node);
            return base.VisitUserDefinedConditionalLogicalOperator(node);
        }

        public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
        {
            CheckLiftedUnaryOp(node);
            return base.VisitUnaryOperator(node);
        }

        public override BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            BoundExpression operand = node.Operand;
            if (operand.Kind == BoundKind.FieldAccess)
            {
                CheckFieldAddress((BoundFieldAccess)operand, consumerOpt: null);
            }
            return base.VisitAddressOfOperator(node);
        }
        
        public override BoundNode VisitConversion(BoundConversion node)
        {
            bool oldReportedUnsafe = _reportedUnsafe;
            switch (node.ConversionKind)
            {
                case ConversionKind.MethodGroup:
                    VisitMethodGroup((BoundMethodGroup)node.Operand, parentIsConversion: true);
                    return node;

                case ConversionKind.AnonymousFunction:
                    break;

                case ConversionKind.ExplicitTuple:
                case ConversionKind.ExplicitTupleLiteral:
                case ConversionKind.ImplicitTuple:
                case ConversionKind.ImplicitTupleLiteral:
                    break;

                default:
                    break;
            }

            var result = base.VisitConversion(node);
            _reportedUnsafe = oldReportedUnsafe;
            return result;
        }

        public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            if (node.Argument.Kind != BoundKind.MethodGroup)
            {
                this.Visit(node.Argument);
            }

            return null;
        }

        public override BoundNode VisitMethodGroup(BoundMethodGroup node)
        {
            return VisitMethodGroup(node, parentIsConversion: false);
        }

        private BoundNode VisitMethodGroup(BoundMethodGroup node, bool parentIsConversion)
        {
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitMethodGroup(node);
        }

        public override BoundNode VisitNameOfOperator(BoundNameOfOperator node)
        {
            // The nameof(...) operator collapses to a constant in an expression tree,
            // so it does not matter what is recursively within it.
            return node;
        }
    }
}
