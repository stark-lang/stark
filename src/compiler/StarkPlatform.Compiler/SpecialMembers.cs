// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.Compiler.RuntimeMembers;
using StarkPlatform.Reflection.Metadata;

namespace StarkPlatform.Compiler
{
    internal static class SpecialMembers
    {
        private readonly static ImmutableArray<MemberDescriptor> s_descriptors;

        static SpecialMembers()
        {
            byte[] initializationBytes = new byte[]
            {
                // System_String__CtorSZArrayChar
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Rune,

                // System_String__ConcatStringString
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_String__ConcatStringStringString
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    3,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_String__ConcatStringStringStringString
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    4,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_String__ConcatStringArray
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_String__ConcatObject
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_String__ConcatObjectObject
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_String__ConcatObjectObjectObject
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    3,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_String__ConcatObjectArray
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_String__op_Equality
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_String__op_Inequality
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_String__Length
                (byte)MemberFlags.PropertyGet,                                                                              // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_String__Chars
                (byte)MemberFlags.PropertyGet,                                                                              // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Rune,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_String__Format
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_Double__IsNaN
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Float64,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Single__IsNaN
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Float32,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_Delegate__Combine
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Delegate,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,

                // System_Delegate__Remove
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Delegate,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,

                // System_Delegate__op_Equality
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Delegate,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,

                // System_Delegate__op_Inequality
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Delegate,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,

                // System_DateTime__MinValue
                (byte)(MemberFlags.Field | MemberFlags.Static),                                                             // Flags
                (byte)SpecialType.System_DateTime,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,                                  // Field Signature

                // System_DateTime__CtorInt64
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)SpecialType.System_DateTime,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int64,

                // System_DateTime__CompareDateTimeDateTime
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_DateTime,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,

                // System_DateTime__op_Equality
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_DateTime,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,

                // System_DateTime__op_Inequality
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_DateTime,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,

                // System_DateTime__op_GreaterThan
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_DateTime,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,

                // System_DateTime__op_GreaterThanOrEqual
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_DateTime,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,

                // System_DateTime__op_LessThan
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_DateTime,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,

                // System_DateTime__op_LessThanOrEqual
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_DateTime,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_DateTime,

                // core_Iterable_T_TIterator__iterate_begin
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.core_Iterable_T_TIterator,                                                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,

                // core_Iterable_T_TIterator__iterate_has_current
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.core_Iterable_T_TIterator,                                                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.ByReference,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,

                // core_Iterable_T_TIterator__iterate_current
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.core_Iterable_T_TIterator,                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.ByReference,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,

                // core_Iterable_T_TIterator__iterate_next
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.core_Iterable_T_TIterator,                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.ByReference,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,

                // core_Iterable_T_TIterator__iterate_end
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.core_Iterable_T_TIterator,                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.ByReference,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,

                // core_MutableIterable_T_TIterator__iterate_current
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.core_MutableIterable_T_TIterator,                                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.ByReference,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.ByReference,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,

                // System_IDisposable__Dispose
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.System_IDisposable,                                                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,

                // core_Array__size
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)SpecialType.core_Array,                                                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,

                // core_Array_T__item
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)SpecialType.core_Array_T,                                                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.ByReference,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,

