// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal partial class DebugInfoInjector
    {
        private BoundStatement AddSequencePoint(BoundStatement node)
        {
            return new BoundSequencePoint(node.Syntax, node);
        }

        internal static BoundStatement AddSequencePoint(VariableDeclarationSyntax declaratorSyntax, BoundStatement rewrittenStatement)
        {
            SyntaxNode node;
            TextSpan? part;
            GetBreakpointSpan(declaratorSyntax, out node, out part);
            var result = BoundSequencePoint.Create(declaratorSyntax, part, rewrittenStatement);
            result.WasCompilerGenerated = rewrittenStatement.WasCompilerGenerated;
            return result;
        }

        internal static BoundStatement AddSequencePoint(PropertyDeclarationSyntax declarationSyntax, BoundStatement rewrittenStatement)
        {
            Debug.Assert(declarationSyntax.Initializer != null);
            int start = declarationSyntax.Initializer.Value.SpanStart;
            int end = declarationSyntax.Initializer.Span.End;
            TextSpan part = TextSpan.FromBounds(start, end);

            var result = BoundSequencePoint.Create(declarationSyntax, part, rewrittenStatement);
            result.WasCompilerGenerated = rewrittenStatement.WasCompilerGenerated;
            return result;
        }

        internal static BoundStatement AddSequencePoint(UsingStatementSyntax usingSyntax, BoundStatement rewrittenStatement)
        {
            int start = usingSyntax.Span.Start;
            int end = usingSyntax.CloseParenToken.Span.End;
            TextSpan span = TextSpan.FromBounds(start, end);
            return new BoundSequencePointWithSpan(usingSyntax, rewrittenStatement, span);
        }

        private static TextSpan CreateSpanForConstructorInitializer(ConstructorDeclarationSyntax constructorSyntax)
        {
            if (constructorSyntax.Initializer != null)
            {
                //  [SomeAttribute] public MyCtorName(params int[] values): [|base()|] { ... }
                var start = constructorSyntax.Initializer.ThisOrBaseKeyword.SpanStart;
                var end = constructorSyntax.Initializer.ArgumentList.CloseParenToken.Span.End;
                return TextSpan.FromBounds(start, end);
            }

            if (constructorSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                // [SomeAttribute] static MyCtorName(...) [|{|] ... }
                var start = constructorSyntax.Body.OpenBraceToken.SpanStart;
                var end = constructorSyntax.Body.OpenBraceToken.Span.End;
                return TextSpan.FromBounds(start, end);
            }

            //  [SomeAttribute] [|public MyCtorName(params int[] values)|] { ... }
            return CreateSpan(constructorSyntax.Modifiers, constructorSyntax.ConstructorKeyword, constructorSyntax.ParameterList.CloseParenToken);
        }

        private static TextSpan CreateSpan(SyntaxTokenList startOpt, SyntaxNodeOrToken startFallbackOpt, SyntaxNodeOrToken endOpt)
        {
            Debug.Assert(startFallbackOpt != default(SyntaxNodeOrToken) || endOpt != default(SyntaxNodeOrToken));

            int startPos;
            if (startOpt.Count > 0)
            {
                startPos = startOpt.First().SpanStart;
            }
            else if (startFallbackOpt != default(SyntaxNodeOrToken))
            {
                startPos = startFallbackOpt.SpanStart;
            }
            else
            {
                startPos = endOpt.SpanStart;
            }

            int endPos;
            if (endOpt != default(SyntaxNodeOrToken))
            {
                endPos = GetEndPosition(endOpt);
            }
            else
            {
                endPos = GetEndPosition(startFallbackOpt);
            }

            return TextSpan.FromBounds(startPos, endPos);
        }

        private static int GetEndPosition(SyntaxNodeOrToken nodeOrToken)
        {
            if (nodeOrToken.IsToken)
            {
                return nodeOrToken.Span.End;
            }
            else
            {
                return nodeOrToken.AsNode().GetLastToken().Span.End;
            }
        }

        internal static void GetBreakpointSpan(VariableDeclarationSyntax declarationSyntax, out SyntaxNode node, out TextSpan? part)
        {
            switch (declarationSyntax.Parent.Kind())
            {
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.FieldDeclaration:
                    var modifiers = ((BaseFieldDeclarationSyntax)declarationSyntax.Parent).Modifiers;
                    GetFirstLocalOrFieldBreakpointSpan(modifiers, declarationSyntax, out node, out part);
                    break;

                case SyntaxKind.LocalDeclarationStatement:
                    // only const locals have modifiers and those don't have a sequence point:
                    Debug.Assert(!((LocalDeclarationStatementSyntax)declarationSyntax.Parent).IsConst);
                    GetFirstLocalOrFieldBreakpointSpan(default(SyntaxTokenList), declarationSyntax, out node, out part);
                    break;

                case SyntaxKind.UsingStatement:
                case SyntaxKind.FixedStatement:
                    // for ([|int i = 1|]; i < 10; i++)
                    // for ([|int i = 1|], j = 0; i < 10; i++)
                    node = declarationSyntax;
                    part = TextSpan.FromBounds(declarationSyntax.SpanStart, declarationSyntax.Span.End);
                    break;

                default:
                    throw ExceptionUtilities.UnexpectedValue(declarationSyntax.Parent.Kind());
            }
        }

        internal static void GetFirstLocalOrFieldBreakpointSpan(SyntaxTokenList modifiers, VariableDeclarationSyntax declarationSyntax, out SyntaxNode node, out TextSpan? part)
        {
            int start = modifiers.Any() ? modifiers[0].SpanStart : declarationSyntax.SpanStart;

            int end;
            // [|int x = 1;|]
            // [|public static int x = 1;|]
            end = declarationSyntax.Parent.Span.End;

            part = TextSpan.FromBounds(start, end);
            node = declarationSyntax.Parent;
        }

        private static BoundExpression AddConditionSequencePoint(BoundExpression condition, SyntaxNode synthesizedVariableSyntax, SyntheticBoundNodeFactory factory)
        {
            if (!factory.Compilation.Options.EnableEditAndContinue)
            {
                return condition;
            }

            // The local has to be associated with a syntax that is tracked by EnC source mapping.
            // At most one ConditionalBranchDiscriminator variable shall be associated with any given EnC tracked syntax node.
            var local = factory.SynthesizedLocal(condition.Type, synthesizedVariableSyntax, kind: SynthesizedLocalKind.ConditionalBranchDiscriminator);

            // Add hidden sequence point unless the condition is a constant expression.
            // Constant expression must stay a const to not invalidate results of control flow analysis.
            var valueExpression = (condition.ConstantValue == null) ?
                new BoundSequencePointExpression(syntax: null, expression: factory.Local(local), type: condition.Type) :
                condition;

            return new BoundSequence(
                condition.Syntax,
                ImmutableArray.Create(local),
                ImmutableArray.Create<BoundExpression>(factory.AssignmentExpression(factory.Local(local), condition)),
                valueExpression,
                condition.Type);
        }
    }
}
