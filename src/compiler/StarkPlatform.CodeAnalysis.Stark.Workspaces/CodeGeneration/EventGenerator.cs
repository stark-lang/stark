// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using StarkPlatform.CodeAnalysis.CodeGeneration;
using StarkPlatform.CodeAnalysis.Stark.Extensions;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using Roslyn.Utilities;

using static StarkPlatform.CodeAnalysis.CodeGeneration.CodeGenerationHelpers;
using static StarkPlatform.CodeAnalysis.Stark.CodeGeneration.CSharpCodeGenerationHelpers;

namespace StarkPlatform.CodeAnalysis.Stark.CodeGeneration
{
    internal static class EventGenerator
    {
        private static MemberDeclarationSyntax AfterMember(
            SyntaxList<MemberDeclarationSyntax> members,
            MemberDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration.Kind() == SyntaxKind.EventFieldDeclaration)
            {
                // Field style events go after the last field event, or after the last field.
                var lastEvent = members.LastOrDefault(m => m is EventFieldDeclarationSyntax);

                return lastEvent ?? LastField(members);
            }

            if (eventDeclaration.Kind() == SyntaxKind.EventDeclaration)
            {
                // Property style events go after existing events, then after existing constructors.
                var lastEvent = members.LastOrDefault(m => m is EventDeclarationSyntax);

                return lastEvent ?? LastConstructor(members);
            }

            return null;
        }

        private static MemberDeclarationSyntax BeforeMember(
            SyntaxList<MemberDeclarationSyntax> members,
            MemberDeclarationSyntax eventDeclaration)
        {
            // If it's a field style event, then it goes before everything else if we don't have any
            // existing fields/events.
            if (eventDeclaration.Kind() == SyntaxKind.FieldDeclaration)
            {
                return members.FirstOrDefault();
            }

            // Otherwise just place it before the methods.
            return FirstMethod(members);
        }

        private static MemberDeclarationSyntax GenerateEventDeclarationWorker(
            IEventSymbol @event, CodeGenerationDestination destination, CodeGenerationOptions options)
        {
            var explicitInterfaceSpecifier = GenerateExplicitInterfaceSpecifier(@event.ExplicitInterfaceImplementations);

            return AddFormatterAndCodeGeneratorAnnotationsTo(SyntaxFactory.EventDeclaration(
                attributeLists: AttributeGenerator.GenerateAttributeLists(@event.GetAttributes(), options),
                modifiers: GenerateModifiers(@event, destination, options),
                type: @event.Type.GenerateTypeSyntax(),
                explicitInterfaceSpecifier: explicitInterfaceSpecifier,
                identifier: @event.Name.ToIdentifierToken(),
                default,
                accessorList: GenerateAccessorList(@event, destination, options)));
        }

        private static AccessorListSyntax GenerateAccessorList(
            IEventSymbol @event, CodeGenerationDestination destination, CodeGenerationOptions options)
        {
            var accessors = new List<AccessorDeclarationSyntax>
            {
                GenerateAccessorDeclaration(@event, @event.AddMethod, SyntaxKind.AddAccessorDeclaration, destination, options),
                GenerateAccessorDeclaration(@event, @event.RemoveMethod, SyntaxKind.RemoveAccessorDeclaration, destination, options),
            };

            return SyntaxFactory.AccessorList(accessors.WhereNotNull().ToSyntaxList());
        }

        private static AccessorDeclarationSyntax GenerateAccessorDeclaration(
            IEventSymbol @event,
            IMethodSymbol accessor,
            SyntaxKind kind,
            CodeGenerationDestination destination,
            CodeGenerationOptions options)
        {
            var hasBody = options.GenerateMethodBodies && HasAccessorBodies(@event, destination, accessor);
            return accessor == null
                ? null
                : GenerateAccessorDeclaration(accessor, kind, hasBody);
        }

        private static AccessorDeclarationSyntax GenerateAccessorDeclaration(
            IMethodSymbol accessor,
            SyntaxKind kind,
            bool hasBody)
        {
            return AddAnnotationsTo(accessor, SyntaxFactory.AccessorDeclaration(kind)
                                .WithBody(hasBody ? GenerateBlock(accessor) : null)
                                .WithEosToken(hasBody ? default : SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
        }

        private static BlockSyntax GenerateBlock(IMethodSymbol accessor)
        {
            return SyntaxFactory.Block(
                StatementGenerator.GenerateStatements(CodeGenerationMethodInfo.GetStatements(accessor)));
        }

        private static bool HasAccessorBodies(
            IEventSymbol @event,
            CodeGenerationDestination destination,
            IMethodSymbol accessor)
        {
            return destination != CodeGenerationDestination.InterfaceType &&
                !@event.IsAbstract &&
                accessor != null &&
                !accessor.IsAbstract;
        }

        private static SyntaxTokenList GenerateModifiers(
            IEventSymbol @event, CodeGenerationDestination destination, CodeGenerationOptions options)
        {
            var tokens = new List<SyntaxToken>();

            // Most modifiers not allowed if we're an explicit impl.
            if (!@event.ExplicitInterfaceImplementations.Any())
            {
                if (destination != CodeGenerationDestination.InterfaceType)
                {
                    AddAccessibilityModifiers(@event.DeclaredAccessibility, tokens, options, Accessibility.Private);

                    if (@event.IsStatic)
                    {
                        tokens.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                    }

                    if (@event.IsAbstract)
                    {
                        tokens.Add(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));
                    }

                    if (@event.IsOverride)
                    {
                        tokens.Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
                    }
                }
            }

            if (CodeGenerationEventInfo.GetIsUnsafe(@event))
            {
                tokens.Add(SyntaxFactory.Token(SyntaxKind.UnsafeKeyword));
            }

            return tokens.ToSyntaxTokenList();
        }
    }
}
