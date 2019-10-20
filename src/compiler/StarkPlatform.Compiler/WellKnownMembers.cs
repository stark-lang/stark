// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using StarkPlatform.Compiler.RuntimeMembers;
using StarkPlatform.Reflection.Metadata;

namespace StarkPlatform.Compiler
{
    internal static class WellKnownMembers
    {
        private readonly static ImmutableArray<MemberDescriptor> s_descriptors;

        static WellKnownMembers()
        {
            byte[] initializationBytes = new byte[]
            {
                // System_Math__RoundDouble
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Math,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Math__PowDoubleDouble
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Math,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Array__Empty
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Array,                                                                           // DeclaringTypeId
                1,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.GenericMethodParameter, 0, // Return Type

                // System_Array__Copy
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Array,                                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    5,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.core_Array_T,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.core_Array_T,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_Convert__ToBooleanInt32
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_Convert__ToBooleanUInt32
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt32,

                // System_Convert__ToBooleanInt64
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int64,

                // System_Convert__ToBooleanUInt64
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt64,

                // System_Convert__ToBooleanSingle
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_Convert__ToBooleanDouble
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Convert__ToSByteDouble
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int8, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Convert__ToSByteSingle
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int8, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_Convert__ToByteDouble
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt8, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Convert__ToByteSingle
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt8, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_Convert__ToInt16Double
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Convert__ToInt16Single
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_Convert__ToUInt16Double
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt16, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Convert__ToUInt16Single
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt16, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_Convert__ToInt32Double
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Convert__ToInt32Single
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_Convert__ToUInt32Double
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt32, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Convert__ToUInt32Single
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt32, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_Convert__ToInt64Double
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int64, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Convert__ToInt64Single
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int64, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_Convert__ToUInt64Double
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt64, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Convert__ToUInt64Single
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Convert,                                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt64, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float32,

                // System_CLSCompliantAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_CLSCompliantAttribute,                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,

                // System_FlagsAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_FlagsAttribute,                                                                  // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Guid__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Guid,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Type__GetTypeFromCLSID
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Type,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Guid,

                // System_Type__GetTypeFromHandle
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Type,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_RuntimeTypeHandle,

                // System_Type__Missing
                (byte)(MemberFlags.Field | MemberFlags.Static),                                                             // Flags
                (byte)WellKnownType.core_Type,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,                                    // Field Signature

                // System_Reflection_AssemblyKeyFileAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Reflection_AssemblyKeyFileAttribute,                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Reflection_AssemblyKeyNameAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Reflection_AssemblyKeyNameAttribute,                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Reflection_MethodBase__GetMethodFromHandle
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Reflection_MethodBase,                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Reflection_MethodBase, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_RuntimeMethodHandle,

                // System_Reflection_MethodBase__GetMethodFromHandle2
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Reflection_MethodBase,                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Reflection_MethodBase, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_RuntimeMethodHandle,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_RuntimeTypeHandle,

                // System_Reflection_MethodInfo__CreateDelegate
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)WellKnownType.core_Reflection_MethodInfo,                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_Delegate__CreateDelegate
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Delegate,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    3,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Reflection_MethodInfo,

                // System_Delegate__CreateDelegate4
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_Delegate,                                                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    4,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Reflection_MethodInfo,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,

                // System_Reflection_FieldInfo__GetFieldFromHandle
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Reflection_FieldInfo,                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Reflection_FieldInfo, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_RuntimeFieldHandle,

                // System_Reflection_FieldInfo__GetFieldFromHandle2
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Reflection_FieldInfo,                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Reflection_FieldInfo, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_RuntimeFieldHandle,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_RuntimeTypeHandle,

                // System_Reflection_Missing__Value
                (byte)(MemberFlags.Field | MemberFlags.Static),                                                             // Flags
                (byte)WellKnownType.core_Reflection_Missing,                                                              // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Reflection_Missing,                      // Field Signature

                // System_IEquatable_T__Equals
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)WellKnownType.core_IEquatable_T,                                                                    // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // System_Collections_Generic_EqualityComparer_T__Equals
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)WellKnownType.core_Collections_Generic_EqualityComparer_T,                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // System_Collections_Generic_EqualityComparer_T__GetHashCode
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)WellKnownType.core_Collections_Generic_EqualityComparer_T,                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // System_Collections_Generic_EqualityComparer_T__get_Default
                (byte)(MemberFlags.PropertyGet | MemberFlags.Static),                                                       // Flags
                (byte)WellKnownType.core_Collections_Generic_EqualityComparer_T,                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Collections_Generic_EqualityComparer_T,// Return Type

                // System_AttributeUsageAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_AttributeUsageAttribute,                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, 0,

                // System_AttributeUsageAttribute__AllowMultiple
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)WellKnownType.core_AttributeUsageAttribute,                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type

                // System_AttributeUsageAttribute__Inherited
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)WellKnownType.core_AttributeUsageAttribute,                                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type

                // System_ParamArrayAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_ParamArrayAttribute,                                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_STAThreadAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_STAThreadAttribute,                                                              // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Reflection_DefaultMemberAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Reflection_DefaultMemberAttribute,                                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Diagnostics_Debugger__Break
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Diagnostics_Debugger,                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Diagnostics_DebuggerDisplayAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggerDisplayAttribute,                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Diagnostics_DebuggerDisplayAttribute__Type
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggerDisplayAttribute,                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String, // Return Type

                // System_Diagnostics_DebuggerNonUserCodeAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggerNonUserCodeAttribute,                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Diagnostics_DebuggerHiddenAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggerHiddenAttribute,                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Diagnostics_DebuggerBrowsableAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggerBrowsableAttribute,                                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Diagnostics_DebuggerBrowsableState,

                // System_Diagnostics_DebuggerStepThroughAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggerStepThroughAttribute,                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Diagnostics_DebuggableAttribute__ctorDebuggingModes
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggableAttribute,                                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Diagnostics_DebuggableAttribute__DebuggingModes,

                // System_Diagnostics_DebuggableAttribute_DebuggingModes__Default
                (byte)(MemberFlags.Field | MemberFlags.Static),                                                             // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggableAttribute__DebuggingModes,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Diagnostics_DebuggableAttribute__DebuggingModes, // Field Signature

                // System_Diagnostics_DebuggableAttribute_DebuggingModes__DisableOptimizations
                (byte)(MemberFlags.Field | MemberFlags.Static),                                                             // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggableAttribute__DebuggingModes,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Diagnostics_DebuggableAttribute__DebuggingModes, // Field Signature

                // System_Diagnostics_DebuggableAttribute_DebuggingModes__EnableEditAndContinue
                (byte)(MemberFlags.Field | MemberFlags.Static),                                                             // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggableAttribute__DebuggingModes,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Diagnostics_DebuggableAttribute__DebuggingModes, // Field Signature

                // System_Diagnostics_DebuggableAttribute_DebuggingModes__IgnoreSymbolStoreSequencePoints
                (byte)(MemberFlags.Field | MemberFlags.Static),                                                             // Flags
                (byte)WellKnownType.core_Diagnostics_DebuggableAttribute__DebuggingModes,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Diagnostics_DebuggableAttribute__DebuggingModes, // Field Signature

                // System_Runtime_InteropServices_ClassInterfaceAttribute__ctorClassInterfaceType
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_ClassInterfaceAttribute,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_ClassInterfaceType,

                // System_Runtime_InteropServices_CoClassAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_CoClassAttribute,                                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,

                // System_Runtime_InteropServices_ComAwareEventInfo__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_ComAwareEventInfo,                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Runtime_InteropServices_ComAwareEventInfo__AddEventHandler
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)WellKnownType.core_runtime_ComAwareEventInfo,                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,

                // System_Runtime_InteropServices_ComAwareEventInfo__RemoveEventHandler
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)WellKnownType.core_runtime_ComAwareEventInfo,                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Delegate,

                // System_Runtime_InteropServices_ComEventInterfaceAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_ComEventInterfaceAttribute,                              // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,

                // System_Runtime_InteropServices_ComSourceInterfacesAttribute__ctorString
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_ComSourceInterfacesAttribute,                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Runtime_InteropServices_ComVisibleAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_ComVisibleAttribute,                                     // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,

                // System_Runtime_InteropServices_DispIdAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_DispIdAttribute,                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_Runtime_InteropServices_GuidAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_GuidAttribute,                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Runtime_InteropServices_InterfaceTypeAttribute__ctorComInterfaceType
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_InterfaceTypeAttribute,                                  // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_ComInterfaceType,

                // System_Runtime_InteropServices_InterfaceTypeAttribute__ctorInt16
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_InterfaceTypeAttribute,                                  // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16,

                // System_Runtime_InteropServices_Marshal__GetTypeFromCLSID
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_Marshal,                                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Guid,

                // System_Runtime_InteropServices_TypeIdentifierAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_TypeIdentifierAttribute,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_InteropServices_TypeIdentifierAttribute__ctorStringString
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_TypeIdentifierAttribute,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Runtime_InteropServices_BestFitMappingAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_BestFitMappingAttribute,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,

                // System_Runtime_InteropServices_DefaultParameterValueAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_DefaultParameterValueAttribute,                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_Runtime_InteropServices_LCIDConversionAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_LCIDConversionAttribute,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_Runtime_InteropServices_UnmanagedFunctionPointerAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_UnmanagedFunctionPointerAttribute,                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_CallingConvention,

                // System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__AddEventHandler
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationTokenTable_T,            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationToken,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__GetOrCreateEventRegistrationTokenTable
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationTokenTable_T,            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericTypeInstance, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationTokenTable_T,
                    1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericTypeInstance,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationTokenTable_T,
                    1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__InvocationList
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationTokenTable_T,            // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__RemoveEventHandler
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationTokenTable_T,            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationToken,

                // System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__AddEventHandler_T
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_WindowsRuntime_WindowsRuntimeMarshal,                    // DeclaringTypeId
                1,                                                                                                          // Arity
                    3,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeInstance,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Func_T2,
                    2,
                    (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationToken,
                    (byte)SignatureTypeCode.GenericTypeInstance,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Action_T,
                    1,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationToken,
                    (byte)SignatureTypeCode.GenericMethodParameter, 0,

                // System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveAllEventHandlers
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_WindowsRuntime_WindowsRuntimeMarshal,                    // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeInstance,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Action_T,
                    1,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationToken,

                // System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveEventHandler_T
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_WindowsRuntime_WindowsRuntimeMarshal,                    // DeclaringTypeId
                1,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.GenericTypeInstance,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Action_T,
                    1,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_WindowsRuntime_EventRegistrationToken,
                    (byte)SignatureTypeCode.GenericMethodParameter, 0,

                // System_Runtime_CompilerServices_ExtensionAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_ExtensionAttribute,                                     // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_CompilerGeneratedAttribute,                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_CompilerServices_AccessedThroughPropertyAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_AccessedThroughPropertyAttribute,                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_CompilationRelaxationsAttribute,                        // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_RuntimeCompatibilityAttribute,                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)WellKnownType.core_runtime_compiler_RuntimeCompatibilityAttribute,                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type

                // System_Runtime_CompilerServices_UnsafeValueTypeAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_UnsafeValueTypeAttribute,                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_CompilerServices_FixedBufferAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_FixedBufferAttribute,                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_Runtime_CompilerServices_DynamicAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_DynamicAttribute,                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_CompilerServices_DynamicAttribute__ctorTransformFlags
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_DynamicAttribute,                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,

                // System_Runtime_CompilerServices_CallSite_T__Create
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_compiler_CallSite_T,                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericTypeInstance, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_compiler_CallSite_T,
                    1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_compiler_CallSiteBinder,

                // System_Runtime_CompilerServices_CallSite_T__Target
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_runtime_compiler_CallSite_T,                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,                                                        // Field Signature

                // System_Runtime_CompilerServices_RuntimeHelpers__GetObjectValueObject
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_compiler_RuntimeHelpers,                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_Runtime_CompilerServices_RuntimeHelpers__InitializeArrayArrayRuntimeFieldHandle
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_compiler_RuntimeHelpers,                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.core_Array_T,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_RuntimeFieldHandle,

                // System_Runtime_CompilerServices_RuntimeHelpers__get_OffsetToStringData
                (byte)(MemberFlags.PropertyGet | MemberFlags.Static),                                                       // Flags
                (byte)WellKnownType.core_runtime_compiler_RuntimeHelpers,                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32, // Return Type

                // System_Runtime_ExceptionServices_ExceptionDispatchInfo__Capture
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Runtime_ExceptionServices_ExceptionDispatchInfo,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Runtime_ExceptionServices_ExceptionDispatchInfo,
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Exception,

                // System_Runtime_ExceptionServices_ExceptionDispatchInfo__Throw
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_Runtime_ExceptionServices_ExceptionDispatchInfo,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Security_UnverifiableCodeAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Security_UnverifiableCodeAttribute,                                              // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Security_Permissions_SecurityAction__RequestMinimum
                (byte)(MemberFlags.Field | MemberFlags.Static),                                                             // Flags
                (byte)WellKnownType.core_Security_Permissions_SecurityAction,                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Security_Permissions_SecurityAction,     // Field Signature

                // System_Security_Permissions_SecurityPermissionAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Security_Permissions_SecurityPermissionAttribute,                                // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Security_Permissions_SecurityAction,

                // System_Security_Permissions_SecurityPermissionAttribute__SkipVerification
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)WellKnownType.core_Security_Permissions_SecurityPermissionAttribute,                                // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type

                // System_Activator__CreateInstance
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Activator,                                                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,

                // System_Activator__CreateInstance_T
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Activator,                                                                       // DeclaringTypeId
                1,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericMethodParameter, 0, // Return Type

                // System_Threading_Interlocked__CompareExchange_T
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Threading_Interlocked,                                                           // DeclaringTypeId
                1,                                                                                                          // Arity
                    3,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericMethodParameter, 0, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.GenericMethodParameter, 0,

                // System_Threading_Monitor__Enter
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Threading_Monitor,                                                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_Threading_Monitor__Enter2
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Threading_Monitor,                                                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,

                // System_Threading_Monitor__Exit
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Threading_Monitor,                                                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_Threading_Thread__CurrentThread
                (byte)(MemberFlags.Property | MemberFlags.Static),                                                          // Flags
                (byte)WellKnownType.core_Threading_Thread,                                                                // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Threading_Thread, // Return Type

                // System_Threading_Thread__ManagedThreadId
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)WellKnownType.core_Threading_Thread,                                                                // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32, // Return Type

                // System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)WellKnownType.core_runtime_compiler_IAsyncStateMachine,                                     // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                           // Flags
                (byte)WellKnownType.core_runtime_compiler_IAsyncStateMachine,                                     // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_compiler_IAsyncStateMachine,

                // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Create
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncVoidMethodBuilder,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_compiler_AsyncVoidMethodBuilder,

                // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetException
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncVoidMethodBuilder,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Exception,

                // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetResult
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncVoidMethodBuilder,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitOnCompleted
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncVoidMethodBuilder,                                 // DeclaringTypeId
                2,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, (byte)SpecialType.System_Object,

                // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitUnsafeOnCompleted
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncVoidMethodBuilder,                                 // DeclaringTypeId
                2,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, (byte)SpecialType.System_Object,

                // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Start_T
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncVoidMethodBuilder,                                 // DeclaringTypeId
                1,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,

                // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetStateMachine
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncVoidMethodBuilder,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_compiler_IAsyncStateMachine,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Create
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetException
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Exception,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetResult
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitOnCompleted
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder,                                 // DeclaringTypeId
                2,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, (byte)SpecialType.System_Object,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitUnsafeOnCompleted
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder,                                 // DeclaringTypeId
                2,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, (byte)SpecialType.System_Object,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Start_T
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder,                                 // DeclaringTypeId
                1,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetStateMachine
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_compiler_IAsyncStateMachine,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Task
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder,                                 // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Threading_Tasks_Task, // Return Type

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Create
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder_T,                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder_T,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetException
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder_T,                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Exception,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetResult
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder_T,                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitOnCompleted
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder_T,                               // DeclaringTypeId
                2,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, (byte)SpecialType.System_Object,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitUnsafeOnCompleted
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder_T,                               // DeclaringTypeId
                2,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, (byte)SpecialType.System_Object,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Start_T
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder_T,                               // DeclaringTypeId
                1,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetStateMachine
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder_T,                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_runtime_compiler_IAsyncStateMachine,

                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Task
                (byte)MemberFlags.Property,                                                                                 // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncTaskMethodBuilder_T,                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.GenericTypeInstance, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Threading_Tasks_Task_T,
                    1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_AsyncStateMachineAttribute,                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,

                // System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_runtime_compiler_IteratorStateMachineAttribute,                          // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,

                // System_Xml_Linq_XElement__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Xml_Linq_XElement,                                                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Xml_Linq_XName,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_Xml_Linq_XElement__ctor2
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_Xml_Linq_XElement,                                                               // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Xml_Linq_XName,
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_Xml_Linq_XNamespace__Get
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Xml_Linq_XNamespace,                                                             // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Xml_Linq_XNamespace, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_Windows_Forms_Application__RunForm
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Windows_Forms_Application,                                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Windows_Forms_Form,

                // System_Environment__CurrentManagedThreadId
                (byte)(MemberFlags.Property | MemberFlags.Static),                                                          // Flags
                (byte)WellKnownType.core_Environment,                                                                     // DeclaringTypeId
                0,                                                                                                          // Arity
                    0,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32, // Return Type

                // System_ComponentModel_EditorBrowsableAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_ComponentModel_EditorBrowsableAttribute,                                         // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_ComponentModel_EditorBrowsableState,

                // System_Runtime_GCLatencyMode__SustainedLowLatency
                (byte)(MemberFlags.Field | MemberFlags.Static),                                                             // Flags
                (byte)WellKnownType.core_Runtime_GCLatencyMode,                                                           // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Runtime_GCLatencyMode,                   // Field Signature

                // System_ValueTuple_T1__Item1
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T1,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,                                                        // Field Signature

                // System_ValueTuple_T2__Item1
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T2,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,                                                        // Field Signature

                // System_ValueTuple_T2__Item2
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T2,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,                                                        // Field Signature

                // System_ValueTuple_T3__Item1
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T3,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,                                                        // Field Signature

                // System_ValueTuple_T3__Item2
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T3,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,                                                        // Field Signature

                // System_ValueTuple_T3__Item3
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T3,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,                                                        // Field Signature

                // System_ValueTuple_T4__Item1
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T4,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,                                                        // Field Signature

                // System_ValueTuple_T4__Item2
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T4,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,                                                        // Field Signature

                // System_ValueTuple_T4__Item3
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T4,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,                                                        // Field Signature

                // System_ValueTuple_T4__Item4
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T4,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,                                                        // Field Signature

                // System_ValueTuple_T5__Item1
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T5,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,                                                        // Field Signature

                // System_ValueTuple_T5__Item2
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T5,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,                                                        // Field Signature

                // System_ValueTuple_T5__Item3
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T5,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,                                                        // Field Signature

                // System_ValueTuple_T5__Item4
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T5,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,                                                        // Field Signature

                // System_ValueTuple_T5__Item5
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T5,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 4,                                                        // Field Signature

                // System_ValueTuple_T6__Item1
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T6,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,                                                        // Field Signature

                // System_ValueTuple_T6__Item2
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T6,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,                                                        // Field Signature

                // System_ValueTuple_T6__Item3
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T6,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,                                                        // Field Signature

                // System_ValueTuple_T6__Item4
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T6,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,                                                        // Field Signature

                // System_ValueTuple_T6__Item5
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T6,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 4,                                                        // Field Signature

                // System_ValueTuple_T6__Item6
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.core_ValueTuple_T6,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 5,                                                        // Field Signature

                // System_ValueTuple_T7__Item1
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_T7 - WellKnownType.ExtSentinel),    // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,                                                        // Field Signature

                // System_ValueTuple_T7__Item2
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_T7 - WellKnownType.ExtSentinel),    // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,                                                        // Field Signature

                // System_ValueTuple_T7__Item3
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_T7 - WellKnownType.ExtSentinel),    // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,                                                        // Field Signature

                // System_ValueTuple_T7__Item4
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_T7 - WellKnownType.ExtSentinel),    // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,                                                        // Field Signature

                // System_ValueTuple_T7__Item5
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_T7 - WellKnownType.ExtSentinel),    // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 4,                                                        // Field Signature

                // System_ValueTuple_T7__Item6
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_T7 - WellKnownType.ExtSentinel),    // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 5,                                                        // Field Signature

                // System_ValueTuple_T7__Item7
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_T7 - WellKnownType.ExtSentinel),    // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 6,                                                        // Field Signature

                // System_ValueTuple_TRest__Item1
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_TRest - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,                                                        // Field Signature

                // System_ValueTuple_TRest__Item2
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_TRest - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,                                                        // Field Signature

                // System_ValueTuple_TRest__Item3
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_TRest - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,                                                        // Field Signature

                // System_ValueTuple_TRest__Item4
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_TRest - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,                                                        // Field Signature

                // System_ValueTuple_TRest__Item5
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_TRest - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 4,                                                        // Field Signature

                // System_ValueTuple_TRest__Item6
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_TRest - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 5,                                                        // Field Signature

                // System_ValueTuple_TRest__Item7
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_TRest - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 6,                                                        // Field Signature

                // System_ValueTuple_TRest__Rest
                (byte)MemberFlags.Field,                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_TRest - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                          // Arity
                    (byte)SignatureTypeCode.GenericTypeParameter, 7,                                                        // Field Signature

                // System_ValueTuple_T1__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_ValueTuple_T1,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,

                // System_ValueTuple_T2__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_ValueTuple_T2,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,

                // System_ValueTuple_T3__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_ValueTuple_T3,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    3,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,

                 // System_ValueTuple_T4__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_ValueTuple_T4,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    4,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,

                // System_ValueTuple_T_T2_T3_T4_T5__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_ValueTuple_T5,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    5,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,
                    (byte)SignatureTypeCode.GenericTypeParameter, 4,

                // System_ValueTuple_T6__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.core_ValueTuple_T6,                                                                   // DeclaringTypeId
                0,                                                                                                          // Arity
                    6,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,
                    (byte)SignatureTypeCode.GenericTypeParameter, 4,
                    (byte)SignatureTypeCode.GenericTypeParameter, 5,

                // System_ValueTuple_T7__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_T7 - WellKnownType.ExtSentinel),    // DeclaringTypeId
                0,                                                                                                          // Arity
                    7,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,
                    (byte)SignatureTypeCode.GenericTypeParameter, 4,
                    (byte)SignatureTypeCode.GenericTypeParameter, 5,
                    (byte)SignatureTypeCode.GenericTypeParameter, 6,

                // System_ValueTuple_TRest__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ValueTuple_TRest - WellKnownType.ExtSentinel),  // DeclaringTypeId
                0,                                                                                                          // Arity
                    8,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.GenericTypeParameter, 1,
                    (byte)SignatureTypeCode.GenericTypeParameter, 2,
                    (byte)SignatureTypeCode.GenericTypeParameter, 3,
                    (byte)SignatureTypeCode.GenericTypeParameter, 4,
                    (byte)SignatureTypeCode.GenericTypeParameter, 5,
                    (byte)SignatureTypeCode.GenericTypeParameter, 6,
                    (byte)SignatureTypeCode.GenericTypeParameter, 7,

                // System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames
                (byte)MemberFlags.Constructor,                                                                                   // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_TupleElementNamesAttribute // DeclaringTypeId
                                                        - WellKnownType.ExtSentinel),
                0,                                                                                                               // Arity
                    1,                                                                                                           // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,

                // System_String__Format_IFormatProvider
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    3,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_IFormatProvider,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,

                // System_String__Substring
                (byte)MemberFlags.Method,                                                                                   // Flags
                (byte)SpecialType.System_String,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    2,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningSingleFile
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.Microsoft_CodeAnalysis_Runtime_Instrumentation - WellKnownType.ExtSentinel),  // DeclaringTypeId
                0,                                                                                                                                  // Arity
                    5,                                                                                                                              // Method Signature
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Guid,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningMultipleFiles
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                                                    // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.Microsoft_CodeAnalysis_Runtime_Instrumentation - WellKnownType.ExtSentinel),  // DeclaringTypeId
                0,                                                                                                                                  // Arity
                    5,                                                                                                                              // Method Signature
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Guid,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                // System_Runtime_CompilerServices_NullableAttribute__ctorByte
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_NullableAttribute - WellKnownType.ExtSentinel),                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt8,

                // System_Runtime_CompilerServices_NullableAttribute__ctorTransformFlags
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_NullableAttribute - WellKnownType.ExtSentinel),                                       // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.Array, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_UInt8,

                // System_Runtime_CompilerServices_ReferenceAssemblyAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                                                                  // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_ReferenceAssemblyAttribute - WellKnownType.ExtSentinel),  // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    0,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                 // System_Runtime_CompilerServices_IsReadOnlyAttribute__ctor
                 (byte)(MemberFlags.Constructor),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_ReadOnlyAttribute - WellKnownType.ExtSentinel),        // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     0,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                 // System_Runtime_CompilerServices_IsByRefLikeAttribute__ctor
                 (byte)(MemberFlags.Constructor),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_ByRefLikeAttribute - WellKnownType.ExtSentinel),       // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     0,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                 // System_ObsoleteAttribute__ctor
                 (byte)(MemberFlags.Constructor),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ObsoleteAttribute - WellKnownType.ExtSentinel),                                   // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     2,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_String,
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,
                     
                 // System_Span__ctor
                 (byte)(MemberFlags.Constructor),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Span_T - WellKnownType.ExtSentinel),                                              // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     2,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                     (byte)SignatureTypeCode.Pointer, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                 // System_Span__get_Item
                 (byte)(MemberFlags.PropertyGet),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Span_T - WellKnownType.ExtSentinel),                                              // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericTypeParameter, 0, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                 // System_Span__get_Length
                 (byte)(MemberFlags.PropertyGet),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Span_T - WellKnownType.ExtSentinel),                                              // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                    0,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32, // Return Type

                 // System_ReadOnlySpan__ctor
                 (byte)(MemberFlags.Constructor),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ReadOnlySpan_T - WellKnownType.ExtSentinel),                                      // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     2,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                     (byte)SignatureTypeCode.Pointer, (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                 // System_ReadOnlySpan__get_Item
                 (byte)(MemberFlags.PropertyGet),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ReadOnlySpan_T - WellKnownType.ExtSentinel),                                      // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericTypeParameter, 0, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                 // System_ReadOnlySpan__get_Length
                 (byte)(MemberFlags.PropertyGet),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_ReadOnlySpan_T - WellKnownType.ExtSentinel),                                      // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                    0,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32, // Return Type

                // System_Runtime_CompilerServices_IsUnmanagedAttribute__ctor
                 (byte)(MemberFlags.Constructor),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_UnmanagedAttribute - WellKnownType.ExtSentinel),       // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     0,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Math__CeilingDouble
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Math,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Math__FloorDouble
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Math,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                // System_Math__TruncateDouble
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                            // Flags
                (byte)WellKnownType.core_Math,                                                                            // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Float64,

                 // core_Index__ctor
                 (byte)(MemberFlags.Constructor),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Index - WellKnownType.ExtSentinel),                                               // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     2,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int,
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,

                 // core_Index__value
                 (byte)(MemberFlags.Property),                                                                                                                  // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Index - WellKnownType.ExtSentinel),                                               // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     0,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int, // Return type

                 // core_Range__ctor
                 (byte)(MemberFlags.Constructor),                                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Range - WellKnownType.ExtSentinel),                                               // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     2,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                     (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Index - WellKnownType.ExtSentinel),
                     (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Index - WellKnownType.ExtSentinel),

                 // core_Range__begin
                 (byte)(MemberFlags.Property),                                                                                                                  // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Index - WellKnownType.ExtSentinel),                                               // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     0,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Index - WellKnownType.ExtSentinel),

                 // core_Range__end
                 (byte)(MemberFlags.Property),                                                                                                                  // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Index - WellKnownType.ExtSentinel),                                               // DeclaringTypeId
                 0,                                                                                                                                             // Arity
                     0,                                                                                                                                         // Method Signature
                     (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Index - WellKnownType.ExtSentinel),

                // System_Runtime_CompilerServices_AsyncIteratorStateMachineAttribute__ctor
                (byte)MemberFlags.Constructor,                                                                              // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_AsyncIteratorStateMachineAttribute - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                          // Arity
                    1,                                                                                                      // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Type,

                // System_IAsyncDisposable__DisposeAsync
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                                                              // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_IAsyncDisposable - WellKnownType.ExtSentinel),                                    // DeclaringTypeId
                0,                                                                                                                                             // Arity
                    0,                                                                                                                                         // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_ValueTask - WellKnownType.ExtSentinel), // Return Type: ValueTask

                // System_Collections_Generic_IAsyncEnumerable_T__GetAsyncEnumerator
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                                                               // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Collections_Generic_IAsyncEnumerable_T - WellKnownType.ExtSentinel),               // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.GenericTypeInstance, // Return Type: IAsyncEnumerator<T>
                        (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Collections_Generic_IAsyncEnumerator_T - WellKnownType.ExtSentinel),
                        1,
                        (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.TypeHandle, // Argument: CancellationToken
                        (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_CancellationToken - WellKnownType.ExtSentinel),

                // System_Collections_Generic_IAsyncEnumerator_T__MoveNextAsync
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                                                               // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Collections_Generic_IAsyncEnumerator_T - WellKnownType.ExtSentinel),               // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    0,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.GenericTypeInstance, // Return Type: ValueTask<bool>
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_ValueTask_T - WellKnownType.ExtSentinel),
                    1,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Boolean,

                // System_Collections_Generic_IAsyncEnumerator_T__get_Current
                (byte)(MemberFlags.PropertyGet | MemberFlags.Virtual),                                                                                          // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Collections_Generic_IAsyncEnumerator_T - WellKnownType.ExtSentinel),               // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    0,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.GenericTypeParameter, 0, // Return Type: T

                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetResult,
                (byte)MemberFlags.Method,                                                                                                                       // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T - WellKnownType.ExtSentinel),     // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.GenericTypeParameter, 0, // Return Type: T
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument: short

                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetStatus,
                (byte)MemberFlags.Method,                                                                                                                       // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T - WellKnownType.ExtSentinel),     // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ValueTaskSourceStatus - WellKnownType.ExtSentinel), // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument: short

                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__OnCompleted,
                (byte)MemberFlags.Method,                                                                                                                       // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T - WellKnownType.ExtSentinel),     // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    4,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeInstance, // Argument: Action<object>
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Action_T,
                    1,
                        (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object, // Argument
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ValueTaskSourceOnCompletedFlags - WellKnownType.ExtSentinel), // Argument

                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__Reset
                (byte)MemberFlags.Method,                                                                                                                       // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T - WellKnownType.ExtSentinel),     // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    0,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetException,
                (byte)MemberFlags.Method,                                                                                                                       // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T - WellKnownType.ExtSentinel),     // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Exception, // Argument

                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetResult,
                (byte)MemberFlags.Method,                                                                                                                       // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T - WellKnownType.ExtSentinel),     // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0, // Argument: T

                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__get_Version,
                (byte)MemberFlags.PropertyGet,                                                                                                                  // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T - WellKnownType.ExtSentinel),     // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    0,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16,

                // System_Threading_Tasks_Sources_IValueTaskSource_T__GetResult,
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                                                               // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_IValueTaskSource_T - WellKnownType.ExtSentinel),           // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.GenericTypeParameter, 0, // Return Type: T
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument: short

                // System_Threading_Tasks_Sources_IValueTaskSource_T__GetStatus,
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                                                               // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_IValueTaskSource_T - WellKnownType.ExtSentinel),           // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ValueTaskSourceStatus - WellKnownType.ExtSentinel), // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument: short

                // System_Threading_Tasks_Sources_IValueTaskSource_T__OnCompleted,
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                                                               // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_IValueTaskSource_T - WellKnownType.ExtSentinel),           // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    4,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeInstance, // Argument: Action<object>
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Action_T,
                    1,
                        (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object, // Argument
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ValueTaskSourceOnCompletedFlags - WellKnownType.ExtSentinel), // Argument

                // System_Threading_Tasks_Sources_IValueTaskSource__GetResult,
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                                                               // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_IValueTaskSource - WellKnownType.ExtSentinel),             // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument: short

                // System_Threading_Tasks_Sources_IValueTaskSource__GetStatus,
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                                                               // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_IValueTaskSource - WellKnownType.ExtSentinel),             // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ValueTaskSourceStatus - WellKnownType.ExtSentinel), // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument: short

                // System_Threading_Tasks_Sources_IValueTaskSource__OnCompleted,
                (byte)(MemberFlags.Method | MemberFlags.Virtual),                                                                                               // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_IValueTaskSource - WellKnownType.ExtSentinel),             // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    4,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeInstance, // Argument: Action<object>
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.core_Action_T,
                    1,
                        (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object, // Argument
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_ValueTaskSourceOnCompletedFlags - WellKnownType.ExtSentinel), // Argument

                // System_Threading_Tasks_ValueTask_T__ctorSourceAndToken
                (byte)MemberFlags.Constructor,                                                                                                                  // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_ValueTask_T - WellKnownType.ExtSentinel),                          // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    2,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeInstance, // Argument: IValueTaskSource<T>
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_IValueTaskSource_T - WellKnownType.ExtSentinel),
                    1,
                        (byte)SignatureTypeCode.GenericTypeParameter, 0,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument

                // System_Threading_Tasks_ValueTask_T__ctorValue
                (byte)MemberFlags.Constructor,                                                                                                                  // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_ValueTask_T - WellKnownType.ExtSentinel),                          // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    1,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.GenericTypeParameter, 0, // Argument: T

                // System_Threading_Tasks_ValueTask__ctor
                (byte)MemberFlags.Constructor,                                                                                                                  // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_ValueTask - WellKnownType.ExtSentinel),                            // DeclaringTypeId
                0,                                                                                                                                              // Arity
                    2,                                                                                                                                          // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_Threading_Tasks_Sources_IValueTaskSource - WellKnownType.ExtSentinel), // Argument: IValueTaskSource
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int16, // Argument

                // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Create
                (byte)(MemberFlags.Method | MemberFlags.Static),                                                                                               // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_AsyncIteratorMethodBuilder - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                                                             // Arity
                    0,                                                                                                                                         // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_AsyncIteratorMethodBuilder - WellKnownType.ExtSentinel),

                // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Complete
                (byte)MemberFlags.Method,                                                                                                                      // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_AsyncIteratorMethodBuilder - WellKnownType.ExtSentinel), // DeclaringTypeId
                0,                                                                                                                                             // Arity
                    0,                                                                                                                                         // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type

                // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitOnCompleted
                (byte)MemberFlags.Method,                                                                                                                      // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_AsyncIteratorMethodBuilder - WellKnownType.ExtSentinel), // DeclaringTypeId
                2,                                                                                                                                             // Arity
                    2,                                                                                                                                         // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, (byte)SpecialType.System_Object,

                // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitUnsafeOnCompleted
                (byte)MemberFlags.Method,                                                                                                                      // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_AsyncIteratorMethodBuilder - WellKnownType.ExtSentinel), // DeclaringTypeId
                2,                                                                                                                                             // Arity
                    2,                                                                                                                                         // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, (byte)SpecialType.System_Object,

                // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__MoveNext_T
                (byte)MemberFlags.Method,                                                                                                                      // Flags
                (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_AsyncIteratorMethodBuilder - WellKnownType.ExtSentinel), // DeclaringTypeId
                1,                                                                                                                                             // Arity
                    1,                                                                                                                                         // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void, // Return Type
                    (byte)SignatureTypeCode.ByReference, (byte)SignatureTypeCode.GenericMethodParameter, 0,
                 // System_Runtime_CompilerServices_ITuple__get_Item
                 (byte)(MemberFlags.PropertyGet | MemberFlags.Virtual),                                                                       // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_ITuple - WellKnownType.ExtSentinel),   // DeclaringTypeId
                 0,                                                                                                                           // Arity
                    1,                                                                                                                        // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object, // Return Type
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32,

                 // System_Runtime_CompilerServices_ITuple__get_Length
                 (byte)(MemberFlags.PropertyGet | MemberFlags.Virtual),                                                                       // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_ITuple - WellKnownType.ExtSentinel),   // DeclaringTypeId
                 0,                                                                                                                           // Arity
                    0,                                                                                                                        // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Int32, // Return Type

                 // System_InvalidOperationException__ctor
                 (byte)MemberFlags.Constructor,                                                                                               // Flags
                  (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_InvalidOperationException - WellKnownType.ExtSentinel),        // DeclaringTypeId
                 0,                                                                                                                           // Arity
                    0,                                                                                                                        // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,

                 // System_Runtime_CompilerServices_SwitchExpressionException__ctor
                 (byte)MemberFlags.Constructor,                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_SwitchExpressionException - WellKnownType.ExtSentinel),// DeclaringTypeId
                 0,                                                                                                                           // Arity
                    0,                                                                                                                        // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,

                 // System_Runtime_CompilerServices_SwitchExpressionException__ctorObject
                 (byte)MemberFlags.Constructor,                                                                                               // Flags
                 (byte)WellKnownType.ExtSentinel, (byte)(WellKnownType.core_runtime_compiler_SwitchExpressionException - WellKnownType.ExtSentinel),// DeclaringTypeId
                 0,                                                                                                                           // Arity
                    1,                                                                                                                        // Method Signature
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Void,
                    (byte)SignatureTypeCode.TypeHandle, (byte)SpecialType.System_Object,
            };

            string[] allNames = new string[(int)WellKnownMember.Count]
            {
                "Round",                                    // System_Math__RoundDouble
                "Pow",                                      // System_Math__PowDoubleDouble
                "Empty",                                    // System_Array__Empty
                "Copy",                                     // System_Array__Copy
                "ToBoolean",                                // System_Convert__ToBooleanInt32
                "ToBoolean",                                // System_Convert__ToBooleanUInt32
                "ToBoolean",                                // System_Convert__ToBooleanInt64
                "ToBoolean",                                // System_Convert__ToBooleanUInt64
                "ToBoolean",                                // System_Convert__ToBooleanSingle
                "ToBoolean",                                // System_Convert__ToBooleanDouble
                "ToSByte",                                  // System_Convert__ToSByteDouble
                "ToSByte",                                  // System_Convert__ToSByteSingle
                "ToByte",                                   // System_Convert__ToByteDouble
                "ToByte",                                   // System_Convert__ToByteSingle
                "ToInt16",                                  // System_Convert__ToInt16Double
                "ToInt16",                                  // System_Convert__ToInt16Single
                "ToUInt16",                                 // System_Convert__ToUInt16Double
                "ToUInt16",                                 // System_Convert__ToUInt16Single
                "ToInt32",                                  // System_Convert__ToInt32Double
                "ToInt32",                                  // System_Convert__ToInt32Single
                "ToUInt32",                                 // System_Convert__ToUInt32Double
                "ToUInt32",                                 // System_Convert__ToUInt32Single
                "ToInt64",                                  // System_Convert__ToInt64Double
                "ToInt64",                                  // System_Convert__ToInt64Single
                "ToUInt64",                                 // System_Convert__ToUInt64Double
                "ToUInt64",                                 // System_Convert__ToUInt64Single
                ".ctor",                                    // System_CLSCompliantAttribute__ctor
                ".ctor",                                    // System_FlagsAttribute__ctor
                ".ctor",                                    // System_Guid__ctor
                "GetTypeFromCLSID",                         // System_Type__GetTypeFromCLSID
                "GetTypeFromHandle",                        // System_Type__GetTypeFromHandle
                "Missing",                                  // System_Type__Missing
                ".ctor",                                    // System_Reflection_AssemblyKeyFileAttribute__ctor
                ".ctor",                                    // System_Reflection_AssemblyKeyNameAttribute__ctor
                "GetMethodFromHandle",                      // System_Reflection_MethodBase__GetMethodFromHandle
                "GetMethodFromHandle",                      // System_Reflection_MethodBase__GetMethodFromHandle2
                "CreateDelegate",                           // System_Reflection_MethodInfo__CreateDelegate
                "CreateDelegate",                           // System_Delegate__CreateDelegate
                "CreateDelegate",                           // System_Delegate__CreateDelegate4
                "GetFieldFromHandle",                       // System_Reflection_FieldInfo__GetFieldFromHandle
                "GetFieldFromHandle",                       // System_Reflection_FieldInfo__GetFieldFromHandle2
                "Value",                                    // System_Reflection_Missing__Value
                "Equals",                                   // System_IEquatable_T__Equals
                "Equals",                                   // System_Collections_Generic_EqualityComparer_T__Equals
                "GetHashCode",                              // System_Collections_Generic_EqualityComparer_T__GetHashCode
                "get_Default",                              // System_Collections_Generic_EqualityComparer_T__get_Default
                ".ctor",                                    // System_AttributeUsageAttribute__ctor
                "AllowMultiple",                            // System_AttributeUsageAttribute__AllowMultiple
                "Inherited",                                // System_AttributeUsageAttribute__Inherited
                ".ctor",                                    // System_ParamArrayAttribute__ctor
                ".ctor",                                    // System_STAThreadAttribute__ctor
                ".ctor",                                    // System_Reflection_DefaultMemberAttribute__ctor
                "Break",                                    // System_Diagnostics_Debugger__Break
                ".ctor",                                    // System_Diagnostics_DebuggerDisplayAttribute__ctor
                "Type",                                     // System_Diagnostics_DebuggerDisplayAttribute__Type
                ".ctor",                                    // System_Diagnostics_DebuggerNonUserCodeAttribute__ctor
                ".ctor",                                    // System_Diagnostics_DebuggerHiddenAttribute__ctor
                ".ctor",                                    // System_Diagnostics_DebuggerBrowsableAttribute__ctor
                ".ctor",                                    // System_Diagnostics_DebuggerStepThroughAttribute__ctor
                ".ctor",                                    // System_Diagnostics_DebuggableAttribute__ctorDebuggingModes
                "Default",                                  // System_Diagnostics_DebuggableAttribute_DebuggingModes__Default
                "DisableOptimizations",                     // System_Diagnostics_DebuggableAttribute_DebuggingModes__DisableOptimizations
                "EnableEditAndContinue",                    // System_Diagnostics_DebuggableAttribute_DebuggingModes__EnableEditAndContinue
                "IgnoreSymbolStoreSequencePoints",          // System_Diagnostics_DebuggableAttribute_DebuggingModes__IgnoreSymbolStoreSequencePoints
                ".ctor",                                    // System_Runtime_InteropServices_ClassInterfaceAttribute__ctorClassInterfaceType
                ".ctor",                                    // System_Runtime_InteropServices_CoClassAttribute__ctor
                ".ctor",                                    // System_Runtime_InteropServices_ComAwareEventInfo__ctor
                "AddEventHandler",                          // System_Runtime_InteropServices_ComAwareEventInfo__AddEventHandler
                "RemoveEventHandler",                       // System_Runtime_InteropServices_ComAwareEventInfo__RemoveEventHandler
                ".ctor",                                    // System_Runtime_InteropServices_ComEventInterfaceAttribute__ctor
                ".ctor",                                    // System_Runtime_InteropServices_ComSourceInterfacesAttribute__ctorString
                ".ctor",                                    // System_Runtime_InteropServices_ComVisibleAttribute__ctor
                ".ctor",                                    // System_Runtime_InteropServices_DispIdAttribute__ctor
                ".ctor",                                    // System_Runtime_InteropServices_GuidAttribute__ctor
                ".ctor",                                    // System_Runtime_InteropServices_InterfaceTypeAttribute__ctorComInterfaceType
                ".ctor",                                    // System_Runtime_InteropServices_InterfaceTypeAttribute__ctorInt16
                "GetTypeFromCLSID",                         // System_Runtime_InteropServices_Marshal__GetTypeFromCLSID
                ".ctor",                                    // System_Runtime_InteropServices_TypeIdentifierAttribute__ctor
                ".ctor",                                    // System_Runtime_InteropServices_TypeIdentifierAttribute__ctorStringString
                ".ctor",                                    // System_Runtime_InteropServices_BestFitMappingAttribute__ctor
                ".ctor",                                    // System_Runtime_InteropServices_DefaultParameterValueAttribute__ctor
                ".ctor",                                    // System_Runtime_InteropServices_LCIDConversionAttribute__ctor
                ".ctor",                                    // System_Runtime_InteropServices_UnmanagedFunctionPointerAttribute__ctor
                "AddEventHandler",                          // System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__AddEventHandler
                "GetOrCreateEventRegistrationTokenTable",   // System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__GetOrCreateEventRegistrationTokenTable
                "InvocationList",                           // System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__InvocationList
                "RemoveEventHandler",                       // System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__RemoveEventHandler
                "AddEventHandler",                          // System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__AddEventHandler_T
                "RemoveAllEventHandlers",                   // System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveAllEventHandlers
                "RemoveEventHandler",                       // System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveEventHandler_T
                ".ctor",                                    // System_Runtime_CompilerServices_ExtensionAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_AccessedThroughPropertyAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32
                ".ctor",                                    // System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor
                "WrapNonExceptionThrows",                   // System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows
                ".ctor",                                    // System_Runtime_CompilerServices_UnsafeValueTypeAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_FixedBufferAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_DynamicAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_DynamicAttribute__ctorTransformFlags
                "Create",                                   // System_Runtime_CompilerServices_CallSite_T__Create
                "Target",                                   // System_Runtime_CompilerServices_CallSite_T__Target
                "GetObjectValue",                           // System_Runtime_CompilerServices_RuntimeHelpers__GetObjectValueObject
                "InitializeArray",                          // System_Runtime_CompilerServices_RuntimeHelpers__InitializeArrayArrayRuntimeFieldHandle
                "get_OffsetToStringData",                   // System_Runtime_CompilerServices_RuntimeHelpers__get_OffsetToStringData
                "Capture",                                  // System_Runtime_ExceptionServices_ExceptionDispatchInfo__Capture
                "Throw",                                    // System_Runtime_ExceptionServices_ExceptionDispatchInfo__Throw
                ".ctor",                                    // System_Security_UnverifiableCodeAttribute__ctor
                "RequestMinimum",                           // System_Security_Permissions_SecurityAction__RequestMinimum
                ".ctor",                                    // System_Security_Permissions_SecurityPermissionAttribute__ctor
                "SkipVerification",                         // System_Security_Permissions_SecurityPermissionAttribute__SkipVerification
                "CreateInstance",                           // System_Activator__CreateInstance
                "CreateInstance",                           // System_Activator__CreateInstance_T
                "CompareExchange",                          // System_Threading_Interlocked__CompareExchange_T
                "Enter",                                    // System_Threading_Monitor__Enter
                "Enter",                                    // System_Threading_Monitor__Enter2
                "Exit",                                     // System_Threading_Monitor__Exit
                "CurrentThread",                            // System_Threading_Thread__CurrentThread
                "ManagedThreadId",                          // System_Threading_Thread__ManagedThreadId
                "MoveNext",                                 // System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext
                "SetStateMachine",                          // System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine
                "Create",                                   // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Create
                "SetException",                             // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetException
                "SetResult",                                // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetResult
                "AwaitOnCompleted",                         // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitOnCompleted
                "AwaitUnsafeOnCompleted",                   // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitUnsafeOnCompleted
                "Start",                                    // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Start_T
                "SetStateMachine",                          // System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetStateMachine
                "Create",                                   // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Create
                "SetException",                             // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetException
                "SetResult",                                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetResult
                "AwaitOnCompleted",                         // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitOnCompleted
                "AwaitUnsafeOnCompleted",                   // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitUnsafeOnCompleted
                "Start",                                    // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Start_T
                "SetStateMachine",                          // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetStateMachine
                "Task",                                     // System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Task
                "Create",                                   // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Create
                "SetException",                             // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetException
                "SetResult",                                // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetResult
                "AwaitOnCompleted",                         // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitOnCompleted
                "AwaitUnsafeOnCompleted",                   // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitUnsafeOnCompleted
                "Start",                                    // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Start_T
                "SetStateMachine",                          // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetStateMachine
                "Task",                                     // System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Task
                ".ctor",                                    // System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor
                ".ctor",                                    // System_Xml_Linq_XElement__ctor
                ".ctor",                                    // System_Xml_Linq_XElement__ctor2
                "Get",                                      // System_Xml_Linq_XNamespace__Get
                "Run",                                      // System_Windows_Forms_Application__RunForm
                "CurrentManagedThreadId",                   // System_Environment__CurrentManagedThreadId
                ".ctor",                                    // System_ComponentModel_EditorBrowsableAttribute__ctor
                "SustainedLowLatency",                      // System_Runtime_GCLatencyMode__SustainedLowLatency

                "Item1",                                    // System_ValueTuple_T1__Item1

                "Item1",                                    // System_ValueTuple_T2__Item1
                "Item2",                                    // System_ValueTuple_T2__Item2

                "Item1",                                    // System_ValueTuple_T3__Item1
                "Item2",                                    // System_ValueTuple_T3__Item2
                "Item3",                                    // System_ValueTuple_T3__Item3

                "Item1",                                    // System_ValueTuple_T4__Item1
                "Item2",                                    // System_ValueTuple_T4__Item2
                "Item3",                                    // System_ValueTuple_T4__Item3
                "Item4",                                    // System_ValueTuple_T4__Item4

                "Item1",                                    // System_ValueTuple_T5__Item1
                "Item2",                                    // System_ValueTuple_T5__Item2
                "Item3",                                    // System_ValueTuple_T5__Item3
                "Item4",                                    // System_ValueTuple_T5__Item4
                "Item5",                                    // System_ValueTuple_T5__Item5

                "Item1",                                    // System_ValueTuple_T6__Item1
                "Item2",                                    // System_ValueTuple_T6__Item2
                "Item3",                                    // System_ValueTuple_T6__Item3
                "Item4",                                    // System_ValueTuple_T6__Item4
                "Item5",                                    // System_ValueTuple_T6__Item5
                "Item6",                                    // System_ValueTuple_T6__Item6

                "Item1",                                    // System_ValueTuple_T7__Item1
                "Item2",                                    // System_ValueTuple_T7__Item2
                "Item3",                                    // System_ValueTuple_T7__Item3
                "Item4",                                    // System_ValueTuple_T7__Item4
                "Item5",                                    // System_ValueTuple_T7__Item5
                "Item6",                                    // System_ValueTuple_T7__Item6
                "Item7",                                    // System_ValueTuple_T7__Item7

                "Item1",                                    // System_ValueTuple_TRest__Item1
                "Item2",                                    // System_ValueTuple_TRest__Item2
                "Item3",                                    // System_ValueTuple_TRest__Item3
                "Item4",                                    // System_ValueTuple_TRest__Item4
                "Item5",                                    // System_ValueTuple_TRest__Item5
                "Item6",                                    // System_ValueTuple_TRest__Item6
                "Item7",                                    // System_ValueTuple_TRest__Item7
                "Rest",                                     // System_ValueTuple_TRest__Rest

                ".ctor",                                    // System_ValueTuple_T1__ctor
                ".ctor",                                    // System_ValueTuple_T2__ctor
                ".ctor",                                    // System_ValueTuple_T3__ctor
                ".ctor",                                    // System_ValueTuple_T4__ctor
                ".ctor",                                    // System_ValueTuple_T5__ctor
                ".ctor",                                    // System_ValueTuple_T6__ctor
                ".ctor",                                    // System_ValueTuple_T7__ctor
                ".ctor",                                    // System_ValueTuple_TRest__ctor

                ".ctor",                                    // System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames

                "Format",                                   // System_String__Format_IFormatProvider
                "Substring",                                // System_string__Substring

                "CreatePayload",                            // Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningSingleFile
                "CreatePayload",                            // Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningMultipleFiles

                ".ctor",                                    // System_Runtime_CompilerServices_NullableAttribute__ctorByte
                ".ctor",                                    // System_Runtime_CompilerServices_NullableAttribute__ctorTransformFlags
                ".ctor",                                    // System_Runtime_CompilerServices_ReferenceAssemblyAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_IsReadOnlyAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_IsByRefLikeAttribute__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_ObsoleteAttribute__ctor
                ".ctor",                                    // System_Span__ctor
                "get_Item",                                 // System_Span__get_Item
                "get_Length",                               // System_Span__get_Length
                ".ctor",                                    // System_ReadOnlySpan__ctor
                "get_Item",                                 // System_ReadOnlySpan__get_Item
                "get_Length",                               // System_ReadOnlySpan__get_Length
                ".ctor",                                    // System_Runtime_CompilerServices_IsUnmanagedAttribute__ctor

                "Ceiling",                                  // System_Math__CeilingDouble
                "Floor",                                    // System_Math__FloorDouble
                "Truncate",                                 // System_Math__TruncateDouble

                ".ctor",                                    // core_Index__ctor
                "value",                                    // core_Index__value
                ".ctor",                                    // core_Range__ctor
                "begin",                                    // core_Range__begin
                "end",                                      // core_Range__end

                ".ctor",                                    // System_Runtime_CompilerServices_AsyncIteratorStateMachineAttribute__ctor

                "DisposeAsync",                             // System_IAsyncDisposable__DisposeAsync
                "GetAsyncEnumerator",                       // System_Collections_Generic_IAsyncEnumerable_T__GetAsyncEnumerator
                "MoveNextAsync",                            // System_Collections_Generic_IAsyncEnumerator_T__MoveNextAsync
                "get_Current",                              // System_Collections_Generic_IAsyncEnumerator_T__get_Current

                "GetResult",                                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetResult
                "GetStatus",                                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetStatus
                "OnCompleted",                              // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__OnCompleted
                "Reset",                                    // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__Reset
                "SetException",                             // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetException
                "SetResult",                                // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetResult
                "get_Version",                              // System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__get_Version
                "GetResult",                                // System_Threading_Tasks_Sources_IValueTaskSource_T__GetResult
                "GetStatus",                                // System_Threading_Tasks_Sources_IValueTaskSource_T__GetStatus
                "OnCompleted",                              // System_Threading_Tasks_Sources_IValueTaskSource_T__OnCompleted
                "GetResult",                                // System_Threading_Tasks_Sources_IValueTaskSource__GetResult
                "GetStatus",                                // System_Threading_Tasks_Sources_IValueTaskSource__GetStatus
                "OnCompleted",                              // System_Threading_Tasks_Sources_IValueTaskSource__OnCompleted
                ".ctor",                                    // System_Threading_Tasks_ValueTask_T__ctor
                ".ctor",                                    // System_Threading_Tasks_ValueTask_T__ctorValue
                ".ctor",                                    // System_Threading_Tasks_ValueTask__ctor
                "Create",                                   // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Create
                "Complete",                                 // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Complete
                "AwaitOnCompleted",                         // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitOnCompleted
                "AwaitUnsafeOnCompleted",                   // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitUnsafeOnCompleted
                "MoveNext",                                 // System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__MoveNext_T
                "get_Item",                                 // System_Runtime_CompilerServices_ITuple__get_Item
                "get_Length",                               // System_Runtime_CompilerServices_ITuple__get_Length
                ".ctor",                                    // System_InvalidOperationException__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_SwitchExpressionException__ctor
                ".ctor",                                    // System_Runtime_CompilerServices_SwitchExpressionException__ctorObject
            };

            s_descriptors = MemberDescriptor.InitializeFromStream(new System.IO.MemoryStream(initializationBytes, writable: false), allNames);
        }

        public static MemberDescriptor GetDescriptor(WellKnownMember member)
        {
            return s_descriptors[(int)member];
        }

        /// <summary>
        /// This function defines whether an attribute is optional or not.
        /// </summary>
        /// <param name="attributeMember">The attribute member.</param>
        internal static bool IsSynthesizedAttributeOptional(WellKnownMember attributeMember)
        {
            switch (attributeMember)
            {
                case WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor:
                case WellKnownMember.System_Diagnostics_DebuggableAttribute__ctorDebuggingModes:
                case WellKnownMember.System_Diagnostics_DebuggerBrowsableAttribute__ctor:
                case WellKnownMember.System_Diagnostics_DebuggerHiddenAttribute__ctor:
                case WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor:
                case WellKnownMember.System_Diagnostics_DebuggerStepThroughAttribute__ctor:
                case WellKnownMember.System_Diagnostics_DebuggerNonUserCodeAttribute__ctor:
                case WellKnownMember.System_STAThreadAttribute__ctor:
                case WellKnownMember.System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor:
                case WellKnownMember.System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor:
                case WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorStateMachineAttribute__ctor:
                    return true;

                default:
                    return false;
            }
        }
    }
}
