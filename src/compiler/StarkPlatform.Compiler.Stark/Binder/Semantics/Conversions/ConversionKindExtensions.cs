﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using StarkPlatform.Compiler.Stark.Symbols;
using StarkPlatform.Compiler.Stark.Syntax;
using StarkPlatform.Compiler.Text;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    internal static class ConversionKindExtensions
    {
        // Is the particular conversion an implicit conversion?
        public static bool IsImplicitConversion(this ConversionKind conversionKind)
        {
            switch (conversionKind)
            {
                case ConversionKind.NoConversion:
                    return false;

                case ConversionKind.Identity:
                case ConversionKind.ImplicitNumeric:
                case ConversionKind.ImplicitTupleLiteral:
                case ConversionKind.ImplicitTuple:
                case ConversionKind.ImplicitEnumeration:
                case ConversionKind.ImplicitThrow:
                case ConversionKind.ImplicitNullable:
                case ConversionKind.DefaultOrNullLiteral:
                case ConversionKind.ImplicitReference:
                case ConversionKind.ImplicitConstant:
                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.AnonymousFunction:
                case ConversionKind.MethodGroup:
                case ConversionKind.PointerToVoid:
                case ConversionKind.NullToPointer:
                case ConversionKind.InterpolatedString:
                case ConversionKind.Deconstruction:
                case ConversionKind.StackAllocToPointerType:
                case ConversionKind.StackAllocToSpanType:
                    return true;

                case ConversionKind.ExplicitNumeric:
                case ConversionKind.ExplicitTuple:
                case ConversionKind.ExplicitTupleLiteral:
                case ConversionKind.ExplicitEnumeration:
                case ConversionKind.ExplicitNullable:
                case ConversionKind.ExplicitReference:
                case ConversionKind.Unboxing:
                case ConversionKind.ExplicitUserDefined:
                case ConversionKind.PointerToPointer:
                case ConversionKind.PointerToInteger:
                case ConversionKind.IntegerToPointer:
                case ConversionKind.IntPtr:
                    return false;

                default:
                    throw ExceptionUtilities.UnexpectedValue(conversionKind);
            }
        }

        // Is the particular conversion a used-defined conversion?
        public static bool IsUserDefinedConversion(this ConversionKind conversionKind)
        {
            switch (conversionKind)
            {
                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.ExplicitUserDefined:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsPointerConversion(this ConversionKind kind)
        {
            switch (kind)
            {
                case ConversionKind.PointerToVoid:
                case ConversionKind.PointerToPointer:
                case ConversionKind.PointerToInteger:
                case ConversionKind.IntegerToPointer:
                case ConversionKind.NullToPointer:
                    return true;
                default:
                    return false;
            }
        }
    }
}