                // core_Index__value
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)SpecialType.core_Index,                                                                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,
                    
                // System_Object__GetHashCode
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.System_Object,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_Object__Equals
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.System_Object,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_Object__ToString
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)SpecialType.System_Object,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Object__ReferenceEquals
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Object,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_IntPtr__op_Explicit_ToPointer
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Int,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.Pointer, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,

                // System_IntPtr__op_Explicit_ToInt32
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Int,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,

                // System_IntPtr__op_Explicit_ToInt64
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Int,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int64,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,

                // System_IntPtr__op_Explicit_FromPointer
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Int,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,
                    (byte)SignatureTypeCode.Pointer, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,

                // System_IntPtr__op_Explicit_FromInt32
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Int,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_IntPtr__op_Explicit_FromInt64
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Int,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int64,

                // System_UIntPtr__op_Explicit_ToPointer
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_UInt,                                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.Pointer, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt,

                // System_UIntPtr__op_Explicit_ToUInt32
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_UInt,                                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt32,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt,

                // System_UIntPtr__op_Explicit_ToUInt64
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_UInt,                                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt64,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt,

                // System_UIntPtr__op_Explicit_FromPointer
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_UInt,                                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt,
                    (byte)SignatureTypeCode.Pointer, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,

                // System_UIntPtr__op_Explicit_FromUInt32
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_UInt,                                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt32,

                // System_UIntPtr__op_Explicit_FromUInt64
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_UInt,                                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt64,

                // core_Option_T_GetValueOrDefault
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)SpecialType.core_Option_T,                                                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // core_Option_T_get_value
                (byte)MemberFlags.PropertyGet,                                                                              // Flags
                (byte)SpecialType.core_Option_T,                                                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // core_Option_T_get_has_value
                (byte)MemberFlags.PropertyGet,                                                                              // Flags
                (byte)SpecialType.core_Option_T,                                                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,

                // core_Option_T__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)SpecialType.core_Option_T,                                                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // core_Option_T__op_Implicit_FromT
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.core_Option_T,                                                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.core_Option_T,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // core_Option_T__op_Explicit_ToT
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.core_Option_T,                                                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.core_Option_T,
            };

            string[] allNames = new string[(int)SpecialMember.Count]
            {
                ".ctor",                                    // System_String__CtorSZArrayChar
                "Concat",                                   // System_String__ConcatStringString
                "Concat",                                   // System_String__ConcatStringStringString
                "Concat",                                   // System_String__ConcatStringStringStringString
                "Concat",                                   // System_String__ConcatStringArray
                "Concat",                                   // System_String__ConcatObject
                "Concat",                                   // System_String__ConcatObjectObject
                "Concat",                                   // System_String__ConcatObjectObjectObject
                "Concat",                                   // System_String__ConcatObjectArray
                "op_Equality",                              // System_String__op_Equality
                "op_Inequality",                            // System_String__op_Inequality
                "get_Length",                               // System_String__Length
                "get_Chars",                                // System_String__Chars
                "Format",                                   // System_String__Format
                "IsNaN",                                    // System_Double__IsNaN
                "IsNaN",                                    // System_Single__IsNaN
                "Combine",                                  // System_Delegate__Combine
                "Remove",                                   // System_Delegate__Remove
                "op_Equality",                              // System_Delegate__op_Equality
                "op_Inequality",                            // System_Delegate__op_Inequality
                "MinValue",                                 // System_DateTime__MinValue
                ".ctor",                                    // System_DateTime__CtorInt64
                "Compare",                                  // System_DateTime__CompareDateTimeDateTime
                "op_Equality",                              // System_DateTime__op_Equality
                "op_Inequality",                            // System_DateTime__op_Inequality
                "op_GreaterThan",                           // System_DateTime__op_GreaterThan
                "op_GreaterThanOrEqual",                    // System_DateTime__op_GreaterThanOrEqual
                "op_LessThan",                              // System_DateTime__op_LessThan
                "op_LessThanOrEqual",                       // System_DateTime__op_LessThanOrEqual
                "iterate_begin",                            // core_Iterable_T_TIterator__iterate_begin
                "iterate_has_current",                      // core_Iterable_T_TIterator__iterate_has_current
                "iterate_current",                          // core_Iterable_T_TIterator__iterate_current
                "iterate_next",                             // core_Iterable_T_TIterator__iterate_next
                "iterate_end",                              // core_Iterable_T_TIterator__iterate_end
                "iterate_current",                          // core_MutableIterable_T_TIterator__iterate_current
                "Dispose",                                  // System_IDisposable__Dispose
                "size",                                     // core_Array__size
                "this[]",                                   // core_Array_T__item
                "value",                                    // core_Index__value
                "GetHashCode",                              // System_Object__GetHashCode
                "Equals",                                   // System_Object__Equals
                "ToString",                                 // System_Object__ToString
                "ReferenceEquals",                          // System_Object__ReferenceEquals
                "op_Explicit",                              // System_IntPtr__op_Explicit_ToPointer
                "op_Explicit",                              // System_IntPtr__op_Explicit_ToInt32
                "op_Explicit",                              // System_IntPtr__op_Explicit_ToInt64
                "op_Explicit",                              // System_IntPtr__op_Explicit_FromPointer
                "op_Explicit",                              // System_IntPtr__op_Explicit_FromInt32
                "op_Explicit",                              // System_IntPtr__op_Explicit_FromInt64
                "op_Explicit",                              // System_UIntPtr__op_Explicit_ToPointer
                "op_Explicit",                              // System_UIntPtr__op_Explicit_ToUInt32
                "op_Explicit",                              // System_UIntPtr__op_Explicit_ToUInt64
                "op_Explicit",                              // System_UIntPtr__op_Explicit_FromPointer
                "op_Explicit",                              // System_UIntPtr__op_Explicit_FromUInt32
                "op_Explicit",                              // System_UIntPtr__op_Explicit_FromUInt64
                "GetValueOrDefault",                        // core_Option_T_GetValueOrDefault
                "get_value",                                // core_Option_T_get_value
                "get_has_value",                            // core_Option_T_get_has_value
                ".ctor",                                    // core_Option_T__ctor
                "op_Implicit",                              // core_Option_T__op_Implicit_FromT
                "op_Explicit",                              // core_Option_T__op_Explicit_ToT
            };

            s_descriptors = MemberDescriptor.InitializeFromStream(new System.IO.MemoryStream(initializationBytes, writable: false), allNames);
        }

        public static MemberDescriptor GetDescriptor(SpecialMember member)
        {
            return s_descriptors[(int)member];
        }
    }
}
