// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Text;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark.Syntax.InternalSyntax
{
    using StarkPlatform.Compiler.Syntax.InternalSyntax;

    internal partial class LanguageParser : SyntaxParser
    {
        // list pools - allocators for lists that are used to build sequences of nodes. The lists
        // can be reused (hence pooled) since the syntax factory methods don't keep references to
        // them

        private readonly SyntaxListPool _pool = new SyntaxListPool(); // Don't need to reset this.

        private readonly SyntaxFactoryContext _syntaxFactoryContext; // Fields are resettable.
        private readonly ContextAwareSyntax _syntaxFactory; // Has context, the fields of which are resettable.

        private int _recursionDepth;
        private TerminatorState _termState; // Resettable
        private bool _isInTry; // Resettable

        // NOTE: If you add new state, you should probably add it to ResetPoint as well.

        internal LanguageParser(
            Lexer lexer,
            Stark.CSharpSyntaxNode oldTree,
            IEnumerable<TextChangeRange> changes,
            LexerMode lexerMode = LexerMode.Syntax,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(lexer, lexerMode, oldTree, changes, allowModeReset: false,
                preLexIfNotIncremental: true, cancellationToken: cancellationToken)
        {
            _syntaxFactoryContext = new SyntaxFactoryContext();
            _syntaxFactory = new ContextAwareSyntax(_syntaxFactoryContext);
        }

        private static bool IsSomeWord(SyntaxKind kind)
        {
            return kind == SyntaxKind.IdentifierToken || SyntaxFacts.IsKeywordKind(kind);
        }

        // Parsing rule terminating conditions.  This is how we know if it is 
        // okay to abort the current parsing rule when unexpected tokens occur.

        [Flags]
        internal enum TerminatorState
        {
            EndOfFile = 0,
            IsNamespaceMemberStartOrStop = 1 << 0,
            IsPossibleAggregateClauseStartOrStop = 1 << 2,
            IsPossibleMemberStartOrStop = 1 << 3,
            IsEndOfReturnType = 1 << 4,
            IsEndOfParameterList = 1 << 5,
            IsEndOfFieldDeclaration = 1 << 6,
            IsPossibleEndOfVariableDeclaration = 1 << 7,
            IsEndOfTypeArgumentList = 1 << 8,
            IsPossibleStatementStartOrStop = 1 << 9,
            IsEndOfFixedStatement = 1 << 10,
            IsEndOfTryBlock = 1 << 11,
            IsEndOfCatchClause = 1 << 12,
            IsEndOfilterClause = 1 << 13,
            IsEndOfCatchBlock = 1 << 14,
            IsEndOfDoWhileExpression = 1 << 15,
            IsEndOfForStatementArgument = 1 << 16,
            IsEndOfDeclarationClause = 1 << 17,
            IsEndOfArgumentList = 1 << 18,
            IsSwitchSectionStart = 1 << 19,
            IsEndOfTypeParameterList = 1 << 20,
            IsEndOfMethodSignature = 1 << 21,
            IsEndOfNameInExplicitInterface = 1 << 22,
            IsEndOfIfBlock = 1 << 23,
        }

        private const int LastTerminatorState = (int)TerminatorState.IsEndOfNameInExplicitInterface;

        private bool IsTerminator()
        {
            if (this.CurrentToken.Kind == SyntaxKind.EndOfFileToken)
            {
                return true;
            }

            for (int i = 1; i <= LastTerminatorState; i <<= 1)
            {
                TerminatorState isolated = _termState & (TerminatorState)i;
                if (isolated != 0)
                {
                    switch (isolated)
                    {
                        case TerminatorState.IsNamespaceMemberStartOrStop:
                            if (this.IsNamespaceMemberStartOrStop())
                            {
                                return true;
                            }
                            break;

                        case TerminatorState.IsPossibleAggregateClauseStartOrStop:
                            if (this.IsPossibleAggregateClauseStartOrStop())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsPossibleMemberStartOrStop:
                            if (this.IsPossibleMemberStartOrStop())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfReturnType:
                            if (this.IsEndOfReturnType())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfParameterList:
                            if (this.IsEndOfParameterList())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfFieldDeclaration:
                            if (this.IsEndOfFieldDeclaration())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsPossibleEndOfVariableDeclaration:
                            if (this.IsPossibleEndOfVariableDeclaration())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfTypeArgumentList:
                            if (this.IsEndOfTypeArgumentList())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsPossibleStatementStartOrStop:
                            if (this.IsPossibleStatementStartOrStop())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfFixedStatement:
                            if (this.IsEndOfFixedStatement())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfTryBlock:
                            if (this.IsEndOfTryBlock())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfCatchClause:
                            if (this.IsEndOfCatchClause())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfIfBlock:
                            if (this.IsEndOfIfBlock())
                            {
                                return true;
                            }

                            break;

                        case TerminatorState.IsEndOfilterClause:
                            if (this.IsEndOfFilterClause())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfCatchBlock:
                            if (this.IsEndOfCatchBlock())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfDoWhileExpression:
                            if (this.IsEndOfDoWhileExpression())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfForStatementArgument:
                            if (this.IsEndOfForStatementArgument())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfDeclarationClause:
                            if (this.IsEndOfDeclarationClause())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsEndOfArgumentList:
                            if (this.IsEndOfArgumentList())
                            {
                                return true;
                            }

                            break;
                        case TerminatorState.IsSwitchSectionStart:
                            if (this.IsPossibleSwitchSection())
                            {
                                return true;
                            }

                            break;

                        case TerminatorState.IsEndOfTypeParameterList:
                            if (this.IsEndOfTypeParameterList())
                            {
                                return true;
                            }

                            break;

                        case TerminatorState.IsEndOfMethodSignature:
                            if (this.IsEndOfMethodSignature())
                            {
                                return true;
                            }

                            break;

                        case TerminatorState.IsEndOfNameInExplicitInterface:
                            if (this.IsEndOfNameInExplicitInterface())
                            {
                                return true;
                            }

                            break;
                    }
                }
            }

            return false;
        }

        private static Stark.CSharpSyntaxNode GetOldParent(Stark.CSharpSyntaxNode node)
        {
            return node != null ? node.Parent : null;
        }

        private struct NamespaceBodyBuilder
        {
            public SyntaxListBuilder<ExternAliasDirectiveSyntax> Externs;
            public SyntaxListBuilder<ImportDirectiveSyntax> Imports;
            public SyntaxListBuilder<AttributeSyntax> Attributes;
            public SyntaxListBuilder<MemberDeclarationSyntax> Members;

            public NamespaceBodyBuilder(SyntaxListPool pool)
            {
                Externs = pool.Allocate<ExternAliasDirectiveSyntax>();
                Imports = pool.Allocate<ImportDirectiveSyntax>();
                Attributes = pool.Allocate<AttributeSyntax>();
                Members = pool.Allocate<MemberDeclarationSyntax>();
            }

            internal void Free(SyntaxListPool pool)
            {
                pool.Free(Members);
                pool.Free(Attributes);
                pool.Free(Imports);
                pool.Free(Externs);
            }
        }

        internal CompilationUnitSyntax ParseCompilationUnit()
        {
            return ParseWithStackGuard(
                ParseCompilationUnitCore,
                () => SyntaxFactory.CompilationUnit(
                        new SyntaxList<ExternAliasDirectiveSyntax>(),
                        new SyntaxList<ImportDirectiveSyntax>(),
                        new SyntaxList<AttributeSyntax>(),
                        new SyntaxList<MemberDeclarationSyntax>(),
                        SyntaxFactory.Token(SyntaxKind.EndOfFileToken)));
        }

        internal CompilationUnitSyntax ParseCompilationUnitCore()
        {
            SyntaxToken tmp = null;
            SyntaxListBuilder initialBadNodes = null;
            var body = new NamespaceBodyBuilder(_pool);
            try
            {
                this.ParseNamespaceBody(ref tmp, ref body, ref initialBadNodes, SyntaxKind.CompilationUnit);

                var eof = this.EatToken(SyntaxKind.EndOfFileToken);
                var result = _syntaxFactory.CompilationUnit(body.Externs, body.Imports, body.Attributes, body.Members, eof);

                if (initialBadNodes != null)
                {
                    // attach initial bad nodes as leading trivia on first token
                    result = AddLeadingSkippedSyntax(result, initialBadNodes.ToListNode());
                    _pool.Free(initialBadNodes);
                }

                return result;
            }
            finally
            {
                body.Free(_pool);
            }
        }

        internal TNode ParseWithStackGuard<TNode>(Func<TNode> parseFunc, Func<TNode> createEmptyNodeFunc) where TNode : CSharpSyntaxNode
        {
            // If this value is non-zero then we are nesting calls to ParseWithStackGuard which should not be 
            // happening.  It's not a bug but it's inefficient and should be changed.
            Debug.Assert(_recursionDepth == 0);

            try
            {
                return parseFunc();
            }
            catch (InsufficientExecutionStackException)
            {
                return CreateForGlobalFailure(lexer.TextWindow.Position, createEmptyNodeFunc());
            }
        }

        private TNode CreateForGlobalFailure<TNode>(int position, TNode node) where TNode : CSharpSyntaxNode
        {
            // Turn the complete input into a single skipped token. This avoids running the lexer, and therefore
            // the preprocessor directive parser, which may itself run into the same problem that caused the
            // original failure.
            var builder = new SyntaxListBuilder(1);
            builder.Add(SyntaxFactory.BadToken(null, lexer.TextWindow.Text.ToString(), null));
            var fileAsTrivia = _syntaxFactory.SkippedTokensTrivia(builder.ToList<SyntaxToken>());
            node = AddLeadingSkippedSyntax(node, fileAsTrivia);
            ForceEndOfFile(); // force the scanner to report that it is at the end of the input.
            return AddError(node, position, 0, ErrorCode.ERR_InsufficientStack);
        }

        private NamespaceDeclarationSyntax ParseNamespaceDeclaration()
        {
            _recursionDepth++;
            StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
            var result = ParseNamespaceDeclarationCore();
            _recursionDepth--;
            return result;
        }

        private NamespaceDeclarationSyntax ParseNamespaceDeclarationCore()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.NamespaceDeclaration)
            {
                return (NamespaceDeclarationSyntax)this.EatNode();
            }

            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.NamespaceKeyword);
            var namespaceToken = this.EatToken(SyntaxKind.NamespaceKeyword);

            if (IsScript)
            {
                namespaceToken = this.AddError(namespaceToken, ErrorCode.ERR_NamespaceNotAllowedInScript);
            }

            var name = this.ParseQualifiedName();

            SyntaxToken openBrace;
            if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken || IsPossibleNamespaceMemberDeclaration())
            {
                //either we see the brace we expect here or we see something that could come after a brace
                //so we insert a missing one
                openBrace = this.EatToken(SyntaxKind.OpenBraceToken);
            }
            else
            {
                //the next character is neither the brace we expect, nor a token that could follow the expected
                //brace so we assume it's a mistake and replace it with a missing brace 
                openBrace = this.EatTokenWithPrejudice(SyntaxKind.OpenBraceToken);
                openBrace = this.ConvertToMissingWithTrailingTrivia(openBrace, SyntaxKind.OpenBraceToken);
            }

            var body = new NamespaceBodyBuilder(_pool);
            SyntaxListBuilder initialBadNodes = null;
            try
            {
                this.ParseNamespaceBody(ref openBrace, ref body, ref initialBadNodes, SyntaxKind.NamespaceDeclaration);

                var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);
                Debug.Assert(initialBadNodes == null); // init bad nodes should have been attached to open brace...
                return _syntaxFactory.NamespaceDeclaration(namespaceToken, name, openBrace, body.Externs, body.Imports, body.Members, closeBrace);
            }
            finally
            {
                body.Free(_pool);
            }
        }

        private static bool IsPossibleStartOfTypeDeclaration(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.EnumKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.ModuleKeyword:
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.StructKeyword:
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.SealedKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.UnsafeKeyword:
                case SyntaxKind.AtToken:
                    return true;
                default:
                    return false;
            }
        }

        private void AddSkippedNamespaceText(
            ref SyntaxToken openBrace,
            ref NamespaceBodyBuilder body,
            ref SyntaxListBuilder initialBadNodes,
            CSharpSyntaxNode skippedSyntax)
        {
            if (body.Members.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Members, skippedSyntax);
            }
            else if (body.Attributes.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Attributes, skippedSyntax);
            }
            else if (body.Imports.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Imports, skippedSyntax);
            }
            else if (body.Externs.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Externs, skippedSyntax);
            }
            else if (openBrace != null)
            {
                openBrace = AddTrailingSkippedSyntax(openBrace, skippedSyntax);
            }
            else
            {
                if (initialBadNodes == null)
                {
                    initialBadNodes = _pool.Allocate();
                }

                initialBadNodes.AddRange(skippedSyntax);
            }
        }

        // Parts of a namespace declaration in the order they can be defined.
        private enum NamespaceParts
        {
            None = 0,
            ExternAliases = 1,
            Imports = 2,
            GlobalAttributes = 3,
            MembersAndStatements = 4,
        }

        private void ParseNamespaceBody(ref SyntaxToken openBrace, ref NamespaceBodyBuilder body, ref SyntaxListBuilder initialBadNodes, SyntaxKind parentKind)
        {
            // "top-level" expressions and statements should never occur inside an asynchronous context
            Debug.Assert(!IsInAsync);

            bool isGlobal = openBrace == null;
            bool isGlobalScript = isGlobal && this.IsScript;

            var saveTerm = _termState;
            _termState |= TerminatorState.IsNamespaceMemberStartOrStop;
            NamespaceParts seen = NamespaceParts.None;
            var pendingIncompleteMembers = _pool.Allocate<MemberDeclarationSyntax>();
            bool reportUnexpectedToken = true;

            try
            {
                while (true)
                {
                    switch (this.CurrentToken.Kind)
                    {
                        case SyntaxKind.NamespaceKeyword:
                            // incomplete members must be processed before we add any nodes to the body:
                            AddIncompleteMembers(ref pendingIncompleteMembers, ref body);

                            body.Members.Add(this.ParseNamespaceDeclaration());
                            seen = NamespaceParts.MembersAndStatements;
                            reportUnexpectedToken = true;
                            break;

                        case SyntaxKind.CloseBraceToken:
                            // A very common user error is to type an additional } 
                            // somewhere in the file.  This will cause us to stop parsing
                            // the root (global) namespace too early and will make the 
                            // rest of the file unparseable and unusable by intellisense.
                            // We detect that case here and we skip the close curly and
                            // continue parsing as if we did not see the }
                            if (isGlobal)
                            {
                                // incomplete members must be processed before we add any nodes to the body:
                                ReduceIncompleteMembers(ref pendingIncompleteMembers, ref openBrace, ref body, ref initialBadNodes);

                                var token = this.EatToken();
                                token = this.AddError(token,
                                    IsScript ? ErrorCode.ERR_GlobalDefinitionOrStatementExpected : ErrorCode.ERR_EOFExpected);

                                this.AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, token);
                                reportUnexpectedToken = true;
                                break;
                            }
                            else
                            {
                                // This token marks the end of a namespace body
                                return;
                            }

                        case SyntaxKind.EndOfFileToken:
                            // This token marks the end of a namespace body
                            return;

                        case SyntaxKind.ExternKeyword:
                            if (isGlobalScript && !ScanExternAliasDirective())
                            {
                                // extern member
                                goto default;
                            }
                            else
                            {
                                // incomplete members must be processed before we add any nodes to the body:
                                ReduceIncompleteMembers(ref pendingIncompleteMembers, ref openBrace, ref body, ref initialBadNodes);

                                var @extern = ParseExternAliasDirective();
                                if (seen > NamespaceParts.ExternAliases)
                                {
                                    @extern = this.AddErrorToFirstToken(@extern, ErrorCode.ERR_ExternAfterElements);
                                    this.AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, @extern);
                                }
                                else
                                {
                                    body.Externs.Add(@extern);
                                    seen = NamespaceParts.ExternAliases;
                                }

                                reportUnexpectedToken = true;
                                break;
                            }

                        case SyntaxKind.UsingKeyword:
                            if (isGlobalScript && this.PeekToken(1).Kind == SyntaxKind.OpenParenToken)
                            {
                                // incomplete members must be processed before we add any nodes to the body:
                                AddIncompleteMembers(ref pendingIncompleteMembers, ref body);

                                body.Members.Add(_syntaxFactory.GlobalStatement(ParseUsingStatement()));
                                seen = NamespaceParts.MembersAndStatements;
                            }
                            reportUnexpectedToken = true;
                            break;

                        case SyntaxKind.ImportKeyword:
                            // incomplete members must be processed before we add any nodes to the body:
                            ReduceIncompleteMembers(ref pendingIncompleteMembers, ref openBrace, ref body, ref initialBadNodes);

                            var @using = this.ParseImportDirective();
                            if (seen > NamespaceParts.Imports)
                            {
                                @using = this.AddError(@using, ErrorCode.ERR_UsingAfterElements);
                                this.AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, @using);
                            }
                            else
                            {
                                body.Imports.Add(@using);
                                seen = NamespaceParts.Imports;
                            }

                            reportUnexpectedToken = true;
                            break;

                        case SyntaxKind.AtToken:
                            if (this.IsPossibleGlobalAttributeDeclaration())
                            {
                                // incomplete members must be processed before we add any nodes to the body:
                                ReduceIncompleteMembers(ref pendingIncompleteMembers, ref openBrace, ref body, ref initialBadNodes);

                                var attribute = this.ParseAttribute();
                                if (!isGlobal || seen > NamespaceParts.GlobalAttributes)
                                {
                                    // TODO: FIX global attribute error
                                    attribute = this.AddError(attribute, attribute.Target.Identifier, ErrorCode.ERR_GlobalAttributesNotFirst);
                                    this.AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, attribute);
                                }
                                else
                                {
                                    body.Attributes.Add(attribute);
                                    seen = NamespaceParts.GlobalAttributes;
                                }

                                reportUnexpectedToken = true;
                                break;
                            }

                            goto default;

                        default:
                            var memberOrStatement = this.ParseMemberDeclarationOrStatement(parentKind);
                            if (memberOrStatement == null)
                            {
                                // incomplete members must be processed before we add any nodes to the body:
                                ReduceIncompleteMembers(ref pendingIncompleteMembers, ref openBrace, ref body, ref initialBadNodes);

                                // eat one token and try to parse declaration or statement again:
                                var skippedToken = EatToken();
                                if (reportUnexpectedToken && !skippedToken.ContainsDiagnostics)
                                {
                                    skippedToken = this.AddError(skippedToken,
                                        IsScript ? ErrorCode.ERR_GlobalDefinitionOrStatementExpected : ErrorCode.ERR_EOFExpected);

                                    // do not report the error multiple times for subsequent tokens:
                                    reportUnexpectedToken = false;
                                }

                                this.AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, skippedToken);
                            }
                            else if (memberOrStatement.Kind == SyntaxKind.IncompleteMember && seen < NamespaceParts.MembersAndStatements)
                            {
                                pendingIncompleteMembers.Add(memberOrStatement);
                                reportUnexpectedToken = true;
                            }
                            else
                            {
                                // incomplete members must be processed before we add any nodes to the body:
                                AddIncompleteMembers(ref pendingIncompleteMembers, ref body);

                                body.Members.Add(memberOrStatement);
                                seen = NamespaceParts.MembersAndStatements;
                                reportUnexpectedToken = true;
                            }
                            break;
                    }
                }
            }
            finally
            {
                _termState = saveTerm;

                // adds pending incomplete nodes:
                AddIncompleteMembers(ref pendingIncompleteMembers, ref body);
                _pool.Free(pendingIncompleteMembers);
            }
        }

        private static void AddIncompleteMembers(ref SyntaxListBuilder<MemberDeclarationSyntax> incompleteMembers, ref NamespaceBodyBuilder body)
        {
            if (incompleteMembers.Count > 0)
            {
                body.Members.AddRange(incompleteMembers);
                incompleteMembers.Clear();
            }
        }

        private void ReduceIncompleteMembers(ref SyntaxListBuilder<MemberDeclarationSyntax> incompleteMembers,
            ref SyntaxToken openBrace, ref NamespaceBodyBuilder body, ref SyntaxListBuilder initialBadNodes)
        {
            for (int i = 0; i < incompleteMembers.Count; i++)
            {
                this.AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, incompleteMembers[i]);
            }
            incompleteMembers.Clear();
        }

        private bool IsPossibleNamespaceMemberDeclaration()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.ExternKeyword:
                case SyntaxKind.ImportKeyword:
                case SyntaxKind.NamespaceKeyword:
                    return true;
                case SyntaxKind.IdentifierToken:
                    return IsPartialInNamespaceMemberDeclaration();
                default:
                    return IsPossibleStartOfTypeDeclaration(this.CurrentToken.Kind);
            }
        }

        private bool IsPartialInNamespaceMemberDeclaration()
        {
            if (this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword)
            {
                if (this.IsPartialType())
                {
                    return true;
                }
                else if (this.PeekToken(1).Kind == SyntaxKind.NamespaceKeyword)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsEndOfNamespace()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseBraceToken;
        }

        public bool IsGobalAttributesTerminator()
        {
            return this.IsEndOfNamespace()
                || this.IsPossibleNamespaceMemberDeclaration();
        }

        private bool IsNamespaceMemberStartOrStop()
        {
            return this.IsEndOfNamespace()
                || this.IsPossibleNamespaceMemberDeclaration();
        }

        /// <summary>
        /// Returns true if the lookahead tokens compose extern alias directive.
        /// </summary>
        private bool ScanExternAliasDirective()
        {
            // The check also includes the ending semicolon so that we can disambiguate among:
            //   extern alias goo;
            //   extern alias goo();
            //   extern alias goo { get; }

            return this.CurrentToken.Kind == SyntaxKind.ExternKeyword
                && this.PeekToken(1).Kind == SyntaxKind.IdentifierToken && this.PeekToken(1).ContextualKind == SyntaxKind.AliasKeyword
                && this.PeekToken(2).Kind == SyntaxKind.IdentifierToken
                && this.PeekToken(3).Kind == SyntaxKind.SemicolonToken;
        }

        private ExternAliasDirectiveSyntax ParseExternAliasDirective()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.ExternAliasDirective)
            {
                return (ExternAliasDirectiveSyntax)this.EatNode();
            }

            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ExternKeyword);

            var externToken = this.EatToken(SyntaxKind.ExternKeyword);
            var aliasToken = this.EatContextualToken(SyntaxKind.AliasKeyword);
            externToken = CheckFeatureAvailability(externToken, MessageID.IDS_FeatureExternAlias);

            var name = this.ParseIdentifierToken();

            var eos = this.EatEos(ref name);

            return _syntaxFactory.ExternAliasDirective(externToken, aliasToken, name, eos);
        }

        private NameEqualsSyntax ParseNameEquals()
        {
            Debug.Assert(this.IsNamedAssignment());
            return _syntaxFactory.NameEquals(
                _syntaxFactory.IdentifierName(this.ParseIdentifierToken()),
                this.EatToken(SyntaxKind.EqualsToken));
        }

        private ImportDirectiveSyntax ParseImportDirective()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.ImportDirective)
            {
                return (ImportDirectiveSyntax)this.EatNode();
            }

            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ImportKeyword);

            var usingToken = this.EatToken(SyntaxKind.ImportKeyword);

            var staticToken = default(SyntaxToken);
            if (this.CurrentToken.Kind == SyntaxKind.StaticKeyword)
            {
                staticToken = this.EatToken(SyntaxKind.StaticKeyword);
            }

            var alias = this.IsNamedAssignment() ? ParseNameEquals() : null;

            NameSyntax name;
            SyntaxToken eos;

            if (IsPossibleNamespaceMemberDeclaration())
            {
                //We're worried about the case where someone already has a correct program
                //and they've gone back to add a using directive, but have not finished the
                //new directive.  e.g.
                //
                //    using 
                //    namespace Goo {
                //        //...
                //    }
                //
                //If the token we see after "using" could be its own top-level construct, then
                //we just want to insert a missing identifier and semicolon and then return to
                //parsing at the top-level.
                //
                //NB: there's no way this could be true for a set of tokens that form a valid 
                //using directive, so there's no danger in checking the error case first.

                name = WithAdditionalDiagnostics(CreateMissingIdentifierName(), GetExpectedTokenError(SyntaxKind.IdentifierToken, this.CurrentToken.Kind));
                eos = SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken);
            }
            else
            {
                name = this.ParseQualifiedName();
                // Eat End-Of-Statement: an end of line or a semi-colon
                eos = EatEos(ref name);
            }

            var usingDirective = _syntaxFactory.ImportDirective(usingToken, staticToken, alias, name, eos);
            if (staticToken != default(SyntaxToken))
            {
                usingDirective = CheckFeatureAvailability(usingDirective, MessageID.IDS_FeatureUsingStatic);
            }

            return usingDirective;
        }

        private bool IsPossibleGlobalAttributeDeclaration()
        {
            return this.CurrentToken.Kind == SyntaxKind.AtToken
                && IsGlobalAttributeTarget(this.PeekToken(1))
                && this.PeekToken(2).Kind == SyntaxKind.ColonToken;
        }

        private static bool IsGlobalAttributeTarget(SyntaxToken token)
        {
            switch (token.ToAttributeLocation())
            {
                case AttributeLocation.Assembly:
                case AttributeLocation.Module:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsPossibleAttributeDeclaration()
        {
            return this.CurrentToken.Kind == SyntaxKind.AtToken;
        }

        private void ParseAttributeSyntaxList(SyntaxListBuilder list)
        {
            while (this.IsPossibleAttributeDeclaration())
            {
                list.Add(this.ParseAttribute());
            }
        }

        private bool IsPossibleAttribute()
        {
            return this.IsTrueIdentifier();
        }

        private AttributeSyntax ParseAttribute()
        {
            var atToken = this.EatToken(SyntaxKind.AtToken);

            // Check for optional location :
            AttributeTargetSpecifierSyntax attrLocation = null;
            if (IsSomeWord(this.CurrentToken.Kind) && this.PeekToken(1).Kind == SyntaxKind.ColonToken)
            {
                var id = ConvertToKeyword(this.EatToken());
                var colon = this.EatToken(SyntaxKind.ColonToken);
                attrLocation = _syntaxFactory.AttributeTargetSpecifier(id, colon);
            }

            if (attrLocation != null && attrLocation.Identifier.ToAttributeLocation() == AttributeLocation.Module)
            {
                attrLocation = CheckFeatureAvailability(attrLocation, MessageID.IDS_FeatureModuleAttrLoc);
            }

            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.Attribute)
            {
                return (AttributeSyntax)this.EatNode();
            }

            var name = this.ParseQualifiedName();

            var argList = this.ParseAttributeArgumentList();
            return _syntaxFactory.Attribute(atToken, attrLocation, name, argList);
        }

        internal AttributeArgumentListSyntax ParseAttributeArgumentList()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.AttributeArgumentList)
            {
                return (AttributeArgumentListSyntax)this.EatNode();
            }

            AttributeArgumentListSyntax argList = null;
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                var openParen = this.EatToken(SyntaxKind.OpenParenToken);
                var argNodes = _pool.AllocateSeparated<AttributeArgumentSyntax>();
                try
                {
tryAgain:
                    if (this.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                    {
                        if (this.IsPossibleAttributeArgument() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                        {
                            // first argument
                            argNodes.Add(this.ParseAttributeArgument());

                            // comma + argument or end?
                            while (true)
                            {
                                if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken)
                                {
                                    break;
                                }
                                else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleAttributeArgument())
                                {
                                    argNodes.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                                    argNodes.Add(this.ParseAttributeArgument());
                                }
                                else if (this.SkipBadAttributeArgumentTokens(ref openParen, argNodes, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                                {
                                    break;
                                }
                            }
                        }
                        else if (this.SkipBadAttributeArgumentTokens(ref openParen, argNodes, SyntaxKind.IdentifierToken) == PostSkipAction.Continue)
                        {
                            goto tryAgain;
                        }
                    }

                    var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                    argList = _syntaxFactory.AttributeArgumentList(openParen, argNodes, closeParen);
                }
                finally
                {
                    _pool.Free(argNodes);
                }
            }

            return argList;
        }

        private PostSkipAction SkipBadAttributeArgumentTokens(ref SyntaxToken openParen, SeparatedSyntaxListBuilder<AttributeArgumentSyntax> list, SyntaxKind expected)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref openParen, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleAttributeArgument(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseParenToken || p.IsTerminator(),
                expected);
        }

        private bool IsPossibleAttributeArgument()
        {
            return this.IsPossibleExpression();
        }

        private AttributeArgumentSyntax ParseAttributeArgument()
        {
            // Need to parse both "real" named arguments and attribute-style named arguments.
            // We track attribute-style named arguments only with fShouldHaveName.

            NameEqualsSyntax nameEquals = null;
            NameColonSyntax nameColon = null;
            if (this.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                SyntaxKind nextTokenKind = this.PeekToken(1).Kind;
                switch (nextTokenKind)
                {
                    case SyntaxKind.EqualsToken:
                        {
                            var name = this.ParseIdentifierToken();
                            var equals = this.EatToken(SyntaxKind.EqualsToken);
                            nameEquals = _syntaxFactory.NameEquals(_syntaxFactory.IdentifierName(name), equals);
                        }

                        break;
                    case SyntaxKind.ColonToken:
                        {
                            var name = this.ParseIdentifierName();
                            var colonToken = this.EatToken(SyntaxKind.ColonToken);
                            nameColon = _syntaxFactory.NameColon(name, colonToken);
                            nameColon = CheckFeatureAvailability(nameColon, MessageID.IDS_FeatureNamedArgument);
                        }
                        break;
                }
            }

            return _syntaxFactory.AttributeArgument(
                nameEquals, nameColon, this.ParseExpressionCore());
        }

        private static DeclarationModifiers GetModifier(SyntaxToken token)
            => GetModifier(token.Kind, token.ContextualKind);

        internal static DeclarationModifiers GetModifier(SyntaxKind kind, SyntaxKind contextualKind)
        {
            switch (kind)
            {
                case SyntaxKind.PublicKeyword:
                    return DeclarationModifiers.Public;
                case SyntaxKind.InternalKeyword:
                    return DeclarationModifiers.Internal;
                case SyntaxKind.ProtectedKeyword:
                    return DeclarationModifiers.Protected;
                case SyntaxKind.PrivateKeyword:
                    return DeclarationModifiers.Private;
                case SyntaxKind.SealedKeyword:
                    return DeclarationModifiers.Sealed;
                case SyntaxKind.AbstractKeyword:
                    return DeclarationModifiers.Abstract;
                case SyntaxKind.StaticKeyword:
                    return DeclarationModifiers.Static;
                case SyntaxKind.VirtualKeyword:
                    return DeclarationModifiers.Virtual;
                case SyntaxKind.ExternKeyword:
                    return DeclarationModifiers.Extern;
                case SyntaxKind.NewKeyword:
                    return DeclarationModifiers.New;
                case SyntaxKind.OverrideKeyword:
                    return DeclarationModifiers.Override;
                case SyntaxKind.ReadOnlyKeyword:
                    return DeclarationModifiers.Let;
                case SyntaxKind.VolatileKeyword:
                    return DeclarationModifiers.Volatile;
                case SyntaxKind.TransientKeyword:
                    return DeclarationModifiers.Transient;
                case SyntaxKind.UnsafeKeyword:
                    return DeclarationModifiers.Unsafe;
                case SyntaxKind.PartialKeyword:
                    return DeclarationModifiers.Partial;
                case SyntaxKind.AsyncKeyword:
                    return DeclarationModifiers.Async;
                case SyntaxKind.IdentifierToken:
                    switch (contextualKind)
                    {
                        case SyntaxKind.PartialKeyword:
                            return DeclarationModifiers.Partial;
                        case SyntaxKind.AsyncKeyword:
                            return DeclarationModifiers.Async;
                    }

                    goto default;
                default:
                    return DeclarationModifiers.None;
            }
        }

        private void ParseModifiers(SyntaxListBuilder tokens, bool forAccessors)
        {
            while (true)
            {
                var newMod = GetModifier(this.CurrentToken);
                if (newMod == DeclarationModifiers.None)
                {
                    break;
                }
                var modTok = this.EatToken();
                tokens.Add(modTok);
            }
        }

        private static bool IsNonContextualModifier(SyntaxToken nextToken)
        {
            return GetModifier(nextToken) != DeclarationModifiers.None && !SyntaxFacts.IsContextualKeyword(nextToken.ContextualKind);
        }

        private bool IsPartialType()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword);
            switch (this.PeekToken(1).Kind)
            {
                case SyntaxKind.StructKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.ModuleKeyword:
                case SyntaxKind.InterfaceKeyword:
                    return true;
            }

            return false;
        }

        private bool IsPartialMember()
        {
            // note(cyrusn): this could have been written like so:
            //
            //  return
            //    this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword &&
            //    this.PeekToken(1).Kind == SyntaxKind.VoidKeyword;
            //
            // However, we want to be lenient and allow the user to write 
            // 'partial' in most modifier lists.  We will then provide them with
            // a more specific message later in binding that they are doing 
            // something wrong.
            //
            // Some might argue that the simple check would suffice.
            // However, we'd like to maintain behavior with 
            // previously shipped versions, and so we're keeping this code.

            // Here we check for:
            //   partial ReturnType MemberName
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword);
            var point = this.GetResetPoint();
            try
            {
                this.EatToken(); // partial

                if (this.ScanType() == ScanTypeFlags.NotType)
                {
                    return false;
                }

                return IsPossibleMemberName();
            }
            finally
            {
                this.Reset(ref point);
                this.Release(ref point);
            }
        }

        private bool IsPossibleMemberName()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.ThisKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private MemberDeclarationSyntax ParseTypeDeclaration(SyntaxListBuilder<AttributeSyntax> attributes, SyntaxListBuilder modifiers)
        {
            // "top-level" expressions and statements should never occur inside an asynchronous context
            Debug.Assert(!IsInAsync);

            cancellationToken.ThrowIfCancellationRequested();

            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.ClassKeyword:
                    // report use of "static class" if feature is unsupported 
                    CheckForVersionSpecificModifiers(modifiers, SyntaxKind.StaticKeyword, MessageID.IDS_FeatureStaticClasses);
                    return this.ParseClassOrStructOrInterfaceDeclaration(attributes, modifiers);

                case SyntaxKind.ModuleKeyword:
                    return this.ParseClassOrStructOrInterfaceDeclaration(attributes, modifiers);

                case SyntaxKind.StructKeyword:
                    // report use of "readonly struct" if feature is unsupported
                    CheckForVersionSpecificModifiers(modifiers, SyntaxKind.ReadOnlyKeyword, MessageID.IDS_FeatureReadOnlyStructs);
                    return this.ParseClassOrStructOrInterfaceDeclaration(attributes, modifiers);

                case SyntaxKind.InterfaceKeyword:
                    return this.ParseClassOrStructOrInterfaceDeclaration(attributes, modifiers);

                case SyntaxKind.DelegateKeyword:
                    return this.ParseDelegateDeclaration(attributes, modifiers);

                case SyntaxKind.EnumKeyword:
                    return this.ParseEnumDeclaration(attributes, modifiers);

                default:
                    throw ExceptionUtilities.UnexpectedValue(this.CurrentToken.Kind);
            }
        }

        /// <summary>
        /// checks for modifiers whose feature is not available
        /// </summary>
        private void CheckForVersionSpecificModifiers(SyntaxListBuilder modifiers, SyntaxKind kind, MessageID feature)
        {
            for (int i = 0, n = modifiers.Count; i < n; i++)
            {
                if (modifiers[i].RawKind == (int)kind)
                {
                    modifiers[i] = CheckFeatureAvailability(modifiers[i], feature);
                }
            }
        }

        private TypeDeclarationSyntax ParseClassOrStructOrInterfaceDeclaration(SyntaxListBuilder<AttributeSyntax> attributes, SyntaxListBuilder modifiers)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ClassKeyword ||
                this.CurrentToken.Kind == SyntaxKind.StructKeyword ||
                this.CurrentToken.Kind == SyntaxKind.ModuleKeyword ||
                this.CurrentToken.Kind == SyntaxKind.InterfaceKeyword);

            // "top-level" expressions and statements should never occur inside an asynchronous context
            Debug.Assert(!IsInAsync);

            bool isModule = this.CurrentToken.Kind == SyntaxKind.ModuleKeyword;

            var classOrStructOrInterface = this.EatToken();
            var saveTerm = _termState;
            _termState |= TerminatorState.IsPossibleAggregateClauseStartOrStop;
            var name = this.ParseIdentifierToken();
            var typeParameters = isModule ? null : this.ParseTypeParameterList();

            _termState = saveTerm;
            var extendList = isModule ? null : this.ParseExtendList();
            var implementList = isModule ? null : this.ParseImplementList();

            // Parse class body
            bool parseMembers = true;
            SyntaxListBuilder<MemberDeclarationSyntax> members = default(SyntaxListBuilder<MemberDeclarationSyntax>);
            var constraints = default(SyntaxListBuilder<TypeParameterConstraintClauseSyntax>);
            try
            {
                if (!isModule &&  this.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
                {
                    constraints = _pool.Allocate<TypeParameterConstraintClauseSyntax>();
                    this.ParseTypeParameterConstraintClauses(constraints);
                }

                var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);

                // ignore members if missing type name or missing open curly
                if (name.IsMissing || openBrace.IsMissing)
                {
                    parseMembers = false;
                }

                // even if we saw a { or think we should parse members bail out early since
                // we know namespaces can't be nested inside types
                if (parseMembers)
                {
                    members = _pool.Allocate<MemberDeclarationSyntax>();

                    while (true)
                    {
                        SyntaxKind kind = this.CurrentToken.Kind;

                        if (CanStartMember(kind))
                        {
                            // This token can start a member -- go parse it
                            var saveTerm2 = _termState;
                            _termState |= TerminatorState.IsPossibleMemberStartOrStop;

                            var memberOrStatement = this.ParseMemberDeclarationOrStatement(classOrStructOrInterface.Kind);
                            if (memberOrStatement != null)
                            {
                                // statements are accepted here, a semantic error will be reported later
                                members.Add(memberOrStatement);
                            }
                            else
                            {
                                // we get here if we couldn't parse the lookahead as a statement or a declaration (we haven't consumed any tokens):
                                this.SkipBadMemberListTokens(ref openBrace, members);
                            }

                            _termState = saveTerm2;
                        }
                        else if (kind == SyntaxKind.CloseBraceToken || kind == SyntaxKind.EndOfFileToken || this.IsTerminator())
                        {
                            // This marks the end of members of this class
                            break;
                        }
                        else
                        {
                            // Error -- try to sync up with intended reality
                            this.SkipBadMemberListTokens(ref openBrace, members);
                        }
                    }
                }

                SyntaxToken closeBrace;
                if (openBrace.IsMissing)
                {
                    closeBrace = SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken);
                    closeBrace = WithAdditionalDiagnostics(closeBrace, this.GetExpectedTokenError(SyntaxKind.CloseBraceToken, this.CurrentToken.Kind));
                }
                else
                {
                    closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);
                }

                switch (classOrStructOrInterface.Kind)
                {
                    case SyntaxKind.ModuleKeyword:
                        return _syntaxFactory.ModuleDeclaration(
                            attributes,
                            modifiers.ToList(),
                            classOrStructOrInterface,
                            name,
                            typeParameters,
                            extendList,
                            implementList,
                            constraints,
                            openBrace,
                            members,
                            closeBrace,
                            null);

                    case SyntaxKind.ClassKeyword:
                        return _syntaxFactory.ClassDeclaration(
                            attributes,
                            modifiers.ToList(),
                            classOrStructOrInterface,
                            name,
                            typeParameters,
                            extendList,
                            implementList,
                            constraints,
                            openBrace,
                            members,
                            closeBrace,
                            null);

                    case SyntaxKind.StructKeyword:
                        return _syntaxFactory.StructDeclaration(
                            attributes,
                            modifiers.ToList(),
                            classOrStructOrInterface,
                            name,
                            typeParameters,
                            extendList,
                            implementList,
                            constraints,
                            openBrace,
                            members,
                            closeBrace,
                            null);

                    case SyntaxKind.InterfaceKeyword:
                        return _syntaxFactory.InterfaceDeclaration(
                            attributes,
                            modifiers.ToList(),
                            classOrStructOrInterface,
                            name,
                            typeParameters,
                            extendList,
                            implementList,
                            constraints,
                            openBrace,
                            members,
                            closeBrace,
                            null);

                    default:
                        throw ExceptionUtilities.UnexpectedValue(classOrStructOrInterface.Kind);
                }
            }
            finally
            {
                if (!members.IsNull)
                {
                    _pool.Free(members);
                }

                if (!constraints.IsNull)
                {
                    _pool.Free(constraints);
                }
            }
        }

        private void SkipBadMemberListTokens(ref SyntaxToken openBrace, SyntaxListBuilder members)
        {
            if (members.Count > 0)
            {
                var tmp = members[members.Count - 1];
                this.SkipBadMemberListTokens(ref tmp);
                members[members.Count - 1] = tmp;
            }
            else
            {
                GreenNode tmp = openBrace;
                this.SkipBadMemberListTokens(ref tmp);
                openBrace = (SyntaxToken)tmp;
            }
        }

        private void SkipBadMemberListTokens(ref GreenNode previousNode)
        {
            int curlyCount = 0;
            var tokens = _pool.Allocate();
            try
            {
                bool done = false;

                // always consume at least one token.
                var token = this.EatToken();
                token = this.AddError(token, ErrorCode.ERR_InvalidMemberDecl, token.Text);
                tokens.Add(token);

                while (!done)
                {
                    SyntaxKind kind = this.CurrentToken.Kind;

                    // If this token can start a member, we're done
                    if (CanStartMember(kind) &&
                        !(kind == SyntaxKind.DelegateKeyword && (this.PeekToken(1).Kind == SyntaxKind.OpenBraceToken || this.PeekToken(1).Kind == SyntaxKind.OpenParenToken)))
                    {
                        done = true;
                        continue;
                    }

                    // <UNDONE>  UNDONE: Seems like this makes sense, 
                    // but if this token can start a namespace element, but not a member, then
                    // perhaps we should bail back up to parsing a namespace body somehow...</UNDONE>

                    // Watch curlies and look for end of file/close curly
                    switch (kind)
                    {
                        case SyntaxKind.OpenBraceToken:
                            curlyCount++;
                            break;

                        case SyntaxKind.CloseBraceToken:
                            if (curlyCount-- == 0)
                            {
                                done = true;
                                continue;
                            }

                            break;

                        case SyntaxKind.EndOfFileToken:
                            done = true;
                            continue;

                        default:
                            break;
                    }

                    tokens.Add(this.EatToken());
                }

                previousNode = AddTrailingSkippedSyntax((CSharpSyntaxNode)previousNode, tokens.ToListNode());
            }
            finally
            {
                _pool.Free(tokens);
            }
        }

        private bool IsPossibleMemberStartOrStop()
        {
            return this.IsPossibleMemberStart() || this.CurrentToken.Kind == SyntaxKind.CloseBraceToken;
        }

        private bool IsPossibleAggregateClauseStartOrStop()
        {
            return this.CurrentToken.Kind == SyntaxKind.ColonToken
                || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
                || this.IsCurrentTokenWhereOfConstraintClause();
        }

        private ExtendListSyntax ParseExtendList()
        {
            if (this.CurrentToken.Kind != SyntaxKind.ExtendsKeyword)
            {
                return null;
            }
            return (ExtendListSyntax)ParseBaseList(SyntaxKind.ExtendsKeyword);
        }

        private ImplementListSyntax ParseImplementList()
        {
            if (this.CurrentToken.Kind != SyntaxKind.ImplementsKeyword)
            {
                return null;
            }
            return (ImplementListSyntax)ParseBaseList(SyntaxKind.ImplementsKeyword);
        }

        private BaseListSyntax ParseBaseList(SyntaxKind expectedToken)
        {
            var extendsOrImplements = this.EatToken(expectedToken);
            var list = _pool.AllocateSeparated<BaseTypeSyntax>();
            try
            {
                // first type
                TypeSyntax firstType = this.ParseType();
                list.Add(_syntaxFactory.SimpleBaseType(firstType));

                // any additional types
                while (true)
                {
                    if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken ||
                        (expectedToken == SyntaxKind.ExtendsKeyword && this.CurrentToken.Kind == SyntaxKind.ImplementsKeyword) ||
                        this.IsCurrentTokenWhereOfConstraintClause())
                    {
                        break;
                    }
                    else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleType())
                    {
                        list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                        list.Add(_syntaxFactory.SimpleBaseType(this.ParseType()));
                        continue;
                    }
                    else if (this.SkipBadBaseListTokens(ref extendsOrImplements, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }

                return extendsOrImplements.Kind == SyntaxKind.ExtendsKeyword ? (BaseListSyntax)_syntaxFactory.ExtendList(extendsOrImplements, list) : _syntaxFactory.ImplementList(extendsOrImplements, list);
            }
            finally
            {
                _pool.Free(list);
            }
        }


        private ThrowsListSyntax ParseThrowsList()
        {
            var throws = this.EatContextualToken(SyntaxKind.ThrowsKeyword);
            var list = _pool.AllocateSeparated<TypeSyntax>();
            try
            {
                // first type
                TypeSyntax firstType = this.ParseType();
                list.Add(firstType);

                // any additional types
                while (true)
                {
                    if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
                    {
                        break;
                    }
                    else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleType())
                    {
                        list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                        list.Add(this.ParseType());
                        continue;
                    }
                    else if (this.SkipBadBaseListTokens(ref throws, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }

                return _syntaxFactory.ThrowsList(throws, list);
            }
            finally
            {
                _pool.Free(list);
            }
        }

        private PostSkipAction SkipBadBaseListTokens<TNode>(ref SyntaxToken colon, SeparatedSyntaxListBuilder<TNode> list, SyntaxKind expected)
            where TNode : CSharpSyntaxNode
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref colon, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleAttribute(),
                p => p.CurrentToken.Kind == SyntaxKind.OpenBraceToken || p.IsCurrentTokenWhereOfConstraintClause() || p.IsTerminator(),
                expected);
        }

        private bool IsCurrentTokenWhereOfConstraintClause()
        {
            return
                this.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword &&
                this.PeekToken(1).Kind == SyntaxKind.IdentifierToken &&
                this.PeekToken(2).Kind == SyntaxKind.ColonToken;
        }

        private void ParseTypeParameterConstraintClauses(SyntaxListBuilder list)
        {
            while (this.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
            {
                list.Add(this.ParseTypeParameterConstraintClause());
            }
        }

        private void ParseContractClauses(SyntaxListBuilder list)
        {
            while (IsContractKeyword(this.CurrentToken.ContextualKind))
            {
                list.Add(this.ParseContractClause());
            }
        }

        private static bool IsContractKeyword(SyntaxKind kind)
        {
            return kind == SyntaxKind.RequiresKeyword || kind == SyntaxKind.EnsuresKeyword;
        }

        private ContractClauseSyntax ParseContractClause()
        {
            var contractKeyword = CurrentToken.Kind == SyntaxKind.RequiresKeyword ? 
                this.EatContextualToken(SyntaxKind.RequiresKeyword) : 
                this.EatContextualToken(SyntaxKind.EnsuresKeyword);

            var condition = this.ParseExpressionCore();

            return _syntaxFactory.ContractClause(contractKeyword, condition);
        }

        private TypeParameterConstraintClauseSyntax ParseTypeParameterConstraintClause()
        {
            var where = this.EatContextualToken(SyntaxKind.WhereKeyword);
            var name = !IsTrueIdentifier()
                ? this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_IdentifierExpected)
                : this.ParseIdentifierName();

            var colon = this.EatToken(SyntaxKind.ColonToken);

            var bounds = _pool.AllocateSeparated<TypeParameterConstraintSyntax>();
            try
            {
                // first bound
                if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken || this.IsCurrentTokenWhereOfConstraintClause())
                {
                    // TODO
                    //bounds.Add(_syntaxFactory.TypeConstraint(this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_TypeExpected)));
                    this.AddError(CurrentToken, ErrorCode.ERR_TypeExpected);
                }
                else
                {
                    bounds.Add(this.ParseTypeParameterConstraint());

                    // remaining bounds
                    while (true)
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
                            || this.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken
                            || this.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
                        {
                            break;
                        }
                        else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleTypeParameterConstraint())
                        {
                            bounds.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                            if (this.IsCurrentTokenWhereOfConstraintClause())
                            {
                                // TODO: error
                                this.AddError(CurrentToken, ErrorCode.ERR_TypeExpected);
                                //bounds.Add(_syntaxFactory.TypeConstraint(this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_TypeExpected)));
                                break;
                            }
                            else
                            {
                                bounds.Add(this.ParseTypeParameterConstraint());
                            }
                        }
                        else if (this.SkipBadTypeParameterConstraintTokens(bounds, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }

                return _syntaxFactory.TypeParameterConstraintClause(where, name, colon, bounds);
            }
            finally
            {
                _pool.Free(bounds);
            }
        }

        private bool IsPossibleTypeParameterConstraint()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.IsKeyword:
                case SyntaxKind.HasKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private TypeParameterConstraintSyntax ParseTypeParameterConstraint()
        {
            SyntaxToken questionToken = null;
            var syntaxKind = this.CurrentToken.Kind;

            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.NewKeyword:
                    var newToken = this.EatToken();
                    var open = this.EatToken(SyntaxKind.OpenParenToken);
                    var close = this.EatToken(SyntaxKind.CloseParenToken);
                    return _syntaxFactory.ConstructorConstraint(newToken, open, close);

                case SyntaxKind.ExtendsKeyword:
                case SyntaxKind.ImplementsKeyword:
                    var kind = CurrentToken.Kind;
                    var extendsOrImplements = EatToken();

                    var type = this.ParseType();
                    return _syntaxFactory.ExtendsOrImplementsTypeConstraint(kind == SyntaxKind.ExtendsKeyword ? SyntaxKind.ExtendsTypeConstraint : SyntaxKind.ImplementsTypeConstraint, extendsOrImplements, type);

                default:
                    // TODO: we should handle the error differently?
                    bool hasIsKeyword = CurrentToken.Kind == SyntaxKind.IsKeyword;
                    var isKeyword = EatToken(SyntaxKind.IsKeyword, ErrorCode.ERR_InvalidTypeConstraint);
                    switch (this.CurrentToken.Kind)
                    {
                        case SyntaxKind.StructKeyword:
                            var structToken = this.EatToken();

                            if (this.CurrentToken.Kind == SyntaxKind.QuestionToken)
                            {
                                questionToken = this.EatToken();
                                questionToken = this.AddError(questionToken, ErrorCode.ERR_UnexpectedToken, questionToken.Text);
                            }

                            return _syntaxFactory.ClassOrStructConstraint(SyntaxKind.StructConstraint, isKeyword, structToken, questionToken);

                        case SyntaxKind.ConstKeyword:
                            var constKeyword = EatToken();
                            var constType = ParseUnderlyingType();
                            return _syntaxFactory.ConstConstraint(isKeyword, constKeyword, constType);

                        default:
                            var classToken = this.EatToken(SyntaxKind.ClassKeyword, ErrorCode.ERR_InvalidTypeConstraintAfterIs, hasIsKeyword);

                            if (this.CurrentToken.Kind == SyntaxKind.QuestionToken)
                            {
                                questionToken = this.EatToken();
                            }
                            return _syntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint, isKeyword, classToken, questionToken);
                    }
            }
        }

        private PostSkipAction SkipBadTypeParameterConstraintTokens(SeparatedSyntaxListBuilder<TypeParameterConstraintSyntax> list, SyntaxKind expected)
        {
            CSharpSyntaxNode tmp = null;
            Debug.Assert(list.Count > 0);
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref tmp, list,
                p => this.CurrentToken.Kind != SyntaxKind.CommaToken && !this.IsPossibleTypeParameterConstraint(),
                p => this.CurrentToken.Kind == SyntaxKind.OpenBraceToken || this.IsCurrentTokenWhereOfConstraintClause() || this.IsTerminator(),
                expected);
        }

        private bool IsPossibleMemberStart()
        {
            return CanStartMember(this.CurrentToken.Kind);
        }

        private static bool CanStartMember(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.AtToken:
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.ModuleKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.EnumKeyword:
                case SyntaxKind.ExternKeyword:
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.OverrideKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.SealedKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.StructKeyword:
                case SyntaxKind.UnsafeKeyword:
                case SyntaxKind.VirtualKeyword:
                case SyntaxKind.VarKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.LetKeyword:
                case SyntaxKind.FuncKeyword:
                case SyntaxKind.ConstructorKeyword:
                    return true;

                default:
                    return false;
            }
        }

        private bool IsTypeDeclarationStart()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.ModuleKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.EnumKeyword:
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.StructKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private static bool CanReuseMemberDeclaration(
            Stark.Syntax.MemberDeclarationSyntax member)
        {
            switch (member?.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.ModuleDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                    return true;
                default:
                    return false;
            }
        }

        public MemberDeclarationSyntax ParseMemberDeclaration()
        {
            // Use a parent kind that causes inclusion of only member declarations that could appear in a struct
            // e.g. including fixed member declarations, but not statements.
            const SyntaxKind parentKind = SyntaxKind.StructDeclaration;
            return ParseWithStackGuard(
                () => this.ParseMemberDeclarationOrStatement(parentKind),
                () => createEmptyNodeFunc());

            // Creates a dummy declaration node to which we can attach a stack overflow message
            MemberDeclarationSyntax createEmptyNodeFunc()
            {
                return _syntaxFactory.IncompleteMember(
                    new SyntaxList<AttributeSyntax>(),
                    new SyntaxList<SyntaxToken>(),
                    CreateMissingIdentifierName()
                    );
            }
        }

        // Returns null if we can't parse anything (even partially).
        internal MemberDeclarationSyntax ParseMemberDeclarationOrStatement(SyntaxKind parentKind)
        {
            _recursionDepth++;
            StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
            var result = ParseMemberDeclarationOrStatementCore(parentKind);
            _recursionDepth--;
            return result;
        }

        // Returns null if we can't parse anything (even partially).
        private MemberDeclarationSyntax ParseMemberDeclarationOrStatementCore(SyntaxKind parentKind)
        {
            // "top-level" expressions and statements should never occur inside an asynchronous context
            Debug.Assert(!IsInAsync);

            cancellationToken.ThrowIfCancellationRequested();

            bool isGlobalScript = parentKind == SyntaxKind.CompilationUnit && this.IsScript;
            bool acceptStatement = isGlobalScript;

            // don't reuse members if they were previously declared under a different type keyword kind
            if (this.IsIncrementalAndFactoryContextMatches)
            {
                var member = this.CurrentNode as Stark.Syntax.MemberDeclarationSyntax;
                if (CanReuseMemberDeclaration(member))
                {
                    return (MemberDeclarationSyntax)this.EatNode();
                }
            }

            var attributes = _pool.Allocate<AttributeSyntax>();
            var modifiers = _pool.Allocate();

            var saveTermState = _termState;

            try
            {
                this.ParseAttributeSyntaxList(attributes);

                if (attributes.Count > 0)
                {
                    acceptStatement = false;
                }

                //
                // Check for the following cases to disambiguate between member declarations and expressions.
                // Doing this before parsing modifiers simplifies further analysis since some of these keywords can act as modifiers as well.
                //
                // unsafe { ... }
                // fixed (...) { ... } 
                // delegate (...) { ... }
                // delegate { ... }
                // new { ... }
                // new[] { ... }
                // new T (...)
                // new T [...]
                //
                if (acceptStatement)
                {
                    switch (this.CurrentToken.Kind)
                    {
                        case SyntaxKind.UnsafeKeyword:
                            if (IsPossibleUnsafeStatement())
                            {
                                return _syntaxFactory.GlobalStatement(ParseUnsafeStatement());
                            }
                            break;

                        case SyntaxKind.FixedKeyword:
                            if (this.PeekToken(1).Kind == SyntaxKind.OpenParenToken)
                            {
                                return _syntaxFactory.GlobalStatement(ParseFixedStatement());
                            }
                            break;

                        case SyntaxKind.DelegateKeyword:
                            switch (this.PeekToken(1).Kind)
                            {
                                case SyntaxKind.OpenParenToken:
                                case SyntaxKind.OpenBraceToken:
                                    return _syntaxFactory.GlobalStatement(ParseExpressionStatement());
                            }
                            break;

                        case SyntaxKind.NewKeyword:
                            if (IsPossibleNewExpression())
                            {
                                return _syntaxFactory.GlobalStatement(ParseExpressionStatement());
                            }
                            break;
                    }
                }

                // All modifiers that might start an expression are processed above.
                this.ParseModifiers(modifiers, forAccessors: false);
                if (modifiers.Count > 0)
                {
                    acceptStatement = false;
                }

                switch (this.CurrentToken.Kind)
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.LetKeyword:
                    case SyntaxKind.VarKeyword:
                        return this.ParseFieldDeclaration(attributes, modifiers, parentKind);

                    case SyntaxKind.ConstructorKeyword:
                        return this.ParseConstructorDeclaration(attributes, modifiers);

                    case SyntaxKind.FuncKeyword:
                        return this.ParseFuncDeclaration(attributes, modifiers, parentKind);

                    case SyntaxKind.ClassKeyword:
                    case SyntaxKind.DelegateKeyword:
                    case SyntaxKind.EnumKeyword:
                    case SyntaxKind.InterfaceKeyword:
                    case SyntaxKind.StructKeyword:
                    case SyntaxKind.ModuleKeyword:
                        return this.ParseTypeDeclaration(attributes, modifiers);

                    case SyntaxKind.NamespaceKeyword:
                        if (parentKind == SyntaxKind.CompilationUnit)
                        {
                            // we found a namespace with modifier or an attribute: ignore the attribute/modifier and parse as namespace
                            if (attributes.Count > 0)
                            {
                                attributes[0] = this.AddError(attributes[0], ErrorCode.ERR_BadModifiersOnNamespace);
                            }
                            else
                            {
                                // if were no attributes and no modifiers we should have parsed it already in namespace body:
                                Debug.Assert(modifiers.Count > 0);

                                modifiers[0] = this.AddError(modifiers[0], ErrorCode.ERR_BadModifiersOnNamespace);
                            }

                            var namespaceDecl = ParseNamespaceDeclaration();

                            if (modifiers.Count > 0)
                            {
                                namespaceDecl = AddLeadingSkippedSyntax(namespaceDecl, modifiers.ToListNode());
                            }

                            if (attributes.Count > 0)
                            {
                                namespaceDecl = AddLeadingSkippedSyntax(namespaceDecl, attributes.ToListNode());
                            }

                            return namespaceDecl;
                        }
                        break;
                }

                // Workaround in case of a parsing error right after attributes/modifiers
                // we will emit an invalid member declaration
                // but we will eat the offending token to avoid an infinite loop
                //=======================================================================

                var skippedToken = EatToken();

                var incompleteMember = _syntaxFactory.IncompleteMember(attributes, modifiers.ToList(), null);

                var builder = new SyntaxListBuilder(1);
                builder.Add(skippedToken);
                var fileAsTrivia = _syntaxFactory.SkippedTokensTrivia(builder.ToList<SyntaxToken>());
                incompleteMember = AddTrailingSkippedSyntax(incompleteMember, fileAsTrivia);

                //the error position should indicate skippedToken
                var error = this.AddError(
                    incompleteMember,
                    incompleteMember.FullWidth - skippedToken.FullWidth,
                    skippedToken.Width,
                    ErrorCode.ERR_InvalidMemberDecl,
                    skippedToken.Text);

                return error;
            }
            finally
            {
                _pool.Free(modifiers);
                _pool.Free(attributes);
                _termState = saveTermState;
            }

            // Null if we were not able to parse anything
            return null;
        }

        private MemberDeclarationSyntax ParseFuncDeclaration(SyntaxListBuilder<AttributeSyntax> attributes,
            SyntaxListBuilder modifiers,
            SyntaxKind parentKind)
        {
            var funcToken = EatToken();
            Debug.Assert(funcToken.Kind == SyntaxKind.FuncKeyword);

            SyntaxToken operatorToken = null;
            if (CurrentToken.Kind == SyntaxKind.OperatorKeyword)
            {
                operatorToken = EatToken();

                if (CurrentToken.Kind ==  SyntaxKind.ExplicitKeyword ||
                    CurrentToken.Kind == SyntaxKind.ImplicitKeyword ||
                    CurrentToken.Kind == SyntaxKind.AsKeyword)
                {
                    return this.ParseConversionOperatorDeclaration(funcToken, operatorToken, attributes, modifiers);
                }
            }

            // At this point we can either have indexers, methods, or 
            // properties (or something unknown).  Try to break apart
            // the following name and determine what to do from there.
            ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt;
            SyntaxToken identifierOrThisOpt;
            TypeParameterListSyntax typeParameterListOpt;
            this.ParseMemberName(out explicitInterfaceOpt, out identifierOrThisOpt, out typeParameterListOpt, isEvent: false);

            // First, check if we got absolutely nothing.  If so, then 
            // We need to consume a bad member and try again.
            if (operatorToken == null && explicitInterfaceOpt == null && identifierOrThisOpt == null && typeParameterListOpt == null)
            {
                if (attributes.Count == 0 && modifiers.Count == 0)
                {
                    // we haven't advanced, the caller needs to consume the tokens ahead
                    return null;
                }

                var incompleteMember = _syntaxFactory.IncompleteMember(attributes, modifiers.ToList(), null);
                if (incompleteMember.ContainsDiagnostics)
                {
                    return incompleteMember;
                }
                else if (parentKind == SyntaxKind.NamespaceDeclaration ||
                         parentKind == SyntaxKind.CompilationUnit && !IsScript)
                {
                    return this.AddErrorToLastToken(incompleteMember, ErrorCode.ERR_NamespaceUnexpected);
                }
                else
                {
                    //the error position should indicate CurrentToken
                    return this.AddError(
                        incompleteMember,
                        incompleteMember.FullWidth + this.CurrentToken.GetLeadingTriviaWidth(),
                        this.CurrentToken.Width,
                        ErrorCode.ERR_InvalidMemberDecl,
                        this.CurrentToken.Text);
                }
            }

            if (operatorToken != null)
            {
                return ParseIndexerDeclaration(attributes, modifiers, funcToken, operatorToken, explicitInterfaceOpt, typeParameterListOpt);
            }
            else if (CurrentToken.Kind != SyntaxKind.OpenParenToken)
            {
                return ParsePropertyDeclaration(attributes, modifiers, funcToken, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt);
            }
            else
            {
                return ParseMethodDeclaration(attributes, modifiers, funcToken, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt);
            }
        }

        // if the modifiers do not contain async or replace and the type is the identifier "async" or "replace", then
        // add that identifier to the modifiers and assign a new type from the identifierOrThisOpt and the
        // type parameter list
        private bool ReconsiderTypeAsAsyncModifier(
            ref SyntaxListBuilder modifiers,
            ref TypeSyntax type,
            ref ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt,
            SyntaxToken identifierOrThisOpt,
            TypeParameterListSyntax typeParameterListOpt)
        {
            if (type.Kind != SyntaxKind.IdentifierName) return false;
            if (identifierOrThisOpt.Kind != SyntaxKind.IdentifierToken) return false;

            var identifier = ((IdentifierNameSyntax)type).Identifier;
            var contextualKind = identifier.ContextualKind;
            if (contextualKind != SyntaxKind.AsyncKeyword ||
                modifiers.Any((int)contextualKind))
            {
                return false;
            }

            modifiers.Add(ConvertToKeyword(identifier));
            SimpleNameSyntax newType = typeParameterListOpt == null
                ? (SimpleNameSyntax)_syntaxFactory.IdentifierName(identifierOrThisOpt)
                : _syntaxFactory.GenericName(identifierOrThisOpt, TypeArgumentFromTypeParameters(typeParameterListOpt));
            type = (explicitInterfaceOpt == null)
                ? (TypeSyntax)newType
                : _syntaxFactory.QualifiedName(explicitInterfaceOpt.Name, explicitInterfaceOpt.DotToken, newType);
            explicitInterfaceOpt = null;
            identifierOrThisOpt = default(SyntaxToken);
            typeParameterListOpt = default(TypeParameterListSyntax);
            return true;
        }

        private TypeArgumentListSyntax TypeArgumentFromTypeParameters(TypeParameterListSyntax typeParameterList)
        {
            var types = _pool.AllocateSeparated<TypeSyntax>();
            foreach (var p in typeParameterList.Parameters.GetWithSeparators())
            {
                switch ((SyntaxKind)p.RawKind)
                {
                    case SyntaxKind.TypeParameter:
                        var typeParameter = (TypeParameterSyntax)p;
                        var typeArgument = _syntaxFactory.IdentifierName(typeParameter.Identifier);
                        // NOTE: reverse order of variance keyword and attributes list so they come out in the right order.
                        if (typeParameter.VarianceKeyword != null)
                        {
                            // This only happens in error scenarios, so don't bother to produce a diagnostic about
                            // having a variance keyword on a type argument.
                            typeArgument = AddLeadingSkippedSyntax(typeArgument, typeParameter.VarianceKeyword);
                        }
                        if (typeParameter.AttributeLists.Node != null)
                        {
                            // This only happens in error scenarios, so don't bother to produce a diagnostic about
                            // having an attribute on a type argument.
                            typeArgument = AddLeadingSkippedSyntax(typeArgument, typeParameter.AttributeLists.Node);
                        }
                        types.Add(typeArgument);
                        break;
                    case SyntaxKind.CommaToken:
                        types.AddSeparator((SyntaxToken)p);
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(p.RawKind);
                }
            }

            var result = _syntaxFactory.TypeArgumentList(typeParameterList.LessThanToken, types.ToList(), typeParameterList.GreaterThanToken);
            _pool.Free(types);
            return result;
        }

        //private bool ReconsiderTypeAsAsyncModifier(ref SyntaxListBuilder modifiers, ref type, ref identifierOrThisOpt, ref typeParameterListOpt))
        //        {
        //            goto parse_member_name;
        //        }

        private bool IsFieldDeclaration(bool isEvent)
        {
            if (this.CurrentToken.Kind != SyntaxKind.IdentifierToken)
            {
                return false;
            }

            // Treat this as a field, unless we have anything following that
            // makes us:
            //   a) explicit
            //   b) generic
            //   c) a property
            //   d) a method (unless we already know we're parsing an event)
            var kind = this.PeekToken(1).Kind;
            switch (kind)
            {
                case SyntaxKind.DotToken:                   // Goo.     explicit
                case SyntaxKind.ColonColonToken:            // Goo::    explicit
                case SyntaxKind.LessThanToken:            // Goo<     explicit or generic method
                case SyntaxKind.OpenBraceToken:        // Goo {    property
                case SyntaxKind.EqualsGreaterThanToken:     // Goo =>   property
                    return false;
                case SyntaxKind.OpenParenToken:             // Goo(     method
                    return isEvent;
                default:
                    return true;
            }
        }

        public static bool IsComplete(CSharpSyntaxNode node)
        {
            if (node == null)
            {
                return false;
            }

            foreach (var child in node.ChildNodesAndTokens().Reverse())
            {
                var token = child as SyntaxToken;
                if (token == null)
                {
                    return IsComplete((CSharpSyntaxNode)child);
                }

                if (token.IsMissing)
                {
                    return false;
                }

                if (token.Kind != SyntaxKind.None)
                {
                    return true;
                }

                // if token was optional, consider the next one..
            }

            return true;
        }

        private ConstructorDeclarationSyntax ParseConstructorDeclaration(
            SyntaxListBuilder<AttributeSyntax> attributes, SyntaxListBuilder modifiers)
        {
            var constructor = EatToken(SyntaxKind.ConstructorKeyword);

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfMethodSignature;
            var contracts = default(SyntaxListBuilder<ContractClauseSyntax>);
            try
            {
                var paramList = this.ParseParenthesizedParameterList();

                ConstructorInitializerSyntax initializer = this.CurrentToken.Kind == SyntaxKind.ColonToken
                    ? this.ParseConstructorInitializer()
                    : null;

                // Parse any contract clauses
                if (IsContractKeyword(this.CurrentToken.ContextualKind))
                {
                    contracts = _pool.Allocate<ContractClauseSyntax>();
                    this.ParseContractClauses(contracts);
                }

                this.ParseBlockAndExpressionBodies(out BlockSyntax body, out ArrowExpressionClauseSyntax expressionBody, requestedExpressionBodyFeature: MessageID.IDS_FeatureExpressionBodiedDeOrConstructor);

                SyntaxToken eosToken = null;
                if (body == null)
                {
                    eosToken = EatEos(ref initializer);
                }

                return _syntaxFactory.ConstructorDeclaration(attributes, modifiers.ToList(), constructor, paramList, initializer, contracts, body, expressionBody, eosToken);
            }
            finally
            {
                if (!contracts.IsNull)
                {
                    _pool.Free(contracts);
                }
                _termState = saveTerm;
            }
        }

        private ConstructorInitializerSyntax ParseConstructorInitializer()
        {
            var colon = this.EatToken(SyntaxKind.ColonToken);

            var reportError = true;
            var kind = this.CurrentToken.Kind == SyntaxKind.BaseKeyword
                ? SyntaxKind.BaseConstructorInitializer
                : SyntaxKind.ThisConstructorInitializer;

            SyntaxToken token;
            if (this.CurrentToken.Kind == SyntaxKind.BaseKeyword || this.CurrentToken.Kind == SyntaxKind.ThisKeyword)
            {
                token = this.EatToken();
            }
            else
            {
                token = this.EatToken(SyntaxKind.ThisKeyword, ErrorCode.ERR_ThisOrBaseExpected);

                // No need to report further errors at this point:
                reportError = false;
            }

            ArgumentListSyntax argumentList;
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                argumentList = this.ParseParenthesizedArgumentList();
            }
            else
            {
                var openToken = this.EatToken(SyntaxKind.OpenParenToken, reportError);
                var closeToken = this.EatToken(SyntaxKind.CloseParenToken, reportError);
                argumentList = _syntaxFactory.ArgumentList(openToken, default(SeparatedSyntaxList<ArgumentSyntax>), closeToken);
            }

            return _syntaxFactory.ConstructorInitializer(kind, colon, token, argumentList);
        }

        /// <summary>
        /// Parses any block or expression bodies that are present. Also parses
        /// the trailing semicolon if one is present.
        /// </summary>
        private void ParseBlockAndExpressionBodies(
            out BlockSyntax blockBody,
            out ArrowExpressionClauseSyntax expressionBody,
            bool parseSemicolonAfterBlock = true,
            MessageID requestedExpressionBodyFeature = MessageID.IDS_FeatureExpressionBodiedMethod)
        {
            blockBody = null;
            expressionBody = null;

            if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                blockBody = this.ParseBlock(isMethodBody: true);
            }

            if (this.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                Debug.Assert(requestedExpressionBodyFeature == MessageID.IDS_FeatureExpressionBodiedMethod
                             || requestedExpressionBodyFeature == MessageID.IDS_FeatureExpressionBodiedAccessor
                             || requestedExpressionBodyFeature == MessageID.IDS_FeatureExpressionBodiedDeOrConstructor,
                    "Only IDS_FeatureExpressionBodiedMethod, IDS_FeatureExpressionBodiedAccessor or IDS_FeatureExpressionBodiedDeOrConstructor can be requested");
                expressionBody = this.ParseArrowExpressionClause();
                expressionBody = CheckFeatureAvailability(expressionBody, requestedExpressionBodyFeature);
            }
        }

        private bool IsEndOfTypeParameterList()
        {
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                // void Goo<T (
                return true;
            }

            if (this.CurrentToken.Kind == SyntaxKind.ColonToken)
            {
                // class C<T :
                return true;
            }

            if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                // class C<T {
                return true;
            }

            if (IsCurrentTokenWhereOfConstraintClause())
            {
                // class C<T where T :
                return true;
            }

            return false;
        }

        private bool IsEndOfMethodSignature()
        {
            return this.CurrentToken.Kind == SyntaxKind.SemicolonToken || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
        }

        private bool IsEndOfNameInExplicitInterface()
        {
            return this.CurrentToken.Kind == SyntaxKind.DotToken || this.CurrentToken.Kind == SyntaxKind.ColonColonToken;
        }

        private MethodDeclarationSyntax ParseMethodDeclaration(
            SyntaxListBuilder<AttributeSyntax> attributes,
            SyntaxListBuilder modifiers,
            SyntaxToken funcToken,
            ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt,
            SyntaxToken identifier,
            TypeParameterListSyntax typeParameterList)
        {
            // Parse the name (it could be qualified)
            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfMethodSignature;

            var paramList = this.ParseParenthesizedParameterList();

            SyntaxToken minusGreaterThanForReturnType = null;
            TypeSyntax returnType = null;

            if (CurrentToken.Kind == SyntaxKind.MinusGreaterThanToken)
            {
                minusGreaterThanForReturnType = EatToken();
                returnType = ParseType(); // ParseTypeOrVoid();
            }
            var constraints = default(SyntaxListBuilder<TypeParameterConstraintClauseSyntax>);
            var contracts = default(SyntaxListBuilder<ContractClauseSyntax>);
            try
            {
                if (this.CurrentToken.Kind == SyntaxKind.ColonToken)
                {
                    // Use else if, rather than if, because if we see both a constructor initializer and a constraint clause, we're too lost to recover.
                    var colonToken = this.CurrentToken;

                    ConstructorInitializerSyntax initializer = this.ParseConstructorInitializer();
                    initializer = this.AddErrorToFirstToken(initializer, ErrorCode.ERR_UnexpectedToken, colonToken.Text);
                    paramList = AddTrailingSkippedSyntax(paramList, initializer);

                    // CONSIDER: Parsing an invalid constructor initializer could, conceivably, get us way
                    // off track.  If this becomes a problem, an alternative approach would be to generalize
                    // EatTokenWithPrejudice in such a way that we can just skip everything until we recognize
                    // our context again (perhaps an open brace).
                }
                else if (this.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
                {
                    constraints = _pool.Allocate<TypeParameterConstraintClauseSyntax>();
                    this.ParseTypeParameterConstraintClauses(constraints);
                }

                // Parse any contract clauses
                if (IsContractKeyword(this.CurrentToken.ContextualKind))
                {
                    contracts = _pool.Allocate<ContractClauseSyntax>();
                    this.ParseContractClauses(contracts);
                }

                ThrowsListSyntax throwsList = default;
                if (this.CurrentToken.ContextualKind == SyntaxKind.ThrowsKeyword)
                {
                    throwsList = ParseThrowsList();
                }

                _termState = saveTerm;

                BlockSyntax blockBody;
                ArrowExpressionClauseSyntax expressionBody;
                SyntaxToken semicolon;

                // Method declarations cannot be nested or placed inside async lambdas, and so cannot occur in an
                // asynchronous context. Therefore the IsInAsync state of the parent scope is not saved and
                // restored, just assumed to be false and reset accordingly after parsing the method body.
                Debug.Assert(!IsInAsync);

                IsInAsync = modifiers.Any((int)SyntaxKind.AsyncKeyword);

                this.ParseBlockAndExpressionBodies(out blockBody, out expressionBody);


                var constraintsList = (SyntaxList<TypeParameterConstraintClauseSyntax>)constraints;

                // If we don't have a block body, we need to recover a Eos
                SyntaxToken eosToken = null;
                if (blockBody == null)
                {
                    if (expressionBody != null)
                    {
                        eosToken = EatEos(ref expressionBody);
                    }
                    else if (!constraints.IsNull && constraints.Count > 0)
                    {
                        var lastClause = constraints[constraints.Count - 1];
                        eosToken = EatEos(ref lastClause);
                        constraints[constraints.Count - 1] = lastClause;
                    }
                    else if (returnType != null)
                    {
                        eosToken = EatEos(ref returnType);
                    }
                    else
                    {
                        Debug.Assert(paramList != null);
                        eosToken = EatEos(ref paramList);
                    }
                }

                IsInAsync = false;

                return _syntaxFactory.MethodDeclaration(
                    attributes,
                    modifiers.ToList(),
                    funcToken,
                    explicitInterfaceOpt,
                    identifier,
                    typeParameterList,
                    paramList,
                    minusGreaterThanForReturnType,
                    returnType,
                    constraints,
                    contracts,
                    throwsList,
                    blockBody,
                    expressionBody,
                    eosToken);
            }
            finally
            {
                if (!constraints.IsNull)
                {
                    _pool.Free(constraints);
                }
                if (!contracts.IsNull)
                {
                    _pool.Free(contracts);
                }
            }
        }

        private TypeSyntax ParseReturnType()
        {
            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfReturnType;
            var type = this.ParseType(); //var type = this.ParseTypeOrVoid();
            _termState = saveTerm;
            return type;
        }

        private bool IsEndOfReturnType()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.SemicolonToken:
                    return true;
                default:
                    return false;
            }
        }

        private ConversionOperatorDeclarationSyntax ParseConversionOperatorDeclaration(SyntaxToken funcKeyword, SyntaxToken operatorKeyword, SyntaxListBuilder<AttributeSyntax> attributes, SyntaxListBuilder modifiers)
        {
            SyntaxToken implicitKeyword = null;
            if (this.CurrentToken.Kind == SyntaxKind.ImplicitKeyword)
            {
                implicitKeyword = this.EatToken();
            }

            // func operator implicit as(...) -> ReturnType
            // func operator as(...) -> ReturnType

            var asKeyword = EatToken(SyntaxKind.AsKeyword);

            var paramList = this.ParseParenthesizedParameterList();

            var minusGreaterThanForReturnType = ExpectMinusGreaterThanForReturnType();

            var returnType = ParseType();
            
            var contracts = default(SyntaxListBuilder<ContractClauseSyntax>);
            try
            {
                // Parse any contract clauses
                if (IsContractKeyword(this.CurrentToken.ContextualKind))
                {
                    contracts = _pool.Allocate<ContractClauseSyntax>();
                    this.ParseContractClauses(contracts);
                }
                
                BlockSyntax blockBody;
                ArrowExpressionClauseSyntax expressionBody;
                this.ParseBlockAndExpressionBodies(out blockBody, out expressionBody);

                // If we don't have a block body, we need to recover a Eos
                SyntaxToken eosToken = null;
                if (blockBody == null)
                {
                    if (expressionBody != null)
                    {
                        eosToken = EatEos(ref expressionBody);
                    }
                    else
                    {

                        Debug.Assert(paramList != null);
                        eosToken = EatEos(ref paramList);
                    }
                }

                return _syntaxFactory.ConversionOperatorDeclaration(
                    attributes,
                    modifiers.ToList(),
                    funcKeyword,
                    operatorKeyword,
                    implicitKeyword,
                    asKeyword,
                    paramList,
                    minusGreaterThanForReturnType,
                    returnType,
                    contracts,
                    blockBody,
                    expressionBody,
                    eosToken);
            }
            finally
            {
                if (!contracts.IsNull)
                {
                    _pool.Free(contracts);
                }
            }
        }

        private OperatorDeclarationSyntax ParseOperatorDeclaration(
            SyntaxListBuilder<AttributeSyntax> attributes,
            SyntaxListBuilder modifiers)
        {
            var opKeyword = this.EatToken(SyntaxKind.OperatorKeyword);
            SyntaxToken opToken;
            int opTokenErrorOffset;
            int opTokenErrorWidth;

            if (SyntaxFacts.IsAnyOverloadableOperator(this.CurrentToken.Kind))
            {
                opToken = this.EatToken();
                Debug.Assert(!opToken.IsMissing);
                opTokenErrorOffset = opToken.GetLeadingTriviaWidth();
                opTokenErrorWidth = opToken.Width;
            }
            else
            {
                if (this.CurrentToken.Kind == SyntaxKind.ImplicitKeyword || this.CurrentToken.Kind == SyntaxKind.ExplicitKeyword)
                {
                    // Grab the offset and width before we consume the invalid keyword and change our position.
                    GetDiagnosticSpanForMissingToken(out opTokenErrorOffset, out opTokenErrorWidth);
                    opToken = this.ConvertToMissingWithTrailingTrivia(this.EatToken(), SyntaxKind.PlusToken);
                }
                else
                {
                    //Consume whatever follows the operator keyword as the operator token.  If it is not
                    //we'll add an error below (when we can guess the arity).
                    opToken = EatToken();
                    Debug.Assert(!opToken.IsMissing);
                    opTokenErrorOffset = opToken.GetLeadingTriviaWidth();
                    opTokenErrorWidth = opToken.Width;
                }
            }

            // check for >>
            var opKind = opToken.Kind;
            var tk = this.CurrentToken;
            if (opToken.Kind == SyntaxKind.GreaterThanToken && tk.Kind == SyntaxKind.GreaterThanToken)
            {
                // no trailing trivia and no leading trivia
                if (opToken.GetTrailingTriviaWidth() == 0 && tk.GetLeadingTriviaWidth() == 0)
                {
                    var opToken2 = this.EatToken();
                    opToken = SyntaxFactory.Token(opToken.GetLeadingTrivia(), SyntaxKind.GreaterThanGreaterThanToken, opToken2.GetTrailingTrivia());
                }
            }

            var paramList = this.ParseParenthesizedParameterList();

            switch (paramList.Parameters.Count)
            {
                case 1:
                    if (opToken.IsMissing || !SyntaxFacts.IsOverloadableUnaryOperator(opKind))
                    {
                        SyntaxDiagnosticInfo diagInfo = MakeError(opTokenErrorOffset, opTokenErrorWidth, ErrorCode.ERR_OvlUnaryOperatorExpected);
                        opToken = WithAdditionalDiagnostics(opToken, diagInfo);
                    }

                    break;
                case 2:
                    if (opToken.IsMissing || !SyntaxFacts.IsOverloadableBinaryOperator(opKind))
                    {
                        SyntaxDiagnosticInfo diagInfo = MakeError(opTokenErrorOffset, opTokenErrorWidth, ErrorCode.ERR_OvlBinaryOperatorExpected);
                        opToken = WithAdditionalDiagnostics(opToken, diagInfo);
                    }

                    break;
                default:
                    if (opToken.IsMissing)
                    {
                        SyntaxDiagnosticInfo diagInfo = MakeError(opTokenErrorOffset, opTokenErrorWidth, ErrorCode.ERR_OvlOperatorExpected);
                        opToken = WithAdditionalDiagnostics(opToken, diagInfo);
                    }
                    else if (SyntaxFacts.IsOverloadableBinaryOperator(opKind))
                    {
                        opToken = this.AddError(opToken, ErrorCode.ERR_BadBinOpArgs, SyntaxFacts.GetText(opKind));
                    }
                    else if (SyntaxFacts.IsOverloadableUnaryOperator(opKind))
                    {
                        opToken = this.AddError(opToken, ErrorCode.ERR_BadUnOpArgs, SyntaxFacts.GetText(opKind));
                    }
                    else
                    {
                        opToken = this.AddError(opToken, ErrorCode.ERR_OvlOperatorExpected);
                    }

                    break;
            }

            // An operator has always a return type
            TypeSyntax type = ParseReturnType();
            Debug.Assert(type != null); // How could it be?  The only caller got it from ParseReturnType.

            if (type.IsMissing)
            {
                SyntaxDiagnosticInfo diagInfo = MakeError(opTokenErrorOffset, opTokenErrorWidth, ErrorCode.ERR_BadOperatorSyntax, SyntaxFacts.GetText(SyntaxKind.PlusToken));
                opToken = WithAdditionalDiagnostics(opToken, diagInfo);
            }
            else
            {
                // Dev10 puts this error on the type (if there is one).
                type = this.AddError(type, ErrorCode.ERR_BadOperatorSyntax, SyntaxFacts.GetText(SyntaxKind.PlusToken));
            }

            var contracts = default(SyntaxListBuilder<ContractClauseSyntax>);
            try
            {
                // Parse any contract clauses
                if (IsContractKeyword(this.CurrentToken.ContextualKind))
                {
                    contracts = _pool.Allocate<ContractClauseSyntax>();
                    this.ParseContractClauses(contracts);
                }

                BlockSyntax blockBody;
                ArrowExpressionClauseSyntax expressionBody;
                this.ParseBlockAndExpressionBodies(out blockBody, out expressionBody);

                // If we don't have a block body, we need to recover a Eos
                SyntaxToken eosToken = null;
                if (blockBody == null)
                {
                    if (expressionBody != null)
                    {
                        eosToken = EatEos(ref expressionBody);
                    }
                    else
                    {

                        Debug.Assert(paramList != null);
                        eosToken = EatEos(ref paramList);
                    }
                }

                // if the operator is invalid, then switch it to plus (which will work either way) so that
                // we can finish building the tree
                if (!(opKind == SyntaxKind.IsKeyword ||
                      SyntaxFacts.IsOverloadableUnaryOperator(opKind) ||
                      SyntaxFacts.IsOverloadableBinaryOperator(opKind)))
                {
                    opToken = ConvertToMissingWithTrailingTrivia(opToken, SyntaxKind.PlusToken);
                }

                return _syntaxFactory.OperatorDeclaration(
                    attributes,
                    modifiers.ToList(),
                    type,
                    opKeyword,
                    opToken,
                    paramList,
                    contracts,
                    blockBody,
                    expressionBody,
                    eosToken);
            }
            finally
            {
                if (!contracts.IsNull)
                {
                    _pool.Free(contracts);
                }
            }
        }

        private IndexerDeclarationSyntax ParseIndexerDeclaration(
            SyntaxListBuilder<AttributeSyntax> attributes,
            SyntaxListBuilder modifiers,
            SyntaxToken funcKeyword,
            SyntaxToken operatorKeyword,
            ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt,
            TypeParameterListSyntax typeParameterList)
        {
            // TODO: this part is no longer valid in stark
            // check to see if the user tried to create a generic indexer.
            if (typeParameterList != null)
            {
                operatorKeyword = AddTrailingSkippedSyntax(operatorKeyword, typeParameterList);
                operatorKeyword = this.AddError(operatorKeyword, ErrorCode.ERR_UnexpectedGenericName);
            }

            var parameterList = this.ParseBracketedParameterList();

            var minusGreaterThanForReturnType = ExpectMinusGreaterThanForReturnType();
            var returnType = ParseType();

            var contracts = default(SyntaxListBuilder<ContractClauseSyntax>);
            try
            {
                // Parse any contract clauses
                if (IsContractKeyword(this.CurrentToken.ContextualKind))
                {
                    contracts = _pool.Allocate<ContractClauseSyntax>();
                    this.ParseContractClauses(contracts);
                }

                AccessorListSyntax accessorList = null;
                ArrowExpressionClauseSyntax expressionBody = null;
                SyntaxToken eosToken = null;
                // Try to parse accessor list unless there is an expression
                // body and no accessor list
                if (this.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken)
                {
                    expressionBody = this.ParseArrowExpressionClause();
                    expressionBody = CheckFeatureAvailability(expressionBody, MessageID.IDS_FeatureExpressionBodiedIndexer);
                    eosToken = this.EatEos(ref expressionBody);
                }
                else
                {
                    accessorList = this.ParseAccessorList(isEvent: false);
                }

                // If the user has erroneously provided both an accessor list
                // and an expression body, but no semicolon, we want to parse
                // the expression body and report the error (which is done later)
                if (this.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken
                    && eosToken == null)
                {
                    expressionBody = this.ParseArrowExpressionClause();
                    expressionBody = CheckFeatureAvailability(expressionBody, MessageID.IDS_FeatureExpressionBodiedIndexer);
                    eosToken = this.EatEos(ref expressionBody);
                }

                return _syntaxFactory.IndexerDeclaration(
                    attributes,
                    modifiers.ToList(),
                    funcKeyword,
                    operatorKeyword,
                    explicitInterfaceOpt,
                    parameterList,
                    minusGreaterThanForReturnType,
                    returnType,
                    contracts,
                    accessorList,
                    expressionBody,
                    eosToken);
            }
            finally
            {
                if (!contracts.IsNull)
                {
                    _pool.Free(contracts);
                }
            }
        }

        private PropertyDeclarationSyntax ParsePropertyDeclaration(
            SyntaxListBuilder<AttributeSyntax> attributes,
            SyntaxListBuilder modifiers,
            SyntaxToken funcKeyword,
            ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt,
            SyntaxToken identifier,
            TypeParameterListSyntax typeParameterList
            )
        {
            // check to see if the user tried to create a generic property.
            if (typeParameterList != null)
            {
                identifier = AddTrailingSkippedSyntax(identifier, typeParameterList);
                identifier = this.AddError(identifier, ErrorCode.ERR_UnexpectedGenericName);
            }

            var minusGreaterThanForReturnType = ExpectMinusGreaterThanForReturnType();

            var returnType = ParseType();

            var contracts = default(SyntaxListBuilder<ContractClauseSyntax>);
            try
            {
                // Parse any contract clauses
                if (IsContractKeyword(this.CurrentToken.ContextualKind))
                {
                    contracts = _pool.Allocate<ContractClauseSyntax>();
                    this.ParseContractClauses(contracts);
                }

                AccessorListSyntax accessorList = null;
                if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
                {
                    accessorList = this.ParseAccessorList(isEvent: false);
                }

                ArrowExpressionClauseSyntax expressionBody = null;
                EqualsValueClauseSyntax initializer = null;

                // Check for expression body
                if (this.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken)
                {
                    expressionBody = this.ParseArrowExpressionClause();
                    expressionBody = CheckFeatureAvailability(expressionBody, MessageID.IDS_FeatureExpressionBodiedProperty);
                }
                // Check if we have an initializer
                else if (this.CurrentToken.Kind == SyntaxKind.EqualsToken)
                {
                    var equals = this.EatToken(SyntaxKind.EqualsToken);
                    var value = this.ParseVariableInitializer();
                    initializer = _syntaxFactory.EqualsValueClause(equals, value: value);
                    initializer = CheckFeatureAvailability(initializer, MessageID.IDS_FeatureAutoPropertyInitializer);
                }

                // Force the parsing of an accessor list if we didn't find any before
                if (accessorList == null && expressionBody == null && initializer == null)
                {
                    accessorList = this.ParseAccessorList(isEvent: false);
                }
                
                SyntaxToken eosToken = null;
                if (expressionBody != null)
                {
                    eosToken = EatEos(ref expressionBody);
                }
                else if (initializer != null)
                {
                    eosToken = EatEos(ref initializer);
                }

                return _syntaxFactory.PropertyDeclaration(
                    attributes,
                    modifiers.ToList(),
                    funcKeyword,
                    explicitInterfaceOpt,
                    identifier,
                    minusGreaterThanForReturnType,
                    returnType,
                    contracts,
                    accessorList,
                    expressionBody,
                    initializer,
                    eosToken);
            } finally
            {
                if (!contracts.IsNull)
                {
                    _pool.Free(contracts);
                }
            }
        }

        private AccessorListSyntax ParseAccessorList(bool isEvent)
        {
            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);
            var accessors = default(SyntaxList<AccessorDeclarationSyntax>);

            if (!openBrace.IsMissing || !this.IsTerminator())
            {
                // parse property accessors
                var builder = _pool.Allocate<AccessorDeclarationSyntax>();
                try
                {
                    while (true)
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            break;
                        }
                        else if (this.IsPossibleAccessor())
                        {
                            var acc = this.ParseAccessorDeclaration(isEvent);
                            builder.Add(acc);
                        }
                        else if (this.SkipBadAccessorListTokens(ref openBrace, builder,
                            isEvent ? ErrorCode.ERR_AddOrRemoveExpected : ErrorCode.ERR_GetOrSetExpected) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }

                    accessors = builder.ToList();
                }
                finally
                {
                    _pool.Free(builder);
                }
            }

            var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);
            return _syntaxFactory.AccessorList(openBrace, accessors, closeBrace);
        }

        private ArrowExpressionClauseSyntax ParseArrowExpressionClause()
        {
            var arrowToken = this.EatToken(SyntaxKind.EqualsGreaterThanToken);
            return _syntaxFactory.ArrowExpressionClause(arrowToken, ParsePossibleRefExpression());
        }

        private ExpressionSyntax ParsePossibleRefExpression()
        {
            var refKeyword = default(SyntaxToken);
            if (this.CurrentToken.Kind == SyntaxKind.RefKeyword || this.CurrentToken.Kind == SyntaxKind.InKeyword)
            {
                refKeyword = this.EatToken();
                refKeyword = CheckFeatureAvailability(refKeyword, MessageID.IDS_FeatureRefLocalsReturns);
            }

            var expression = this.ParseExpressionCore();
            if (refKeyword != default(SyntaxToken))
            {
                expression = _syntaxFactory.RefExpression(refKeyword, expression);
            }

            return expression;
        }

        private PostSkipAction SkipBadAccessorListTokens(ref SyntaxToken openBrace, SyntaxListBuilder<AccessorDeclarationSyntax> list, ErrorCode error)
        {
            return this.SkipBadListTokensWithErrorCode(ref openBrace, list,
                p => p.CurrentToken.Kind != SyntaxKind.CloseBraceToken && !p.IsPossibleAccessor(),
                p => p.IsTerminator(),
                error);
        }

        private bool IsPossibleAccessor()
        {
            return this.CurrentToken.Kind == SyntaxKind.IdentifierToken
                || IsPossibleAttributeDeclaration()
                || SyntaxFacts.GetAccessorDeclarationKind(this.CurrentToken.ContextualKind) != SyntaxKind.None
                || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken  // for accessor blocks w/ missing keyword
                || IsPossibleAccessorModifier();
        }

        private bool IsPossibleAccessorModifier()
        {
            // We only want to accept a modifier as the start of an accessor if the modifiers are
            // actually followed by "get/set/add/remove".  Otherwise, we might thing think we're 
            // starting an accessor when we're actually starting a normal class member.  For example:
            //
            //      class C {
            //          public int Prop { get { this.
            //          private DateTime x;
            //
            // We don't want to think of the "private" in "private DateTime x" as starting an accessor
            // here.  If we do, we'll get totally thrown off in parsing the remainder and that will
            // throw off the rest of the features that depend on a good syntax tree.
            // 
            // Note: we allow all modifiers here.  That's because we want to parse things like
            // "abstract get" as an accessor.  This way we can provide a good error message
            // to the user that this is not allowed.

            if (GetModifier(this.CurrentToken) == DeclarationModifiers.None)
            {
                return false;
            }

            var peekIndex = 1;
            while (GetModifier(this.PeekToken(peekIndex)) != DeclarationModifiers.None)
            {
                peekIndex++;
            }

            var token = this.PeekToken(peekIndex);
            if (token.Kind == SyntaxKind.CloseBraceToken || token.Kind == SyntaxKind.EndOfFileToken)
            {
                // If we see "{ get { } public }
                // then we will think that "public" likely starts an accessor.
                return true;
            }

            switch (token.ContextualKind)
            {
                case SyntaxKind.GetKeyword:
                case SyntaxKind.SetKeyword:
                case SyntaxKind.AddKeyword:
                case SyntaxKind.RemoveKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private enum PostSkipAction
        {
            Continue,
            Abort
        }

        private PostSkipAction SkipBadSeparatedListTokensWithExpectedKind<T, TNode>(
            ref T startToken,
            SeparatedSyntaxListBuilder<TNode> list,
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            SyntaxKind expected)
            where T : CSharpSyntaxNode
            where TNode : CSharpSyntaxNode
        {
            // We're going to cheat here and pass the underlying SyntaxListBuilder of "list" to the helper method so that
            // it can append skipped trivia to the last element, regardless of whether that element is a node or a token.
            GreenNode trailingTrivia;
            var action = this.SkipBadListTokensWithExpectedKindHelper(list.UnderlyingBuilder, isNotExpectedFunction, abortFunction, expected, out trailingTrivia);
            if (trailingTrivia != null)
            {
                startToken = AddTrailingSkippedSyntax(startToken, trailingTrivia);
            }
            return action;
        }

        private PostSkipAction SkipBadListTokensWithExpectedKind<T, TNode>(
            ref T startToken,
            SyntaxListBuilder<TNode> list,
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            SyntaxKind expected)
            where T : CSharpSyntaxNode
            where TNode : CSharpSyntaxNode
        {
            GreenNode trailingTrivia;
            var action = this.SkipBadListTokensWithExpectedKindHelper(list, isNotExpectedFunction, abortFunction, expected, out trailingTrivia);
            if (trailingTrivia != null)
            {
                startToken = AddTrailingSkippedSyntax(startToken, trailingTrivia);
            }
            return action;
        }

        private PostSkipAction SkipBadListTokensWithErrorCode<T, TNode>(
            ref T startToken,
            SyntaxListBuilder<TNode> list,
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            ErrorCode error)
            where T : CSharpSyntaxNode
            where TNode : CSharpSyntaxNode
        {
            GreenNode trailingTrivia;
            var action = this.SkipBadListTokensWithErrorCodeHelper(list, isNotExpectedFunction, abortFunction, error, out trailingTrivia);
            if (trailingTrivia != null)
            {
                startToken = AddTrailingSkippedSyntax(startToken, trailingTrivia);
            }
            return action;
        }

        /// <remarks>
        /// WARNING: it is possible that "list" is really the underlying builder of a SeparateSyntaxListBuilder,
        /// so it is important that we not add anything to the list.
        /// </remarks>
        private PostSkipAction SkipBadListTokensWithExpectedKindHelper(
            SyntaxListBuilder list,
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            SyntaxKind expected,
            out GreenNode trailingTrivia)
        {
            if (list.Count == 0)
            {
                return SkipBadTokensWithExpectedKind(isNotExpectedFunction, abortFunction, expected, out trailingTrivia);
            }
            else
            {
                GreenNode lastItemTrailingTrivia;
                var action = SkipBadTokensWithExpectedKind(isNotExpectedFunction, abortFunction, expected, out lastItemTrailingTrivia);
                if (lastItemTrailingTrivia != null)
                {
                    AddTrailingSkippedSyntax(list, lastItemTrailingTrivia);
                }
                trailingTrivia = null;
                return action;
            }
        }

        private PostSkipAction SkipBadListTokensWithErrorCodeHelper<TNode>(
            SyntaxListBuilder<TNode> list,
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            ErrorCode error,
            out GreenNode trailingTrivia) where TNode : CSharpSyntaxNode
        {
            if (list.Count == 0)
            {
                return SkipBadTokensWithErrorCode(isNotExpectedFunction, abortFunction, error, out trailingTrivia);
            }
            else
            {
                GreenNode lastItemTrailingTrivia;
                var action = SkipBadTokensWithErrorCode(isNotExpectedFunction, abortFunction, error, out lastItemTrailingTrivia);
                if (lastItemTrailingTrivia != null)
                {
                    AddTrailingSkippedSyntax(list, lastItemTrailingTrivia);
                }
                trailingTrivia = null;
                return action;
            }
        }

        private PostSkipAction SkipBadTokensWithExpectedKind(
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            SyntaxKind expected,
            out GreenNode trailingTrivia)
        {
            var nodes = _pool.Allocate();
            try
            {
                bool first = true;
                var action = PostSkipAction.Continue;
                while (isNotExpectedFunction(this))
                {
                    if (abortFunction(this))
                    {
                        action = PostSkipAction.Abort;
                        break;
                    }

                    var token = (first && !this.CurrentToken.ContainsDiagnostics) ? this.EatTokenWithPrejudice(expected) : this.EatToken();
                    first = false;
                    nodes.Add(token);
                }

                trailingTrivia = (nodes.Count > 0) ? nodes.ToListNode() : null;
                return action;
            }
            finally
            {
                _pool.Free(nodes);
            }
        }

        private PostSkipAction SkipBadTokensWithErrorCode(
            Func<LanguageParser, bool> isNotExpectedFunction,
            Func<LanguageParser, bool> abortFunction,
            ErrorCode errorCode,
            out GreenNode trailingTrivia)
        {
            var nodes = _pool.Allocate();
            try
            {
                bool first = true;
                var action = PostSkipAction.Continue;
                while (isNotExpectedFunction(this))
                {
                    if (abortFunction(this))
                    {
                        action = PostSkipAction.Abort;
                        break;
                    }

                    var token = (first && !this.CurrentToken.ContainsDiagnostics) ? this.EatTokenWithPrejudice(errorCode) : this.EatToken();
                    first = false;
                    nodes.Add(token);
                }

                trailingTrivia = (nodes.Count > 0) ? nodes.ToListNode() : null;
                return action;
            }
            finally
            {
                _pool.Free(nodes);
            }
        }

        private AccessorDeclarationSyntax ParseAccessorDeclaration(bool isEvent)
        {
            if (this.IsIncrementalAndFactoryContextMatches && CanReuseAccessorDeclaration())
            {
                return (AccessorDeclarationSyntax)this.EatNode();
            }

            var accAttrs = _pool.Allocate<AttributeSyntax>();
            var accMods = _pool.Allocate();
            try
            {
                this.ParseAttributeSyntaxList(accAttrs);
                this.ParseModifiers(accMods, forAccessors: true);

                if (!isEvent)
                {
                    if (accMods != null && accMods.Count > 0)
                    {
                        accMods[0] = CheckFeatureAvailability(accMods[0], MessageID.IDS_FeaturePropertyAccessorMods);
                    }
                }

                var accessorName = this.EatToken(SyntaxKind.IdentifierToken,
                    isEvent ? ErrorCode.ERR_AddOrRemoveExpected : ErrorCode.ERR_GetOrSetExpected);
                var accessorKind = GetAccessorKind(accessorName);

                // Only convert the identifier to a keyword if it's a valid one.  Otherwise any
                // other contextual keyword (like 'partial') will be converted into a keyword
                // and will be invalid.
                if (accessorKind == SyntaxKind.UnknownAccessorDeclaration)
                {
                    // We'll have an UnknownAccessorDeclaration either because we didn't have
                    // an IdentifierToken or because we have an IdentifierToken which is not
                    // add/remove/get/set.  In the former case, we'll already have reported
                    // an error and will have a missing token.  But in the latter case we need 
                    // to report that the identifier is incorrect.
                    if (!accessorName.IsMissing)
                    {
                        accessorName = this.AddError(accessorName,
                            isEvent ? ErrorCode.ERR_AddOrRemoveExpected : ErrorCode.ERR_GetOrSetExpected);
                    }
                    else
                    {
                        Debug.Assert(accessorName.ContainsDiagnostics);
                    }
                }
                else
                {
                    accessorName = ConvertToKeyword(accessorName);
                }

                BlockSyntax blockBody = null;
                ArrowExpressionClauseSyntax expressionBody = null;
                SyntaxToken eosToken = null;

                bool currentTokenIsArrow = this.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken;
                bool currentTokenIsOpenBraceToken = this.CurrentToken.Kind == SyntaxKind.OpenBraceToken;

                if (currentTokenIsOpenBraceToken || currentTokenIsArrow)
                {
                    this.ParseBlockAndExpressionBodies(out blockBody, out expressionBody, requestedExpressionBodyFeature: MessageID.IDS_FeatureExpressionBodiedAccessor);
                }

                // If we don't have a block body, we need to recover a Eos
                if (blockBody == null)
                {
                    if (expressionBody != null)
                    {
                        eosToken = EatEos(ref expressionBody);
                    }
                    else
                    {
                        if (accessorKind == SyntaxKind.AddAccessorDeclaration ||
                            accessorKind == SyntaxKind.RemoveAccessorDeclaration)
                        {
                            eosToken = EatEos(ref accessorName);
                            eosToken = this.AddError(eosToken, ErrorCode.ERR_AddRemoveMustHaveBody);
                        }
                    }
                }

                return _syntaxFactory.AccessorDeclaration(
                    accessorKind, accAttrs, accMods.ToList(), accessorName,
                    blockBody, expressionBody, eosToken);
            }
            finally
            {
                _pool.Free(accMods);
                _pool.Free(accAttrs);
            }
        }

        private SyntaxKind GetAccessorKind(SyntaxToken accessorName)
        {
            switch (accessorName.ContextualKind)
            {
                case SyntaxKind.GetKeyword: return SyntaxKind.GetAccessorDeclaration;
                case SyntaxKind.SetKeyword: return SyntaxKind.SetAccessorDeclaration;
                case SyntaxKind.AddKeyword: return SyntaxKind.AddAccessorDeclaration;
                case SyntaxKind.RemoveKeyword: return SyntaxKind.RemoveAccessorDeclaration;
            }

            return SyntaxKind.UnknownAccessorDeclaration;
        }

        private bool CanReuseAccessorDeclaration()
        {
            switch (this.CurrentNodeKind)
            {
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                    return true;
            }

            return false;
        }

        internal ParameterListSyntax ParseParenthesizedParameterList()
        {
            if (this.IsIncrementalAndFactoryContextMatches && CanReuseParameterList(this.CurrentNode as Stark.Syntax.ParameterListSyntax))
            {
                return (ParameterListSyntax)this.EatNode();
            }

            var parameters = _pool.AllocateSeparated<ParameterSyntax>();

            try
            {
                var openKind = SyntaxKind.OpenParenToken;
                var closeKind = SyntaxKind.CloseParenToken;

                SyntaxToken open;
                SyntaxToken close;
                this.ParseParameterList(out open, parameters, out close, openKind, closeKind);
                return _syntaxFactory.ParameterList(open, parameters, close);
            }
            finally
            {
                _pool.Free(parameters);
            }
        }

        internal BracketedParameterListSyntax ParseBracketedParameterList()
        {
            if (this.IsIncrementalAndFactoryContextMatches && CanReuseBracketedParameterList(this.CurrentNode as Stark.Syntax.BracketedParameterListSyntax))
            {
                return (BracketedParameterListSyntax)this.EatNode();
            }

            var parameters = _pool.AllocateSeparated<ParameterSyntax>();

            try
            {
                var openKind = SyntaxKind.OpenBracketToken;
                var closeKind = SyntaxKind.CloseBracketToken;

                SyntaxToken open;
                SyntaxToken close;
                this.ParseParameterList(out open, parameters, out close, openKind, closeKind);
                return _syntaxFactory.BracketedParameterList(open, parameters, close);
            }
            finally
            {
                _pool.Free(parameters);
            }
        }

        private static bool CanReuseParameterList(Stark.Syntax.ParameterListSyntax list)
        {
            if (list == null)
            {
                return false;
            }

            if (list.OpenParenToken.IsMissing)
            {
                return false;
            }

            if (list.CloseParenToken.IsMissing)
            {
                return false;
            }

            foreach (var parameter in list.Parameters)
            {
                if (!CanReuseParameter(parameter))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanReuseBracketedParameterList(Stark.Syntax.BracketedParameterListSyntax list)
        {
            if (list == null)
            {
                return false;
            }

            if (list.OpenBracketToken.IsMissing)
            {
                return false;
            }

            if (list.CloseBracketToken.IsMissing)
            {
                return false;
            }

            foreach (var parameter in list.Parameters)
            {
                if (!CanReuseParameter(parameter))
                {
                    return false;
                }
            }

            return true;
        }

        private void ParseParameterList(
            out SyntaxToken open,
            SeparatedSyntaxListBuilder<ParameterSyntax> nodes,
            out SyntaxToken close,
            SyntaxKind openKind,
            SyntaxKind closeKind)
        {
            open = this.EatToken(openKind);

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfParameterList;

            var attributes = _pool.Allocate<AttributeSyntax>();
            try
            {
                if (this.CurrentToken.Kind != closeKind)
                {
tryAgain:
                    if (this.IsPossibleParameter() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        // first parameter
                        attributes.Clear();
                        var parameter = this.ParseParameter(attributes);
                        nodes.Add(parameter);

                        // additional parameters
                        while (true)
                        {
                            if (this.CurrentToken.Kind == closeKind)
                            {
                                break;
                            }
                            else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleParameter())
                            {
                                nodes.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                                attributes.Clear();
                                parameter = this.ParseParameter(attributes);
                                if (parameter.IsMissing && this.IsPossibleParameter())
                                {
                                    // ensure we always consume tokens
                                    parameter = AddTrailingSkippedSyntax(parameter, this.EatToken());
                                }

                                nodes.Add(parameter);
                                continue;
                            }
                            else if (this.SkipBadParameterListTokens(ref open, nodes, SyntaxKind.CommaToken, closeKind) == PostSkipAction.Abort)
                            {
                                break;
                            }
                        }
                    }
                    else if (this.SkipBadParameterListTokens(ref open, nodes, SyntaxKind.IdentifierToken, closeKind) == PostSkipAction.Continue)
                    {
                        goto tryAgain;
                    }
                }

                _termState = saveTerm;
                close = this.EatToken(closeKind);
            }
            finally
            {
                _pool.Free(attributes);
            }
        }

        private bool IsEndOfParameterList()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                || this.CurrentToken.Kind == SyntaxKind.CloseBracketToken;
        }

        private PostSkipAction SkipBadParameterListTokens(
            ref SyntaxToken open, SeparatedSyntaxListBuilder<ParameterSyntax> list, SyntaxKind expected, SyntaxKind closeKind)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref open, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleParameter(),
                p => p.CurrentToken.Kind == closeKind || p.IsTerminator(),
                expected);
        }

        private bool IsPossibleParameter()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.AtToken: // attribute
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.OpenParenToken:   // tuple
                    return true;

                case SyntaxKind.IdentifierToken:
                    return this.IsTrueIdentifier();

                default:
                    return IsParameterModifier(this.CurrentToken.Kind) || IsPredefinedType(this.CurrentToken.Kind);
            }
        }

        private static bool CanReuseParameter(Stark.Syntax.ParameterSyntax parameter, SyntaxListBuilder<AttributeSyntax> attributes)
        {
            if (parameter == null)
            {
                return false;
            }

            // cannot reuse parameter if it had attributes.
            //
            // TODO(cyrusn): Why?  We can reuse other constructs if they have attributes.
            if (attributes.Count != 0 || parameter.AttributeLists.Count != 0)
            {
                return false;
            }

            return CanReuseParameter(parameter);
        }

        private static bool CanReuseParameter(Stark.Syntax.ParameterSyntax parameter)
        {
            // cannot reuse a node that possibly ends in an expression
            if (parameter.Default != null)
            {
                return false;
            }

            // cannot reuse lambda parameters as normal parameters (parsed with
            // different rules)
            Stark.CSharpSyntaxNode parent = parameter.Parent;
            if (parent != null)
            {
                if (parent.Kind() == SyntaxKind.SimpleLambdaExpression)
                {
                    return false;
                }

                Stark.CSharpSyntaxNode grandparent = parent.Parent;
                if (grandparent != null && grandparent.Kind() == SyntaxKind.ParenthesizedLambdaExpression)
                {
                    Debug.Assert(parent.Kind() == SyntaxKind.ParameterList);
                    return false;
                }
            }

            return true;
        }

        private ParameterSyntax ParseParameter(SyntaxListBuilder<AttributeSyntax> attributes)
        {
            if (this.IsIncrementalAndFactoryContextMatches && CanReuseParameter(this.CurrentNode as Stark.Syntax.ParameterSyntax, attributes))
            {
                return (ParameterSyntax)this.EatNode();
            }

            this.ParseAttributeSyntaxList(attributes);

            TypeSyntax type;
            SyntaxToken name;
            name = this.ParseIdentifierToken();
            var colon = ExpectColon();
            type = this.ParseType(mode: ParseTypeMode.Parameter);

            EqualsValueClauseSyntax def = null;
            if (this.CurrentToken.Kind == SyntaxKind.EqualsToken)
            {
                var equals = this.EatToken(SyntaxKind.EqualsToken);
                var value = this.ParseExpressionCore();
                def = _syntaxFactory.EqualsValueClause(equals, value: value);
                def = CheckFeatureAvailability(def, MessageID.IDS_FeatureOptionalParameter);
            }
            return _syntaxFactory.Parameter(attributes, name, colon, type, def);
        }

        private SyntaxToken ExpectColon()
        {
            return this.EatToken(SyntaxKind.ColonToken);
        }

        private SyntaxToken ExpectMinusGreaterThanForReturnType()
        {
            SyntaxToken colonToken;
            if (this.CurrentToken.Kind == SyntaxKind.MinusGreaterThanToken)
            {
                colonToken = this.EatToken(SyntaxKind.MinusGreaterThanToken);
            }
            else
            {
                colonToken = this.EatToken();
                colonToken = this.AddError(colonToken, ErrorCode.ERR_MinusGreaterThanExpectedForReturnType);
            }

            return colonToken;
        }

        private static bool IsParameterModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ThisKeyword:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.ParamsKeyword:
                    return true;
            }

            return false;
        }

        private void ParseParameterModifiers(SyntaxListBuilder modifiers)
        {
            while (IsParameterModifier(this.CurrentToken.Kind))
            {
                var modifier = this.EatToken();

                switch (modifier.Kind)
                {
                    case SyntaxKind.ThisKeyword:
                        modifier = CheckFeatureAvailability(modifier, MessageID.IDS_FeatureExtensionMethod);
                        break;

                    case SyntaxKind.RefKeyword:
                        {
                            if (this.CurrentToken.Kind == SyntaxKind.ThisKeyword)
                            {
                                modifier = CheckFeatureAvailability(modifier, MessageID.IDS_FeatureRefExtensionMethods);
                            }

                            break;
                        }

                    case SyntaxKind.InKeyword:
                        {
                            modifier = CheckFeatureAvailability(modifier, MessageID.IDS_FeatureReadOnlyReferences);

                            if (this.CurrentToken.Kind == SyntaxKind.ThisKeyword)
                            {
                                modifier = CheckFeatureAvailability(modifier, MessageID.IDS_FeatureRefExtensionMethods);
                            }

                            break;
                        }
                }

                modifiers.Add(modifier);
            }
        }

        private FieldDeclarationSyntax ParseFixedSizeBufferDeclaration(
            SyntaxListBuilder<AttributeSyntax> attributes,
            SyntaxListBuilder modifiers,
            SyntaxKind parentKind)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.FixedKeyword);

            throw new NotImplementedException();

            //var fixedToken = this.EatToken();
            //fixedToken = CheckFeatureAvailability(fixedToken, MessageID.IDS_FeatureFixedBuffer);
            //modifiers.Add(fixedToken);

            //var type = this.ParseType();

            //var saveTerm = _termState;
            //_termState |= TerminatorState.IsEndOfFieldDeclaration;
            //var variables = _pool.AllocateSeparated<VariableDeclarationSyntax>();
            //try
            //{
            //    this.ParseVariableDeclaration(type, VariableFlags.Fixed, variables, parentKind);

            //    var semicolon = this.EatToken(SyntaxKind.SemicolonToken);

            //    return _syntaxFactory.FieldDeclaration(
            //        attributes, modifiers.ToList(),
            //        _syntaxFactory.VariableDeclaration(_syntaxFactory. type, variables),
            //        semicolon);
            //}
            //finally
            //{
            //    _termState = saveTerm;
            //    _pool.Free(variables);
            //}
        }

        private TNode EatUnexpectedTrailingSemicolon<TNode>(TNode decl) where TNode : CSharpSyntaxNode
        {
            // allow for case of one unexpected semicolon...
            if (this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                var semi = this.EatToken();
                semi = this.AddError(semi, ErrorCode.ERR_UnexpectedSemicolon);
                decl = AddTrailingSkippedSyntax(decl, semi);
            }

            return decl;
        }

        private FieldDeclarationSyntax ParseFieldDeclaration(
            SyntaxListBuilder<AttributeSyntax> attributes,
            SyntaxListBuilder modifiers,
            SyntaxKind parentKind)
        {
            var variableDeclaration = ParseVariableDeclaration(false);
            var eos = EatEos(ref variableDeclaration);
            return _syntaxFactory.FieldDeclaration(attributes, modifiers.ToList(), variableDeclaration, eos);
        }

        private VariableDeclarationSyntax ParseVariableDeclaration(bool isLocal = true)
        {
            SyntaxToken variableKeywordToken;
            if (!(CurrentToken.Kind == SyntaxKind.VarKeyword ||
                  CurrentToken.Kind == SyntaxKind.LetKeyword ||
                  CurrentToken.Kind == SyntaxKind.ConstKeyword))
            {
                variableKeywordToken = EatToken(SyntaxKind.VarKeyword);
            }
            else
            {
                variableKeywordToken = EatToken();
            }

            var name = this.ParseIdentifierToken();

            var isConst = variableKeywordToken.Kind == SyntaxKind.ConstKeyword;

            SyntaxToken colonToken = null;
            TypeSyntax typeSyntax = null;

            if (CurrentToken.Kind == SyntaxKind.ColonToken || !isLocal)
            {
                // if isLocal we force to try to read a colon token / type as it is mandatory
                colonToken = EatToken(SyntaxKind.ColonToken);
                typeSyntax = ParseType();
            }
            else
            {
                //typeSyntax = _syntaxFactory.IdentifierName(variableKeywordToken);
            }

            // TODO: Add check that a Type must be specified when the variable is a field

            EqualsValueClauseSyntax initializer = null;

            if (CurrentToken.Kind == SyntaxKind.EqualsToken)
            {
                var equals = this.EatToken();

                SyntaxToken refKeyword = null;
                if (isLocal && !isConst &&
                    this.CurrentToken.Kind == SyntaxKind.RefKeyword)
                {
                    refKeyword = this.EatToken();
                    refKeyword = CheckFeatureAvailability(refKeyword, MessageID.IDS_FeatureRefLocalsReturns);
                }

                var init = this.ParseVariableInitializer();
                if (refKeyword != null)
                {
                    init = _syntaxFactory.RefExpression(refKeyword, init);
                }

                initializer = _syntaxFactory.EqualsValueClause(equals, init);
            }

            return _syntaxFactory.VariableDeclaration(variableKeywordToken, name, colonToken, typeSyntax, initializer);
        }

        private bool IsEndOfFieldDeclaration()
        {
            return this.CurrentToken.Kind == SyntaxKind.SemicolonToken;
        }

        [Flags]
        private enum VariableFlags
        {
            Fixed = 0x01,
            Const = 0x02,
            Local = 0x04
        }

        private static SyntaxTokenList GetOriginalModifiers(Stark.CSharpSyntaxNode decl)
        {
            if (decl != null)
            {
                switch (decl.Kind())
                {
                    case SyntaxKind.FieldDeclaration:
                        return ((Stark.Syntax.FieldDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.MethodDeclaration:
                        return ((Stark.Syntax.MethodDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.ConstructorDeclaration:
                        return ((Stark.Syntax.ConstructorDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.DestructorDeclaration:
                        return ((Stark.Syntax.DestructorDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.PropertyDeclaration:
                        return ((Stark.Syntax.PropertyDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.EventFieldDeclaration:
                        return ((Stark.Syntax.EventFieldDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.AddAccessorDeclaration:
                    case SyntaxKind.RemoveAccessorDeclaration:
                    case SyntaxKind.GetAccessorDeclaration:
                    case SyntaxKind.SetAccessorDeclaration:
                        return ((Stark.Syntax.AccessorDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.ModuleDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                        return ((Stark.Syntax.TypeDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.DelegateDeclaration:
                        return ((Stark.Syntax.DelegateDeclarationSyntax)decl).Modifiers;
                }
            }

            return default(SyntaxTokenList);
        }

        private static VariableFlags GetOriginalVariableFlags(Stark.Syntax.VariableDeclarationSyntax old)
        {
            var parent = GetOldParent(old);
            var mods = GetOriginalModifiers(parent);
            VariableFlags flags = default(VariableFlags);
            if (mods.Any(SyntaxKind.FixedKeyword))
            {
                flags |= VariableFlags.Fixed;
            }

            if (mods.Any(SyntaxKind.ConstKeyword))
            {
                flags |= VariableFlags.Const;
            }

            if (parent != null && (parent.Kind() == SyntaxKind.VariableDeclaration || parent.Kind() == SyntaxKind.LocalDeclarationStatement))
            {
                flags |= VariableFlags.Local;
            }

            return flags;
        }

        private static bool CanReuseVariableDeclaration(Stark.Syntax.VariableDeclarationSyntax old, VariableFlags flags, bool isFirst)
        {
            if (old == null)
            {
                return false;
            }

            SyntaxKind oldKind;

            return (flags == GetOriginalVariableFlags(old))
                && old.Initializer == null  // can't reuse node that possibly ends in an expression
                && (oldKind = GetOldParent(old).Kind()) != SyntaxKind.VariableDeclaration // or in a method body
                && oldKind != SyntaxKind.LocalDeclarationStatement;
        }

        private bool IsPossibleEndOfVariableDeclaration()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.CommaToken:
                case SyntaxKind.SemicolonToken:
                    return true;
                default:
                    return false;
            }
        }

        private ExpressionSyntax ParseVariableInitializer()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return this.ParseArrayInitializer();
                default:
                    return this.ParseExpressionCore();
            }
        }

        private bool IsPossibleVariableInitializer()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBraceToken || this.IsPossibleExpression();
        }

        private DelegateDeclarationSyntax ParseDelegateDeclaration(SyntaxListBuilder<AttributeSyntax> attributes, SyntaxListBuilder modifiers)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.DelegateKeyword);

            var delegateToken = this.EatToken(SyntaxKind.DelegateKeyword);
            var type = this.ParseReturnType();
            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfMethodSignature;
            var name = this.ParseIdentifierToken();
            var typeParameters = this.ParseTypeParameterList();
            var parameterList = this.ParseParenthesizedParameterList();
            var constraints = default(SyntaxListBuilder<TypeParameterConstraintClauseSyntax>);
            try
            {
                if (this.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
                {
                    constraints = _pool.Allocate<TypeParameterConstraintClauseSyntax>();
                    this.ParseTypeParameterConstraintClauses(constraints);
                }

                _termState = saveTerm;

                SyntaxToken eos = null;
                if (!constraints.IsNull && constraints.Count > 0)
                {
                    var lastContraint = constraints[constraints.Count - 1];
                    eos = this.EatEos(ref lastContraint);
                    constraints[constraints.Count - 1] = lastContraint;
                }
                else
                {
                    Debug.Assert(parameterList != null);
                    eos = this.EatEos(ref parameterList);
                }
                
                return _syntaxFactory.DelegateDeclaration(attributes, modifiers.ToList(), delegateToken, type, name, typeParameters, parameterList, constraints, eos);
            }
            finally
            {
                if (!constraints.IsNull)
                {
                    _pool.Free(constraints);
                }
            }
        }

        private EnumDeclarationSyntax ParseEnumDeclaration(SyntaxListBuilder<AttributeSyntax> attributes, SyntaxListBuilder modifiers)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.EnumKeyword);

            var enumToken = this.EatToken(SyntaxKind.EnumKeyword);
            var name = this.ParseIdentifierToken();

            // check to see if the user tried to create a generic enum.
            var typeParameters = this.ParseTypeParameterList();

            if (typeParameters != null)
            {
                name = AddTrailingSkippedSyntax(name, typeParameters);
                name = this.AddError(name, ErrorCode.ERR_UnexpectedGenericName);
            }

            ExtendListSyntax extendList = null;
            if (this.CurrentToken.Kind == SyntaxKind.ExtendsKeyword)
            {
                var extendsKeyword = this.EatToken(SyntaxKind.ExtendsKeyword);
                var type = this.ParseType();
                var tmpList = _pool.AllocateSeparated<BaseTypeSyntax>();
                tmpList.Add(_syntaxFactory.SimpleBaseType(type));
                extendList = _syntaxFactory.ExtendList(extendsKeyword, tmpList);
                _pool.Free(tmpList);
            }

            var members = default(SyntaxList<EnumMemberDeclarationSyntax>);
            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);

            if (!openBrace.IsMissing)
            {
                var builder = _pool.Allocate<EnumMemberDeclarationSyntax>();
                try
                {
                    this.ParseEnumMemberDeclarations(ref openBrace, builder);
                    members = builder.ToList();
                }
                finally
                {
                    _pool.Free(builder);
                }
            }

            var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);

            SyntaxToken semicolon = null;

            return _syntaxFactory.EnumDeclaration(
                attributes,
                modifiers.ToList(),
                enumToken,
                name,
                extendList,
                null,
                openBrace,
                members,
                closeBrace,
                semicolon);
        }

        private void ParseEnumMemberDeclarations(
            ref SyntaxToken openBrace,
            SyntaxListBuilder<EnumMemberDeclarationSyntax> members)
        {
            if (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
            {
tryAgain:
                if (this.IsPossibleEnumMemberDeclaration())
                {
                    // first member
                    members.Add(this.ParseEnumMemberDeclaration());

                    // additional members
                    while (true)
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            break;
                        }
                        else if (this.IsPossibleEnumMemberDeclaration())
                        {
                            members.Add(this.ParseEnumMemberDeclaration());
                            continue;
                        }
                        else if (this.SkipBadEnumMemberListTokens(ref openBrace, members, SyntaxKind.IdentifierToken) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                else if (this.SkipBadEnumMemberListTokens(ref openBrace, members, SyntaxKind.IdentifierToken) == PostSkipAction.Continue)
                {
                    goto tryAgain;
                }
            }
        }

        private PostSkipAction SkipBadEnumMemberListTokens(ref SyntaxToken openBrace, SyntaxListBuilder<EnumMemberDeclarationSyntax> list, SyntaxKind expected)
        {
            return this.SkipBadListTokensWithExpectedKind(ref openBrace, list,
                p => !p.IsPossibleEnumMemberDeclaration(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(),
                expected);
        }

        private EnumMemberDeclarationSyntax ParseEnumMemberDeclaration()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.EnumMemberDeclaration)
            {
                return (EnumMemberDeclarationSyntax)this.EatNode();
            }

            var memberAttrs = _pool.Allocate<AttributeSyntax>();
            try
            {
                this.ParseAttributeSyntaxList(memberAttrs);
                var memberName = this.ParseIdentifierToken();
                EqualsValueClauseSyntax equalsValue = null;
                if (this.CurrentToken.Kind == SyntaxKind.EqualsToken)
                {
                    var equals = this.EatToken(SyntaxKind.EqualsToken);
                    ExpressionSyntax value;
                    if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                    {
                        //an identifier is a valid expression
                        value = this.ParseIdentifierName(ErrorCode.ERR_ConstantExpected);
                    }
                    else
                    {
                        value = this.ParseExpressionCore();
                    }

                    equalsValue = _syntaxFactory.EqualsValueClause(equals, value: value);
                }

                return _syntaxFactory.EnumMemberDeclaration(memberAttrs, memberName, equalsValue);
            }
            finally
            {
                _pool.Free(memberAttrs);
            }
        }

        private bool IsPossibleEnumMemberDeclaration()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBracketToken || this.IsTrueIdentifier();
        }

        private bool IsDotOrColonColon()
        {
            return this.CurrentToken.Kind == SyntaxKind.DotToken || this.CurrentToken.Kind == SyntaxKind.ColonColonToken;
        }

        // This is public and parses open types. You probably don't want to use it.
        public NameSyntax ParseName()
        {
            return this.ParseQualifiedName();
        }

        private IdentifierNameSyntax CreateMissingIdentifierName()
        {
            return _syntaxFactory.IdentifierName(CreateMissingIdentifierToken());
        }

        private static SyntaxToken CreateMissingIdentifierToken()
        {
            return SyntaxFactory.MissingToken(SyntaxKind.IdentifierToken);
        }

        [Flags]
        private enum NameOptions
        {
            None = 0,
            InExpression = 1 << 0, // Used to influence parser ambiguity around "<" and generics vs. expressions. Used in ParseSimpleName.
            InTypeList = 1 << 1, // Allows attributes to appear within the generic type argument list. Used during ParseInstantiation.
            PossiblePattern = 1 << 2, // Used to influence parser ambiguity around "<" and generics vs. expressions on the right of 'is'
            AfterIs = 1 << 3,
            DefinitePattern = 1 << 4,
            AfterOut = 1 << 5,
            AfterTupleComma = 1 << 6,
            FirstElementOfPossibleTupleLiteral = 1 << 7,
        }

        /// <summary>
        /// True if current identifier token is not really some contextual keyword
        /// </summary>
        /// <returns></returns>
        private bool IsTrueIdentifier()
        {
            if (this.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                if (!IsCurrentTokenPartialKeywordOfPartialMethodOrType() &&
                    !IsCurrentTokenQueryKeywordInQuery() &&
                    !IsCurrentTokenWhereOfConstraintClause())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// True if the given token is not really some contextual keyword.
        /// This method is for use in executable code, as it treats <c>partial</c> as an identifier.
        /// </summary>
        private bool IsTrueIdentifier(SyntaxToken token)
        {
            return
                token.Kind == SyntaxKind.IdentifierToken &&
                !(this.IsInQuery && IsTokenQueryContextualKeyword(token));
        }

        private IdentifierNameSyntax ParseIdentifierName(ErrorCode code = ErrorCode.ERR_IdentifierExpected)
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.IdentifierName)
            {
                if (!SyntaxFacts.IsContextualKeyword(((Stark.Syntax.IdentifierNameSyntax)this.CurrentNode).Identifier.Kind()))
                {
                    return (IdentifierNameSyntax)this.EatNode();
                }
            }

            var tk = ParseIdentifierToken(code);
            return SyntaxFactory.IdentifierName(tk);
        }

        private SyntaxToken ParseIdentifierToken(ErrorCode code = ErrorCode.ERR_IdentifierExpected)
        {
            var ctk = this.CurrentToken.Kind;
            if (ctk == SyntaxKind.IdentifierToken)
            {
                // Error tolerance for IntelliSense. Consider the following case: [EditorBrowsable( partial class Goo {
                // } Because we're parsing an attribute argument we'll end up consuming the "partial" identifier and
                // we'll eventually end up in an pretty confused state.  Because of that it becomes very difficult to
                // show the correct parameter help in this case.  So, when we see "partial" we check if it's being used
                // as an identifier or as a contextual keyword.  If it's the latter then we bail out.  See
                // Bug: vswhidbey/542125
                if (IsCurrentTokenPartialKeywordOfPartialMethodOrType() || IsCurrentTokenQueryKeywordInQuery())
                {
                    var result = CreateMissingIdentifierToken();
                    result = this.AddError(result, ErrorCode.ERR_InvalidExprTerm, this.CurrentToken.Text);
                    return result;
                }

                SyntaxToken identifierToken = this.EatToken();

                if (this.IsInAsync && identifierToken.ContextualKind == SyntaxKind.AwaitKeyword)
                {
                    identifierToken = this.AddError(identifierToken, ErrorCode.ERR_BadAwaitAsIdentifier);
                }

                return identifierToken;
            }
            else
            {
                var name = CreateMissingIdentifierToken();
                name = this.AddError(name, code);
                return name;
            }
        }

        private bool IsCurrentTokenQueryKeywordInQuery()
        {
            return this.IsInQuery && this.IsCurrentTokenQueryContextualKeyword;
        }

        private bool IsCurrentTokenPartialKeywordOfPartialMethodOrType()
        {
            if (this.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword)
            {
                if (this.IsPartialType() || this.IsPartialMember())
                {
                    return true;
                }
            }

            return false;
        }

        private TypeParameterListSyntax ParseTypeParameterList()
        {
            if (this.CurrentToken.Kind != SyntaxKind.LessThanToken)
            {
                return null;
            }

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfTypeParameterList;
            try
            {
                var parameters = _pool.AllocateSeparated<TypeParameterSyntax>();
                var open = this.EatToken(SyntaxKind.LessThanToken);
                open = CheckFeatureAvailability(open, MessageID.IDS_FeatureGenerics);

                // first parameter
                parameters.Add(this.ParseTypeParameter());

                // remaining parameter & commas
                while (true)
                {
                    if (this.CurrentToken.Kind == SyntaxKind.GreaterThanToken || this.IsCurrentTokenWhereOfConstraintClause())
                    {
                        break;
                    }
                    else if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        parameters.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                        parameters.Add(this.ParseTypeParameter());
                    }
                    else if (this.SkipBadTypeParameterListTokens(parameters, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }

                var close = this.EatToken(SyntaxKind.GreaterThanToken);

                return _syntaxFactory.TypeParameterList(open, parameters, close);
            }
            finally
            {
                _termState = saveTerm;
            }
        }

        private PostSkipAction SkipBadTypeParameterListTokens(SeparatedSyntaxListBuilder<TypeParameterSyntax> list, SyntaxKind expected)
        {
            CSharpSyntaxNode tmp = null;
            Debug.Assert(list.Count > 0);
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref tmp, list,
                p => this.CurrentToken.Kind != SyntaxKind.CommaToken,
                p => this.CurrentToken.Kind == SyntaxKind.GreaterThanToken || this.IsTerminator(),
                expected);
        }

        private TypeParameterSyntax ParseTypeParameter()
        {
            if (this.IsCurrentTokenWhereOfConstraintClause())
            {
                return _syntaxFactory.TypeParameter(
                    default(SyntaxList<AttributeSyntax>),
                    default(SyntaxToken),
                    this.AddError(CreateMissingIdentifierToken(), ErrorCode.ERR_IdentifierExpected));
            }

            var attrs = _pool.Allocate<AttributeSyntax>();
            try
            {
                if (this.CurrentToken.Kind == SyntaxKind.AtToken)
                {
                    var saveTerm = _termState;
                    _termState = TerminatorState.IsEndOfTypeArgumentList;
                    this.ParseAttributeSyntaxList(attrs);
                    _termState = saveTerm;
                }

                SyntaxToken varianceToken = null;
                if (this.CurrentToken.Kind == SyntaxKind.InKeyword ||
                    this.CurrentToken.Kind == SyntaxKind.OutKeyword)
                {
                    varianceToken = CheckFeatureAvailability(this.EatToken(), MessageID.IDS_FeatureTypeVariance);
                }

                return _syntaxFactory.TypeParameter(attrs, varianceToken, this.ParseIdentifierToken());
            }
            finally
            {
                _pool.Free(attrs);
            }
        }

        // Parses the parts of the names between Dots and ColonColons.
        private SimpleNameSyntax ParseSimpleName(NameOptions options = NameOptions.None)
        {
            var id = this.ParseIdentifierName();
            if (id.Identifier.IsMissing)
            {
                return id;
            }

            // You can pass ignore generics if you don't even want the parser to consider generics at all.
            // The name parsing will then stop at the first "<". It doesn't make sense to pass both Generic and IgnoreGeneric.

            SimpleNameSyntax name = id;
            if (this.CurrentToken.Kind == SyntaxKind.LessThanToken)
            {
                var pt = this.GetResetPoint();
                var kind = this.ScanTypeArgumentList(options);
                this.Reset(ref pt);
                this.Release(ref pt);

                if (kind == ScanTypeArgumentListKind.DefiniteTypeArgumentList || (kind == ScanTypeArgumentListKind.PossibleTypeArgumentList && (options & NameOptions.InTypeList) != 0))
                {
                    Debug.Assert(this.CurrentToken.Kind == SyntaxKind.LessThanToken);
                    SyntaxToken open;
                    var types = _pool.AllocateSeparated<TypeSyntax>();
                    SyntaxToken close;
                    this.ParseTypeArgumentList(out open, types, out close);
                    name = _syntaxFactory.GenericName(id.Identifier,
                        _syntaxFactory.TypeArgumentList(open, types, close));
                    _pool.Free(types);
                }
            }

            return name;
        }

        private enum ScanTypeArgumentListKind
        {
            NotTypeArgumentList,
            PossibleTypeArgumentList,
            DefiniteTypeArgumentList
        }

        private ScanTypeArgumentListKind ScanTypeArgumentList(NameOptions options)
        {
            if (this.CurrentToken.Kind != SyntaxKind.LessThanToken)
            {
                return ScanTypeArgumentListKind.NotTypeArgumentList;
            }

            if ((options & NameOptions.InExpression) == 0)
            {
                return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
            }

            // We're in an expression context, and we have a < token.  This could be a 
            // type argument list, or it could just be a relational expression.  
            //
            // Scan just the type argument list portion (i.e. the part from < to > ) to
            // see what we think it could be.  This will give us one of three possibilities:
            //
            //      result == ScanTypeFlags.NotType.
            //
            // This is absolutely not a type-argument-list.  Just return that result immediately.
            //
            //      result != ScanTypeFlags.NotType && isDefinitelyTypeArgumentList.
            //
            // This is absolutely a type-argument-list.  Just return that result immediately
            // 
            //      result != ScanTypeFlags.NotType && !isDefinitelyTypeArgumentList.
            //
            // This could be a type-argument list, or it could be an expression.  Need to see
            // what came after the last '>' to find out which it is.

            // Scan for a type argument list. If we think it's a type argument list
            // then assume it is unless we see specific tokens following it.
            SyntaxToken lastTokenOfList = null;
            ScanTypeFlags possibleTypeArgumentFlags = ScanPossibleTypeArgumentList(
                ref lastTokenOfList, out bool isDefinitelyTypeArgumentList);

            if (possibleTypeArgumentFlags == ScanTypeFlags.NotType)
            {
                return ScanTypeArgumentListKind.NotTypeArgumentList;
            }

            if (isDefinitelyTypeArgumentList)
            {
                return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
            }

            // If we did not definitively determine from immediate syntax that it was or
            // was not a type argument list, we must have scanned the entire thing up through
            // the closing greater-than token. In that case we will disambiguate based on the
            // token that follows it.
            Debug.Assert(lastTokenOfList.Kind == SyntaxKind.GreaterThanToken);

            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.CloseBracketToken:
                case SyntaxKind.CloseBraceToken:
                case SyntaxKind.ColonToken:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.DotToken:
                case SyntaxKind.QuestionToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.CaretToken:
                    // These tokens are from 7.5.4.2 Grammar Ambiguities
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                case SyntaxKind.AmpersandAmpersandToken: // e.g. `e is A<B> && e`
                case SyntaxKind.BarBarToken:             // e.g. `e is A<B> || e`
                case SyntaxKind.AmpersandToken:          // e.g. `e is A<B> & e`
                case SyntaxKind.OpenBracketToken:        // e.g. `e is A<B>[]`
                case SyntaxKind.LessThanToken:           // e.g. `e is A<B> < C`
                case SyntaxKind.LessThanEqualsToken:     // e.g. `e is A<B> <= C`
                case SyntaxKind.GreaterThanEqualsToken:  // e.g. `e is A<B> >= C`
                case SyntaxKind.IsKeyword:               // e.g. `e is A<B> is bool`
                case SyntaxKind.AsOptKeyword:               // e.g. `e is A<B> as bool`
                    // These tokens are added to 7.5.4.2 Grammar Ambiguities in C#7
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                case SyntaxKind.GreaterThanToken when ((options & NameOptions.AfterIs) != 0) && this.PeekToken(1).Kind != SyntaxKind.GreaterThanToken:
                    // This token is added to 7.5.4.2 Grammar Ambiguities in C#7 for the special case in which
                    // the possible generic is following an `is` keyword, e.g. `e is A<B> > C`.
                    // We test one further token ahead because a right-shift operator `>>` looks like a pair of greater-than
                    // tokens at this stage, but we don't intend to be handling the right-shift operator.
                    // The upshot is that we retain compatibility with the two previous behaviors:
                    // `(x is A<B>>C)` is parsed as `(x is A<B>) > C`
                    // `A<B>>C` elsewhere is parsed as `A < (B >> C)`
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                case SyntaxKind.IdentifierToken:
                    // C#7: In certain contexts, we treat *identifier* as a disambiguating token. Those
                    // contexts are where the sequence of tokens being disambiguated is immediately preceded by one
                    // of the keywords is, case, or out, or arises while parsing the first element of a tuple literal
                    // (in which case the tokens are preceded by `(` and the identifier is followed by a `,`) or a
                    // subsequent element of a tuple literal (in which case the tokens are preceded by `,` and the
                    // identifier is followed by a `,` or `)`).
                    // In C#8 (or whenever recursive patterns are introduced) we also treat an identifier as a
                    // disambiguating token if we're parsing the type of a pattern.
                    // Note that we treat query contextual keywords (which appear here as identifiers) as disambiguating tokens as well.
                    if ((options & (NameOptions.AfterIs | NameOptions.DefinitePattern | NameOptions.AfterOut)) != 0 ||
                        (options & NameOptions.AfterTupleComma) != 0 && (this.PeekToken(1).Kind == SyntaxKind.CommaToken || this.PeekToken(1).Kind == SyntaxKind.CloseParenToken) ||
                        (options & NameOptions.FirstElementOfPossibleTupleLiteral) != 0 && this.PeekToken(1).Kind == SyntaxKind.CommaToken
                        )
                    {
                        // we allow 'G<T,U> x' as a pattern-matching operation and a declaration expression in a tuple.
                        return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
                    }

                    return ScanTypeArgumentListKind.PossibleTypeArgumentList;

                case SyntaxKind.EndOfFileToken:          // e.g. `e is A<B>`
                    // This is useful for parsing expressions in isolation
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;

                default:
                    return ScanTypeArgumentListKind.PossibleTypeArgumentList;
            }
        }

        private ScanTypeFlags ScanPossibleTypeArgumentList(
            ref SyntaxToken lastTokenOfList, out bool isDefinitelyTypeArgumentList)
        {
            isDefinitelyTypeArgumentList = false;

            if (this.CurrentToken.Kind == SyntaxKind.LessThanToken)
            {
                ScanTypeFlags result = ScanTypeFlags.GenericTypeOrExpression;

                do
                {
                    lastTokenOfList = this.EatToken();

                    // Type arguments cannot contain attributes, so if this is an open square, we early out and assume it is not a type argument
                    if (this.CurrentToken.Kind == SyntaxKind.AtToken)
                    {
                        lastTokenOfList = null;
                        return ScanTypeFlags.NotType;
                    }

                    if (this.CurrentToken.Kind == SyntaxKind.GreaterThanToken)
                    {
                        lastTokenOfList = EatToken();
                        return result;
                    }

                    switch (this.ScanType(out lastTokenOfList))
                    {
                        case ScanTypeFlags.NotType:
                            lastTokenOfList = null;
                            return ScanTypeFlags.NotType;

                        case ScanTypeFlags.MustBeType:
                            // We're currently scanning a possible type-argument list.  But we're
                            // not sure if this is actually a type argument list, or is maybe some
                            // complex relational expression with <'s and >'s.  One thing we can
                            // tell though is that if we have a predefined type (like 'int' or 'string')
                            // before a comma or > then this is definitely a type argument list. i.e.
                            // if you have:
                            // 
                            //      var v = ImmutableDictionary<int,
                            //
                            // then there's no legal interpretation of this as an expression (since a
                            // standalone predefined type is not a valid simple term.  Contrast that
                            // with :
                            //
                            //  var v = ImmutableDictionary<Int32,
                            //
                            // Here this might actually be a relational expression and the comma is meant
                            // to separate out the variable declarator 'v' from the next variable.
                            //
                            // Note: we check if we got 'MustBeType' which triggers for predefined types,
                            // (int, string, etc.), or array types (Goo[], A<T>[][] etc.), or pointer types
                            // of things that must be types (int*, void**, etc.).
                            isDefinitelyTypeArgumentList = DetermineIfDefinitelyTypeArgumentList(isDefinitelyTypeArgumentList);
                            result = ScanTypeFlags.GenericTypeOrMethod;
                            break;

                        // case ScanTypeFlags.TupleType:
                        // It would be nice if we saw a tuple to state that we definitely had a 
                        // type argument list.  However, there are cases where this would not be
                        // true.  For example:
                        //
                        // public class C
                        // {
                        //     public static void Main()
                        //     {
                        //         XX X = default;
                        //         int a = 1, b = 2;
                        //         bool z = X < (a, b), w = false;
                        //     }
                        // }
                        //
                        // struct XX
                        // {
                        //     public static bool operator <(XX x, (int a, int b) arg) => true;
                        //     public static bool operator >(XX x, (int a, int b) arg) => false;
                        // }

                        case ScanTypeFlags.NullableType:
                            // See above.  If we have X<Y?,  or X<Y?>, then this is definitely a type argument list.
                            isDefinitelyTypeArgumentList = DetermineIfDefinitelyTypeArgumentList(isDefinitelyTypeArgumentList);
                            if (isDefinitelyTypeArgumentList)
                            {
                                result = ScanTypeFlags.GenericTypeOrMethod;
                            }

                            // Note: we intentionally fall out without setting 'result'. 
                            // Seeing a nullable type (not followed by a , or > ) is not enough 
                            // information for us to determine what this is yet.  i.e. the user may have:
                            //
                            //      X < Y ? Z : W
                            //
                            // We'd see a nullable type here, but htis is definitely not a type arg list.

                            break;

                        case ScanTypeFlags.GenericTypeOrExpression:
                            // See above.  If we have  X<Y<Z>,  then this would definitely be a type argument list.
                            // However, if we have  X<Y<Z>> then this might not be type argument list.  This could just
                            // be some sort of expression where we're comparing, and then shifting values.
                            if (!isDefinitelyTypeArgumentList)
                            {
                                isDefinitelyTypeArgumentList = this.CurrentToken.Kind == SyntaxKind.CommaToken;
                                result = ScanTypeFlags.GenericTypeOrMethod;
                            }
                            break;

                        case ScanTypeFlags.GenericTypeOrMethod:
                            result = ScanTypeFlags.GenericTypeOrMethod;
                            break;
                    }
                }
                while (this.CurrentToken.Kind == SyntaxKind.CommaToken);

                if (this.CurrentToken.Kind != SyntaxKind.GreaterThanToken)
                {
                    lastTokenOfList = null;
                    return ScanTypeFlags.NotType;
                }

                lastTokenOfList = this.EatToken();
                return result;
            }

            return ScanTypeFlags.NonGenericTypeOrExpression;
        }

        private bool DetermineIfDefinitelyTypeArgumentList(bool isDefinitelyTypeArgumentList)
        {
            if (!isDefinitelyTypeArgumentList)
            {
                isDefinitelyTypeArgumentList =
                    this.CurrentToken.Kind == SyntaxKind.CommaToken ||
                    this.CurrentToken.Kind == SyntaxKind.GreaterThanToken;
            }

            return isDefinitelyTypeArgumentList;
        }

        // ParseInstantiation: Parses the generic argument/parameter parts of the name.
        private void ParseTypeArgumentList(out SyntaxToken open, SeparatedSyntaxListBuilder<TypeSyntax> types, out SyntaxToken close)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.LessThanToken);
            open = this.EatToken(SyntaxKind.LessThanToken);
            open = CheckFeatureAvailability(open, MessageID.IDS_FeatureGenerics);

            if (this.IsOpenName())
            {
                // NOTE: trivia will be attached to comma, not omitted type argument
                var omittedTypeArgumentInstance = _syntaxFactory.OmittedTypeArgument(SyntaxFactory.Token(SyntaxKind.OmittedTypeArgumentToken));
                types.Add(omittedTypeArgumentInstance);
                while (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    types.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                    types.Add(omittedTypeArgumentInstance);
                }

                close = this.EatToken(SyntaxKind.GreaterThanToken);

                return;
            }

            // first type
            types.Add(this.ParseTypeArgument());

            // remaining types & commas
            while (true)
            {
                if (this.CurrentToken.Kind == SyntaxKind.GreaterThanToken)
                {
                    break;
                }
                else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleType())
                {
                    types.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                    types.Add(this.ParseTypeArgument());
                }
                else if (this.SkipBadTypeArgumentListTokens(types, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                {
                    break;
                }
            }

            close = this.EatToken(SyntaxKind.GreaterThanToken);
        }

        private PostSkipAction SkipBadTypeArgumentListTokens(SeparatedSyntaxListBuilder<TypeSyntax> list, SyntaxKind expected)
        {
            CSharpSyntaxNode tmp = null;
            Debug.Assert(list.Count > 0);
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref tmp, list,
                p => this.CurrentToken.Kind != SyntaxKind.CommaToken && !this.IsPossibleType(),
                p => this.CurrentToken.Kind == SyntaxKind.GreaterThanToken || this.IsTerminator(),
                expected);
        }

        // Parses the individual generic parameter/arguments in a name.
        private TypeSyntax ParseTypeArgument()
        {
            var attrs = _pool.Allocate<AttributeSyntax>();
            try
            {
                if (this.CurrentToken.Kind == SyntaxKind.AtToken)
                {
                    // Here, if we see a "[" that looks like it has something in it, we parse
                    // it as an attribute and then later put an error on the whole type if
                    // it turns out that attributes are not allowed. 
                    // TODO: should there be another flag that controls this behavior? we have
                    // "allowAttrs" but should there also be a "recognizeAttrs" that we can
                    // set to false in an expression context?

                    var saveTerm = _termState;
                    _termState = TerminatorState.IsEndOfTypeArgumentList;
                    this.ParseAttributeSyntaxList(attrs);
                    _termState = saveTerm;
                }

                SyntaxToken varianceToken = null;
                if (this.CurrentToken.Kind == SyntaxKind.InKeyword || this.CurrentToken.Kind == SyntaxKind.OutKeyword)
                {
                    // Recognize the variance syntax, but give an error as it's
                    // only appropriate in a type parameter list.
                    varianceToken = this.EatToken();
                    varianceToken = CheckFeatureAvailability(varianceToken, MessageID.IDS_FeatureTypeVariance);
                    varianceToken = this.AddError(varianceToken, ErrorCode.ERR_IllegalVarianceSyntax);
                }

                var result = this.ParseType();

                // Consider the case where someone supplies an invalid type argument
                // Such as Action<0> or Action<static>.  In this case we generate a missing 
                // identifier in ParseType, but if we continue as is we'll immediately start to 
                // interpret 0 as the start of a new expression when we can tell it's most likely
                // meant to be part of the type list.  
                //
                // To solve this we check if the current token is not comma or greater than and 
                // the next token is a comma or greater than. If so we assume that the found 
                // token is part of this expression and we attempt to recover. This does open 
                // the door for cases where we have an  incomplete line to be interpretted as 
                // a single expression.  For example:
                //
                // Action< // Incomplete line
                // a>b;
                //
                // However, this only happens when the following expression is of the form a>... 
                // or a,... which  means this case should happen less frequently than what we're 
                // trying to solve here so we err on the side of better error messages
                // for the majority of cases.
                SyntaxKind nextTokenKind = SyntaxKind.None;

                if (result.IsMissing &&
                    (this.CurrentToken.Kind != SyntaxKind.CommaToken && this.CurrentToken.Kind != SyntaxKind.GreaterThanToken) &&
                    ((nextTokenKind = this.PeekToken(1).Kind) == SyntaxKind.CommaToken || nextTokenKind == SyntaxKind.GreaterThanToken))
                {
                    // Eat the current token and add it as skipped so we recover
                    result = AddTrailingSkippedSyntax(result, this.EatToken());
                }

                if (varianceToken != null)
                {
                    result = AddLeadingSkippedSyntax(result, varianceToken);
                }

                if (attrs.Count > 0)
                {
                    result = AddLeadingSkippedSyntax(result, attrs.ToListNode());
                    result = this.AddError(result, ErrorCode.ERR_TypeExpected);
                }

                return result;
            }
            finally
            {
                _pool.Free(attrs);
            }
        }

        private bool IsEndOfTypeArgumentList()
        {
            return this.CurrentToken.Kind == SyntaxKind.GreaterThanToken;
        }

        private bool IsOpenName()
        {
            bool isOpen = true;
            int n = 0;
            while (this.PeekToken(n).Kind == SyntaxKind.CommaToken)
            {
                n++;
            }

            if (this.PeekToken(n).Kind != SyntaxKind.GreaterThanToken)
            {
                isOpen = false;
            }

            return isOpen;
        }

        private void ParseMemberName(
            out ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt,
            out SyntaxToken identifierOrThisOpt,
            out TypeParameterListSyntax typeParameterListOpt,
            bool isEvent)
        {
            identifierOrThisOpt = null;
            explicitInterfaceOpt = null;
            typeParameterListOpt = null;

            if (!IsPossibleMemberName())
            {
                // No clue what this is.  Just bail.  Our caller will have to
                // move forward and try again.
                return;
            }

            NameSyntax explicitInterfaceName = null;
            SyntaxToken separator = null;

            ResetPoint beforeIdentifierPoint = default(ResetPoint);
            bool beforeIdentifierPointSet = false;

            try
            {
                while (true)
                {
                    // Check if we got 'this'.  If so, then we have an indexer.
                    // Note: we parse out type parameters here as well so that
                    // we can give a useful error about illegal generic indexers.
                    if (this.CurrentToken.Kind == SyntaxKind.ThisKeyword)
                    {
                        beforeIdentifierPoint = GetResetPoint();
                        beforeIdentifierPointSet = true;
                        identifierOrThisOpt = this.EatToken();
                        typeParameterListOpt = this.ParseTypeParameterList();
                        break;
                    }

                    // now, scan past the next name.  if it's followed by a dot then
                    // it's part of the explicit name we're building up.  Otherwise,
                    // it's the name of the member.
                    var point = GetResetPoint();
                    bool isMemberName;
                    try
                    {
                        ScanNamedTypePart();
                        isMemberName = !IsDotOrColonColon();
                    }
                    finally
                    {
                        this.Reset(ref point);
                        this.Release(ref point);
                    }

                    if (isMemberName)
                    {
                        // We're past any explicit interface portion and We've 
                        // gotten to the member name.  
                        beforeIdentifierPoint = GetResetPoint();
                        beforeIdentifierPointSet = true;

                        if (separator != null && separator.Kind == SyntaxKind.ColonColonToken)
                        {
                            separator = this.AddError(separator, ErrorCode.ERR_AliasQualAsExpression);
                            separator = this.ConvertToMissingWithTrailingTrivia(separator, SyntaxKind.DotToken);
                        }

                        identifierOrThisOpt = this.ParseIdentifierToken();
                        typeParameterListOpt = this.ParseTypeParameterList();
                        break;
                    }
                    else
                    {
                        // If we saw a . or :: then we must have something explicit.
                        // first parse the upcoming name portion.

                        var saveTerm = _termState;
                        _termState |= TerminatorState.IsEndOfNameInExplicitInterface;

                        if (explicitInterfaceName == null)
                        {
                            // If this is the first time, then just get the next simple
                            // name and store it as the explicit interface name.
                            explicitInterfaceName = this.ParseSimpleName(NameOptions.InTypeList);

                            // Now, get the next separator.
                            separator = this.CurrentToken.Kind == SyntaxKind.ColonColonToken
                                ? this.EatToken() // fine after the first identifier
                                : this.EatToken(SyntaxKind.DotToken);
                        }
                        else
                        {
                            // Parse out the next part and combine it with the 
                            // current explicit name to form the new explicit name.
                            var tmp = this.ParseQualifiedNameRight(NameOptions.InTypeList, explicitInterfaceName, separator);
                            Debug.Assert(!ReferenceEquals(tmp, explicitInterfaceName), "We should have consumed something and updated explicitInterfaceName");
                            explicitInterfaceName = tmp;

                            // Now, get the next separator.
                            separator = this.CurrentToken.Kind == SyntaxKind.ColonColonToken
                                ? this.ConvertToMissingWithTrailingTrivia(this.EatToken(), SyntaxKind.DotToken)
                                : this.EatToken(SyntaxKind.DotToken);
                        }

                        _termState = saveTerm;
                    }
                }

                if (explicitInterfaceName != null)
                {
                    if (separator.Kind != SyntaxKind.DotToken)
                    {
                        separator = WithAdditionalDiagnostics(separator, GetExpectedTokenError(SyntaxKind.DotToken, separator.Kind, separator.GetLeadingTriviaWidth(), separator.Width));
                        separator = ConvertToMissingWithTrailingTrivia(separator, SyntaxKind.DotToken);
                    }

                    if (isEvent && this.CurrentToken.Kind != SyntaxKind.OpenBraceToken)
                    {
                        // CS0071: If you're explicitly implementing an event field, you have to use the accessor form
                        //
                        // Good:
                        //   event EventDelegate Parent.E
                        //   {
                        //      add { ... }
                        //      remove { ... }
                        //   }
                        //
                        // Bad:
                        //   event EventDelegate Parent.E; //(or anything else where the next token isn't open brace
                        //
                        // To recover: rollback to before the name of the field was parsed (just the part after the last
                        // dot), insert a missing identifier for the field name, insert missing accessors, and then treat
                        // the event name that's actually there as the beginning of a new member. e.g.
                        //
                        //   event EventDelegate Parent./*Missing nodes here*/
                        //
                        //   E;
                        //
                        // Rationale: The identifier could be the name of a type at the beginning of an existing member
                        // declaration (above which someone has started to type an explicit event implementation).

                        explicitInterfaceOpt = _syntaxFactory.ExplicitInterfaceSpecifier(
                            explicitInterfaceName,
                            AddError(separator, ErrorCode.ERR_ExplicitEventFieldImpl));

                        Debug.Assert(beforeIdentifierPointSet);
                        Reset(ref beforeIdentifierPoint);

                        //clear fields that were populated after the reset point
                        identifierOrThisOpt = null;
                        typeParameterListOpt = null;
                    }
                    else
                    {
                        explicitInterfaceOpt = _syntaxFactory.ExplicitInterfaceSpecifier(explicitInterfaceName, separator);
                    }
                }
            }
            finally
            {
                if (beforeIdentifierPointSet)
                {
                    Release(ref beforeIdentifierPoint);
                }
            }
        }

        private NameSyntax ParseAliasQualifiedName(NameOptions allowedParts = NameOptions.None)
        {
            NameSyntax name = this.ParseSimpleName(allowedParts);
            if (this.CurrentToken.Kind == SyntaxKind.ColonColonToken)
            {
                var token = this.EatToken();

                name = ParseQualifiedNameRight(allowedParts, name, token);
            }
            return name;
        }

        private NameSyntax ParseQualifiedName(NameOptions options = NameOptions.None)
        {
            NameSyntax name = this.ParseAliasQualifiedName(options);

            while (this.IsDotOrColonColon())
            {
                if (this.PeekToken(1).Kind == SyntaxKind.ThisKeyword)
                {
                    break;
                }

                var separator = this.EatToken();
                name = ParseQualifiedNameRight(options, name, separator);
            }

            return name;
        }

        private NameSyntax ParseQualifiedNameRight(
            NameOptions options,
            NameSyntax left,
            SyntaxToken separator)
        {
            var right = this.ParseSimpleName(options);

            if (separator.Kind == SyntaxKind.DotToken)
            {
                return _syntaxFactory.QualifiedName(left, separator, right);
            }
            else if (separator.Kind == SyntaxKind.ColonColonToken)
            {
                if (left.Kind != SyntaxKind.IdentifierName)
                {
                    separator = this.AddError(separator, ErrorCode.ERR_UnexpectedAliasedName, separator.ToString());
                }

                // If the left hand side is not an identifier name then the user has done
                // something like Goo.Bar::Blah. We've already made an error node for the
                // ::, so just pretend that they typed Goo.Bar.Blah and continue on.

                var identifierLeft = left as IdentifierNameSyntax;
                if (identifierLeft == null)
                {
                    separator = this.ConvertToMissingWithTrailingTrivia(separator, SyntaxKind.DotToken);
                    return _syntaxFactory.QualifiedName(left, separator, right);
                }
                else
                {
                    if (identifierLeft.Identifier.ContextualKind == SyntaxKind.GlobalKeyword)
                    {
                        identifierLeft = _syntaxFactory.IdentifierName(ConvertToKeyword(identifierLeft.Identifier));
                    }

                    identifierLeft = CheckFeatureAvailability(identifierLeft, MessageID.IDS_FeatureGlobalNamespace);

                    // If the name on the right had errors or warnings then we need to preserve
                    // them in the tree.
                    return WithAdditionalDiagnostics(_syntaxFactory.AliasQualifiedName(identifierLeft, separator, right), left.GetDiagnostics());
                }
            }
            else
            {
                return left;
            }
        }

        private SyntaxToken ConvertToMissingWithTrailingTrivia(SyntaxToken token, SyntaxKind expectedKind)
        {
            var newToken = SyntaxFactory.MissingToken(expectedKind);
            newToken = AddTrailingSkippedSyntax(newToken, token);
            return newToken;
        }

        private enum ScanTypeFlags
        {
            /// <summary>
            /// Definitely not a type name.
            /// </summary>
            NotType,

            /// <summary>
            /// Definitely a type name: either a predefined type (int, string, etc.) or an array
            /// type (ending with a [] brackets), or a pointer type (ending with *s).
            /// </summary>
            MustBeType,

            /// <summary>
            /// Might be a generic (qualified) type name or a method name.
            /// </summary>
            GenericTypeOrMethod,

            /// <summary>
            /// Might be a generic (qualified) type name or an expression or a method name.
            /// </summary>
            GenericTypeOrExpression,

            /// <summary>
            /// Might be a non-generic (qualified) type name or an expression.
            /// </summary>
            NonGenericTypeOrExpression,

            /// <summary>
            /// A type name with alias prefix (Alias::Name)
            /// </summary>
            AliasQualifiedName,

            /// <summary>
            /// Nullable type (ending with ?).
            /// </summary>
            NullableType,

            /// <summary>
            /// Might be a pointer type or a multiplication.
            /// </summary>
            PointerOrMultiplication,

            /// <summary>
            /// Might be a tuple type.
            /// </summary>
            TupleType,
        }

        private bool IsPossibleType()
        {
            var tk = this.CurrentToken.Kind;
            return IsPredefinedType(tk) || this.IsTrueIdentifier();
        }

        private bool IsPossibleName()
        {
            return this.IsTrueIdentifier();
        }

        private ScanTypeFlags ScanType(bool forPattern = false)
        {
            SyntaxToken lastTokenOfType;
            return ScanType(out lastTokenOfType, forPattern);
        }

        private ScanTypeFlags ScanType(out SyntaxToken lastTokenOfType, bool forPattern = false)
        {
            return ScanType(forPattern ? ParseTypeMode.DefinitePattern : ParseTypeMode.Normal, out lastTokenOfType);
        }

        private ScanTypeFlags ScanType(ParseTypeMode mode, out SyntaxToken lastTokenOfType)
        {
            ScanTypeFlags result;

            // We might start with a by ref
            if (this.CurrentToken.Kind == SyntaxKind.RefKeyword)
            {
                // in a ref local or ref return, we treat "ref" and "ref readonly" as part of the type
                this.EatToken();

                if (this.CurrentToken.Kind == SyntaxKind.ReadOnlyKeyword)
                {
                    this.EatToken();
                }
            }

            // Finally, check for array types.
            while (this.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
            {
                this.EatToken();

                // In stark, we don't allow multi-dimensional arrays
                if (this.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
                {
                    lastTokenOfType = null;
                    return ScanTypeFlags.NotType;
                }

                lastTokenOfType = this.EatToken();
            }

            result = this.ScanNonArrayType(mode, out lastTokenOfType);
            if (result == ScanTypeFlags.NotType)
            {
                return result;
            }

            if (this.CurrentToken.Kind == SyntaxKind.QuestionToken)
            {
                lastTokenOfType = this.EatToken();
            }

            result = ScanTypeFlags.MustBeType;

            return result;
        }

        private void ScanNamedTypePart()
        {
            SyntaxToken lastTokenOfType;
            ScanNamedTypePart(out lastTokenOfType);
        }

        private ScanTypeFlags ScanNamedTypePart(out SyntaxToken lastTokenOfType)
        {
            if (this.CurrentToken.Kind != SyntaxKind.IdentifierToken || !this.IsTrueIdentifier())
            {
                lastTokenOfType = null;
                return ScanTypeFlags.NotType;
            }

            lastTokenOfType = this.EatToken();
            if (this.CurrentToken.Kind == SyntaxKind.LessThanToken)
            {
                return this.ScanPossibleTypeArgumentList(ref lastTokenOfType, out _);
            }
            else
            {
                return ScanTypeFlags.NonGenericTypeOrExpression;
            }
        }

        private ScanTypeFlags ScanNonArrayType(ParseTypeMode mode, out SyntaxToken lastTokenOfType)
        {
            ScanTypeFlags result;

            while (this.CurrentToken.Kind == SyntaxKind.AsteriskToken)
            {
                lastTokenOfType = this.EatToken();
            }

            // In Stark the ref is coming before the array
            if (this.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                result = this.ScanNamedTypePart(out lastTokenOfType);
                if (result == ScanTypeFlags.NotType)
                {
                    return ScanTypeFlags.NotType;
                }

                bool isAlias = this.CurrentToken.Kind == SyntaxKind.ColonColonToken;

                // Scan a name
                for (bool firstLoop = true; IsDotOrColonColon(); firstLoop = false)
                {
                    if (!firstLoop && isAlias)
                    {
                        isAlias = false;
                    }

                    lastTokenOfType = this.EatToken();

                    result = this.ScanNamedTypePart(out lastTokenOfType);
                    if (result == ScanTypeFlags.NotType)
                    {
                        return ScanTypeFlags.NotType;
                    }
                }

                if (isAlias)
                {
                    result = ScanTypeFlags.AliasQualifiedName;
                }
            }
            else if (IsPredefinedType(this.CurrentToken.Kind))
            {
                // Simple type...
                lastTokenOfType = this.EatToken();
                result = ScanTypeFlags.MustBeType;
            }
            else if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                lastTokenOfType = this.EatToken();

                result = this.ScanTupleType(out lastTokenOfType);
                if (result == ScanTypeFlags.NotType || mode == ParseTypeMode.DefinitePattern && this.CurrentToken.Kind != SyntaxKind.OpenBracketToken)
                {
                    // A tuple type can appear in a pattern only if it is the element type of an array type.
                    return ScanTypeFlags.NotType;
                }
            }
            else
            {
                // Can't be a type!
                lastTokenOfType = null;
                return ScanTypeFlags.NotType;
            }

            if (this.CurrentToken.Kind == SyntaxKind.QuestionToken)
            {
                lastTokenOfType = this.EatToken();
                result = ScanTypeFlags.NullableType;
            }

            return result;
        }

        /// <summary>
        /// Returns TupleType when a possible tuple type is found.
        /// Note that this is not MustBeType, so that the caller can consider deconstruction syntaxes.
        /// The caller is expected to have consumed the opening paren.
        /// </summary>
        private ScanTypeFlags ScanTupleType(out SyntaxToken lastTokenOfType)
        {
            var tupleElementType = ScanType(out lastTokenOfType);
            if (tupleElementType != ScanTypeFlags.NotType)
            {
                if (IsTrueIdentifier())
                {
                    lastTokenOfType = this.EatToken();
                }

                if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    do
                    {
                        lastTokenOfType = this.EatToken();
                        tupleElementType = ScanType(out lastTokenOfType);

                        if (tupleElementType == ScanTypeFlags.NotType)
                        {
                            lastTokenOfType = this.EatToken();
                            return ScanTypeFlags.NotType;
                        }

                        if (IsTrueIdentifier())
                        {
                            lastTokenOfType = this.EatToken();
                        }
                    }
                    while (this.CurrentToken.Kind == SyntaxKind.CommaToken);

                    if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken)
                    {
                        lastTokenOfType = this.EatToken();
                        return ScanTypeFlags.TupleType;
                    }
                }
            }

            // Can't be a type!
            lastTokenOfType = null;
            return ScanTypeFlags.NotType;
        }

        private static bool IsPredefinedType(SyntaxKind keyword)
        {
            return SyntaxFacts.IsPredefinedType(keyword);
        }

        public TypeSyntax ParseTypeName()
        {
            return ParseType();
        }

        private TypeSyntax ParseTypeOrVoid()
        {
            if (this.CurrentToken.Kind == SyntaxKind.VoidKeyword && this.PeekToken(1).Kind != SyntaxKind.AsteriskToken)
            {
                // Must be 'void' type, so create such a type node and return it.
                return _syntaxFactory.PredefinedType(this.EatToken());
            }

            return this.ParseType();
        }

        private bool IsTerm()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.MakeRefKeyword:
                case SyntaxKind.RefTypeKeyword:
                case SyntaxKind.RefValueKeyword:
                case SyntaxKind.BaseKeyword:
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.NullKeyword:
                case SyntaxKind.SizeOfKeyword:
                case SyntaxKind.ThisKeyword:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.TypeOfKeyword:
                case SyntaxKind.UncheckedKeyword:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.StringKeyword:
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.RuneLiteralToken:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.EqualsGreaterThanToken:
                case SyntaxKind.InterpolatedStringToken:
                case SyntaxKind.InterpolatedStringStartToken:
                    return true;
                case SyntaxKind.IdentifierToken:
                    return this.IsTrueIdentifier();
                default:
                    return false;
            }
        }

        private enum ParseTypeMode
        {
            Normal,
            Parameter,
            AfterIs,
            DefinitePattern,
            AfterOut,
            AfterTupleComma,
            AsExpression,
            ArrayCreation,
            FirstElementOfPossibleTupleLiteral
        }

        private TypeSyntax ParseType(ParseTypeMode mode = ParseTypeMode.Normal)
        {
            NameOptions nameOptions;
            switch (mode)
            {
                case ParseTypeMode.AfterIs:
                    nameOptions = NameOptions.InExpression | NameOptions.AfterIs | NameOptions.PossiblePattern;
                    break;
                case ParseTypeMode.DefinitePattern:
                    nameOptions = NameOptions.InExpression | NameOptions.DefinitePattern | NameOptions.PossiblePattern;
                    break;
                case ParseTypeMode.AfterOut:
                    nameOptions = NameOptions.InExpression | NameOptions.AfterOut;
                    break;
                case ParseTypeMode.AfterTupleComma:
                    nameOptions = NameOptions.InExpression | NameOptions.AfterTupleComma;
                    break;
                case ParseTypeMode.FirstElementOfPossibleTupleLiteral:
                    nameOptions = NameOptions.InExpression | NameOptions.FirstElementOfPossibleTupleLiteral;
                    break;
                case ParseTypeMode.ArrayCreation:
                case ParseTypeMode.AsExpression:
                case ParseTypeMode.Normal:
                case ParseTypeMode.Parameter:
                    nameOptions = NameOptions.None;
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(mode);
            }


            TypeSyntax type = null;
            SyntaxToken letKeyword = null;
            SyntaxToken refKeyword = null;
            var typeModifiers = default(SyntaxListBuilder<SyntaxToken>);
            try
            {
                if (CurrentToken.Kind == SyntaxKind.LetKeyword)
                {
                    letKeyword = EatToken();
                }

                switch (this.CurrentToken.Kind)
                {
                    case SyntaxKind.RefKeyword:
                    case SyntaxKind.InKeyword:
                    case SyntaxKind.OutKeyword:
                        refKeyword = EatToken();
                        break;
                    default:
                        // If we have a let keyword, we expect a ref keyword
                        if (letKeyword != null)
                        {
                            refKeyword = EatToken(SyntaxKind.RefKeyword);
                        }
                        break;
                }

                // Parse any modifiers
                switch (this.CurrentToken.Kind)
                {
                    case SyntaxKind.TransientKeyword:
                    case SyntaxKind.ReadOnlyKeyword:
                        typeModifiers = _pool.Allocate<SyntaxToken>();
                        ParseTypeModifiers(typeModifiers);
                        break;
                }

                switch (this.CurrentToken.Kind)
                {
                    case SyntaxKind.QuestionToken:
                        var question = EatToken();
                        type = ParseType(mode);
                        type = _syntaxFactory.NullableType(question, type);
                        break;

                    case SyntaxKind.AsteriskToken:
                        var ptrToken = this.EatToken();
                        type = ParseType(mode);
                        type = _syntaxFactory.PointerType(ptrToken, type);
                        break;

                    case SyntaxKind.OpenBracketToken:
                        type = ParseArrayType();
                        break;

                    default:
                        type = ParseUnderlyingType(nameOptions);
                        break;
                }
            } finally
            {
                if (!typeModifiers.IsNull)
                {
                    if (refKeyword != null)
                    {
                        type = _syntaxFactory.RefType(letKeyword, refKeyword, typeModifiers, type);
                    }
                    else
                    {
                        type = _syntaxFactory.SimpleExtendedType(typeModifiers, type);
                    }
                    _pool.Free(typeModifiers);
                }
                else if (refKeyword != null)
                {
                    type = _syntaxFactory.RefType(letKeyword, refKeyword, typeModifiers, type);
                }
            }

            return type;
        }

        private void ParseTypeModifiers(SyntaxListBuilder<SyntaxToken> modifiers)
        {
            while (IsExtendedTypeModifier(CurrentToken))
            {
                modifiers.Add(EatToken());
            }
        }


        private ArrayTypeSyntax ParseArrayType()
        {
            var ranks = _pool.Allocate<ArrayRankSpecifierSyntax>();
            try
            {
                while (CurrentToken.Kind == SyntaxKind.OpenBracketToken)
                {
                    var arrayRank = ParseArrayRankSpecifier();
                    ranks.Add(arrayRank);
                }
                return _syntaxFactory.ArrayType(ranks.ToList(), ParseType());
            }
            finally
            {
                _pool.Free(ranks);
            }
        }

        private static bool IsExtendedTypeModifier(SyntaxToken token)
        {
            switch (token.Kind)
            {
                case SyntaxKind.TransientKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.UnsafeKeyword:
                    return true;
            }
            return false;
        }

        private ArrayRankSpecifierSyntax ParseArrayRankSpecifier()
        {
            var open = this.EatToken(SyntaxKind.OpenBracketToken);
            ExpressionSyntax size = null;
            if (this.IsPossibleExpression())
            {
                size = this.ParseExpressionCore();
            }
            var close = this.EatToken(SyntaxKind.CloseBracketToken);
            return _syntaxFactory.ArrayRankSpecifier(open, size, close);
        }

        private TupleTypeSyntax ParseTupleType()
        {
            var open = this.EatToken(SyntaxKind.OpenParenToken);
            var list = _pool.AllocateSeparated<TupleElementSyntax>();
            try
            {
                if (this.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                {
                    var element = ParseTupleElement();
                    list.Add(element);

                    while (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        var comma = this.EatToken(SyntaxKind.CommaToken);
                        list.AddSeparator(comma);

                        element = ParseTupleElement();
                        list.Add(element);
                    }
                }

                if (list.Count < 2)
                {
                    if (list.Count < 1)
                    {
                        list.Add(_syntaxFactory.TupleElement(this.CreateMissingIdentifierName(), identifier: null));
                    }

                    list.AddSeparator(SyntaxFactory.MissingToken(SyntaxKind.CommaToken));
                    var missing = this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_TupleTooFewElements);
                    list.Add(_syntaxFactory.TupleElement(missing, identifier: null));
                }

                var close = this.EatToken(SyntaxKind.CloseParenToken);
                var result = _syntaxFactory.TupleType(open, list, close);

                result = CheckFeatureAvailability(result, MessageID.IDS_FeatureTuples);

                return result;
            }
            finally
            {
                _pool.Free(list);
            }
        }

        private TupleElementSyntax ParseTupleElement()
        {
            var type = ParseType();
            SyntaxToken name = null;

            if (IsTrueIdentifier())
            {
                name = this.ParseIdentifierToken();
            }

            return _syntaxFactory.TupleElement(type, name);
        }

        private TypeSyntax ParseUnderlyingType(NameOptions options = NameOptions.None)
        {
            if (IsPredefinedType(this.CurrentToken.Kind))
            {
                // This is a predefined type
                var token = this.EatToken();
                if (token.Kind == SyntaxKind.VoidKeyword)
                {
                    token = this.AddError(token, ErrorCode.ERR_NoVoidHere);
                }
                return _syntaxFactory.PredefinedType(token);
            }
            else if (IsTrueIdentifier())
            {
                return this.ParseQualifiedName(options);
            }
            else if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                return this.ParseTupleType();
            }
            else if (CurrentToken.Kind == SyntaxKind.TrueKeyword ||
                     CurrentToken.Kind == SyntaxKind.NullKeyword ||
                     CurrentToken.Kind == SyntaxKind.NumericLiteralToken ||
                     CurrentToken.Kind == SyntaxKind.StringLiteralToken ||
                     CurrentToken.Kind == SyntaxKind.RuneLiteralToken)
            {
                var expression = _syntaxFactory.LiteralExpression(SyntaxFacts.GetLiteralExpression(CurrentToken.Kind), this.EatToken());
                return _syntaxFactory.ConstLiteralType(expression);
            }
            else
            {
                var name = this.CreateMissingIdentifierName();
                return this.AddError(name, ErrorCode.ERR_TypeExpected);
            }
        }

        public StatementSyntax ParseStatement()
        {
            return ParseWithStackGuard(
                () => ParseStatementCore() ?? ParseExpressionStatement(),
                () => SyntaxFactory.EmptyStatement(SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken)));
        }

        private StatementSyntax ParseStatementCore()
        {
            try
            {
                _recursionDepth++;
                StackGuard.EnsureSufficientExecutionStack(_recursionDepth);

                if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNode is Stark.Syntax.StatementSyntax)
                {
                    return (StatementSyntax)this.EatNode();
                }

                // First, try to parse as a non-declaration statement. If the statement is a single
                // expression then we only allow legal expression statements. (That is, "new C();",
                // "C();", "x = y;" and so on.)

                return ParseStatementNoDeclaration(allowAnyExpression: false);
            }
            finally
            {
                _recursionDepth--;
            }
        }

        private StatementSyntax ParsePossibleDeclarationOrBadAwaitStatement()
        {
            ResetPoint resetPointBeforeStatement = this.GetResetPoint();
            StatementSyntax result = ParsePossibleDeclarationOrBadAwaitStatement(ref resetPointBeforeStatement);
            this.Release(ref resetPointBeforeStatement);
            return result;
        }

        private StatementSyntax ParsePossibleDeclarationOrBadAwaitStatement(ref ResetPoint resetPointBeforeStatement)
        {
            // Precondition: We have already attempted to parse the statement as a non-declaration and failed.
            //
            // That means that we are in one of the following cases:
            //
            // 1) This is not a statement. This can happen if the start of the statement was an
            //    accessibility modifier, but the rest of the statement did not parse as a local
            //    function. If there was an accessibility modifier and the statement parsed as
            //    local function, that should be marked as a mistake with local function visibility.
            //    Otherwise, it's likely the user just forgot a closing brace on their method.
            // 2) This is a perfectly mundane and correct local declaration statement like "int x;"
            // 3) This is a perfectly mundane but erroneous local declaration statement, like "int X();"
            // 4) We are in the rare case of the code containing "await x;" and the intention is that
            //    "await" is the type of "x".  This only works in a non-async method.
            // 5) We have what would be a legal await statement, like "await X();", but we are not in
            //    an async method, so the parse failed. (Had we been in an async method then the parse
            //    attempt done by our caller would have succeeded.)
            // 6) The statement begins with "await" but is not a legal local declaration and not a legal
            //    await expression regardless of whether the method is marked as "async".

            bool beginsWithAwait = this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword;
            StatementSyntax result = ParseLocalDeclarationStatement();

            // Case (1)
            if (result == null)
            {
                this.Reset(ref resetPointBeforeStatement);
                return null;
            }

            // Cases (2), (3) and (4):
            if (!beginsWithAwait || !result.ContainsDiagnostics)
            {
                return result;
            }

            // The statement begins with "await" and could not be parsed as a legal declaration statement.
            // We know from our precondition that it is not a legal "await X();" statement, though it is
            // possible that it was only not legal because we were not in an async context.

            Debug.Assert(!IsInAsync);

            // Let's see if we're in case (5). Pretend that we're in an async method and see if parsing
            // a non-declaration statement would have succeeded.

            this.Reset(ref resetPointBeforeStatement);
            IsInAsync = true;
            result = ParseStatementNoDeclaration(allowAnyExpression: false);
            IsInAsync = false;

            Debug.Assert(result != null);
            return result;
        }

        /// <summary>
        /// Parses any statement but a declaration statement. Returns null if the lookahead looks like a declaration.
        /// </summary>
        /// <remarks>
        /// Variable declarations in global code are parsed as field declarations so we need to fallback if we encounter a declaration statement.
        /// </remarks>
        private StatementSyntax ParseStatementNoDeclaration(bool allowAnyExpression)
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.FixedKeyword:
                    return this.ParseFixedStatement();
                case SyntaxKind.BreakKeyword:
                    return this.ParseBreakStatement();
                case SyntaxKind.ContinueKeyword:
                    return this.ParseContinueStatement();
                case SyntaxKind.TryKeyword:
                case SyntaxKind.CatchKeyword:
                case SyntaxKind.FinallyKeyword:
                    return this.ParseTryStatement();
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                    return this.ParseCheckedStatement();
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.VarKeyword:
                case SyntaxKind.LetKeyword:
                    return ParseLocalDeclarationStatement();
                case SyntaxKind.DoKeyword:
                    return this.ParseDoStatement();
                case SyntaxKind.ForKeyword:
                    return this.ParseForEachStatement(awaitTokenOpt: default);
                case SyntaxKind.GotoKeyword:
                    return this.ParseGotoStatement();
                case SyntaxKind.IfKeyword:
                    return this.ParseIfStatement();
                case SyntaxKind.LockKeyword:
                    return this.ParseLockStatement();
                case SyntaxKind.ReturnKeyword:
                    return this.ParseReturnStatement();
                case SyntaxKind.SwitchKeyword:
                    return this.ParseSwitchStatement();
                case SyntaxKind.ThrowKeyword:
                    return this.ParseThrowStatement();
                case SyntaxKind.UnsafeKeyword:
                    // Checking for brace to disambiguate between unsafe statement and unsafe local function
                    if (this.IsPossibleUnsafeStatement())
                    {
                        return this.ParseUnsafeStatement();
                    }
                    break;
                case SyntaxKind.UsingKeyword:
                    return PeekToken(1).Kind == SyntaxKind.OpenParenToken ? this.ParseUsingStatement() : this.ParseLocalDeclarationStatement();
                case SyntaxKind.WhileKeyword:
                    return this.ParseWhileStatement();
                case SyntaxKind.OpenBraceToken:
                    return this.ParseBlock();
                case SyntaxKind.IdentifierToken:
                    if (isPossibleAwaitForEach())
                    {
                        return this.ParseForEachStatement(parseAwaitKeywordForAsyncStreams());
                    }
                    else if (isPossibleAwaitUsing())
                    {
                        if (PeekToken(2).Kind == SyntaxKind.OpenParenToken)
                        {
                            return this.ParseUsingStatement(parseAwaitKeywordForAsyncStreams());
                        }
                        else
                        {
                            return this.ParseLocalDeclarationStatement(parseAwaitKeywordForAsyncStreams());
                        }
                    }
                    else if (this.IsPossibleLabeledStatement())
                    {
                        return this.ParseLabeledStatement();
                    }
                    else if (this.IsPossibleYieldStatement())
                    {
                        return this.ParseYieldStatement();
                    }
                    else if (this.IsPossibleAwaitExpressionStatement())
                    {
                        return this.ParseExpressionStatement();
                    }
                    else if (this.IsQueryExpression(mayBeVariableDeclaration: true, mayBeMemberDeclaration: allowAnyExpression))
                    {
                        return this.ParseExpressionStatement(this.ParseQueryExpression(0));
                    }
                    break;
            }

            return this.ParseExpressionStatement();

            bool isPossibleAwaitForEach()
            {
                return this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword &&
                    this.PeekToken(1).Kind == SyntaxKind.ForKeyword;
            }

            bool isPossibleAwaitUsing()
            {
                return this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword &&
                    this.PeekToken(1).Kind == SyntaxKind.UsingKeyword;
            }

            SyntaxToken parseAwaitKeywordForAsyncStreams()
            {
                Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword);
                SyntaxToken awaitToken = this.EatContextualToken(SyntaxKind.AwaitKeyword);
                return CheckFeatureAvailability(awaitToken, MessageID.IDS_FeatureAsyncStreams);
            }
        }

        private bool IsPossibleLabeledStatement()
        {
            return this.PeekToken(1).Kind == SyntaxKind.ColonToken && this.IsTrueIdentifier();
        }

        private bool IsPossibleUnsafeStatement()
        {
            // unsafe {
            // unsafe il {
            var nextToken = this.PeekToken(1);
            var nextTokenKind = nextToken.Kind;
            return nextTokenKind == SyntaxKind.OpenBraceToken ||
                   (nextToken.ContextualKind == SyntaxKind.IlKeyword &&
                    this.PeekToken(2).Kind == SyntaxKind.OpenBraceToken);
        }

        private bool IsPossibleYieldStatement()
        {
            return this.CurrentToken.ContextualKind == SyntaxKind.YieldKeyword && (this.PeekToken(1).Kind == SyntaxKind.ReturnKeyword || this.PeekToken(1).Kind == SyntaxKind.BreakKeyword);
        }

        private bool IsPossibleNewExpression()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.NewKeyword);

            // skip new
            SyntaxToken nextToken = PeekToken(1);

            // new { }
            // new [ ]
            switch (nextToken.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.OpenBracketToken:
                    return true;
            }

            //
            // Declaration with new modifier vs. new expression
            // Parse it as an expression if the type is not followed by an identifier or this keyword.
            //
            // Member declarations:
            //   new T Idf ...
            //   new T this ...
            //   new partial Idf    ("partial" as a type name)
            //   new partial this   ("partial" as a type name)
            //   new partial T Idf
            //   new partial T this
            //   new <modifier>
            //   new <class|interface|struct|enum>
            //   new partial <class|interface|struct|enum>
            //
            // New expressions:
            //   new T []
            //   new T { }
            //   new <non-type>
            //
            if (SyntaxFacts.GetBaseTypeDeclarationKind(nextToken.Kind) != SyntaxKind.None)
            {
                return false;
            }

            DeclarationModifiers modifier = GetModifier(nextToken);
            if (modifier == DeclarationModifiers.Partial)
            {
                if (SyntaxFacts.IsPredefinedType(PeekToken(2).Kind))
                {
                    return false;
                }

                // class, struct, enum, interface keywords, but also other modifiers that are not allowed after 
                // partial keyword but start class declaration, so we can assume the user just swapped them.
                if (IsPossibleStartOfTypeDeclaration(PeekToken(2).Kind))
                {
                    return false;
                }
            }
            else if (modifier != DeclarationModifiers.None)
            {
                return false;
            }

            bool? typedIdentifier = IsPossibleTypedIdentifierStart(nextToken, PeekToken(2), allowThisKeyword: true);
            if (typedIdentifier != null)
            {
                // new Idf Idf
                // new Idf .
                // new partial T
                // new partial .
                return !typedIdentifier.Value;
            }

            var resetPoint = this.GetResetPoint();
            try
            {
                // skips new keyword
                EatToken();

                ScanTypeFlags st = this.ScanType();

                return !IsPossibleMemberName() || st == ScanTypeFlags.NotType;
            }
            finally
            {
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);
            }
        }

        /// <returns>
        /// true if the current token can be the first token of a typed identifier (a type name followed by an identifier),
        /// false if it definitely can't be,
        /// null if we need to scan further to find out.
        /// </returns>
        private bool? IsPossibleTypedIdentifierStart(SyntaxToken current, SyntaxToken next, bool allowThisKeyword)
        {
            if (IsTrueIdentifier(current))
            {
                switch (next.Kind)
                {
                    // tokens that can be in type names...
                    case SyntaxKind.DotToken:
                    case SyntaxKind.AsteriskToken:
                    case SyntaxKind.QuestionToken:
                    case SyntaxKind.OpenBracketToken:
                    case SyntaxKind.LessThanToken:
                    case SyntaxKind.ColonColonToken:
                        return null;

                    case SyntaxKind.OpenParenToken:
                        if (current.IsIdentifierVar())
                        {
                            // potentially either a tuple type in a local declaration (true), or
                            // a tuple lvalue in a deconstruction assignment (false).
                            return null;
                        }
                        else
                        {
                            return false;
                        }

                    case SyntaxKind.IdentifierToken:
                        return IsTrueIdentifier(next);

                    case SyntaxKind.ThisKeyword:
                        return allowThisKeyword;

                    default:
                        return false;
                }
            }

            return null;
        }

        // If "isMethodBody" is true, then this is the immediate body of a method/accessor.
        // In this case, we create a many-child list if the body is not a small single statement.
        // This then allows a "with many weak children" red node when the red node is created.
        // If "isAccessorBody" is true, then we produce a special diagnostic if the open brace is
        // missing.  Also, "isMethodBody" must be true.
        private BlockSyntax ParseBlock(bool isMethodBody = false, bool isAccessorBody = false)
        {
            // Check again for incremental re-use, since ParseBlock is called from a bunch of places
            // other than ParseStatementCore()
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.Block)
            {
                return (BlockSyntax)this.EatNode();
            }

            // There's a special error code for a missing token after an accessor keyword
            var openBrace = isAccessorBody && this.CurrentToken.Kind != SyntaxKind.OpenBraceToken
                ? this.AddError(
                    SyntaxFactory.MissingToken(SyntaxKind.OpenBraceToken),
                    IsFeatureEnabled(MessageID.IDS_FeatureExpressionBodiedAccessor)
                        ? ErrorCode.ERR_SemiOrLBraceOrArrowExpected
                        : ErrorCode.ERR_SemiOrLBraceExpected)
                : CurrentToken.Kind != SyntaxKind.OpenBraceToken
                    ? this.AddError(
                        SyntaxFactory.MissingToken(SyntaxKind.OpenBraceToken), ErrorCode.ERR_OpenBraceExpected)
                    : this.EatToken(SyntaxKind.OpenBraceToken);

            var statements = _pool.Allocate<StatementSyntax>();
            try
            {
                CSharpSyntaxNode tmp = openBrace;
                this.ParseStatements(ref tmp, statements, stopOnSwitchSections: false);
                openBrace = (SyntaxToken)tmp;

                var closeBrace = this.CurrentToken.Kind != SyntaxKind.CloseBraceToken
                    ? this.AddError(
                        SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken), ErrorCode.ERR_CloseBraceExpected)
                    : this.EatToken(SyntaxKind.CloseBraceToken);

                SyntaxList<StatementSyntax> statementList;
                if (isMethodBody && IsLargeEnoughNonEmptyStatementList(statements))
                {
                    // Force creation a many-children list, even if only 1, 2, or 3 elements in the statement list.
                    statementList = new SyntaxList<StatementSyntax>(SyntaxList.List(((SyntaxListBuilder)statements).ToArray()));
                }
                else
                {
                    statementList = statements;
                }

                return _syntaxFactory.Block(openBrace, statementList, closeBrace);
            }
            finally
            {
                _pool.Free(statements);
            }
        }

        // Is this statement list non-empty, and large enough to make using weak children beneficial?
        private static bool IsLargeEnoughNonEmptyStatementList(SyntaxListBuilder<StatementSyntax> statements)
        {
            if (statements.Count == 0)
            {
                return false;
            }
            else if (statements.Count == 1)
            {
                // If we have a single statement, it might be small, like "return null", or large,
                // like a loop or if or switch with many statements inside. Use the width as a proxy for
                // how big it is. If it's small, its better to forgo a many children list anyway, since the
                // weak reference would consume as much memory as is saved.
                return statements[0].Width > 60;
            }
            else
            {
                // For 2 or more statements, go ahead and create a many-children lists.
                return true;
            }
        }

        private void ParseStatements(ref CSharpSyntaxNode previousNode, SyntaxListBuilder<StatementSyntax> statements, bool stopOnSwitchSections)
        {
            var saveTerm = _termState;
            _termState |= TerminatorState.IsPossibleStatementStartOrStop; // partial statements can abort if a new statement starts
            if (stopOnSwitchSections)
            {
                _termState |= TerminatorState.IsSwitchSectionStart;
            }

            while (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken
                && this.CurrentToken.Kind != SyntaxKind.EndOfFileToken
                && !(stopOnSwitchSections && this.IsPossibleSwitchSection()))
            {
                var statement = this.ParseStatementCore();
                if (statement != null)
                {
                    statements.Add(statement);
                    continue;
                }

                GreenNode trailingTrivia;
                var action = this.SkipBadStatementListTokens(statements, SyntaxKind.CloseBraceToken, out trailingTrivia);
                if (trailingTrivia != null)
                {
                    previousNode = AddTrailingSkippedSyntax(previousNode, trailingTrivia);
                }

                if (action == PostSkipAction.Abort)
                {
                    break;
                }
            }

            _termState = saveTerm;
        }

        private bool IsPossibleStatementStartOrStop()
        {
            return this.IsPossibleStatement(acceptAccessibilityMods: true);
        }

        private PostSkipAction SkipBadStatementListTokens(SyntaxListBuilder<StatementSyntax> statements, SyntaxKind expected, out GreenNode trailingTrivia)
        {
            return this.SkipBadListTokensWithExpectedKindHelper(
                statements,
                // We know we have a bad statement, so it can't be a local
                // function, meaning we shouldn't consider accessibility
                // modifiers to be the start of a statement
                p => !p.IsPossibleStatement(acceptAccessibilityMods: false),
                p => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(),
                expected,
                out trailingTrivia
            );
        }

        private bool IsPossibleStatement(bool acceptAccessibilityMods)
        {
            var tk = this.CurrentToken.Kind;
            switch (tk)
            {
                case SyntaxKind.FixedKeyword:
                case SyntaxKind.BreakKeyword:
                case SyntaxKind.ContinueKeyword:
                case SyntaxKind.TryKeyword:
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                case SyntaxKind.DoKeyword:
                case SyntaxKind.ForKeyword:
                case SyntaxKind.GotoKeyword:
                case SyntaxKind.IfKeyword:
                //case SyntaxKind.ElseKeyword:
                case SyntaxKind.LockKeyword:  // TODO: to remove
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.ThrowKeyword:
                case SyntaxKind.UnsafeKeyword:
                case SyntaxKind.UsingKeyword:
                case SyntaxKind.WhileKeyword:
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.VolatileKeyword: // TODO: remove
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.VarKeyword:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.LetKeyword:
                    return true;

                case SyntaxKind.IdentifierToken:
                    return IsTrueIdentifier();

                case SyntaxKind.CatchKeyword:
                case SyntaxKind.FinallyKeyword:
                    return !_isInTry;

                // Accessibility modifiers are not legal in a statement,
                // but a common mistake for local functions. Parse to give a
                // better error message.
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PrivateKeyword:
                    return acceptAccessibilityMods;

                default:
                    return IsPredefinedType(tk)
                        || IsPossibleExpression();
            }
        }

        private bool HasEolInTrailingTrivias(SyntaxListBuilder<SyntaxToken> tokens)
        {
            if (tokens.Count > 0)
            {
                return HasEolInTrailingTrivias(tokens[tokens.Count - 1]);
            }

            return false;
        }

        private InlineILStatementSyntax ParseInlineILStatement()
        {
            var list = default(SyntaxListBuilder<SyntaxToken>);
            try
            {
                list = _pool.Allocate<SyntaxToken>();

                // As we are going to eat any kind of identifiers dot...etc.
                // We are making sure that we break between EOL or semicolon
                if (CurrentToken.Kind == SyntaxKind.SizeOfKeyword)
                {
                    list.Add(EatToken(SyntaxKind.SizeOfKeyword));
                }
                else
                {
                    list.Add(EatToken(SyntaxKind.IdentifierToken));
                }

                bool hasEol = HasEolInTrailingTrivias(list);

                if (!hasEol && IsDotLikeToken(CurrentToken))
                {
                    list.Add(EatToken());
                    hasEol = HasEolInTrailingTrivias(list);

                    while (!hasEol && (CurrentToken.Kind == SyntaxKind.IdentifierToken
                           || SyntaxFacts.IsPredefinedType(CurrentToken.Kind)
                           || CurrentToken.Kind == SyntaxKind.NumericLiteralToken))
                    {
                        list.Add(EatToken());
                        hasEol = HasEolInTrailingTrivias(list);
                        if (!hasEol && IsDotLikeToken(CurrentToken))
                        {
                            list.Add(EatToken());
                            hasEol = HasEolInTrailingTrivias(list);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                var trailingToken = list[list.Count - 1];
                ExpressionSyntax argument = null;
                if (!hasEol && CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
                    argument = ParseSubExpression(Precedence.Expression);
                }

                SyntaxToken eosToken;
                if (argument == null)
                {
                    eosToken = this.EatEos(ref trailingToken);
                    list[list.Count - 1] = trailingToken;
                }
                else
                {
                    eosToken = this.EatEos(ref argument);
                }

                return _syntaxFactory.InlineILStatement(list.ToList(), argument, eosToken);
            }
            finally
            {
                _pool.Free(list);
            }
        }

        private static bool IsDotLikeToken(SyntaxToken token)
        {
            // .1 is parsed as a float so we need to process it on the next loop
            return token.Kind == SyntaxKind.DotToken || token.Kind == SyntaxKind.NumericLiteralToken && token.Text != null && token.Text.StartsWith(".");
        }

        private FixedStatementSyntax ParseFixedStatement()
        {
            var @fixed = this.EatToken(SyntaxKind.FixedKeyword);
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfFixedStatement;
            var decl = ParseVariableDeclaration();
            _termState = saveTerm;

            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
            StatementSyntax statement = this.ParseEmbeddedStatement();
            return _syntaxFactory.FixedStatement(@fixed, openParen, decl, closeParen, statement);
        }

        private bool IsEndOfFixedStatement()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                   || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
        }

        private StatementSyntax ParseEmbeddedStatement()
        {
            // The consumers of embedded statements are expecting to receive a non-null statement 
            // yet there are several error conditions that can lead ParseStatementCore to return 
            // null.  When that occurs create an error empty Statement and return it to the caller.
            StatementSyntax statement = this.ParseStatementCore() ?? SyntaxFactory.EmptyStatement(EatToken(SyntaxKind.SemicolonToken));

            switch (statement.Kind)
            {
                // In scripts, stand-alone expression statements may not be followed by semicolons.
                // ParseExpressionStatement hides the error.
                // However, embedded expression statements are required to be followed by semicolon. 
                case SyntaxKind.ExpressionStatement:
                    if (IsScript)
                    {
                        var expressionStatementSyntax = (ExpressionStatementSyntax)statement;
                        var semicolonToken = expressionStatementSyntax.EosToken;

                        // Do not add a new error if the same error was already added.
                        if (semicolonToken.IsMissing &&
                            !semicolonToken.GetDiagnostics().Contains(diagnosticInfo => (ErrorCode)diagnosticInfo.Code == ErrorCode.ERR_SemicolonExpected))
                        {
                            semicolonToken = this.AddError(semicolonToken, ErrorCode.ERR_SemicolonExpected);
                            statement = expressionStatementSyntax.Update(expressionStatementSyntax.Expression, semicolonToken);
                        }
                    }

                    break;
            }

            return statement;
        }

        private BreakStatementSyntax ParseBreakStatement()
        {
            var breakKeyword = this.EatToken(SyntaxKind.BreakKeyword);
            var eos = this.EatEos(ref breakKeyword);
            return _syntaxFactory.BreakStatement(breakKeyword, eos);
        }

        private ContinueStatementSyntax ParseContinueStatement()
        {
            var continueKeyword = this.EatToken(SyntaxKind.ContinueKeyword);
            var eos = this.EatEos(ref continueKeyword);
            return _syntaxFactory.ContinueStatement(continueKeyword, eos);
        }

        private StatementSyntax ParseTryStatement()
        {
            if (CurrentToken.Kind == SyntaxKind.TryKeyword && PeekToken(1).Kind != SyntaxKind.OpenBraceToken)
            {
                var expression = ParseExpressionCore();
                var eosToken = EatEos(ref expression);
                return _syntaxFactory.ExpressionStatement(expression, eosToken);
            }

            var isInTry = _isInTry;
            _isInTry = true;

            var @try = this.EatToken(SyntaxKind.TryKeyword);

            BlockSyntax block;
            if (@try.IsMissing)
            {
                block = _syntaxFactory.Block(this.EatToken(SyntaxKind.OpenBraceToken), default(SyntaxList<StatementSyntax>), this.EatToken(SyntaxKind.CloseBraceToken));
            }
            else
            {
                var saveTerm = _termState;
                _termState |= TerminatorState.IsEndOfTryBlock;
                block = this.ParseBlock();
                _termState = saveTerm;
            }

            var catches = default(SyntaxListBuilder<CatchClauseSyntax>);
            FinallyClauseSyntax @finally = null;
            try
            {
                bool hasEnd = false;

                if (this.CurrentToken.Kind == SyntaxKind.CatchKeyword)
                {
                    hasEnd = true;
                    catches = _pool.Allocate<CatchClauseSyntax>();
                    while (this.CurrentToken.Kind == SyntaxKind.CatchKeyword)
                    {
                        catches.Add(this.ParseCatchClause());
                    }
                }

                if (this.CurrentToken.Kind == SyntaxKind.FinallyKeyword)
                {
                    hasEnd = true;
                    var fin = this.EatToken();
                    var finBlock = this.ParseBlock();
                    @finally = _syntaxFactory.FinallyClause(fin, finBlock);
                }

                if (!hasEnd)
                {
                    block = this.AddErrorToLastToken(block, ErrorCode.ERR_ExpectedEndTry);

                    // synthesize missing tokens for "finally { }":
                    @finally = _syntaxFactory.FinallyClause(
                        SyntaxToken.CreateMissing(SyntaxKind.FinallyKeyword, null, null),
                        _syntaxFactory.Block(
                            SyntaxToken.CreateMissing(SyntaxKind.OpenBraceToken, null, null),
                            default(SyntaxList<StatementSyntax>),
                            SyntaxToken.CreateMissing(SyntaxKind.CloseBraceToken, null, null)));
                }

                _isInTry = isInTry;

                return _syntaxFactory.TryStatement(@try, block, catches, @finally);
            }
            finally
            {
                if (!catches.IsNull)
                {
                    _pool.Free(catches);
                }
            }
        }

        private bool IsEndOfTryBlock()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseBraceToken
                || this.CurrentToken.Kind == SyntaxKind.CatchKeyword
                || this.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
        }

        private CatchClauseSyntax ParseCatchClause()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.CatchKeyword);

            var @catch = this.EatToken();

            CatchDeclarationSyntax decl = null;
            var saveTerm = _termState;

            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                var openParen = this.EatToken();

                _termState |= TerminatorState.IsEndOfCatchClause;
                var type = this.ParseType();
                SyntaxToken name = null;
                if (this.IsTrueIdentifier())
                {
                    name = this.ParseIdentifierToken();
                }

                _termState = saveTerm;

                var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                decl = _syntaxFactory.CatchDeclaration(openParen, type, name, closeParen);
            }

            CatchFilterClauseSyntax filter = null;

            var keywordKind = this.CurrentToken.ContextualKind;
            if (keywordKind == SyntaxKind.WhenKeyword || keywordKind == SyntaxKind.IfKeyword)
            {
                var whenKeyword = this.EatContextualToken(SyntaxKind.WhenKeyword);
                if (keywordKind == SyntaxKind.IfKeyword)
                {
                    // The initial design of C# exception filters called for the use of the
                    // "if" keyword in this position.  We've since changed to "when", but 
                    // the error recovery experience for early adopters (and for old source
                    // stored in the symbol server) will be better if we consume "if" as
                    // though it were "when".
                    whenKeyword = AddTrailingSkippedSyntax(whenKeyword, EatToken());
                }
                whenKeyword = CheckFeatureAvailability(whenKeyword, MessageID.IDS_FeatureExceptionFilter);
                _termState |= TerminatorState.IsEndOfilterClause;
                var openParen = this.EatToken(SyntaxKind.OpenParenToken);
                var filterExpression = this.ParseExpressionCore();

                _termState = saveTerm;
                var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                filter = _syntaxFactory.CatchFilterClause(whenKeyword, openParen, filterExpression, closeParen);
            }

            _termState |= TerminatorState.IsEndOfCatchBlock;
            var block = this.ParseBlock();
            _termState = saveTerm;

            return _syntaxFactory.CatchClause(@catch, decl, filter, block);
        }

        private bool IsEndOfCatchClause()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
                || this.CurrentToken.Kind == SyntaxKind.CloseBraceToken
                || this.CurrentToken.Kind == SyntaxKind.CatchKeyword
                || this.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
        }

        private bool IsEndOfIfBlock()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseBraceToken
                   || this.CurrentToken.Kind == SyntaxKind.ElseKeyword;
        }

        private bool IsEndOfFilterClause()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
                || this.CurrentToken.Kind == SyntaxKind.CloseBraceToken
                || this.CurrentToken.Kind == SyntaxKind.CatchKeyword
                || this.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
        }
        private bool IsEndOfCatchBlock()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseBraceToken
                || this.CurrentToken.Kind == SyntaxKind.CatchKeyword
                || this.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
        }

        private StatementSyntax ParseCheckedStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.CheckedKeyword || this.CurrentToken.Kind == SyntaxKind.UncheckedKeyword);

            if (this.PeekToken(1).Kind == SyntaxKind.OpenParenToken)
            {
                return this.ParseExpressionStatement();
            }

            var spec = this.EatToken();
            var block = this.ParseBlock();
            return _syntaxFactory.CheckedStatement(SyntaxFacts.GetCheckStatement(spec.Kind), spec, block);
        }

        private DoStatementSyntax ParseDoStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.DoKeyword);
            var @do = this.EatToken(SyntaxKind.DoKeyword);
            var statement = this.ParseBlock();
            var @while = this.EatToken(SyntaxKind.WhileKeyword);

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfDoWhileExpression;
            var expression = this.ParseExpressionCore();
            _termState = saveTerm;
            var eos = this.EatEos(ref expression);
            return _syntaxFactory.DoStatement(@do, statement, @while, expression, eos);
        }

        private bool IsEndOfDoWhileExpression()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                || this.CurrentToken.Kind == SyntaxKind.SemicolonToken;
        }

        private bool IsEndOfForStatementArgument()
        {
            return this.CurrentToken.Kind == SyntaxKind.SemicolonToken
                || this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                || this.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
        }

        private void ParseForStatementExpressionList(ref SyntaxToken startToken, SeparatedSyntaxListBuilder<ExpressionSyntax> list)
        {
            if (this.CurrentToken.Kind != SyntaxKind.CloseParenToken && this.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
tryAgain:
                if (this.IsPossibleExpression() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    // first argument
                    list.Add(this.ParseExpressionCore());

                    // additional arguments
                    while (true)
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken || this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                        {
                            break;
                        }
                        else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleExpression())
                        {
                            list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                            list.Add(this.ParseExpressionCore());
                            continue;
                        }
                        else if (this.SkipBadForStatementExpressionListTokens(ref startToken, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                else if (this.SkipBadForStatementExpressionListTokens(ref startToken, list, SyntaxKind.IdentifierToken) == PostSkipAction.Continue)
                {
                    goto tryAgain;
                }
            }
        }

        private PostSkipAction SkipBadForStatementExpressionListTokens(ref SyntaxToken startToken, SeparatedSyntaxListBuilder<ExpressionSyntax> list, SyntaxKind expected)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleExpression(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseParenToken || p.CurrentToken.Kind == SyntaxKind.SemicolonToken || p.IsTerminator(),
                expected);
        }

        private ForStatementSyntax ParseForEachStatement(SyntaxToken awaitTokenOpt)
        {
            // Can be a 'for' keyword if the user typed: 'for (SomeType t in'
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ForKeyword);

            // Syntax for foreach is either:
            //  foreach [await] ( <type> <identifier> in <expr> ) <embedded-statement>
            // or
            //  foreach [await] ( <deconstruction-declaration> in <expr> ) <embedded-statement>

            var @for = this.EatToken(SyntaxKind.ForKeyword);

            var variable = ParseExpressionOrDeclaration(ParseTypeMode.Normal, feature: MessageID.IDS_FeatureTuples, permitTupleDesignation: true);

            var @in = this.EatToken(SyntaxKind.InKeyword, ErrorCode.ERR_InExpected);
            if (!IsValidForeachVariable(variable))
            {
                @in = this.AddError(@in, ErrorCode.ERR_BadForeachDecl);
            }

            var expression = this.ParseExpressionCore();

            var statement = this.ParseBlock();

            //if (variable is DeclarationExpressionSyntax decl)
            //{
            //    if (decl.Type.Kind == SyntaxKind.RefType)
            //    {
            //        decl = decl.Update(
            //            CheckFeatureAvailability(decl.Type, MessageID.IDS_FeatureRefForEach),
            //            decl.Designation);
            //    }


            //    if (decl.designation.Kind != SyntaxKind.ParenthesizedVariableDesignation)
            //    {
            //        // if we see a foreach declaration that isn't a deconstruction, we use the old form of foreach syntax node.
            //        SyntaxToken identifier;
            //        switch (decl.designation.Kind)
            //        {
            //            case SyntaxKind.SingleVariableDesignation:
            //                identifier = ((SingleVariableDesignationSyntax)decl.designation).identifier;
            //                break;
            //            case SyntaxKind.DiscardDesignation:
            //                // revert the identifier from its contextual underscore back to an identifier.
            //                var discard = ((DiscardDesignationSyntax)decl.designation).underscoreToken;
            //                Debug.Assert(discard.Kind == SyntaxKind.UnderscoreToken);
            //                identifier = SyntaxToken.WithValue(SyntaxKind.IdentifierToken, discard.LeadingTrivia.Node, discard.Text, discard.ValueText, discard.TrailingTrivia.Node);
            //                break;
            //            default:
            //                throw ExceptionUtilities.UnexpectedValue(decl.designation.Kind);
            //        }

            //        return _syntaxFactory.ForEachStatement(awaitTokenOpt, @foreach, decl.Type, identifier, @in, expression, statement);
            //    }
            //}

            return _syntaxFactory.ForStatement(awaitTokenOpt, @for, variable, @in, expression, statement);
        }

        private static bool IsValidForeachVariable(ExpressionSyntax variable)
        {
            switch (variable.Kind)
            {
                case SyntaxKind.TupleExpression:
                    // e.g. `foreach (x, y) in e`
                    return true;
                case SyntaxKind.IdentifierName:
                    // e.g. `foreach x in e`
                    return true;
                default:
                    return false;
            }
        }

        private GotoStatementSyntax ParseGotoStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.GotoKeyword);

            var @goto = this.EatToken(SyntaxKind.GotoKeyword);

            SyntaxToken caseOrDefault = null;
            ExpressionSyntax arg = null;
            SyntaxKind kind;

            if (this.CurrentToken.Kind == SyntaxKind.CaseKeyword || this.CurrentToken.Kind == SyntaxKind.DefaultKeyword)
            {
                caseOrDefault = this.EatToken();
                if (caseOrDefault.Kind == SyntaxKind.CaseKeyword)
                {
                    kind = SyntaxKind.GotoCaseStatement;
                    arg = this.ParseExpressionCore();
                }
                else
                {
                    kind = SyntaxKind.GotoDefaultStatement;
                }
            }
            else
            {
                kind = SyntaxKind.GotoStatement;
                arg = this.ParseIdentifierName();
            }

            SyntaxToken eos = null;
            eos = arg != null ? EatEos(ref arg) : this.EatEos(ref caseOrDefault);

            return _syntaxFactory.GotoStatement(kind, @goto, caseOrDefault, arg, eos);
        }

        private IfStatementSyntax ParseIfStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.IfKeyword);

            var @if = this.EatToken(SyntaxKind.IfKeyword);
            var condition = this.ParseExpressionCore();

            var then = this.EatToken(SyntaxKind.ThenKeyword);

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfIfBlock;
            var statement = this.ParseBlock();
            _termState = saveTerm;
            var elseClause = this.ParseElseClauseOpt();

            return _syntaxFactory.IfStatement(@if, condition, then, statement, elseClause);
        }

        private ElseClauseSyntax ParseElseClauseOpt()
        {
            if (this.CurrentToken.Kind != SyntaxKind.ElseKeyword)
            {
                return null;
            }

            var elseToken = this.EatToken(SyntaxKind.ElseKeyword);
            if (CurrentToken.Kind == SyntaxKind.IfKeyword)
            {
                var elseIf = ParseIfStatement();
                return _syntaxFactory.ElseClause(elseToken, elseIf);
            }

            var elseStatement = this.ParseBlock();
            return _syntaxFactory.ElseClause(elseToken, elseStatement);
        }

        private LockStatementSyntax ParseLockStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.LockKeyword);
            var @lock = this.EatToken(SyntaxKind.LockKeyword);
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);
            var expression = this.ParseExpressionCore();
            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
            var statement = this.ParseEmbeddedStatement();
            return _syntaxFactory.LockStatement(@lock, openParen, expression, closeParen, statement);
        }

        private ReturnStatementSyntax ParseReturnStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ReturnKeyword);
            var @return = this.EatToken(SyntaxKind.ReturnKeyword);
            ExpressionSyntax arg = null;
            if (this.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                arg = this.ParsePossibleRefExpression();
            }

            var eos = arg != null ? EatEos(ref arg) : EatEos(ref @return);
            return _syntaxFactory.ReturnStatement(@return, arg, eos);
        }

        private YieldStatementSyntax ParseYieldStatement()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.YieldKeyword);

            var yieldToken = ConvertToKeyword(this.EatToken());
            SyntaxToken returnOrBreak = null;
            ExpressionSyntax arg = null;
            SyntaxKind kind;

            yieldToken = CheckFeatureAvailability(yieldToken, MessageID.IDS_FeatureIterators);

            if (this.CurrentToken.Kind == SyntaxKind.BreakKeyword)
            {
                kind = SyntaxKind.YieldBreakStatement;
                returnOrBreak = this.EatToken();
            }
            else
            {
                kind = SyntaxKind.YieldReturnStatement;
                returnOrBreak = this.EatToken(SyntaxKind.ReturnKeyword);
                if (this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                {
                    returnOrBreak = this.AddError(returnOrBreak, ErrorCode.ERR_EmptyYield);
                }
                else
                {
                    arg = this.ParseExpressionCore();
                }
            }

            var eos = arg != null ? EatEos(ref arg) : EatEos(ref returnOrBreak);
            return _syntaxFactory.YieldStatement(kind, yieldToken, returnOrBreak, arg, eos);
        }

        private SwitchStatementSyntax ParseSwitchStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.SwitchKeyword);
            var @switch = this.EatToken(SyntaxKind.SwitchKeyword);
            var expression = this.ParseExpressionCore();
            SyntaxToken openParen;
            SyntaxToken closeParen;
            if (expression.Kind == SyntaxKind.ParenthesizedExpression)
            {
                var parenExpression = (ParenthesizedExpressionSyntax)expression;
                openParen = parenExpression.OpenParenToken;
                expression = parenExpression.Expression;
                closeParen = parenExpression.CloseParenToken;

                Debug.Assert(parenExpression.GetDiagnostics().Length == 0);
            }
            else if (expression.Kind == SyntaxKind.TupleExpression)
            {
                // As a special case, when a tuple literal is the governing expression of
                // a switch statement we permit the switch statement's own parentheses to be omitted.
                // LDM 2018-04-04.
                openParen = closeParen = default;
            }
            else
            {
                // Some other expression has appeared without parens. Give a syntax error.
                openParen = SyntaxFactory.MissingToken(SyntaxKind.OpenParenToken);
                expression = this.AddError(expression, ErrorCode.ERR_SwitchGoverningExpressionRequiresParens);
                closeParen = SyntaxFactory.MissingToken(SyntaxKind.CloseParenToken);
            }

            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);

            var sections = _pool.Allocate<SwitchSectionSyntax>();
            try
            {
                while (this.IsPossibleSwitchSection())
                {
                    var swcase = this.ParseSwitchSection();
                    sections.Add(swcase);
                }

                var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);
                return _syntaxFactory.SwitchStatement(@switch, openParen, expression, closeParen, openBrace, sections, closeBrace);
            }
            finally
            {
                _pool.Free(sections);
            }
        }

        private bool IsPossibleSwitchSection()
        {
            return (this.CurrentToken.Kind == SyntaxKind.CaseKeyword) ||
                   (this.CurrentToken.Kind == SyntaxKind.DefaultKeyword && this.PeekToken(1).Kind != SyntaxKind.OpenParenToken);
        }

        private SwitchSectionSyntax ParseSwitchSection()
        {
            Debug.Assert(this.IsPossibleSwitchSection());

            // First, parse case label(s)
            var labels = _pool.Allocate<SwitchLabelSyntax>();
            var statements = _pool.Allocate<StatementSyntax>();
            try
            {
                do
                {
                    SyntaxToken specifier;
                    SwitchLabelSyntax label;
                    SyntaxToken colon;
                    if (this.CurrentToken.Kind == SyntaxKind.CaseKeyword)
                    {
                        ExpressionSyntax expression;
                        specifier = this.EatToken();

                        if (this.CurrentToken.Kind == SyntaxKind.ColonToken)
                        {
                            expression = ParseIdentifierName(ErrorCode.ERR_ConstantExpected);
                            colon = this.EatToken(SyntaxKind.ColonToken);
                            label = _syntaxFactory.CaseSwitchLabel(specifier, expression, colon);
                        }
                        else
                        {
                            var node = CheckRecursivePatternFeature(ParseExpressionOrPattern(whenIsKeyword: true, forSwitchCase: true, precedence: Precedence.Ternary));

                            // if there is a 'when' token, we treat a case expression as a constant pattern.
                            if (this.CurrentToken.ContextualKind == SyntaxKind.WhenKeyword && node is ExpressionSyntax ex)
                                node = _syntaxFactory.ConstantPattern(ex);

                            if (node.Kind == SyntaxKind.DiscardPattern)
                                node = this.AddError(node, ErrorCode.ERR_DiscardPatternInSwitchStatement);

                            if (node is PatternSyntax pat)
                            {
                                var whenClause = ParseWhenClause(Precedence.Expression);
                                colon = this.EatToken(SyntaxKind.ColonToken);
                                label = _syntaxFactory.CasePatternSwitchLabel(specifier, pat, whenClause, colon);
                                label = CheckFeatureAvailability(label, MessageID.IDS_FeaturePatternMatching);
                            }
                            else
                            {
                                colon = this.EatToken(SyntaxKind.ColonToken);
                                label = _syntaxFactory.CaseSwitchLabel(specifier, (ExpressionSyntax)node, colon);
                            }
                        }
                    }
                    else
                    {
                        Debug.Assert(this.CurrentToken.Kind == SyntaxKind.DefaultKeyword);
                        specifier = this.EatToken(SyntaxKind.DefaultKeyword);
                        colon = this.EatToken(SyntaxKind.ColonToken);
                        label = _syntaxFactory.DefaultSwitchLabel(specifier, colon);
                    }

                    labels.Add(label);
                }
                while (IsPossibleSwitchSection());

                // Next, parse statement list stopping for new sections
                CSharpSyntaxNode tmp = labels[labels.Count - 1];
                this.ParseStatements(ref tmp, statements, true);
                labels[labels.Count - 1] = (SwitchLabelSyntax)tmp;

                return _syntaxFactory.SwitchSection(labels, statements);
            }
            finally
            {
                _pool.Free(statements);
                _pool.Free(labels);
            }
        }

        private ThrowStatementSyntax ParseThrowStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.ThrowKeyword);
            var @throw = this.EatToken(SyntaxKind.ThrowKeyword);
            ExpressionSyntax arg = null;
            if (this.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                arg = this.ParseExpressionCore();
            }

            var eos = arg != null ? EatEos(ref arg) : EatEos(ref @throw);
            return _syntaxFactory.ThrowStatement(@throw, arg, eos);
        }

        private UnsafeStatementSyntax ParseUnsafeStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.UnsafeKeyword);
            var @unsafe = this.EatToken(SyntaxKind.UnsafeKeyword);

            SyntaxToken il = null;
            if (CurrentToken.ContextualKind == SyntaxKind.IlKeyword)
            {
                il = ConvertToKeyword(EatToken());
            }

            var block = il != null ? ParseILBlock() : this.ParseBlock();
            return _syntaxFactory.UnsafeStatement(@unsafe, il, block);
        }

        private BlockSyntax ParseILBlock()
        {
            // There's a special error code for a missing token after an accessor keyword
            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);

            var statements = _pool.Allocate<StatementSyntax>();
            try
            {
                while (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken && this.CurrentToken.Kind != SyntaxKind.EndOfFileToken)
                {
                    var inlineILStatementSyntax = this.ParseInlineILStatement();
                    statements.Add(inlineILStatementSyntax);
                }

                //GreenNode trailingTrivia;
                //var action = this.SkipBadStatementListTokens(statements, SyntaxKind.CloseBraceToken, out trailingTrivia);
                //if (trailingTrivia != null)
                //{
                //    previousNode = AddTrailingSkippedSyntax(previousNode, trailingTrivia);
                //}

                //if (action == PostSkipAction.Abort)
                //{
                //    break;
                //}

                var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);

                return _syntaxFactory.Block(openBrace, statements.ToList(), closeBrace);
            }
            finally
            {
                _pool.Free(statements);
            }
        }

        private UsingStatementSyntax ParseUsingStatement(SyntaxToken awaitTokenOpt = default)
        {
            var @using = this.EatToken(SyntaxKind.UsingKeyword);
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);

            VariableDeclarationSyntax declaration = null;
            ExpressionSyntax expression = null;

            var resetPoint = this.GetResetPoint();
            ParseUsingExpression(ref declaration, ref expression, ref resetPoint);
            this.Release(ref resetPoint);

            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
            var statement = this.ParseEmbeddedStatement();

            return _syntaxFactory.UsingStatement(awaitTokenOpt, @using, openParen, declaration, expression, closeParen, statement);
        }

        private void ParseUsingExpression(ref VariableDeclarationSyntax declaration, ref ExpressionSyntax expression, ref ResetPoint resetPoint)
        {
            if (this.IsAwaitExpression())
            {
                expression = this.ParseExpressionCore();
                return;
            }

            // Now, this can be either an expression or a decl list

            ScanTypeFlags st;

            if (this.IsQueryExpression(mayBeVariableDeclaration: true, mayBeMemberDeclaration: false))
            {
                st = ScanTypeFlags.NotType;
            }
            else
            {
                st = this.ScanType();
            }

            if (st == ScanTypeFlags.NullableType)
            {
                // We need to handle:
                // * using (f ? x = a : x = b)
                // * using (f ? x = a)
                // * using (f ? x, y)

                if (this.CurrentToken.Kind != SyntaxKind.IdentifierToken)
                {
                    this.Reset(ref resetPoint);
                    expression = this.ParseExpressionCore();
                }
                else
                {
                    switch (this.PeekToken(1).Kind)
                    {
                        default:
                            this.Reset(ref resetPoint);
                            expression = this.ParseExpressionCore();
                            break;

                        case SyntaxKind.CommaToken:
                        case SyntaxKind.CloseParenToken:
                            this.Reset(ref resetPoint);
                            declaration = ParseVariableDeclaration();
                            break;

                        case SyntaxKind.EqualsToken:
                            // Parse it as a decl. If the next token is a : and only one variable was parsed,
                            // convert the whole thing to ?: expression.
                            this.Reset(ref resetPoint);
                            declaration = ParseVariableDeclaration();

                            // We may have non-nullable types in error scenarios.
                            if (this.CurrentToken.Kind == SyntaxKind.ColonToken &&
                                declaration.Type.Kind == SyntaxKind.NullableType &&
                                SyntaxFacts.IsName(((NullableTypeSyntax)declaration.Type).ElementType.Kind))
                            {
                                // We have "name? id = expr :" so need to convert to a ?: expression.
                                this.Reset(ref resetPoint);
                                declaration = null;
                                expression = this.ParseExpressionCore();
                            }

                            break;
                    }
                }
            }
            else if (IsUsingStatementVariableDeclaration(st))
            {
                this.Reset(ref resetPoint);
                declaration = ParseVariableDeclaration();
            }
            else
            {
                // Must be an expression statement
                this.Reset(ref resetPoint);
                expression = this.ParseExpressionCore();
            }
        }

        private bool IsUsingStatementVariableDeclaration(ScanTypeFlags st)
        {
            Debug.Assert(st != ScanTypeFlags.NullableType);

            bool condition1 = st == ScanTypeFlags.MustBeType && this.CurrentToken.Kind != SyntaxKind.DotToken;
            bool condition2 = st != ScanTypeFlags.NotType && this.CurrentToken.Kind == SyntaxKind.IdentifierToken;
            bool condition3 = st == ScanTypeFlags.NonGenericTypeOrExpression || this.PeekToken(1).Kind == SyntaxKind.EqualsToken;

            return condition1 || (condition2 && condition3);
        }

        private WhileStatementSyntax ParseWhileStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.WhileKeyword);
            var @while = this.EatToken(SyntaxKind.WhileKeyword);
            var condition = this.ParseExpressionCore();
            var statement = this.ParseBlock();
            return _syntaxFactory.WhileStatement(@while, condition, statement);
        }

        private LabeledStatementSyntax ParseLabeledStatement()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.IdentifierToken);

            // We have an identifier followed by a colon. But if the identifier is a contextual keyword in a query context,
            // ParseIdentifier will result in a missing name and Eat(Colon) will fail. We won't make forward progress.
            Debug.Assert(this.IsTrueIdentifier());

            var label = this.ParseIdentifierToken();
            var colon = this.EatToken(SyntaxKind.ColonToken);
            Debug.Assert(!colon.IsMissing);
            var statement = this.ParseStatementCore() ?? SyntaxFactory.EmptyStatement(EatEos(ref colon));
            return _syntaxFactory.LabeledStatement(label, colon, statement);
        }

        /// <summary>
        /// Parses any kind of local declaration statement: local variable or local function.
        /// </summary>
        private StatementSyntax ParseLocalDeclarationStatement(SyntaxToken awaitKeywordOpt = default)
        {
            var usingKeyword = this.CurrentToken.Kind == SyntaxKind.UsingKeyword ? this.EatToken() : null;
            if (usingKeyword != null)
            {
                usingKeyword = CheckFeatureAvailability(usingKeyword, MessageID.IDS_FeatureUsingDeclarations);
            }

            var variableDeclaration = ParseVariableDeclaration(true);

            var eos = EatEos(ref variableDeclaration);
            return _syntaxFactory.LocalDeclarationStatement(
                awaitKeywordOpt,
                usingKeyword,
                variableDeclaration,
                eos
            );
        }

        private VariableDesignationSyntax ParseDesignation(bool forPattern)
        {
            // the two forms of designation are
            // (1) identifier
            // (2) ( designation ... )
            // for pattern-matching, we permit the designation list to be empty
            VariableDesignationSyntax result;
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                var openParen = this.EatToken(SyntaxKind.OpenParenToken);
                var listOfDesignations = _pool.AllocateSeparated<VariableDesignationSyntax>();

                bool done = false;
                if (forPattern)
                {
                    done = (this.CurrentToken.Kind == SyntaxKind.CloseParenToken);
                }
                else
                {
                    listOfDesignations.Add(ParseDesignation(forPattern));
                    listOfDesignations.AddSeparator(EatToken(SyntaxKind.CommaToken));
                }

                if (!done)
                {
                    while (true)
                    {
                        listOfDesignations.Add(ParseDesignation(forPattern));
                        if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                        {
                            listOfDesignations.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                result = _syntaxFactory.ParenthesizedVariableDesignation(openParen, listOfDesignations, closeParen);
                _pool.Free(listOfDesignations);
            }
            else
            {
                result = ParseSimpleDesignation();
            }

            return result;
        }

        /// <summary>
        /// Parse a single variable designation (e.g. <c>x</c>) or a wildcard designation (e.g. <c>_</c>)
        /// </summary>
        /// <returns></returns>
        private VariableDesignationSyntax ParseSimpleDesignation()
        {
            if (CurrentToken.ContextualKind == SyntaxKind.UnderscoreToken)
            {
                var underscore = this.EatContextualToken(SyntaxKind.UnderscoreToken);
                return _syntaxFactory.DiscardDesignation(underscore);
            }
            else
            {
                var identifier = this.EatToken(SyntaxKind.IdentifierToken);
                return _syntaxFactory.SingleVariableDesignation(identifier);
            }
        }

        private WhenClauseSyntax ParseWhenClause(Precedence precedence)
        {
            if (this.CurrentToken.ContextualKind != SyntaxKind.WhenKeyword)
            {
                return null;
            }

            var when = this.EatContextualToken(SyntaxKind.WhenKeyword);
            var condition = ParseSubExpression(precedence);
            return _syntaxFactory.WhenClause(when, condition);
        }

        private bool IsEndOfDeclarationClause()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.ColonToken:
                    return true;
                default:
                    return false;
            }
        }

        private void ParseDeclarationModifiers(SyntaxListBuilder list)
        {
            SyntaxKind k;
            while (IsDeclarationModifier(k = this.CurrentToken.ContextualKind) || IsAdditionalLocalFunctionModifier(k))
            {
                SyntaxToken mod;
                if (k == SyntaxKind.AsyncKeyword)
                {
                    // check for things like "async async()" where async is the type and/or the function name
                    {
                        var resetPoint = this.GetResetPoint();

                        var invalid = !IsPossibleStartOfTypeDeclaration(this.EatToken().Kind) &&
                            !IsDeclarationModifier(this.CurrentToken.Kind) && !IsAdditionalLocalFunctionModifier(this.CurrentToken.Kind) &&
                            (ScanType() == ScanTypeFlags.NotType || this.CurrentToken.Kind != SyntaxKind.IdentifierToken);

                        this.Reset(ref resetPoint);
                        this.Release(ref resetPoint);

                        if (invalid)
                        {
                            break;
                        }
                    }

                    mod = this.EatContextualToken(k);
                    if (k == SyntaxKind.AsyncKeyword)
                    {
                        mod = CheckFeatureAvailability(mod, MessageID.IDS_FeatureAsync);
                    }
                }
                else
                {
                    mod = this.EatToken();
                }

                if (k == SyntaxKind.ReadOnlyKeyword || k == SyntaxKind.VolatileKeyword)
                {
                    mod = this.AddError(mod, ErrorCode.ERR_BadMemberFlag, mod.Text);
                }
                else if (list.Any(mod.RawKind))
                {
                    // check for duplicates, can only be const
                    mod = this.AddError(mod, ErrorCode.ERR_TypeExpected, mod.Text);
                }

                list.Add(mod);
            }
        }

        private static bool IsDeclarationModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.VolatileKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsAdditionalLocalFunctionModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.AsyncKeyword:
                case SyntaxKind.UnsafeKeyword:
                // Not a valid modifier, but we should parse to give a good
                // error message
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PrivateKeyword:
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsAccessibilityModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                // Accessibility modifiers aren't legal in a local function,
                // but a common mistake. Parse to give a better error message.
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PrivateKeyword:
                    return true;

                default:
                    return false;
            }
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            return ParseExpressionStatement(this.ParseExpressionCore());
        }

        private ExpressionStatementSyntax ParseExpressionStatement(ExpressionSyntax expression)
        {
            var eosToken = EatEos(ref expression);
            return _syntaxFactory.ExpressionStatement(expression, eosToken);
        }

        public ExpressionSyntax ParseExpression()
        {
            return ParseWithStackGuard(
                this.ParseExpressionCore,
                this.CreateMissingIdentifierName);
        }

        private ExpressionSyntax ParseExpressionCore()
        {
            return this.ParseSubExpression(Precedence.Expression);
        }

        private bool IsPossibleExpression(bool allowBinaryExpressions = true, bool allowAssignmentExpressions = true)
        {
            var tk = this.CurrentToken.Kind;
            switch (tk)
            {
                case SyntaxKind.TypeOfKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.SizeOfKeyword:
                case SyntaxKind.MakeRefKeyword:
                case SyntaxKind.RefTypeKeyword:
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                case SyntaxKind.RefValueKeyword:
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.BaseKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.ThisKeyword:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.NullKeyword:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.InterpolatedStringStartToken:
                case SyntaxKind.InterpolatedStringToken:
                case SyntaxKind.RuneLiteralToken:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.ColonColonToken: // bad aliased name
                case SyntaxKind.ThrowKeyword:
                case SyntaxKind.StackAllocKeyword:
                case SyntaxKind.DotDotToken:
                    return true;
                case SyntaxKind.IdentifierToken:
                    // Specifically allow the from contextual keyword, because it can always be the start of an
                    // expression (whether it is used as an identifier or a keyword).
                    return this.IsTrueIdentifier() || (this.CurrentToken.ContextualKind == SyntaxKind.FromKeyword);
                default:
                    return (IsPredefinedType(tk) && tk != SyntaxKind.VoidKeyword)
                        || SyntaxFacts.IsAnyUnaryExpression(tk)
                        || (allowBinaryExpressions && SyntaxFacts.IsBinaryExpression(tk))
                        || (allowAssignmentExpressions && SyntaxFacts.IsAssignmentExpressionOperatorToken(tk));
            }
        }

        private static bool IsInvalidSubExpression(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.BreakKeyword:
                case SyntaxKind.CaseKeyword:
                case SyntaxKind.CatchKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.ContinueKeyword:
                case SyntaxKind.DoKeyword:
                case SyntaxKind.FinallyKeyword:
                case SyntaxKind.ForKeyword:
                case SyntaxKind.GotoKeyword:
                case SyntaxKind.ElseKeyword:
                case SyntaxKind.LockKeyword:
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.UsingKeyword:
                case SyntaxKind.WhileKeyword:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsRightAssociative(SyntaxKind op)
        {
            switch (op)
            {
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.CoalesceAssignmentExpression:
                case SyntaxKind.CoalesceExpression:
                    return true;
                default:
                    return false;
            }
        }

        enum Precedence : uint
        {
            Expression = 0, // Loosest possible precedence, used to accept all expressions
            Assignment,
            Lambda = Assignment, // "The => operator has the same precedence as assignment (=) and is right-associative."
            Ternary,
            Coalescing,
            ConditionalOr,
            ConditionalAnd,
            LogicalOr,
            LogicalXor,
            LogicalAnd,
            Equality,
            Relational,
            Shift,
            Range,
            Additive,
            Mutiplicative,
            Unary,
            Cast,
            PointerIndirection,
            AddressOf,
            Primary_UNUSED, // Primaries are parsed in an ad-hoc manner.
        }

        private static Precedence GetPrecedence(SyntaxKind op)
        {
            switch (op)
            {
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.CoalesceAssignmentExpression:
                    return Precedence.Assignment;
                case SyntaxKind.CoalesceExpression:
                    return Precedence.Coalescing;
                case SyntaxKind.LogicalOrExpression:
                    return Precedence.ConditionalOr;
                case SyntaxKind.LogicalAndExpression:
                    return Precedence.ConditionalAnd;
                case SyntaxKind.BitwiseOrExpression:
                    return Precedence.LogicalOr;
                case SyntaxKind.ExclusiveOrExpression:
                    return Precedence.LogicalXor;
                case SyntaxKind.BitwiseAndExpression:
                    return Precedence.LogicalAnd;
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    return Precedence.Equality;
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsOptExpression:
                case SyntaxKind.IsPatternExpression:
                    return Precedence.Relational;
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return Precedence.Shift;
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                    return Precedence.Additive;
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                    return Precedence.Mutiplicative;
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.TypeOfExpression:
                case SyntaxKind.SizeOfExpression:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.UncheckedExpression:
                case SyntaxKind.MakeRefExpression:
                case SyntaxKind.RefValueExpression:
                case SyntaxKind.RefTypeExpression:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.TryExpression:
                case SyntaxKind.IndexExpression:
                    return Precedence.Unary;
                case SyntaxKind.CastExpression:
                    return Precedence.Cast;
                case SyntaxKind.PointerIndirectionExpression:
                    return Precedence.PointerIndirection;
                case SyntaxKind.AddressOfExpression:
                    return Precedence.AddressOf;
                case SyntaxKind.RangeExpression:
                    return Precedence.Range;
                case SyntaxKind.IfExpression:
                case SyntaxKind.SwitchExpression:
                    return Precedence.Expression;
                default:
                    throw ExceptionUtilities.UnexpectedValue(op);
            }
        }

        private static bool IsExpectedPrefixUnaryOperator(SyntaxKind kind)
        {
            return SyntaxFacts.IsPrefixUnaryExpression(kind) && kind != SyntaxKind.RefKeyword && kind != SyntaxKind.OutKeyword;
        }

        private static bool IsExpectedBinaryOperator(SyntaxKind kind)
        {
            return SyntaxFacts.IsBinaryExpression(kind);
        }

        private static bool IsExpectedAssignmentOperator(SyntaxKind kind)
        {
            return SyntaxFacts.IsAssignmentExpressionOperatorToken(kind);
        }

        private bool IsPossibleAwaitExpressionStatement()
        {
            return (this.IsScript || this.IsInAsync) && this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword;
        }

        private bool IsAwaitExpression()
        {
            if (this.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword)
            {
                if (this.IsInAsync)
                {
                    // If we see an await in an async function, parse it as an unop.
                    return true;
                }

                // If we see an await followed by a token that cannot follow an identifier, parse await as a unop.
                // BindAwait() catches the cases where await successfully parses as a unop but is not in an async
                // function, and reports an appropriate ERR_BadAwaitWithoutAsync* error.
                switch (this.PeekToken(1).Kind)
                {
                    case SyntaxKind.IdentifierToken:

                    // Keywords
                    case SyntaxKind.NewKeyword:
                    case SyntaxKind.ThisKeyword:
                    case SyntaxKind.BaseKeyword:
                    case SyntaxKind.DelegateKeyword:
                    case SyntaxKind.TypeOfKeyword:
                    case SyntaxKind.CheckedKeyword:
                    case SyntaxKind.UncheckedKeyword:
                    case SyntaxKind.DefaultKeyword:

                    // Literals
                    case SyntaxKind.TrueKeyword:
                    case SyntaxKind.FalseKeyword:
                    case SyntaxKind.StringLiteralToken:
                    case SyntaxKind.InterpolatedStringStartToken:
                    case SyntaxKind.InterpolatedStringToken:
                    case SyntaxKind.NumericLiteralToken:
                    case SyntaxKind.NullKeyword:
                    case SyntaxKind.RuneLiteralToken:
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Parse a subexpression of the enclosing operator of the given precedence.
        /// </summary>
        private ExpressionSyntax ParseSubExpression(Precedence precedence)
        {
            _recursionDepth++;

            StackGuard.EnsureSufficientExecutionStack(_recursionDepth);

            var result = ParseSubExpressionCore(precedence);

            _recursionDepth--;
            return result;
        }

        private ExpressionSyntax ParseSubExpressionCore(Precedence precedence)
        {
            ExpressionSyntax leftOperand = null;
            Precedence newPrecedence = 0;
            SyntaxKind opKind = SyntaxKind.None;

            // all of these are tokens that start statements and are invalid
            // to start a expression with. if we see one, then we must have
            // something like:
            //
            // return
            // if (...
            // parse out a missing name node for the expression, and keep on going
            var tk = this.CurrentToken.Kind;
            if (IsInvalidSubExpression(tk))
            {
                return this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(tk));
            }

            // No left operand, so we need to parse one -- possibly preceded by a
            // unary operator.
            if (IsExpectedPrefixUnaryOperator(tk))
            {
                opKind = SyntaxFacts.GetPrefixUnaryExpression(tk);
                newPrecedence = GetPrecedence(opKind);
                var opToken = this.EatToken();
                var operand = this.ParseSubExpression(newPrecedence);
                leftOperand = _syntaxFactory.PrefixUnaryExpression(opKind, opToken, operand);
            }
            else if (tk == SyntaxKind.DotDotToken)
            {
                // Operator ".." here can either be a prefix unary operator or a stand alone empty range:
                var opToken = this.EatToken();
                opKind = SyntaxKind.RangeExpression;
                newPrecedence = GetPrecedence(opKind);

                ExpressionSyntax rightOperand;
                if (IsPossibleExpression(allowBinaryExpressions: false, allowAssignmentExpressions: false))
                {
                    rightOperand = this.ParseSubExpression(newPrecedence);
                }
                else
                {
                    rightOperand = null;
                }

                leftOperand = _syntaxFactory.RangeExpression(leftOperand: null, opToken, rightOperand);
            }
            else if (IsAwaitExpression())
            {
                opKind = SyntaxKind.AwaitExpression;
                newPrecedence = GetPrecedence(opKind);
                var awaitToken = this.EatContextualToken(SyntaxKind.AwaitKeyword);
                awaitToken = CheckFeatureAvailability(awaitToken, MessageID.IDS_FeatureAsync);
                var operand = this.ParseSubExpression(newPrecedence);
                leftOperand = _syntaxFactory.AwaitExpression(awaitToken, operand);
            }
            else if (tk == SyntaxKind.TryKeyword)
            {
                opKind = SyntaxKind.TryExpression;
                newPrecedence = GetPrecedence(opKind);
                var tryToken = this.EatToken();
                var operand = this.ParseSubExpression(newPrecedence);
                leftOperand = _syntaxFactory.TryExpression(tryToken, operand);
            }
            else if (this.IsQueryExpression(mayBeVariableDeclaration: false, mayBeMemberDeclaration: false))
            {
                leftOperand = this.ParseQueryExpression(precedence);
            }
            else if (this.CurrentToken.ContextualKind == SyntaxKind.FromKeyword && IsInQuery)
            {
                // If this "from" token wasn't the start of a query then it's not really an expression.
                // Consume it so that we don't try to parse it again as the next argument in an
                // argument list.
                SyntaxToken skipped = this.EatToken(); // consume but skip "from"
                skipped = this.AddError(skipped, ErrorCode.ERR_InvalidExprTerm, this.CurrentToken.Text);
                leftOperand = AddTrailingSkippedSyntax(this.CreateMissingIdentifierName(), skipped);
            }
            else if (tk == SyntaxKind.ThrowKeyword)
            {
                var result = ParseThrowExpression();
                // we parse a throw expression even at the wrong precedence for better recovery
                return (precedence <= Precedence.Coalescing) ? result :
                    this.AddError(result, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(tk));
            }
            else if (this.IsPossibleDeconstructionLeft(precedence))
            {
                leftOperand = ParseDeclarationExpression(ParseTypeMode.Normal, MessageID.IDS_FeatureTuples);
            }
            else
            {
                // Not a unary operator - get a primary expression.
                leftOperand = this.ParseTerm(precedence);
            }

            while (true)
            {
                // We either have a binary or assignment operator here, or we're finished.
                tk = this.CurrentToken.ContextualKind;

                bool isAssignmentOperator = false;
                if (IsExpectedBinaryOperator(tk))
                {
                    opKind = SyntaxFacts.GetBinaryExpression(tk);
                }
                else if (IsExpectedAssignmentOperator(tk))
                {
                    opKind = SyntaxFacts.GetAssignmentExpression(tk);
                    isAssignmentOperator = true;
                }
                else if (tk == SyntaxKind.DotDotToken)
                {
                    opKind = SyntaxKind.RangeExpression;
                }
                else
                {
                    break;
                }

                newPrecedence = GetPrecedence(opKind);

                Debug.Assert(newPrecedence > 0);      // All binary operators must have precedence > 0!

                // check for >> or >>=
                bool doubleOp = false;
                if (tk == SyntaxKind.GreaterThanToken
                    && (this.PeekToken(1).Kind == SyntaxKind.GreaterThanToken || this.PeekToken(1).Kind == SyntaxKind.GreaterThanEqualsToken))
                {
                    // check to see if they really are adjacent
                    if (this.CurrentToken.GetTrailingTriviaWidth() == 0 && this.PeekToken(1).GetLeadingTriviaWidth() == 0)
                    {
                        if (this.PeekToken(1).Kind == SyntaxKind.GreaterThanToken)
                        {
                            opKind = SyntaxFacts.GetBinaryExpression(SyntaxKind.GreaterThanGreaterThanToken);
                        }
                        else
                        {
                            opKind = SyntaxFacts.GetAssignmentExpression(SyntaxKind.GreaterThanGreaterThanEqualsToken);
                            isAssignmentOperator = true;
                        }
                        newPrecedence = GetPrecedence(opKind);
                        doubleOp = true;
                    }
                }

                // Check the precedence to see if we should "take" this operator
                if (newPrecedence < precedence)
                {
                    break;
                }

                // Same precedence, but not right-associative -- deal with this "later"
                if ((newPrecedence == precedence) && !IsRightAssociative(opKind))
                {
                    break;
                }

                // Precedence is okay, so we'll "take" this operator.
                var opToken = this.EatContextualToken(tk);
                if (doubleOp)
                {
                    // combine tokens into a single token
                    var opToken2 = this.EatToken();
                    var kind = opToken2.Kind == SyntaxKind.GreaterThanToken ? SyntaxKind.GreaterThanGreaterThanToken : SyntaxKind.GreaterThanGreaterThanEqualsToken;
                    opToken = SyntaxFactory.Token(opToken.GetLeadingTrivia(), kind, opToken2.GetTrailingTrivia());
                }

                if (opKind == SyntaxKind.AsOptExpression)
                {
                    var type = this.ParseType(ParseTypeMode.AsExpression);
                    leftOperand = _syntaxFactory.BinaryExpression(opKind, leftOperand, opToken, type);
                }
                else if (opKind == SyntaxKind.CastExpression)
                {
                    var type = this.ParseType(ParseTypeMode.AsExpression);
                    leftOperand = _syntaxFactory.CastExpression(leftOperand, opToken, type);
                }
                else if (opKind == SyntaxKind.IsExpression)
                {
                    leftOperand = ParseIsExpression(leftOperand, opToken);
                }
                else
                {
                    if (isAssignmentOperator)
                    {
                        ExpressionSyntax rhs = opKind == SyntaxKind.SimpleAssignmentExpression && CurrentToken.Kind == SyntaxKind.RefKeyword
                            ? rhs = CheckFeatureAvailability(ParsePossibleRefExpression(), MessageID.IDS_FeatureRefReassignment)
                            : rhs = this.ParseSubExpression(newPrecedence);

                        if (opKind == SyntaxKind.CoalesceAssignmentExpression)
                        {
                            opToken = CheckFeatureAvailability(opToken, MessageID.IDS_FeatureCoalesceAssignmentExpression);
                        }

                        leftOperand = _syntaxFactory.AssignmentExpression(opKind, leftOperand, opToken, rhs);
                    }
                    else
                    {
                        if (tk == SyntaxKind.DotDotToken)
                        {
                            // Operator ".." here can either be a binary or a postfix unary operator:
                            Debug.Assert(opKind == SyntaxKind.RangeExpression);

                            ExpressionSyntax rightOperand;
                            if (IsPossibleExpression(allowBinaryExpressions: false, allowAssignmentExpressions: false))
                            {
                                newPrecedence = GetPrecedence(opKind);
                                rightOperand = this.ParseSubExpression(newPrecedence);
                            }
                            else
                            {
                                rightOperand = null;
                            }

                            leftOperand = _syntaxFactory.RangeExpression(leftOperand, opToken, rightOperand);
                        }
                        else
                        {
                            leftOperand = _syntaxFactory.BinaryExpression(opKind, leftOperand, opToken, this.ParseSubExpression(newPrecedence));
                        }
                    }
                }
            }

            // From the proposed language spec:
            //
            // switch-expression:
            //  null-coalescing-expression switch ( switch-expression-case-list )
            // switch-expression-case-list:
            //  switch-expression-case
            //  switch-expression-case , switch-expression-case-list
            // switch-expression-case:
            //  pattern => expression
            //
            // Only take the switch if we're at a precedence less than the null coalescing expression.

            if (tk == SyntaxKind.SwitchKeyword && precedence < Precedence.Coalescing && this.PeekToken(1).Kind == SyntaxKind.OpenBraceToken)
            {
                leftOperand = ParseSwitchExpression(leftOperand);
            }

            return leftOperand;
        }

        private ExpressionSyntax ParseDeclarationExpression(ParseTypeMode mode, MessageID feature)
        {
            TypeSyntax type = this.ParseType(mode);
            var designation = ParseDesignation(forPattern: false);
            if (feature != MessageID.None)
            {
                designation = CheckFeatureAvailability(designation, feature);
            }

            return _syntaxFactory.DeclarationExpression(type, designation);
        }

        private ExpressionSyntax ParseThrowExpression()
        {
            var throwToken = this.EatToken(SyntaxKind.ThrowKeyword);
            var thrown = this.ParseSubExpression(Precedence.Coalescing);
            var result = _syntaxFactory.ThrowExpression(throwToken, thrown);
            return CheckFeatureAvailability(result, MessageID.IDS_FeatureThrowExpression);
        }

        private ExpressionSyntax ParseIsExpression(ExpressionSyntax leftOperand, SyntaxToken opToken)
        {
            var node = this.ParseTypeOrPatternForIsOperator();
            if (node is PatternSyntax)
            {
                var result = _syntaxFactory.IsPatternExpression(leftOperand, opToken, (PatternSyntax)node);
                return CheckFeatureAvailability(result, MessageID.IDS_FeaturePatternMatching);
            }
            else
            {
                Debug.Assert(node is TypeSyntax);
                return _syntaxFactory.BinaryExpression(SyntaxKind.IsExpression, leftOperand, opToken, (TypeSyntax)node);
            }
        }

        private ExpressionSyntax ParseTerm(Precedence precedence)
        {
            ExpressionSyntax expr = null;

            var tk = this.CurrentToken.Kind;
            switch (tk)
            {
                case SyntaxKind.TypeOfKeyword:
                    expr = this.ParseTypeOfExpression();
                    break;
                case SyntaxKind.DefaultKeyword:
                    expr = this.ParseDefaultExpression();
                    break;
                case SyntaxKind.SizeOfKeyword:
                    expr = this.ParseSizeOfExpression();
                    break;
                case SyntaxKind.MakeRefKeyword:
                    expr = this.ParseMakeRefExpression();
                    break;
                case SyntaxKind.RefTypeKeyword:
                    expr = this.ParseRefTypeExpression();
                    break;
                case SyntaxKind.IfKeyword:
                    expr = this.ParseIfExpression();
                    break;
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                    expr = this.ParseCheckedOrUncheckedExpression();
                    break;
                case SyntaxKind.RefValueKeyword:
                    expr = this.ParseRefValueExpression();
                    break;
                case SyntaxKind.ColonColonToken:
                    // misplaced ::
                    // TODO: this should not be a compound name.. (disallow dots)
                    expr = this.ParseQualifiedName(NameOptions.InExpression);
                    break;
                case SyntaxKind.IdentifierToken:
                    if (this.IsTrueIdentifier())
                    {
                        var contextualKind = this.CurrentToken.ContextualKind;
                        if (contextualKind == SyntaxKind.AsyncKeyword && this.PeekToken(1).Kind == SyntaxKind.DelegateKeyword)
                        {
                            expr = this.ParseAnonymousMethodExpression();
                        }
                        else if (this.IsPossibleLambdaExpression(precedence))
                        {
                            expr = this.ParseLambdaExpression();
                        }
                        else if (this.IsPossibleDeconstructionLeft(precedence))
                        {
                            expr = ParseDeclarationExpression(ParseTypeMode.Normal, MessageID.IDS_FeatureTuples);
                        }
                        else
                        {
                            expr = this.ParseAliasQualifiedName(NameOptions.InExpression);
                        }
                    }
                    else
                    {
                        expr = this.CreateMissingIdentifierName();
                        expr = this.AddError(expr, ErrorCode.ERR_InvalidExprTerm, this.CurrentToken.Text);
                    }

                    break;
                case SyntaxKind.ThisKeyword:
                    expr = _syntaxFactory.ThisExpression(this.EatToken());
                    break;
                case SyntaxKind.BaseKeyword:
                    expr = _syntaxFactory.BaseExpression(this.EatToken());
                    break;
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.FalseKeyword:

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.NullKeyword:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.RuneLiteralToken:
                    expr = _syntaxFactory.LiteralExpression(SyntaxFacts.GetLiteralExpression(tk), this.EatToken());
                    break;
                case SyntaxKind.InterpolatedStringStartToken:
                    throw new NotImplementedException(); // this should not occur because these tokens are produced and parsed immediately
                case SyntaxKind.InterpolatedStringToken:
                    expr = this.ParseInterpolatedStringToken();
                    break;
                case SyntaxKind.OpenParenToken:
                    expr = this.ParseParenExpressionOrLambdaOrTuple(precedence);
                    break;
                case SyntaxKind.NewKeyword:
                    expr = this.ParseNewExpression();
                    break;
                case SyntaxKind.StackAllocKeyword:
                    expr = this.ParseStackAllocExpression();
                    break;
                case SyntaxKind.DelegateKeyword:
                    expr = this.ParseAnonymousMethodExpression();
                    break;
                default:
                    // check for intrinsic type followed by '.'
                    if (IsPredefinedType(tk))
                    {
                        expr = _syntaxFactory.PredefinedType(this.EatToken());

                        if (this.CurrentToken.Kind != SyntaxKind.DotToken || tk == SyntaxKind.VoidKeyword)
                        {
                            expr = this.AddError(expr, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(tk));
                        }
                    }
                    else
                    {
                        // Eat the token if it is invalid
                        expr = CreateMissingIdentifierName();

                        if (tk == SyntaxKind.EndOfFileToken)
                        {
                            expr = this.AddError(expr, ErrorCode.ERR_ExpressionExpected);
                        }
                        else
                        {
                            // We skip the invalid token
                            var skippedToken = EatToken();
                            var builder = new SyntaxListBuilder(1);
                            builder.Add(skippedToken);
                            var fileAsTrivia = _syntaxFactory.SkippedTokensTrivia(builder.ToList<SyntaxToken>());
                            expr = AddLeadingSkippedSyntax(expr, fileAsTrivia);

                            expr = this.AddError(expr, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(tk));
                        }
                    }

                    break;
            }

            return this.ParsePostFixExpression(expr);
        }

        private ExpressionSyntax ParseIfExpression()
        {
            var @if = this.EatToken(SyntaxKind.IfKeyword);
            var condition = this.ParseExpressionCore();

            var then = this.EatToken(SyntaxKind.ThenKeyword);

            var trueExpr = this.ParseExpressionCore();

            var @else = this.EatToken(SyntaxKind.ElseKeyword);

            var falseExpr = this.ParseExpressionCore();

            return _syntaxFactory.IfExpression(@if, condition, then, trueExpr, @else, falseExpr);
        }

        /// <summary>
        /// Returns true if...
        /// 1. The precedence is less than or equal to Assignment, and
        /// 2. The current token is the identifier var or a predefined type, and
        /// 3. it is followed by (, and
        /// 4. that ( begins a valid parenthesized designation, and
        /// 5. the token following that designation is =
        /// </summary>
        private bool IsPossibleDeconstructionLeft(Precedence precedence)
        {
            if (precedence > Precedence.Assignment || !(this.CurrentToken.IsIdentifierVar() || IsPredefinedType(this.CurrentToken.Kind)))
            {
                return false;
            }

            var resetPoint = this.GetResetPoint();
            try
            {
                this.EatToken(); // `var`
                return
                    this.CurrentToken.Kind == SyntaxKind.OpenParenToken && ScanDesignator() &&
                    this.CurrentToken.Kind == SyntaxKind.EqualsToken;
            }
            finally
            {
                // Restore current token index
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);
            }
        }

        private bool ScanDesignator()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.IdentifierToken:
                    if (!IsTrueIdentifier())
                    {
                        goto default;
                    }

                    this.EatToken(); // eat the identifier
                    return true;
                case SyntaxKind.OpenParenToken:
                    while (true)
                    {
                        this.EatToken(); // eat the open paren or comma
                        if (!ScanDesignator())
                        {
                            return false;
                        }

                        switch (this.CurrentToken.Kind)
                        {
                            case SyntaxKind.CommaToken:
                                continue;
                            case SyntaxKind.CloseParenToken:
                                this.EatToken(); // eat the close paren
                                return true;
                            default:
                                return false;
                        }
                    }
                default:
                    return false;
            }
        }

        private bool IsPossibleLambdaExpression(Precedence precedence)
        {
            if (precedence <= Precedence.Lambda && this.PeekToken(1).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }

            if (ScanAsyncLambda(precedence))
            {
                return true;
            }

            return false;
        }

        private ExpressionSyntax ParsePostFixExpression(ExpressionSyntax expr)
        {
            Debug.Assert(expr != null);

            while (true)
            {
                SyntaxKind tk = this.CurrentToken.Kind;
                switch (tk)
                {
                    case SyntaxKind.OpenParenToken:
                        expr = _syntaxFactory.InvocationExpression(expr, this.ParseParenthesizedArgumentList());
                        break;

                    case SyntaxKind.OpenBracketToken:
                        expr = _syntaxFactory.ElementAccessExpression(expr, this.ParseBracketedArgumentList());
                        break;

                    case SyntaxKind.PlusPlusToken:
                    case SyntaxKind.MinusMinusToken:
                        expr = _syntaxFactory.PostfixUnaryExpression(SyntaxFacts.GetPostfixUnaryExpression(tk), expr, this.EatToken());
                        break;

                    case SyntaxKind.ColonColonToken:
                        if (this.PeekToken(1).Kind == SyntaxKind.IdentifierToken)
                        {
                            // replace :: with missing dot and annotate with skipped text "::" and error
                            var ccToken = this.EatToken();
                            ccToken = this.AddError(ccToken, ErrorCode.ERR_UnexpectedAliasedName);
                            var dotToken = this.ConvertToMissingWithTrailingTrivia(ccToken, SyntaxKind.DotToken);
                            expr = _syntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expr, dotToken, this.ParseSimpleName(NameOptions.InExpression));
                        }
                        else
                        {
                            // just some random trailing :: ?
                            expr = AddTrailingSkippedSyntax(expr, this.EatTokenWithPrejudice(SyntaxKind.DotToken));
                        }
                        break;

                    case SyntaxKind.MinusGreaterThanToken:
                        expr = _syntaxFactory.MemberAccessExpression(SyntaxKind.PointerMemberAccessExpression, expr, this.EatToken(), this.ParseSimpleName(NameOptions.InExpression));
                        break;
                    case SyntaxKind.DotToken:
                        // if we have the error situation:
                        //
                        //      expr.
                        //      X Y
                        //
                        // Then we don't want to parse this out as "Expr.X"
                        //
                        // It's far more likely the member access expression is simply incomplete and
                        // there is a new declaration on the next line.
                        if (this.CurrentToken.TrailingTrivia.Any((int)SyntaxKind.EndOfLineTrivia) &&
                            this.PeekToken(1).Kind == SyntaxKind.IdentifierToken &&
                            this.PeekToken(2).ContextualKind == SyntaxKind.IdentifierToken)
                        {
                            expr = _syntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression, expr, this.EatToken(),
                                this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_IdentifierExpected));

                            return expr;
                        }

                        expr = _syntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expr, this.EatToken(), this.ParseSimpleName(NameOptions.InExpression));
                        break;

                    case SyntaxKind.QuestionToken:
                        if (CanStartConsequenceExpression(this.PeekToken(1).Kind))
                        {
                            var qToken = this.EatToken();
                            var consequence = ParseConsequenceSyntax();
                            expr = _syntaxFactory.ConditionalAccessExpression(expr, qToken, consequence);
                            expr = CheckFeatureAvailability(expr, MessageID.IDS_FeatureNullPropagatingOperator);
                            break;
                        }

                        goto default;

                    case SyntaxKind.ExclamationToken:
                        expr = _syntaxFactory.PostfixUnaryExpression(SyntaxFacts.GetPostfixUnaryExpression(tk), expr, this.EatToken());
                        expr = CheckFeatureAvailability(expr, MessageID.IDS_FeatureNullableReferenceTypes);
                        break;

                    default:
                        return expr;
                }
            }
        }

        private static bool CanStartConsequenceExpression(SyntaxKind kind)
        {
            return kind == SyntaxKind.DotToken ||
                    kind == SyntaxKind.OpenBracketToken;
        }

        internal ExpressionSyntax ParseConsequenceSyntax()
        {
            SyntaxKind tk = this.CurrentToken.Kind;
            ExpressionSyntax expr = null;
            switch (tk)
            {
                case SyntaxKind.DotToken:
                    expr = _syntaxFactory.MemberBindingExpression(this.EatToken(), this.ParseSimpleName(NameOptions.InExpression));
                    break;

                case SyntaxKind.OpenBracketToken:
                    expr = _syntaxFactory.ElementBindingExpression(this.ParseBracketedArgumentList());
                    break;
            }

            Debug.Assert(expr != null);

            while (true)
            {
                tk = this.CurrentToken.Kind;
                switch (tk)
                {
                    case SyntaxKind.OpenParenToken:
                        expr = _syntaxFactory.InvocationExpression(expr, this.ParseParenthesizedArgumentList());
                        break;

                    case SyntaxKind.OpenBracketToken:
                        expr = _syntaxFactory.ElementAccessExpression(expr, this.ParseBracketedArgumentList());
                        break;

                    case SyntaxKind.DotToken:
                        expr = _syntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expr, this.EatToken(), this.ParseSimpleName(NameOptions.InExpression));
                        break;

                    case SyntaxKind.QuestionToken:
                        if (CanStartConsequenceExpression(this.PeekToken(1).Kind))
                        {
                            var qToken = this.EatToken();
                            var consequence = ParseConsequenceSyntax();
                            expr = _syntaxFactory.ConditionalAccessExpression(expr, qToken, consequence);
                        }
                        return expr;

                    default:
                        return expr;
                }
            }
        }

        internal ArgumentListSyntax ParseParenthesizedArgumentList()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.ArgumentList)
            {
                return (ArgumentListSyntax)this.EatNode();
            }

            ParseArgumentList(
                openToken: out SyntaxToken openToken,
                arguments: out SeparatedSyntaxList<ArgumentSyntax> arguments,
                closeToken: out SyntaxToken closeToken,
                openKind: SyntaxKind.OpenParenToken,
                closeKind: SyntaxKind.CloseParenToken);
            return _syntaxFactory.ArgumentList(openToken, arguments, closeToken);
        }

        internal BracketedArgumentListSyntax ParseBracketedArgumentList()
        {
            if (this.IsIncrementalAndFactoryContextMatches && this.CurrentNodeKind == SyntaxKind.BracketedArgumentList)
            {
                return (BracketedArgumentListSyntax)this.EatNode();
            }

            ParseArgumentList(
                openToken: out SyntaxToken openToken,
                arguments: out SeparatedSyntaxList<ArgumentSyntax> arguments,
                closeToken: out SyntaxToken closeToken,
                openKind: SyntaxKind.OpenBracketToken,
                closeKind: SyntaxKind.CloseBracketToken);
            return _syntaxFactory.BracketedArgumentList(openToken, arguments, closeToken);
        }

        private void ParseArgumentList(
            out SyntaxToken openToken,
            out SeparatedSyntaxList<ArgumentSyntax> arguments,
            out SyntaxToken closeToken,
            SyntaxKind openKind,
            SyntaxKind closeKind)
        {
            Debug.Assert(openKind == SyntaxKind.OpenParenToken || openKind == SyntaxKind.OpenBracketToken);
            Debug.Assert(closeKind == SyntaxKind.CloseParenToken || closeKind == SyntaxKind.CloseBracketToken);
            Debug.Assert((openKind == SyntaxKind.OpenParenToken) == (closeKind == SyntaxKind.CloseParenToken));
            bool isIndexer = openKind == SyntaxKind.OpenBracketToken;

            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken ||
                this.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
            {
                // convert `[` into `(` or vice versa for error recovery
                openToken = this.EatTokenAsKind(openKind);
            }
            else
            {
                openToken = this.EatToken(openKind);
            }

            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfArgumentList;

            SeparatedSyntaxListBuilder<ArgumentSyntax> list = default(SeparatedSyntaxListBuilder<ArgumentSyntax>);
            try
            {
                if (this.CurrentToken.Kind != closeKind && this.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
tryAgain:
                    if (list.IsNull)
                    {
                        list = _pool.AllocateSeparated<ArgumentSyntax>();
                    }

                    if (this.IsPossibleArgumentExpression() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        // first argument
                        list.Add(this.ParseArgumentExpression(isIndexer));

                        // additional arguments
                        while (true)
                        {
                            if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken ||
                                this.CurrentToken.Kind == SyntaxKind.CloseBracketToken ||
                                this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                            {
                                break;
                            }
                            else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleArgumentExpression())
                            {
                                list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                                list.Add(this.ParseArgumentExpression(isIndexer));
                                continue;
                            }
                            else if (this.SkipBadArgumentListTokens(ref openToken, list, SyntaxKind.CommaToken, closeKind) == PostSkipAction.Abort)
                            {
                                break;
                            }
                        }
                    }
                    else if (this.SkipBadArgumentListTokens(ref openToken, list, SyntaxKind.IdentifierToken, closeKind) == PostSkipAction.Continue)
                    {
                        goto tryAgain;
                    }
                }
                else if (isIndexer && this.CurrentToken.Kind == closeKind)
                {
                    // An indexer always expects at least one value. And so we need to give an error
                    // for the case where we see only "[]". ParseArgumentExpression gives it.

                    if (list.IsNull)
                    {
                        list = _pool.AllocateSeparated<ArgumentSyntax>();
                    }

                    list.Add(this.ParseArgumentExpression(isIndexer));
                }

                _termState = saveTerm;

                if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken ||
                    this.CurrentToken.Kind == SyntaxKind.CloseBracketToken)
                {
                    // convert `]` into `)` or vice versa for error recovery
                    closeToken = this.EatTokenAsKind(closeKind);
                }
                else
                {
                    closeToken = this.EatToken(closeKind);
                }

                arguments = list.ToList();
            }
            finally
            {
                if (!list.IsNull)
                {
                    _pool.Free(list);
                }
            }
        }

        private PostSkipAction SkipBadArgumentListTokens(ref SyntaxToken open, SeparatedSyntaxListBuilder<ArgumentSyntax> list, SyntaxKind expected, SyntaxKind closeKind)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref open, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleArgumentExpression(),
                p => p.CurrentToken.Kind == closeKind || p.CurrentToken.Kind == SyntaxKind.SemicolonToken || p.IsTerminator(),
                expected);
        }

        private bool IsEndOfArgumentList()
        {
            return this.CurrentToken.Kind == SyntaxKind.CloseParenToken
                || this.CurrentToken.Kind == SyntaxKind.CloseBracketToken;
        }

        private bool IsPossibleArgumentExpression()
        {
            return IsValidArgumentRefKindKeyword(this.CurrentToken.Kind) || this.IsPossibleExpression();
        }

        private static bool IsValidArgumentRefKindKeyword(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private ArgumentSyntax ParseArgumentExpression(bool isIndexer)
        {
            NameColonSyntax nameColon = null;
            if (this.CurrentToken.Kind == SyntaxKind.IdentifierToken && this.PeekToken(1).Kind == SyntaxKind.ColonToken)
            {
                var name = this.ParseIdentifierName();
                var colon = this.EatToken(SyntaxKind.ColonToken);
                nameColon = _syntaxFactory.NameColon(name, colon);
                nameColon = CheckFeatureAvailability(nameColon, MessageID.IDS_FeatureNamedArgument);
            }

            SyntaxToken refKindKeyword = null;
            if (IsValidArgumentRefKindKeyword(this.CurrentToken.Kind))
            {
                refKindKeyword = this.EatToken();
            }

            ExpressionSyntax expression;

            if (isIndexer && (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.CurrentToken.Kind == SyntaxKind.CloseBracketToken))
            {
                expression = this.ParseIdentifierName(ErrorCode.ERR_ValueExpected);
            }
            else if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
            {
                expression = this.ParseIdentifierName(ErrorCode.ERR_MissingArgument);
            }
            else
            {
                if (refKindKeyword?.Kind == SyntaxKind.InKeyword)
                {
                    refKindKeyword = this.CheckFeatureAvailability(refKindKeyword, MessageID.IDS_FeatureReadOnlyReferences);
                }

                // According to Language Specification, section 7.6.7 Element access
                //      The argument-list of an element-access is not allowed to contain ref or out arguments.
                // However, we actually do support ref indexing of indexed properties in COM interop
                // scenarios, and when indexing an object of static type "dynamic". So we enforce
                // that the ref/out of the argument must match the parameter when binding the argument list.

                expression = (refKindKeyword?.Kind == SyntaxKind.OutKeyword)
                    ? ParseExpressionOrDeclaration(ParseTypeMode.Normal, feature: MessageID.IDS_FeatureOutVar, permitTupleDesignation: false)
                    : ParseSubExpression(Precedence.Expression);
            }

            return _syntaxFactory.Argument(nameColon, refKindKeyword, expression);
        }

        private TypeOfExpressionSyntax ParseTypeOfExpression()
        {
            var keyword = this.EatToken();
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);
            var type = this.ParseTypeOrVoid();
            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);

            return _syntaxFactory.TypeOfExpression(keyword, openParen, type, closeParen);
        }

        private ExpressionSyntax ParseDefaultExpression()
        {
            var keyword = this.EatToken();
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                var openParen = this.EatToken(SyntaxKind.OpenParenToken);
                var type = this.ParseType();
                var closeParen = this.EatToken(SyntaxKind.CloseParenToken);

                keyword = CheckFeatureAvailability(keyword, MessageID.IDS_FeatureDefault);
                return _syntaxFactory.DefaultExpression(keyword, openParen, type, closeParen);
            }
            else
            {
                keyword = CheckFeatureAvailability(keyword, MessageID.IDS_FeatureDefaultLiteral);
                return _syntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, keyword);
            }
        }

        private SizeOfExpressionSyntax ParseSizeOfExpression()
        {
            var keyword = this.EatToken();
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);
            var type = this.ParseType();
            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);

            return _syntaxFactory.SizeOfExpression(keyword, openParen, type, closeParen);
        }

        private MakeRefExpressionSyntax ParseMakeRefExpression()
        {
            var keyword = this.EatToken();
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);
            var expr = this.ParseSubExpression(Precedence.Expression);
            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);

            return _syntaxFactory.MakeRefExpression(keyword, openParen, expr, closeParen);
        }

        private RefTypeExpressionSyntax ParseRefTypeExpression()
        {
            var keyword = this.EatToken();
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);
            var expr = this.ParseSubExpression(Precedence.Expression);
            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);

            return _syntaxFactory.RefTypeExpression(keyword, openParen, expr, closeParen);
        }

        private CheckedExpressionSyntax ParseCheckedOrUncheckedExpression()
        {
            var checkedOrUnchecked = this.EatToken();
            Debug.Assert(checkedOrUnchecked.Kind == SyntaxKind.CheckedKeyword || checkedOrUnchecked.Kind == SyntaxKind.UncheckedKeyword);
            var kind = (checkedOrUnchecked.Kind == SyntaxKind.CheckedKeyword) ? SyntaxKind.CheckedExpression : SyntaxKind.UncheckedExpression;

            var openParen = this.EatToken(SyntaxKind.OpenParenToken);
            var expr = this.ParseSubExpression(Precedence.Expression);
            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);

            return _syntaxFactory.CheckedExpression(kind, checkedOrUnchecked, openParen, expr, closeParen);
        }

        private RefValueExpressionSyntax ParseRefValueExpression()
        {
            var @refvalue = this.EatToken(SyntaxKind.RefValueKeyword);
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);
            var expr = this.ParseSubExpression(Precedence.Expression);
            var comma = this.EatToken(SyntaxKind.CommaToken);
            var type = this.ParseType();
            var closeParen = this.EatToken(SyntaxKind.CloseParenToken);

            return _syntaxFactory.RefValueExpression(@refvalue, openParen, expr, comma, type, closeParen);
        }

        private bool ScanParenthesizedImplicitlyTypedLambda(Precedence precedence)
        {
            if (!(precedence <= Precedence.Lambda))
            {
                return false;
            }

            //  case 1:  ( x ,
            if (this.PeekToken(1).Kind == SyntaxKind.IdentifierToken
                && (!this.IsInQuery || !IsTokenQueryContextualKeyword(this.PeekToken(1)))
                && this.PeekToken(2).Kind == SyntaxKind.CommaToken)
            {
                // Make sure it really looks like a lambda, not just a tuple
                int curTk = 3;
                while (true)
                {
                    var tk = this.PeekToken(curTk++);

                    // skip  identifiers commas and predefined types in any combination for error recovery
                    if (tk.Kind != SyntaxKind.IdentifierToken
                        && !SyntaxFacts.IsPredefinedType(tk.Kind)
                        && tk.Kind != SyntaxKind.CommaToken
                        && (this.IsInQuery || !IsTokenQueryContextualKeyword(tk)))
                    {
                        break;
                    };
                }

                // ) =>
                return this.PeekToken(curTk - 1).Kind == SyntaxKind.CloseParenToken &&
                       this.PeekToken(curTk).Kind == SyntaxKind.EqualsGreaterThanToken;
            }

            //  case 2:  ( x ) =>
            if (IsTrueIdentifier(this.PeekToken(1))
                && this.PeekToken(2).Kind == SyntaxKind.CloseParenToken
                && this.PeekToken(3).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }

            //  case 3:  ( ) =>
            if (this.PeekToken(1).Kind == SyntaxKind.CloseParenToken
                && this.PeekToken(2).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }

            // case 4:  ( params
            // This case is interesting in that it is not legal; this error could be caught at parse time but we would rather
            // recover from the error and let the semantic analyzer deal with it.
            if (this.PeekToken(1).Kind == SyntaxKind.ParamsKeyword)
            {
                return true;
            }

            return false;
        }

        private bool ScanExplicitlyTypedLambda(Precedence precedence)
        {
            if (!(precedence <= Precedence.Lambda))
            {
                return false;
            }

            var resetPoint = this.GetResetPoint();
            try
            {
                bool foundParameterModifier = false;

                // do we have the following:
                //   case 1: ( T x , ... ) =>
                //   case 2: ( T x ) =>
                //   case 3: ( out T x,
                //   case 4: ( ref T x,
                //   case 5: ( out T x ) =>
                //   case 6: ( ref T x ) =>
                //   case 7: ( in T x ) =>
                //
                // if so then parse it as a lambda

                // Note: in the first two cases, we cannot distinguish a lambda from a tuple expression
                // containing declaration expressions, so we scan forwards to the `=>` so we know for sure.

                while (true)
                {
                    // Advance past the open paren or comma.
                    this.EatToken();

                    // Eat 'out' or 'ref' for cases [3, 6]. Even though not allowed in a lambda,
                    // we treat `params` similarly for better error recovery.
                    switch (this.CurrentToken.Kind)
                    {
                        case SyntaxKind.RefKeyword:
                            this.EatToken();
                            foundParameterModifier = true;
                            if (this.CurrentToken.Kind == SyntaxKind.ReadOnlyKeyword)
                            {
                                this.EatToken();
                            }
                            break;
                        case SyntaxKind.OutKeyword:
                        case SyntaxKind.InKeyword:
                        case SyntaxKind.ParamsKeyword:
                            this.EatToken();
                            foundParameterModifier = true;
                            break;
                    }

                    if (this.CurrentToken.Kind == SyntaxKind.EndOfFileToken)
                    {
                        return foundParameterModifier;
                    }

                    // NOTE: advances CurrentToken
                    if (this.ScanType() == ScanTypeFlags.NotType)
                    {
                        return false;
                    }

                    if (this.IsTrueIdentifier())
                    {
                        // eat the identifier
                        this.EatToken();
                    }

                    switch (this.CurrentToken.Kind)
                    {
                        case SyntaxKind.EndOfFileToken:
                            return foundParameterModifier;

                        case SyntaxKind.CommaToken:
                            if (foundParameterModifier)
                            {
                                return true;
                            }

                            continue;

                        case SyntaxKind.CloseParenToken:
                            return this.PeekToken(1).Kind == SyntaxKind.EqualsGreaterThanToken;

                        default:
                            return false;
                    }
                }
            }
            finally
            {
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);
            }
        }

        private ExpressionSyntax ParseParenExpressionOrLambdaOrTuple(Precedence precedence)
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.OpenParenToken);

            var resetPoint = this.GetResetPoint();
            try
            {
                if (ScanParenthesizedImplicitlyTypedLambda(precedence))
                {
                    return this.ParseLambdaExpression();
                }

                this.Reset(ref resetPoint);
                if (this.ScanExplicitlyTypedLambda(precedence))
                {
                    return this.ParseLambdaExpression();
                }

                // Doesn't look like a cast, so parse this as a parenthesized expression or tuple.
                {
                    this.Reset(ref resetPoint);
                    var openParen = this.EatToken(SyntaxKind.OpenParenToken);
                    var expression = this.ParseExpressionOrDeclaration(ParseTypeMode.FirstElementOfPossibleTupleLiteral, feature: 0, permitTupleDesignation: true);

                    //  ( <expr>,    must be a tuple
                    if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        var firstArg = _syntaxFactory.Argument(nameColon: null, refKindKeyword: default(SyntaxToken), expression: expression);
                        return ParseTupleExpressionTail(openParen, firstArg);
                    }

                    // ( name:
                    if (expression.Kind == SyntaxKind.IdentifierName && this.CurrentToken.Kind == SyntaxKind.ColonToken)
                    {
                        var nameColon = _syntaxFactory.NameColon((IdentifierNameSyntax)expression, EatToken());
                        expression = this.ParseExpressionOrDeclaration(ParseTypeMode.FirstElementOfPossibleTupleLiteral, feature: 0, permitTupleDesignation: true);

                        var firstArg = _syntaxFactory.Argument(nameColon, refKindKeyword: default(SyntaxToken), expression: expression);
                        return ParseTupleExpressionTail(openParen, firstArg);
                    }

                    var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                    return _syntaxFactory.ParenthesizedExpression(openParen, expression, closeParen);
                }
            }
            finally
            {
                this.Release(ref resetPoint);
            }
        }

        private TupleExpressionSyntax ParseTupleExpressionTail(SyntaxToken openParen, ArgumentSyntax firstArg)
        {
            var list = _pool.AllocateSeparated<ArgumentSyntax>();
            try
            {
                list.Add(firstArg);

                while (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    var comma = this.EatToken(SyntaxKind.CommaToken);
                    list.AddSeparator(comma);

                    ArgumentSyntax arg;

                    var expression = ParseExpressionOrDeclaration(ParseTypeMode.AfterTupleComma, feature: 0, permitTupleDesignation: true);
                    if (expression.Kind == SyntaxKind.IdentifierName && this.CurrentToken.Kind == SyntaxKind.ColonToken)
                    {
                        var nameColon = _syntaxFactory.NameColon((IdentifierNameSyntax)expression, EatToken());
                        expression = ParseExpressionOrDeclaration(ParseTypeMode.AfterTupleComma, feature: 0, permitTupleDesignation: true);
                        arg = _syntaxFactory.Argument(nameColon, refKindKeyword: default(SyntaxToken), expression: expression);
                    }
                    else
                    {
                        arg = _syntaxFactory.Argument(nameColon: null, refKindKeyword: default(SyntaxToken), expression: expression);
                    }

                    list.Add(arg);
                }

                if (list.Count < 2)
                {
                    list.AddSeparator(SyntaxFactory.MissingToken(SyntaxKind.CommaToken));
                    var missing = this.AddError(this.CreateMissingIdentifierName(), ErrorCode.ERR_TupleTooFewElements);
                    list.Add(_syntaxFactory.Argument(nameColon: null, refKindKeyword: default(SyntaxToken), expression: missing));
                }

                var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                var result = _syntaxFactory.TupleExpression(openParen, list, closeParen);

                result = CheckFeatureAvailability(result, MessageID.IDS_FeatureTuples);
                return result;
            }
            finally
            {
                _pool.Free(list);
            }
        }


        private bool ScanAsyncLambda(Precedence precedence)
        {
            // Adapted from CParser::ScanAsyncLambda

            // Precedence must not exceed that of lambdas
            if (precedence > Precedence.Lambda)
            {
                return false;
            }

            // Async lambda must start with 'async'
            if (this.CurrentToken.ContextualKind != SyntaxKind.AsyncKeyword)
            {
                return false;
            }

            // 'async <identifier> => ...' looks like an async simple lambda
            if (this.PeekToken(1).Kind == SyntaxKind.IdentifierToken && this.PeekToken(2).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }

            // Non-simple async lambda must be of the form 'async (...'
            if (this.PeekToken(1).Kind != SyntaxKind.OpenParenToken)
            {
                return false;
            }

            {
                var resetPoint = this.GetResetPoint();

                // Skip 'async'
                EatToken(SyntaxKind.IdentifierToken);

                // Check whether looks like implicitly or explicitly typed lambda
                bool isAsync = ScanParenthesizedImplicitlyTypedLambda(precedence) || ScanExplicitlyTypedLambda(precedence);

                // Restore current token index
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);

                return isAsync;
            }
        }

        private ExpressionSyntax ParseNewExpression()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.NewKeyword);

            if (this.IsAnonymousType())
            {
                return this.ParseAnonymousTypeExpression();
            }
            // TODO: reverse the way we parse implicit typed arrays
            else
            {
                // assume object creation as default case
                return this.ParseArrayOrObjectCreationExpression();
            }
        }

        private bool IsAnonymousType()
        {
            return this.CurrentToken.Kind == SyntaxKind.NewKeyword && this.PeekToken(1).Kind == SyntaxKind.OpenBraceToken;
        }

        private AnonymousObjectCreationExpressionSyntax ParseAnonymousTypeExpression()
        {
            Debug.Assert(IsAnonymousType());
            var @new = this.EatToken(SyntaxKind.NewKeyword);
            @new = CheckFeatureAvailability(@new, MessageID.IDS_FeatureAnonymousTypes);

            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.OpenBraceToken);

            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);
            var expressions = _pool.AllocateSeparated<AnonymousObjectMemberDeclaratorSyntax>();
            this.ParseAnonymousTypeMemberInitializers(ref openBrace, ref expressions);
            var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);
            var result = _syntaxFactory.AnonymousObjectCreationExpression(@new, openBrace, expressions, closeBrace);
            _pool.Free(expressions);

            return result;
        }

        private void ParseAnonymousTypeMemberInitializers(ref SyntaxToken openBrace, ref SeparatedSyntaxListBuilder<AnonymousObjectMemberDeclaratorSyntax> list)
        {
            if (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
            {
tryAgain:
                if (this.IsPossibleExpression() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    // first argument
                    list.Add(this.ParseAnonymousTypeMemberInitializer());

                    // additional arguments
                    while (true)
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            break;
                        }
                        else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleExpression())
                        {
                            list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));

                            // check for exit case after legal trailing comma
                            if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                break;
                            }
                            else if (!this.IsPossibleExpression())
                            {
                                goto tryAgain;
                            }

                            list.Add(this.ParseAnonymousTypeMemberInitializer());
                            continue;
                        }
                        else if (this.SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                else if (this.SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.IdentifierToken) == PostSkipAction.Continue)
                {
                    goto tryAgain;
                }
            }
        }

        private AnonymousObjectMemberDeclaratorSyntax ParseAnonymousTypeMemberInitializer()
        {
            var nameEquals = this.IsNamedAssignment()
                ? ParseNameEquals()
                : null;

            var expression = this.ParseExpressionCore();
            return _syntaxFactory.AnonymousObjectMemberDeclarator(nameEquals, expression);
        }

        private bool IsInitializerMember()
        {
            return this.IsComplexElementInitializer() ||
                this.IsNamedAssignment() ||
                this.IsDictionaryInitializer() ||
                this.IsPossibleExpression();
        }

        private bool IsComplexElementInitializer()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
        }

        private bool IsNamedAssignment()
        {
            return IsTrueIdentifier() && this.PeekToken(1).Kind == SyntaxKind.EqualsToken;
        }

        private bool IsDictionaryInitializer()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBracketToken;
        }

        private ExpressionSyntax ParseArrayOrObjectCreationExpression()
        {
            SyntaxToken @new = this.EatToken(SyntaxKind.NewKeyword);
            bool isPossibleArrayCreation = this.IsPossibleArrayCreationExpression();
            var type = this.ParseType(ParseTypeMode.Normal);

            if (type.Kind == SyntaxKind.ArrayType)
            {
                // Check for an initializer.
                InitializerExpressionSyntax initializer = null;
                if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
                {
                    initializer = this.ParseArrayInitializer();
                }
                else
                {
                    var rankSpec = ((ArrayTypeSyntax)type).RankSpecifiers[0];
                    if (GetNumberOfNonOmittedArraySizes(rankSpec) == 0)
                    {
                        type = this.AddError(type, rankSpec, ErrorCode.ERR_MissingArraySize);
                    }
                }

                return _syntaxFactory.ArrayCreationExpression(@new, (ArrayTypeSyntax)type, initializer);
            }
            else
            {
                ArgumentListSyntax argumentList = null;
                if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
                {
                    argumentList = this.ParseParenthesizedArgumentList();
                }

                InitializerExpressionSyntax initializer = null;
                if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
                {
                    initializer = this.ParseObjectOrCollectionInitializer();
                }

                // we need one or the other
                if (argumentList == null && initializer == null)
                {
                    argumentList = _syntaxFactory.ArgumentList(
                        this.EatToken(SyntaxKind.OpenParenToken, ErrorCode.ERR_BadNewExpr),
                        default(SeparatedSyntaxList<ArgumentSyntax>),
                        SyntaxFactory.MissingToken(SyntaxKind.CloseParenToken));
                }

                return _syntaxFactory.ObjectCreationExpression(@new, type, argumentList, initializer);
            }
        }

        private static int GetNumberOfNonOmittedArraySizes(ArrayRankSpecifierSyntax rankSpec)
        {
            return rankSpec.Sizes != null && rankSpec.Sizes.Kind != SyntaxKind.OmittedArraySizeExpression ? 1 : 0;
        }

        private bool IsPossibleArrayCreationExpression()
        {
            return this.CurrentToken.Kind == SyntaxKind.OpenBracketToken;
        }

        private InitializerExpressionSyntax ParseObjectOrCollectionInitializer()
        {
            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);

            var initializers = _pool.AllocateSeparated<ExpressionSyntax>();
            try
            {
                bool isObjectInitializer;
                this.ParseObjectOrCollectionInitializerMembers(ref openBrace, initializers, out isObjectInitializer);
                Debug.Assert(initializers.Count > 0 || isObjectInitializer);

                openBrace = CheckFeatureAvailability(openBrace, isObjectInitializer ? MessageID.IDS_FeatureObjectInitializer : MessageID.IDS_FeatureCollectionInitializer);

                var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);
                return _syntaxFactory.InitializerExpression(
                    isObjectInitializer ?
                        SyntaxKind.ObjectInitializerExpression :
                        SyntaxKind.CollectionInitializerExpression,
                    openBrace,
                    initializers,
                    closeBrace);
            }
            finally
            {
                _pool.Free(initializers);
            }
        }

        private void ParseObjectOrCollectionInitializerMembers(ref SyntaxToken startToken, SeparatedSyntaxListBuilder<ExpressionSyntax> list, out bool isObjectInitializer)
        {
            // Empty initializer list must be parsed as an object initializer.
            isObjectInitializer = true;

            if (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
            {
tryAgain:
                if (this.IsInitializerMember() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    // We have at least one initializer expression.
                    // If at least one initializer expression is a named assignment, this is an object initializer.
                    // Otherwise, this is a collection initializer.
                    isObjectInitializer = false;

                    // first argument
                    list.Add(this.ParseObjectOrCollectionInitializerMember(ref isObjectInitializer));

                    // additional arguments
                    while (true)
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            break;
                        }
                        else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsInitializerMember())
                        {
                            list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));

                            // check for exit case after legal trailing comma
                            if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                break;
                            }

                            list.Add(this.ParseObjectOrCollectionInitializerMember(ref isObjectInitializer));
                            continue;
                        }
                        else if (this.SkipBadInitializerListTokens(ref startToken, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                else if (this.SkipBadInitializerListTokens(ref startToken, list, SyntaxKind.IdentifierToken) == PostSkipAction.Continue)
                {
                    goto tryAgain;
                }
            }

            // We may have invalid initializer elements. These will be reported during binding.
        }

        private ExpressionSyntax ParseObjectOrCollectionInitializerMember(ref bool isObjectInitializer)
        {
            if (this.IsComplexElementInitializer())
            {
                return this.ParseComplexElementInitializer();
            }
            else if (IsDictionaryInitializer())
            {
                isObjectInitializer = true;
                var initializer = this.ParseDictionaryInitializer();
                initializer = CheckFeatureAvailability(initializer, MessageID.IDS_FeatureDictionaryInitializer);
                return initializer;
            }
            else if (this.IsNamedAssignment())
            {
                isObjectInitializer = true;
                return this.ParseObjectInitializerNamedAssignment();
            }
            else
            {
                return this.ParseExpressionCore();
            }
        }

        private PostSkipAction SkipBadInitializerListTokens<T>(ref SyntaxToken startToken, SeparatedSyntaxListBuilder<T> list, SyntaxKind expected)
            where T : CSharpSyntaxNode
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleExpression(),
                p => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(),
                expected);
        }

        private ExpressionSyntax ParseObjectInitializerNamedAssignment()
        {
            var identifier = this.ParseIdentifierName();
            var equal = this.EatToken(SyntaxKind.EqualsToken);
            ExpressionSyntax expression;
            if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                expression = this.ParseObjectOrCollectionInitializer();
            }
            else
            {
                expression = this.ParseExpressionCore();
            }

            return _syntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, identifier, equal, expression);
        }

        private ExpressionSyntax ParseDictionaryInitializer()
        {
            var arguments = this.ParseBracketedArgumentList();
            var equal = this.EatToken(SyntaxKind.EqualsToken);
            var expression = this.CurrentToken.Kind == SyntaxKind.OpenBraceToken
                ? this.ParseObjectOrCollectionInitializer()
                : this.ParseExpressionCore();

            var elementAccess = _syntaxFactory.ImplicitElementAccess(arguments);
            return _syntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression, elementAccess, equal, expression);
        }

        private InitializerExpressionSyntax ParseComplexElementInitializer()
        {
            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);
            var initializers = _pool.AllocateSeparated<ExpressionSyntax>();
            try
            {
                DiagnosticInfo closeBraceError;
                this.ParseExpressionsForComplexElementInitializer(ref openBrace, initializers, out closeBraceError);
                var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);
                if (closeBraceError != null)
                {
                    closeBrace = WithAdditionalDiagnostics(closeBrace, closeBraceError);
                }
                return _syntaxFactory.InitializerExpression(SyntaxKind.ComplexElementInitializerExpression, openBrace, initializers, closeBrace);
            }
            finally
            {
                _pool.Free(initializers);
            }
        }

        private void ParseExpressionsForComplexElementInitializer(ref SyntaxToken openBrace, SeparatedSyntaxListBuilder<ExpressionSyntax> list, out DiagnosticInfo closeBraceError)
        {
            closeBraceError = null;

            if (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
            {
tryAgain:
                if (this.IsPossibleExpression() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    // first argument
                    list.Add(this.ParseExpressionCore());

                    // additional arguments
                    while (true)
                    {
                        if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            break;
                        }
                        else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleExpression())
                        {
                            list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                            if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                closeBraceError = MakeError(this.CurrentToken, ErrorCode.ERR_ExpressionExpected);
                                break;
                            }
                            list.Add(this.ParseExpressionCore());
                            continue;
                        }
                        else if (this.SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                else if (this.SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.IdentifierToken) == PostSkipAction.Continue)
                {
                    goto tryAgain;
                }
            }
        }

        private bool IsImplicitlyTypedArray()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.NewKeyword || this.CurrentToken.Kind == SyntaxKind.StackAllocKeyword);
            return this.PeekToken(1).Kind == SyntaxKind.OpenBracketToken;
        }

        private ImplicitArrayCreationExpressionSyntax ParseImplicitlyTypedArrayCreation()
        {
            var @new = this.EatToken(SyntaxKind.NewKeyword);
            @new = CheckFeatureAvailability(@new, MessageID.IDS_FeatureImplicitArray);
            var openBracket = this.EatToken(SyntaxKind.OpenBracketToken);

            var commas = _pool.Allocate();
            try
            {
                int lastTokenPosition = -1;
                while (IsMakingProgress(ref lastTokenPosition))
                {
                    if (this.IsPossibleExpression())
                    {
                        var size = this.AddError(this.ParseExpressionCore(), ErrorCode.ERR_InvalidArray);
                        if (commas.Count == 0)
                        {
                            openBracket = AddTrailingSkippedSyntax(openBracket, size);
                        }
                        else
                        {
                            AddTrailingSkippedSyntax(commas, size);
                        }
                    }

                    if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        commas.Add(this.EatToken());
                        continue;
                    }

                    break;
                }

                var closeBracket = this.EatToken(SyntaxKind.CloseBracketToken);
                var initializer = this.ParseArrayInitializer();

                return _syntaxFactory.ImplicitArrayCreationExpression(@new, openBracket, commas.ToList(), closeBracket, initializer);
            }
            finally
            {
                _pool.Free(commas);
            }
        }

        private InitializerExpressionSyntax ParseArrayInitializer()
        {
            var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);

            // NOTE:  This loop allows " { <initexpr>, } " but not " { , } "
            var list = _pool.AllocateSeparated<ExpressionSyntax>();
            try
            {
                if (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
tryAgain:
                    if (this.IsPossibleVariableInitializer() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        list.Add(this.ParseVariableInitializer());

                        while (true)
                        {
                            if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                break;
                            }
                            else if (this.IsPossibleVariableInitializer() || this.CurrentToken.Kind == SyntaxKind.CommaToken)
                            {
                                list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));

                                // check for exit case after legal trailing comma
                                if (this.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                                {
                                    break;
                                }
                                else if (!this.IsPossibleVariableInitializer())
                                {
                                    goto tryAgain;
                                }

                                list.Add(this.ParseVariableInitializer());
                                continue;
                            }
                            else if (SkipBadArrayInitializerTokens(ref openBrace, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                            {
                                break;
                            }
                        }
                    }
                    else if (SkipBadArrayInitializerTokens(ref openBrace, list, SyntaxKind.CommaToken) == PostSkipAction.Continue)
                    {
                        goto tryAgain;
                    }
                }

                var closeBrace = this.EatToken(SyntaxKind.CloseBraceToken);

                return _syntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, openBrace, list, closeBrace);
            }
            finally
            {
                _pool.Free(list);
            }
        }

        private PostSkipAction SkipBadArrayInitializerTokens(ref SyntaxToken openBrace, SeparatedSyntaxListBuilder<ExpressionSyntax> list, SyntaxKind expected)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref openBrace, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleVariableInitializer(),
                p => this.CurrentToken.Kind == SyntaxKind.CloseBraceToken || this.IsTerminator(),
                expected);
        }

        private ExpressionSyntax ParseStackAllocExpression()
        {
            if (this.IsImplicitlyTypedArray())
            {
                return ParseImplicitlyTypedStackAllocExpression();
            }
            else
            {
                return ParseRegularStackAllocExpression();
            }
        }

        private ExpressionSyntax ParseImplicitlyTypedStackAllocExpression()
        {
            var @stackalloc = this.EatToken(SyntaxKind.StackAllocKeyword);
            @stackalloc = CheckFeatureAvailability(@stackalloc, MessageID.IDS_FeatureStackAllocInitializer);
            var openBracket = this.EatToken(SyntaxKind.OpenBracketToken);

            int lastTokenPosition = -1;
            while (IsMakingProgress(ref lastTokenPosition))
            {
                if (this.IsPossibleExpression())
                {
                    var size = this.AddError(this.ParseExpressionCore(), ErrorCode.ERR_InvalidStackAllocArray);
                    openBracket = AddTrailingSkippedSyntax(openBracket, size);
                }

                if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    var comma = this.AddError(this.EatToken(), ErrorCode.ERR_InvalidStackAllocArray);
                    openBracket = AddTrailingSkippedSyntax(openBracket, comma);
                    continue;
                }

                break;
            }

            var closeBracket = this.EatToken(SyntaxKind.CloseBracketToken);
            var initializer = this.ParseArrayInitializer();
            return _syntaxFactory.ImplicitStackAllocArrayCreationExpression(@stackalloc, openBracket, closeBracket, initializer);
        }

        private ExpressionSyntax ParseRegularStackAllocExpression()
        {
            throw new NotImplementedException("stackalloc not implemented");
            //var @stackalloc = this.EatToken(SyntaxKind.StackAllocKeyword);
            //var elementType = this.ParseType(expectSizes: true);
            //InitializerExpressionSyntax initializer = null;
            //if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            //{
            //    @stackalloc = CheckFeatureAvailability(@stackalloc, MessageID.IDS_FeatureStackAllocInitializer);
            //    initializer = this.ParseArrayInitializer();
            //}
            //else if (elementType.Kind == SyntaxKind.ArrayType)
            //{
            //    var rankSpec = ((ArrayTypeSyntax)elementType).RankSpecifiers[0];
            //    if (GetNumberOfNonOmittedArraySizes(rankSpec) == 0)
            //    {
            //        elementType = this.AddError(elementType, rankSpec, ErrorCode.ERR_MissingArraySize);
            //    }
            //}

            //return _syntaxFactory.StackAllocArrayCreationExpression(@stackalloc, elementType, initializer);
        }

        private AnonymousMethodExpressionSyntax ParseAnonymousMethodExpression()
        {
            bool parentScopeIsInAsync = IsInAsync;
            IsInAsync = false;
            SyntaxToken asyncToken = null;
            if (this.CurrentToken.ContextualKind == SyntaxKind.AsyncKeyword)
            {
                asyncToken = this.EatContextualToken(SyntaxKind.AsyncKeyword);
                asyncToken = CheckFeatureAvailability(asyncToken, MessageID.IDS_FeatureAsync);
                IsInAsync = true;
            }

            var @delegate = this.EatToken(SyntaxKind.DelegateKeyword);
            @delegate = CheckFeatureAvailability(@delegate, MessageID.IDS_FeatureAnonDelegates);

            ParameterListSyntax parameterList = null;
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                parameterList = this.ParseParenthesizedParameterList();
            }

            // In mismatched braces cases (missing a }) it is possible for delegate declarations to be
            // parsed as delegate statement expressions.  When this situation occurs all subsequent 
            // delegate declarations will also be parsed as delegate statement expressions.  In a file with
            // a sufficient number of delegates, common in generated code, it will put considerable 
            // stack pressure on the parser.  
            //
            // To help avoid this problem we don't recursively descend into a delegate expression unless 
            // { } are actually present.  This keeps the stack pressure lower in bad code scenarios.
            if (this.CurrentToken.Kind != SyntaxKind.OpenBraceToken)
            {
                // There's a special error code for a missing token after an accessor keyword
                var openBrace = this.EatToken(SyntaxKind.OpenBraceToken);
                return _syntaxFactory.AnonymousMethodExpression(
                    asyncToken,
                    @delegate,
                    parameterList,
                    _syntaxFactory.Block(
                        openBrace,
                        default(SyntaxList<StatementSyntax>),
                        SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken)));
            }

            var body = this.ParseBlock();
            IsInAsync = parentScopeIsInAsync;
            return _syntaxFactory.AnonymousMethodExpression(asyncToken, @delegate, parameterList, body);
        }

        private LambdaExpressionSyntax ParseLambdaExpression()
        {
            bool parentScopeIsInAsync = IsInAsync;
            SyntaxToken asyncToken = null;
            if (this.CurrentToken.ContextualKind == SyntaxKind.AsyncKeyword &&
                PeekToken(1).Kind != SyntaxKind.EqualsGreaterThanToken)
            {
                asyncToken = this.EatContextualToken(SyntaxKind.AsyncKeyword);
                asyncToken = CheckFeatureAvailability(asyncToken, MessageID.IDS_FeatureAsync);
                IsInAsync = true;
            }

            var result = ParseLambdaExpression(asyncToken);

            IsInAsync = parentScopeIsInAsync;
            return result;
        }

        private LambdaExpressionSyntax ParseLambdaExpression(SyntaxToken asyncToken)
        {
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                var paramList = this.ParseLambdaParameterList();
                var arrow = this.EatToken(SyntaxKind.EqualsGreaterThanToken);
                arrow = CheckFeatureAvailability(arrow, MessageID.IDS_FeatureLambda);
                var body = ParseLambdaBody();

                return _syntaxFactory.ParenthesizedLambdaExpression(asyncToken, paramList, arrow, body);
            }
            else
            {
                var name = this.ParseIdentifierToken();
                var arrow = this.EatToken(SyntaxKind.EqualsGreaterThanToken);
                arrow = CheckFeatureAvailability(arrow, MessageID.IDS_FeatureLambda);

                var parameter = _syntaxFactory.Parameter(
                    default(SyntaxList<AttributeSyntax>), 
                    name, null, null, null);
                var body = ParseLambdaBody();

                return _syntaxFactory.SimpleLambdaExpression(asyncToken, parameter, arrow, body);
            }
        }

        private CSharpSyntaxNode ParseLambdaBody()
        {
            if (this.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                return this.ParseBlock();
            }
            else
            {
                return this.ParsePossibleRefExpression();
            }
        }

        private ParameterListSyntax ParseLambdaParameterList()
        {
            var openParen = this.EatToken(SyntaxKind.OpenParenToken);
            var saveTerm = _termState;
            _termState |= TerminatorState.IsEndOfParameterList;

            var nodes = _pool.AllocateSeparated<ParameterSyntax>();
            try
            {
                if (this.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                {
tryAgain:
                    if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleLambdaParameter())
                    {
                        // first parameter
                        var parameter = this.ParseLambdaParameter();
                        nodes.Add(parameter);

                        // additional parameters
                        int tokenProgress = -1;
                        while (IsMakingProgress(ref tokenProgress))
                        {
                            if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken)
                            {
                                break;
                            }
                            else if (this.CurrentToken.Kind == SyntaxKind.CommaToken || this.IsPossibleLambdaParameter())
                            {
                                nodes.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                                parameter = this.ParseLambdaParameter();
                                nodes.Add(parameter);
                                continue;
                            }
                            else if (this.SkipBadLambdaParameterListTokens(ref openParen, nodes, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken) == PostSkipAction.Abort)
                            {
                                break;
                            }
                        }
                    }
                    else if (this.SkipBadLambdaParameterListTokens(ref openParen, nodes, SyntaxKind.IdentifierToken, SyntaxKind.CloseParenToken) == PostSkipAction.Continue)
                    {
                        goto tryAgain;
                    }
                }

                _termState = saveTerm;
                var closeParen = this.EatToken(SyntaxKind.CloseParenToken);

                return _syntaxFactory.ParameterList(openParen, nodes, closeParen);
            }
            finally
            {
                _pool.Free(nodes);
            }
        }

        private bool IsPossibleLambdaParameter()
        {
            switch (this.CurrentToken.Kind)
            {
                case SyntaxKind.ParamsKeyword:
                // params is not actually legal in a lambda, but we allow it for error
                // recovery purposes and then give an error during semantic analysis.
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.OpenParenToken:   // tuple
                    return true;

                case SyntaxKind.IdentifierToken:
                    return this.IsTrueIdentifier();

                default:
                    return IsPredefinedType(this.CurrentToken.Kind);
            }
        }

        private PostSkipAction SkipBadLambdaParameterListTokens(ref SyntaxToken openParen, SeparatedSyntaxListBuilder<ParameterSyntax> list, SyntaxKind expected, SyntaxKind closeKind)
        {
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref openParen, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleLambdaParameter(),
                p => p.CurrentToken.Kind == closeKind || p.IsTerminator(),
                expected);
        }

        private ParameterSyntax ParseLambdaParameter()
        {
            // Params are actually illegal in a lambda, but we'll allow it for error recovery purposes and
            // give the "params unexpected" error at semantic analysis time.
            bool hasModifier = IsParameterModifier(this.CurrentToken.Kind);

            SyntaxToken paramName = this.ParseIdentifierToken();

            var colonToken = this.ExpectColon();

            TypeSyntax paramType = null;

            if (ShouldParseLambdaParameterType(hasModifier))
            {
                paramType = ParseType(ParseTypeMode.Parameter);
            }
            var parameter = _syntaxFactory.Parameter(default(SyntaxList<AttributeSyntax>), paramName, colonToken, paramType, null);
            return parameter;
        }

        private bool ShouldParseLambdaParameterType(bool hasModifier)
        {
            // If we have "ref/out/in/params" always try to parse out a type.
            if (hasModifier)
            {
                return true;
            }

            // If we have "int/string/etc." always parse out a type.
            if (IsPredefinedType(this.CurrentToken.Kind))
            {
                return true;
            }

            // if we have a tuple type in a lambda.
            if (this.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                return true;
            }

            if (this.IsTrueIdentifier(this.CurrentToken))
            {
                // Don't parse out a type if we see:
                //
                //      (a,
                //      (a)
                //      (a =>
                //      (a {
                //
                // In all other cases, parse out a type.
                var peek1 = this.PeekToken(1);
                if (peek1.Kind != SyntaxKind.CommaToken &&
                    peek1.Kind != SyntaxKind.CloseParenToken &&
                    peek1.Kind != SyntaxKind.EqualsGreaterThanToken &&
                    peek1.Kind != SyntaxKind.OpenBraceToken)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsCurrentTokenQueryContextualKeyword
        {
            get
            {
                return IsTokenQueryContextualKeyword(this.CurrentToken);
            }
        }

        private static bool IsTokenQueryContextualKeyword(SyntaxToken token)
        {
            if (IsTokenStartOfNewQueryClause(token))
            {
                return true;
            }

            switch (token.ContextualKind)
            {
                case SyntaxKind.OnKeyword:
                case SyntaxKind.EqualsKeyword:
                case SyntaxKind.AscendingKeyword:
                case SyntaxKind.DescendingKeyword:
                case SyntaxKind.ByKeyword:
                    return true;
            }

            return false;
        }

        private static bool IsTokenStartOfNewQueryClause(SyntaxToken token)
        {
            switch (token.ContextualKind)
            {
                case SyntaxKind.FromKeyword:
                case SyntaxKind.JoinKeyword:
                case SyntaxKind.IntoKeyword:
                case SyntaxKind.WhereKeyword:
                case SyntaxKind.OrderByKeyword:
                case SyntaxKind.GroupKeyword:
                case SyntaxKind.SelectKeyword:
                case SyntaxKind.LetKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsQueryExpression(bool mayBeVariableDeclaration, bool mayBeMemberDeclaration)
        {
            if (this.CurrentToken.ContextualKind == SyntaxKind.FromKeyword)
            {
                return this.IsQueryExpressionAfterFrom(mayBeVariableDeclaration, mayBeMemberDeclaration);
            }

            return false;
        }

        // from_clause ::= from <type>? <identifier> in expression
        private bool IsQueryExpressionAfterFrom(bool mayBeVariableDeclaration, bool mayBeMemberDeclaration)
        {
            // from x ...
            var pk1 = this.PeekToken(1).Kind;
            if (IsPredefinedType(pk1))
            {
                return true;
            }

            if (pk1 == SyntaxKind.IdentifierToken)
            {
                var pk2 = this.PeekToken(2).Kind;
                if (pk2 == SyntaxKind.InKeyword)
                {
                    return true;
                }

                if (mayBeVariableDeclaration)
                {
                    if (pk2 == SyntaxKind.SemicolonToken ||    // from x;
                        pk2 == SyntaxKind.CommaToken ||        // from x, y;
                        pk2 == SyntaxKind.EqualsToken)         // from x = null;
                    {
                        return false;
                    }
                }

                if (mayBeMemberDeclaration)
                {
                    // from idf { ...   property decl
                    // from idf(...     method decl
                    if (pk2 == SyntaxKind.OpenParenToken ||
                        pk2 == SyntaxKind.OpenBraceToken)
                    {
                        return false;
                    }

                    // otherwise we need to scan a type
                }
                else
                {
                    return true;
                }
            }

            // from T x ...
            var resetPoint = this.GetResetPoint();
            try
            {
                this.EatToken();

                ScanTypeFlags isType = this.ScanType();
                if (isType != ScanTypeFlags.NotType && (this.CurrentToken.Kind == SyntaxKind.IdentifierToken || this.CurrentToken.Kind == SyntaxKind.InKeyword))
                {
                    return true;
                }
            }
            finally
            {
                this.Reset(ref resetPoint);
                this.Release(ref resetPoint);
            }

            return false;
        }

        private QueryExpressionSyntax ParseQueryExpression(Precedence precedence)
        {
            this.EnterQuery();
            var fc = this.ParseFromClause();
            fc = CheckFeatureAvailability(fc, MessageID.IDS_FeatureQueryExpression);
            if (precedence > Precedence.Assignment && IsStrict)
            {
                fc = this.AddError(fc, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(SyntaxKind.FromKeyword));
            }

            var body = this.ParseQueryBody();
            this.LeaveQuery();
            return _syntaxFactory.QueryExpression(fc, body);
        }

        private QueryBodySyntax ParseQueryBody()
        {
            var clauses = _pool.Allocate<QueryClauseSyntax>();
            try
            {
                SelectOrGroupClauseSyntax selectOrGroupBy = null;
                QueryContinuationSyntax continuation = null;

                // from, join, let, where and orderby
                while (true)
                {
                    switch (this.CurrentToken.ContextualKind)
                    {
                        case SyntaxKind.FromKeyword:
                            var fc = this.ParseFromClause();
                            clauses.Add(fc);
                            continue;
                        case SyntaxKind.JoinKeyword:
                            clauses.Add(this.ParseJoinClause());
                            continue;
                        case SyntaxKind.LetKeyword:
                            clauses.Add(this.ParseLetClause());
                            continue;
                        case SyntaxKind.WhereKeyword:
                            clauses.Add(this.ParseWhereClause());
                            continue;
                        case SyntaxKind.OrderByKeyword:
                            clauses.Add(this.ParseOrderByClause());
                            continue;
                    }

                    break;
                }

                // select or group clause
                switch (this.CurrentToken.ContextualKind)
                {
                    case SyntaxKind.SelectKeyword:
                        selectOrGroupBy = this.ParseSelectClause();
                        break;
                    case SyntaxKind.GroupKeyword:
                        selectOrGroupBy = this.ParseGroupClause();
                        break;
                    default:
                        selectOrGroupBy = _syntaxFactory.SelectClause(
                            this.EatToken(SyntaxKind.SelectKeyword, ErrorCode.ERR_ExpectedSelectOrGroup),
                            this.CreateMissingIdentifierName());
                        break;
                }

                // optional query continuation clause
                if (this.CurrentToken.ContextualKind == SyntaxKind.IntoKeyword)
                {
                    continuation = this.ParseQueryContinuation();
                }

                return _syntaxFactory.QueryBody(clauses, selectOrGroupBy, continuation);
            }
            finally
            {
                _pool.Free(clauses);
            }
        }

        private FromClauseSyntax ParseFromClause()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.FromKeyword);
            var @from = this.EatContextualToken(SyntaxKind.FromKeyword);
            @from = CheckFeatureAvailability(@from, MessageID.IDS_FeatureQueryExpression);

            TypeSyntax type = null;
            if (this.PeekToken(1).Kind != SyntaxKind.InKeyword)
            {
                type = this.ParseType();
            }

            SyntaxToken name;
            if (this.PeekToken(1).ContextualKind == SyntaxKind.InKeyword &&
                (this.CurrentToken.Kind != SyntaxKind.IdentifierToken || SyntaxFacts.IsQueryContextualKeyword(this.CurrentToken.ContextualKind)))
            {
                //if this token is a something other than an identifier (someone accidentally used a contextual
                //keyword or a literal, for example), but we can see that the "in" is in the right place, then
                //just replace whatever is here with a missing identifier
                name = this.EatToken();
                name = WithAdditionalDiagnostics(name, this.GetExpectedTokenError(SyntaxKind.IdentifierToken, name.ContextualKind, name.GetLeadingTriviaWidth(), name.Width));
                name = this.ConvertToMissingWithTrailingTrivia(name, SyntaxKind.IdentifierToken);
            }
            else
            {
                name = this.ParseIdentifierToken();
            }
            var @in = this.EatToken(SyntaxKind.InKeyword);
            var expression = this.ParseExpressionCore();
            return _syntaxFactory.FromClause(@from, type, name, @in, expression);
        }

        private JoinClauseSyntax ParseJoinClause()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.JoinKeyword);
            var @join = this.EatContextualToken(SyntaxKind.JoinKeyword);
            TypeSyntax type = null;
            if (this.PeekToken(1).Kind != SyntaxKind.InKeyword)
            {
                type = this.ParseType();
            }

            var name = this.ParseIdentifierToken();
            var @in = this.EatToken(SyntaxKind.InKeyword);
            var inExpression = this.ParseExpressionCore();
            var @on = this.EatContextualToken(SyntaxKind.OnKeyword, ErrorCode.ERR_ExpectedContextualKeywordOn);
            var leftExpression = this.ParseExpressionCore();
            var @equals = this.EatContextualToken(SyntaxKind.EqualsKeyword, ErrorCode.ERR_ExpectedContextualKeywordEquals);
            var rightExpression = this.ParseExpressionCore();
            JoinIntoClauseSyntax joinInto = null;
            if (this.CurrentToken.ContextualKind == SyntaxKind.IntoKeyword)
            {
                var @into = ConvertToKeyword(this.EatToken());
                var intoId = this.ParseIdentifierToken();
                joinInto = _syntaxFactory.JoinIntoClause(@into, intoId);
            }

            return _syntaxFactory.JoinClause(@join, type, name, @in, inExpression, @on, leftExpression, @equals, rightExpression, joinInto);
        }

        private LetClauseSyntax ParseLetClause()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.LetKeyword);
            var @let = this.EatContextualToken(SyntaxKind.LetKeyword);
            var name = this.ParseIdentifierToken();
            var equal = this.EatToken(SyntaxKind.EqualsToken);
            var expression = this.ParseExpressionCore();
            return _syntaxFactory.LetClause(@let, name, equal, expression);
        }

        private WhereClauseSyntax ParseWhereClause()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword);
            var @where = this.EatContextualToken(SyntaxKind.WhereKeyword);
            var condition = this.ParseExpressionCore();
            return _syntaxFactory.WhereClause(@where, condition);
        }

        private OrderByClauseSyntax ParseOrderByClause()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.OrderByKeyword);
            var @orderby = this.EatContextualToken(SyntaxKind.OrderByKeyword);

            var list = _pool.AllocateSeparated<OrderingSyntax>();
            try
            {
                // first argument
                list.Add(this.ParseOrdering());

                // additional arguments
                while (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    if (this.CurrentToken.Kind == SyntaxKind.CloseParenToken || this.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                    {
                        break;
                    }
                    else if (this.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        list.AddSeparator(this.EatToken(SyntaxKind.CommaToken));
                        list.Add(this.ParseOrdering());
                        continue;
                    }
                    else if (this.SkipBadOrderingListTokens(list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }

                return _syntaxFactory.OrderByClause(@orderby, list);
            }
            finally
            {
                _pool.Free(list);
            }
        }

        private PostSkipAction SkipBadOrderingListTokens(SeparatedSyntaxListBuilder<OrderingSyntax> list, SyntaxKind expected)
        {
            CSharpSyntaxNode tmp = null;
            Debug.Assert(list.Count > 0);
            return this.SkipBadSeparatedListTokensWithExpectedKind(ref tmp, list,
                p => p.CurrentToken.Kind != SyntaxKind.CommaToken,
                p => p.CurrentToken.Kind == SyntaxKind.CloseParenToken
                    || p.CurrentToken.Kind == SyntaxKind.SemicolonToken
                    || p.IsCurrentTokenQueryContextualKeyword
                    || p.IsTerminator(),
                expected);
        }

        private OrderingSyntax ParseOrdering()
        {
            var expression = this.ParseExpressionCore();
            SyntaxToken direction = null;
            SyntaxKind kind = SyntaxKind.AscendingOrdering;

            if (this.CurrentToken.ContextualKind == SyntaxKind.AscendingKeyword ||
                this.CurrentToken.ContextualKind == SyntaxKind.DescendingKeyword)
            {
                direction = ConvertToKeyword(this.EatToken());
                if (direction.Kind == SyntaxKind.DescendingKeyword)
                {
                    kind = SyntaxKind.DescendingOrdering;
                }
            }

            return _syntaxFactory.Ordering(kind, expression, direction);
        }

        private SelectClauseSyntax ParseSelectClause()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.SelectKeyword);
            var @select = this.EatContextualToken(SyntaxKind.SelectKeyword);
            var expression = this.ParseExpressionCore();
            return _syntaxFactory.SelectClause(@select, expression);
        }

        private GroupClauseSyntax ParseGroupClause()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.GroupKeyword);
            var @group = this.EatContextualToken(SyntaxKind.GroupKeyword);
            var groupExpression = this.ParseExpressionCore();
            var @by = this.EatContextualToken(SyntaxKind.ByKeyword, ErrorCode.ERR_ExpectedContextualKeywordBy);
            var byExpression = this.ParseExpressionCore();
            return _syntaxFactory.GroupClause(@group, groupExpression, @by, byExpression);
        }

        private QueryContinuationSyntax ParseQueryContinuation()
        {
            Debug.Assert(this.CurrentToken.ContextualKind == SyntaxKind.IntoKeyword);
            var @into = this.EatContextualToken(SyntaxKind.IntoKeyword);
            var name = this.ParseIdentifierToken();
            var body = this.ParseQueryBody();
            return _syntaxFactory.QueryContinuation(@into, name, body);
        }

        private bool IsStrict => this.Options.Features.ContainsKey("strict");

        [Obsolete("Use IsIncrementalAndFactoryContextMatches")]
        private new bool IsIncremental
        {
            get { throw new Exception("Use IsIncrementalAndFactoryContextMatches"); }
        }

        private bool IsIncrementalAndFactoryContextMatches
        {
            get
            {
                if (!base.IsIncremental)
                {
                    return false;
                }

                Stark.CSharpSyntaxNode current = this.CurrentNode;
                return current != null && MatchesFactoryContext(current.Green, _syntaxFactoryContext);
            }
        }

        internal static bool MatchesFactoryContext(GreenNode green, SyntaxFactoryContext context)
        {
            return context.IsInAsync == green.ParsedInAsync &&
                context.IsInQuery == green.ParsedInQuery;
        }

        private bool IsInAsync
        {
            get
            {
                return _syntaxFactoryContext.IsInAsync;
            }
            set
            {
                _syntaxFactoryContext.IsInAsync = value;
            }
        }

        private bool IsInQuery
        {
            get { return _syntaxFactoryContext.IsInQuery; }
        }

        private void EnterQuery()
        {
            _syntaxFactoryContext.QueryDepth++;
        }

        private void LeaveQuery()
        {
            Debug.Assert(_syntaxFactoryContext.QueryDepth > 0);
            _syntaxFactoryContext.QueryDepth--;
        }

        private new ResetPoint GetResetPoint()
        {
            return new ResetPoint(
                base.GetResetPoint(),
                _termState,
                _isInTry,
                _syntaxFactoryContext.IsInAsync,
                _syntaxFactoryContext.QueryDepth);
        }

        private void Reset(ref ResetPoint state)
        {
            _termState = state.TerminatorState;
            _isInTry = state.IsInTry;
            _syntaxFactoryContext.IsInAsync = state.IsInAsync;
            _syntaxFactoryContext.QueryDepth = state.QueryDepth;
            base.Reset(ref state.BaseResetPoint);
        }

        private void Release(ref ResetPoint state)
        {
            base.Release(ref state.BaseResetPoint);
        }

        private new struct ResetPoint
        {
            internal SyntaxParser.ResetPoint BaseResetPoint;
            internal readonly TerminatorState TerminatorState;
            internal readonly bool IsInTry;
            internal readonly bool IsInAsync;
            internal readonly int QueryDepth;

            internal ResetPoint(
                SyntaxParser.ResetPoint resetPoint,
                TerminatorState terminatorState,
                bool isInTry,
                bool isInAsync,
                int queryDepth)
            {
                this.BaseResetPoint = resetPoint;
                this.TerminatorState = terminatorState;
                this.IsInTry = isInTry;
                this.IsInAsync = isInAsync;
                this.QueryDepth = queryDepth;
            }
        }

        internal TNode ConsumeUnexpectedTokens<TNode>(TNode node) where TNode : CSharpSyntaxNode
        {
            if (this.CurrentToken.Kind == SyntaxKind.EndOfFileToken) return node;
            SyntaxListBuilder<SyntaxToken> b = _pool.Allocate<SyntaxToken>();
            while (this.CurrentToken.Kind != SyntaxKind.EndOfFileToken)
            {
                b.Add(this.EatToken());
            }

            var trailingTrash = b.ToList();
            _pool.Free(b);

            node = this.AddError(node, ErrorCode.ERR_UnexpectedToken, trailingTrash[0].ToString());
            node = this.AddTrailingSkippedSyntax(node, trailingTrash.Node);
            return node;
        }
    }
}
