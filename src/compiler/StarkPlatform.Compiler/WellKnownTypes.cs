// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler
{
    /// <summary>
    /// Ids of well known runtime types.
    /// Values should not intersect with SpecialType enum!
    /// </summary>
    /// <remarks></remarks>
    internal enum WellKnownType
    {
        // Value 0 represents an unknown type
        Unknown = SpecialType.None,

        First = SpecialType.Count + 1,

        // The following type ids should be in sync with names in WellKnownTypes.metadataNames array.
        core_Math = First,
        core_Array,
        core_Attribute,
        core_CLSCompliantAttribute,
        core_Convert,
        core_Exception,
        core_FlagsAttribute,
        core_FormattableString,
        core_Guid,
        core_IFormattable,
        core_RuntimeTypeHandle,
        core_RuntimeFieldHandle,
        core_RuntimeMethodHandle,
        core_MarshalByRefObject,
        core_Type,
        core_Reflection_AssemblyKeyFileAttribute,
        core_Reflection_AssemblyKeyNameAttribute,
        core_Reflection_MethodInfo,
        core_Reflection_ConstructorInfo,
        core_Reflection_MethodBase,
        core_Reflection_FieldInfo,
        core_Reflection_MemberInfo,
        core_Reflection_Missing,
        core_runtime_compiler_FormattableStringFactory,
        core_runtime_compiler_RuntimeHelpers,
        core_Runtime_ExceptionServices_ExceptionDispatchInfo,
        core_runtime_StructLayoutAttribute,
        core_runtime_UnknownWrapper,
        core_runtime_DispatchWrapper,
        core_runtime_CallingConvention,
        core_runtime_ClassInterfaceAttribute,
        core_runtime_ClassInterfaceType,
        core_runtime_CoClassAttribute,
        core_runtime_ComAwareEventInfo,
        core_runtime_ComEventInterfaceAttribute,
        core_runtime_ComInterfaceType,
        core_runtime_ComSourceInterfacesAttribute,
        core_runtime_ComVisibleAttribute,
        core_runtime_DispIdAttribute,
        core_runtime_GuidAttribute,
        core_runtime_InterfaceTypeAttribute,
        core_runtime_Marshal,
        core_runtime_TypeIdentifierAttribute,
        core_runtime_BestFitMappingAttribute,
        core_runtime_DefaultParameterValueAttribute,
        core_runtime_LCIDConversionAttribute,
        core_runtime_UnmanagedFunctionPointerAttribute,
        core_Activator,
        core_Threading_Tasks_Task,
        core_Threading_Tasks_Task_T,
        core_Threading_Interlocked,
        core_Threading_Monitor,
        core_Threading_Thread,

        // standard Func delegates - must be ordered by arity
        core_Func_T,
        core_Func_T2,
        core_Func_T3,
        core_Func_T4,
        core_Func_T5,
        core_Func_T6,
        core_Func_T7,
        core_Func_T8,
        core_Func_T9,
        core_Func_T10,
        core_Func_T11,
        core_Func_T12,
        core_Func_T13,
        core_Func_T14,
        core_Func_T15,
        core_Func_T16,
        core_Func_T17,
        core_Func_TMax = core_Func_T17,

        // standard Action delegates - must be ordered by arity
        core_Action,
        core_Action_T,
        core_Action_T2,
        core_Action_T3,
        core_Action_T4,
        core_Action_T5,
        core_Action_T6,
        core_Action_T7,
        core_Action_T8,
        core_Action_T9,
        core_Action_T10,
        core_Action_T11,
        core_Action_T12,
        core_Action_T13,
        core_Action_T14,
        core_Action_T15,
        core_Action_T16,
        core_Action_TMax = core_Action_T16,

        core_AttributeUsageAttribute,
        core_ParamArrayAttribute,
        core_NonSerializedAttribute,
        core_STAThreadAttribute,
        core_Reflection_DefaultMemberAttribute,
        core_runtime_compiler_DateTimeConstantAttribute,
        core_runtime_compiler_IUnknownConstantAttribute,
        core_runtime_compiler_IDispatchConstantAttribute,
        core_runtime_compiler_ExtensionAttribute,
        core_runtime_compiler_INotifyCompletion,
        core_runtime_compiler_InternalsVisibleToAttribute,
        core_runtime_compiler_CompilerGeneratedAttribute,
        core_runtime_compiler_AccessedThroughPropertyAttribute,
        core_runtime_compiler_CompilationRelaxationsAttribute,
        core_runtime_compiler_RuntimeCompatibilityAttribute,
        core_runtime_compiler_UnsafeValueTypeAttribute,
        core_runtime_compiler_FixedBufferAttribute,
        core_runtime_compiler_DynamicAttribute,
        core_runtime_compiler_CallSiteBinder,
        core_runtime_compiler_CallSite,
        core_runtime_compiler_CallSite_T,

        core_runtime_WindowsRuntime_EventRegistrationToken,
        core_runtime_WindowsRuntime_EventRegistrationTokenTable_T,
        core_runtime_WindowsRuntime_WindowsRuntimeMarshal,

        Windows_Foundation_IAsyncAction,
        Windows_Foundation_IAsyncActionWithProgress_T,
        Windows_Foundation_IAsyncOperation_T,
        Windows_Foundation_IAsyncOperationWithProgress_T2,

        core_Diagnostics_Debugger,
        core_Diagnostics_DebuggerDisplayAttribute,
        core_Diagnostics_DebuggerNonUserCodeAttribute,
        core_Diagnostics_DebuggerHiddenAttribute,
        core_Diagnostics_DebuggerBrowsableAttribute,
        core_Diagnostics_DebuggerStepThroughAttribute,
        core_Diagnostics_DebuggerBrowsableState,
        core_Diagnostics_DebuggableAttribute,
        core_Diagnostics_DebuggableAttribute__DebuggingModes,

        core_ComponentModel_DesignerSerializationVisibilityAttribute,

        core_IEquatable_T,

        core_Collections_IList,
        core_Collections_ICollection,
        core_Collections_Generic_EqualityComparer_T,
        core_Collections_Generic_List_T,
        core_Collections_Generic_IDictionary_KV,
        core_Collections_Generic_IReadOnlyDictionary_KV,
        core_Collections_ObjectModel_Collection_T,
        core_Collections_ObjectModel_ReadOnlyCollection_T,
        core_Collections_Specialized_INotifyCollectionChanged,
        core_ComponentModel_INotifyPropertyChanged,
        core_ComponentModel_EditorBrowsableAttribute,
        core_ComponentModel_EditorBrowsableState,

        core_Linq_IQueryable,
        core_Linq_IQueryable_T,

        core_Xml_Linq_Extensions,
        core_Xml_Linq_XAttribute,
        core_Xml_Linq_XCData,
        core_Xml_Linq_XComment,
        core_Xml_Linq_XContainer,
        core_Xml_Linq_XDeclaration,
        core_Xml_Linq_XDocument,
        core_Xml_Linq_XElement,
        core_Xml_Linq_XName,
        core_Xml_Linq_XNamespace,
        core_Xml_Linq_XObject,
        core_Xml_Linq_XProcessingInstruction,

        core_Security_UnverifiableCodeAttribute,
        core_Security_Permissions_SecurityAction,
        core_Security_Permissions_SecurityAttribute,
        core_Security_Permissions_SecurityPermissionAttribute,

        core_NotSupportedException,

        core_runtime_compiler_ICriticalNotifyCompletion,
        core_runtime_compiler_IAsyncStateMachine,
        core_runtime_compiler_AsyncVoidMethodBuilder,
        core_runtime_compiler_AsyncTaskMethodBuilder,
        core_runtime_compiler_AsyncTaskMethodBuilder_T,
        core_runtime_compiler_AsyncStateMachineAttribute,
        core_runtime_compiler_IteratorStateMachineAttribute,

        core_Windows_Forms_Form,
        core_Windows_Forms_Application,

        core_Environment,

        core_Runtime_GCLatencyMode,
        core_IFormatProvider,

        CSharp7Sentinel = core_IFormatProvider, // all types that were known before CSharp7 should remain above this sentinel

        core_ValueTuple_T1,
        core_ValueTuple_T2,
        core_ValueTuple_T3,
        core_ValueTuple_T4,
        core_ValueTuple_T5,
        core_ValueTuple_T6,

        ExtSentinel, // Not a real type, just a marker for types above 255 and strictly below 512

        core_ValueTuple_T7,
        core_ValueTuple_TRest,

        core_runtime_compiler_TupleElementNamesAttribute,

        Microsoft_CodeAnalysis_Runtime_Instrumentation,
        core_runtime_compiler_NullableAttribute,
        core_runtime_compiler_ReferenceAssemblyAttribute,

        core_runtime_ReadOnlyAttribute,
        core_runtime_ByRefLikeAttribute,
        core_runtime_InAttribute,
        core_ObsoleteAttribute,
        core_Span_T,
        core_ReadOnlySpan_T,
        core_runtime_UnmanagedType,
        core_runtime_UnmanagedAttribute,

        core_runtime_compiler_NonNullTypesAttribute,
        core_AttributeTargets,
        Microsoft_CodeAnalysis_EmbeddedAttribute,
        core_runtime_compiler_ITuple,

        core_Index,
        core_Range,

        core_runtime_compiler_AsyncIteratorStateMachineAttribute,
        core_IAsyncDisposable,
        core_Collections_Generic_IAsyncEnumerable_T,
        core_Collections_Generic_IAsyncEnumerator_T,
        core_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T,
        core_Threading_Tasks_Sources_ValueTaskSourceStatus,
        core_Threading_Tasks_Sources_ValueTaskSourceOnCompletedFlags,
        core_Threading_Tasks_Sources_IValueTaskSource_T,
        core_Threading_Tasks_Sources_IValueTaskSource,
        core_Threading_Tasks_ValueTask_T,
        core_Threading_Tasks_ValueTask,
        core_runtime_compiler_AsyncIteratorMethodBuilder,
        core_Threading_CancellationToken,

        core_InvalidOperationException,
        core_runtime_compiler_SwitchExpressionException,

        NextAvailable,

        // Remember to update the AllWellKnownTypes tests when making changes here
    }

    internal static class WellKnownTypes
    {
        /// <summary>
        /// Number of well known types in WellKnownType enum
        /// </summary>
        internal const int Count = WellKnownType.NextAvailable - WellKnownType.First;

        /// <summary>   
        /// Array of names for types.
        /// The names should correspond to ids from WellKnownType enum so
        /// that we could use ids to index into the array
        /// </summary>
        /// <remarks></remarks>
        private static readonly string[] s_metadataNames = new string[]
        {
            "core.Math",
            "core.Array",
            "core.Attribute",
            "core.CLSCompliantAttribute",
            "core.Convert",
            "core.Exception",
            "core.FlagsAttribute",
            "core.FormattableString",
            "core.Guid",
            "core.IFormattable",
            "core.RuntimeTypeHandle",
            "core.RuntimeFieldHandle",
            "core.RuntimeMethodHandle",
            "core.MarshalByRefObject",
            "core.Type",
            "core.Reflection.AssemblyKeyFileAttribute",
            "core.Reflection.AssemblyKeyNameAttribute",
            "core.Reflection.MethodInfo",
            "core.Reflection.ConstructorInfo",
            "core.Reflection.MethodBase",
            "core.Reflection.FieldInfo",
            "core.Reflection.MemberInfo",
            "core.Reflection.Missing",
            "core.runtime.compiler.FormattableStringFactory",
            "core.runtime.compiler.RuntimeHelpers",
            "core.Runtime.ExceptionServices.ExceptionDispatchInfo",
            "core.runtime.StructLayoutAttribute",
            "core.runtime.UnknownWrapper",
            "core.runtime.DispatchWrapper",
            "core.runtime.CallingConvention",
            "core.runtime.ClassInterfaceAttribute",
            "core.runtime.ClassInterfaceType",
            "core.runtime.CoClassAttribute",
            "core.runtime.ComAwareEventInfo",
            "core.runtime.ComEventInterfaceAttribute",
            "core.runtime.ComInterfaceType",
            "core.runtime.ComSourceInterfacesAttribute",
            "core.runtime.ComVisibleAttribute",
            "core.runtime.DispIdAttribute",
            "core.runtime.GuidAttribute",
            "core.runtime.InterfaceTypeAttribute",
            "core.runtime.Marshal",
            "core.runtime.TypeIdentifierAttribute",
            "core.runtime.BestFitMappingAttribute",
            "core.runtime.DefaultParameterValueAttribute",
            "core.runtime.LCIDConversionAttribute",
            "core.runtime.UnmanagedFunctionPointerAttribute",
            "core.Activator",
            "core.Threading.Tasks.Task",
            "core.Threading.Tasks.Task`1",
            "core.Threading.Interlocked",
            "core.Threading.Monitor",
            "core.Threading.Thread",

            "core.Func`1",
            "core.Func`2",
            "core.Func`3",
            "core.Func`4",
            "core.Func`5",
            "core.Func`6",
            "core.Func`7",
            "core.Func`8",
            "core.Func`9",
            "core.Func`10",
            "core.Func`11",
            "core.Func`12",
            "core.Func`13",
            "core.Func`14",
            "core.Func`15",
            "core.Func`16",
            "core.Func`17",
            "core.Action",
            "core.Action`1",
            "core.Action`2",
            "core.Action`3",
            "core.Action`4",
            "core.Action`5",
            "core.Action`6",
            "core.Action`7",
            "core.Action`8",
            "core.Action`9",
            "core.Action`10",
            "core.Action`11",
            "core.Action`12",
            "core.Action`13",
            "core.Action`14",
            "core.Action`15",
            "core.Action`16",

            "core.AttributeUsageAttribute",
            "core.ParamArrayAttribute",
            "core.NonSerializedAttribute",
            "core.STAThreadAttribute",
            "core.Reflection.DefaultMemberAttribute",
            "core.runtime.compiler.DateTimeConstantAttribute",
            "core.runtime.compiler.IUnknownConstantAttribute",
            "core.runtime.compiler.IDispatchConstantAttribute",
            "core.runtime.compiler.ExtensionAttribute",
            "core.runtime.compiler.INotifyCompletion",
            "core.runtime.compiler.InternalsVisibleToAttribute",
            "core.runtime.compiler.CompilerGeneratedAttribute",
            "core.runtime.compiler.AccessedThroughPropertyAttribute",
            "core.runtime.compiler.CompilationRelaxationsAttribute",
            "core.runtime.compiler.RuntimeCompatibilityAttribute",
            "core.runtime.compiler.UnsafeValueTypeAttribute",
            "core.runtime.compiler.FixedBufferAttribute",
            "core.runtime.compiler.DynamicAttribute",
            "core.runtime.compiler.CallSiteBinder",
            "core.runtime.compiler.CallSite",
            "core.runtime.compiler.CallSite`1",

            "core.runtime.WindowsRuntime.EventRegistrationToken",
            "core.runtime.WindowsRuntime.EventRegistrationTokenTable`1",
            "core.runtime.WindowsRuntime.WindowsRuntimeMarshal",

            "Windows.Foundation.IAsyncAction",
            "Windows.Foundation.IAsyncActionWithProgress`1",
            "Windows.Foundation.IAsyncOperation`1",
            "Windows.Foundation.IAsyncOperationWithProgress`2",

            "core.Diagnostics.Debugger",
            "core.Diagnostics.DebuggerDisplayAttribute",
            "core.Diagnostics.DebuggerNonUserCodeAttribute",
            "core.Diagnostics.DebuggerHiddenAttribute",
            "core.Diagnostics.DebuggerBrowsableAttribute",
            "core.Diagnostics.DebuggerStepThroughAttribute",
            "core.Diagnostics.DebuggerBrowsableState",
            "core.Diagnostics.DebuggableAttribute",
            "core.Diagnostics.DebuggableAttribute+DebuggingModes",

            "core.ComponentModel.DesignerSerializationVisibilityAttribute",

            "core.IEquatable`1",

            "core.Collections.IList",
            "core.Collections.ICollection",
            "core.Collections.Generic.EqualityComparer`1",
            "core.Collections.Generic.List`1",
            "core.Collections.Generic.IDictionary`2",
            "core.Collections.Generic.IReadOnlyDictionary`2",
            "core.Collections.ObjectModel.Collection`1",
            "core.Collections.ObjectModel.ReadOnlyCollection`1",
            "core.Collections.Specialized.INotifyCollectionChanged",
            "core.ComponentModel.INotifyPropertyChanged",
            "core.ComponentModel.EditorBrowsableAttribute",
            "core.ComponentModel.EditorBrowsableState",

            "core.Linq.IQueryable",
            "core.Linq.IQueryable`1",

            "core.Xml.Linq.Extensions",
            "core.Xml.Linq.XAttribute",
            "core.Xml.Linq.XCData",
            "core.Xml.Linq.XComment",
            "core.Xml.Linq.XContainer",
            "core.Xml.Linq.XDeclaration",
            "core.Xml.Linq.XDocument",
            "core.Xml.Linq.XElement",
            "core.Xml.Linq.XName",
            "core.Xml.Linq.XNamespace",
            "core.Xml.Linq.XObject",
            "core.Xml.Linq.XProcessingInstruction",

            "core.Security.UnverifiableCodeAttribute",
            "core.Security.Permissions.SecurityAction",
            "core.Security.Permissions.SecurityAttribute",
            "core.Security.Permissions.SecurityPermissionAttribute",

            "core.NotSupportedException",

            "core.runtime.compiler.ICriticalNotifyCompletion",
            "core.runtime.compiler.IAsyncStateMachine",
            "core.runtime.compiler.AsyncVoidMethodBuilder",
            "core.runtime.compiler.AsyncTaskMethodBuilder",
            "core.runtime.compiler.AsyncTaskMethodBuilder`1",
            "core.runtime.compiler.AsyncStateMachineAttribute",
            "core.runtime.compiler.IteratorStateMachineAttribute",

            "core.Windows.Forms.Form",
            "core.Windows.Forms.Application",

            "core.Environment",

            "core.Runtime.GCLatencyMode",

            "core.IFormatProvider",

            "core.ValueTuple`1",
            "core.ValueTuple`2",
            "core.ValueTuple`3",
            "core.ValueTuple`4",
            "core.ValueTuple`5",
            "core.ValueTuple`6",

            "", // extension marker

            "core.ValueTuple`7",
            "core.ValueTuple`8",

            "core.runtime.compiler.TupleElementNamesAttribute",

            "Microsoft.CodeAnalysis.Runtime.Instrumentation",

            "core.runtime.compiler.NullableAttribute",
            "core.runtime.compiler.ReferenceAssemblyAttribute",

            "core.runtime.ReadOnlyAttribute",
            "core.runtime.ByRefLikeAttribute",
            "core.runtime.InAttribute",
            "core.ObsoleteAttribute",
            "core.Span`1",
            "core.ReadOnlySpan`1",
            "core.runtime.UnmanagedType",
            "core.runtime.UnmanagedAttribute",

            "core.runtime.compiler.NonNullTypesAttribute",
            "core.AttributeTargets",
            "Microsoft.CodeAnalysis.EmbeddedAttribute",
            "core.runtime.compiler.ITuple",

            "core.Index",
            "core.Range",

            "core.runtime.compiler.AsyncIteratorStateMachineAttribute",
            "core.IAsyncDisposable",
            "core.Collections.Generic.IAsyncEnumerable`1",
            "core.Collections.Generic.IAsyncEnumerator`1",
            "core.Threading.Tasks.Sources.ManualResetValueTaskSourceCore`1",
            "core.Threading.Tasks.Sources.ValueTaskSourceStatus",
            "core.Threading.Tasks.Sources.ValueTaskSourceOnCompletedFlags",
            "core.Threading.Tasks.Sources.IValueTaskSource`1",
            "core.Threading.Tasks.Sources.IValueTaskSource",
            "core.Threading.Tasks.ValueTask`1",
            "core.Threading.Tasks.ValueTask",
            "core.runtime.compiler.AsyncIteratorMethodBuilder",
            "core.Threading.CancellationToken",

            "core.InvalidOperationException",
            "core.runtime.compiler.SwitchExpressionException"
        };

        private readonly static Dictionary<string, WellKnownType> s_nameToTypeIdMap = new Dictionary<string, WellKnownType>((int)Count);

        static WellKnownTypes()
        {
            AssertEnumAndTableInSync();

            for (int i = 0; i < s_metadataNames.Length; i++)
            {
                var name = s_metadataNames[i];
                var typeId = (WellKnownType)(i + WellKnownType.First);
                s_nameToTypeIdMap.Add(name, typeId);
            }
        }

        [Conditional("DEBUG")]
        private static void AssertEnumAndTableInSync()
        {
            for (int i = 0; i < s_metadataNames.Length; i++)
            {
                var name = s_metadataNames[i];
                var typeId = (WellKnownType)(i + WellKnownType.First);

                string typeIdName;
                switch (typeId)
                {
                    case WellKnownType.First:
                        typeIdName = "core.Math";
                        break;
                    case WellKnownType.CSharp7Sentinel:
                        typeIdName = "core.IFormatProvider";
                        break;
                    case WellKnownType.ExtSentinel:
                        typeIdName = "";
                        break;
                    default:
                        typeIdName = typeId.ToString().Replace("__", "+").Replace('_', '.');
                        break;
                }

                int separator = name.IndexOf('`');
                if (separator >= 0)
                {
                    // Ignore type parameter qualifier for generic types.
                    name = name.Substring(0, separator);
                    typeIdName = typeIdName.Substring(0, separator);
                }

                Debug.Assert(name == typeIdName, $"Enum name and type name must match {name} != {typeIdName}");
            }

            Debug.Assert((int)WellKnownType.ExtSentinel == 214, $"Unexpected WellKnownType.ExtSentinel {(int)WellKnownType.ExtSentinel}");
            Debug.Assert((int)WellKnownType.NextAvailable <= 512, "Time for a new sentinel");
        }

        public static bool IsWellKnownType(this WellKnownType typeId)
        {
            Debug.Assert(typeId != WellKnownType.ExtSentinel);
            return typeId >= WellKnownType.First && typeId < WellKnownType.NextAvailable;
        }

        public static bool IsValueTupleType(this WellKnownType typeId)
        {
            Debug.Assert(typeId != WellKnownType.ExtSentinel);
            return typeId >= WellKnownType.core_ValueTuple_T1 && typeId <= WellKnownType.core_ValueTuple_TRest;
        }

        public static bool IsValid(this WellKnownType typeId)
        {
            return typeId >= WellKnownType.First && typeId < WellKnownType.NextAvailable && typeId != WellKnownType.ExtSentinel;
        }

        public static string GetMetadataName(this WellKnownType id)
        {
            return s_metadataNames[(int)(id - WellKnownType.First)];
        }

        public static WellKnownType GetTypeFromMetadataName(string metadataName)
        {
            WellKnownType id;

            if (s_nameToTypeIdMap.TryGetValue(metadataName, out id))
            {
                return id;
            }

            Debug.Assert(WellKnownType.First != 0);
            return WellKnownType.Unknown;
        }

        // returns WellKnownType.Unknown if given arity isn't available:
        internal static WellKnownType GetWellKnownFunctionDelegate(int invokeArgumentCount)
        {
            Debug.Assert(invokeArgumentCount >= 0);
            return (invokeArgumentCount <= WellKnownType.core_Func_TMax - WellKnownType.core_Func_T) ?
                (WellKnownType)((int)WellKnownType.core_Func_T + invokeArgumentCount) :
                WellKnownType.Unknown;
        }

        // returns WellKnownType.Unknown if given arity isn't available:
        internal static WellKnownType GetWellKnownActionDelegate(int invokeArgumentCount)
        {
            Debug.Assert(invokeArgumentCount >= 0);

            return (invokeArgumentCount <= WellKnownType.core_Action_TMax - WellKnownType.core_Action) ?
                (WellKnownType)((int)WellKnownType.core_Action + invokeArgumentCount) :
                WellKnownType.Unknown;
        }
    }
}
