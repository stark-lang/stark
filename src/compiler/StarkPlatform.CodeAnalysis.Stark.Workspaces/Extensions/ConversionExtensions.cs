// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Shared.Extensions;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Extensions
{
    internal static class ConversionExtensions
    {
        public static bool IsIdentityOrImplicitReference(this Conversion conversion)
        {
            return conversion.IsIdentity ||
                (conversion.IsImplicit && conversion.IsReference);
        }

        public static bool IsImplicitUserDefinedConversion(this Conversion conversion)
        {
            return conversion.IsUserDefined &&
                conversion.MethodSymbol != null &&
                conversion.MethodSymbol.MethodKind == MethodKind.Conversion &&
                conversion.MethodSymbol.Name == "op_Implicit";
        }
    }
}
