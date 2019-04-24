// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal static partial class TypeParameterSymbolExtensions
    {
        public static bool DependsOn(this TypeParameterSymbol typeParameter1, TypeParameterSymbol typeParameter2)
        {
            Debug.Assert((object)typeParameter1 != null);
            Debug.Assert((object)typeParameter2 != null);

            Func<TypeParameterSymbol, IEnumerable<TypeParameterSymbol>> dependencies = x => x.ConstraintTypesNoUseSiteDiagnostics.Select(c => c.TypeSymbol).OfType<TypeParameterSymbol>();
            return dependencies.TransitiveClosure(typeParameter1).Contains(typeParameter2);
        }
    }
}
