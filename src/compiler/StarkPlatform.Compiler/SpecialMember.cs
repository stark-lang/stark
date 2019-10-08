// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace StarkPlatform.Compiler
{
    // members of special types
    internal enum SpecialMember
    {
        System_String__CtorSZArrayChar,

        System_String__ConcatStringString,
        System_String__ConcatStringStringString,
        System_String__ConcatStringStringStringString,
        System_String__ConcatStringArray,

        System_String__ConcatObject,
        System_String__ConcatObjectObject,
        System_String__ConcatObjectObjectObject,
        System_String__ConcatObjectArray,

        System_String__op_Equality,
        System_String__op_Inequality,
        System_String__Length,
        System_String__Chars,
        System_String__Format,

        System_Double__IsNaN,
        System_Single__IsNaN,

        System_Delegate__Combine,
        System_Delegate__Remove,
        System_Delegate__op_Equality,
        System_Delegate__op_Inequality,

        System_DateTime__MinValue,
        System_DateTime__CtorInt64,
        System_DateTime__CompareDateTimeDateTime,

        System_DateTime__op_Equality,
        System_DateTime__op_Inequality,
        System_DateTime__op_GreaterThan,
        System_DateTime__op_GreaterThanOrEqual,
        System_DateTime__op_LessThan,
        System_DateTime__op_LessThanOrEqual,

        core_Iterable_T_TIterator__iterate_begin,
        core_Iterable_T_TIterator__iterate_has_next,
        core_Iterable_T_TIterator__iterate_next,
        core_Iterable_T_TIterator__iterate_end,

        core_MutableIterable_T_TIterator__iterate_item,

        System_IDisposable__Dispose,

        System_Array__Length,
        System_Array__LongLength,
        System_Array__GetLowerBound,
        System_Array__GetUpperBound,

        System_Object__GetHashCode,
        System_Object__Equals,
        System_Object__ToString,
        System_Object__ReferenceEquals,

        System_IntPtr__op_Explicit_ToPointer,
        System_IntPtr__op_Explicit_ToInt32,
        System_IntPtr__op_Explicit_ToInt64,
        System_IntPtr__op_Explicit_FromPointer,
        System_IntPtr__op_Explicit_FromInt32,
        System_IntPtr__op_Explicit_FromInt64,
        System_UIntPtr__op_Explicit_ToPointer,
        System_UIntPtr__op_Explicit_ToUInt32,
        System_UIntPtr__op_Explicit_ToUInt64,
        System_UIntPtr__op_Explicit_FromPointer,
        System_UIntPtr__op_Explicit_FromUInt32,
        System_UIntPtr__op_Explicit_FromUInt64,

        System_Nullable_T_GetValueOrDefault,
        System_Nullable_T_get_Value,
        System_Nullable_T_get_HasValue,
        System_Nullable_T__ctor,
        System_Nullable_T__op_Implicit_FromT,
        System_Nullable_T__op_Explicit_ToT,

        Count
    }
}
