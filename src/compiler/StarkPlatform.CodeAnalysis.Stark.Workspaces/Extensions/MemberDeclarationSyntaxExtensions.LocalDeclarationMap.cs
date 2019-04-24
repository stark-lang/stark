// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using StarkPlatform.CodeAnalysis;
using StarkPlatform.CodeAnalysis.Stark;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal partial class MemberDeclarationSyntaxExtensions
    {
        public struct LocalDeclarationMap
        {
            private readonly Dictionary<string, ImmutableArray<SyntaxToken>> _dictionary;

            internal LocalDeclarationMap(Dictionary<string, ImmutableArray<SyntaxToken>> dictionary)
            {
                _dictionary = dictionary;
            }

            public ImmutableArray<SyntaxToken> this[string identifier]
            {
                get
                {
                    return _dictionary.TryGetValue(identifier, out var result)
                        ? result
                        : ImmutableArray.Create<SyntaxToken>();
                }
            }
        }
    }
}
