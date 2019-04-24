// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace StarkPlatform.CodeAnalysis.Operations
{
    /// <summary>
    /// Represents a declarator that declares multiple individual variables.
    /// </summary>
    /// <para>
    /// Current Usage:
    ///   (1) C# VariableDeclaration
    ///   (2) C# fixed declarations
    ///   (3) C# using declarations
    ///   (4) VB Dim statement declaration groups
    ///   (5) VB Using statement variable declarations
    /// </para>
    /// <remarks>
    /// The initializer of this node is applied to all individual declarations in <see cref="Declarators"/>. There cannot
    /// be initializers in both locations except in invalid code scenarios.
    /// In C#, this node will never have an initializer.
    /// This corresponds to the VariableDeclarationSyntax in C#, and the VariableDeclarationSyntax in Visual Basic.
    ///
    /// This interface is reserved for implementation by its associated APIs. We reserve the right to
    /// change it in the future.
    /// </remarks>
    public interface IVariableDeclarationOperation : IOperation
    {
        /// <summary>
        /// Symbol declared by this variable declaration
        /// </summary>
        ILocalSymbol Symbol { get; }

        /// <summary>
        /// Optional initializer of the variable.
        /// </summary>
        /// <remarks>
        /// In C#, this will always be null.
        /// </remarks>
        IVariableInitializerOperation Initializer { get; }


        /// <summary>
        /// Additional arguments supplied to the declarator in error cases, ignored by the compiler. This only used for the C# case of
        /// DeclaredArgumentSyntax nodes on a VariableDeclarationSyntax.
        /// </summary>
        ImmutableArray<IOperation> IgnoredArguments { get; }
    }
}
