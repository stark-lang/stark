// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis
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
        system_Math = First,
        system_Array,
        system_Attribute,
        system_CLSCompliantAttribute,
        system_Convert,
        system_Exception,
        system_FlagsAttribute,
        system_FormattableString,
        system_Guid,
        system_IFormattable,
        system_RuntimeTypeHandle,
        system_RuntimeFieldHandle,
        system_RuntimeMethodHandle,
        system_MarshalByRefObject,
        system_Type,
        system_Reflection_AssemblyKeyFileAttribute,
        system_Reflection_AssemblyKeyNameAttribute,
        system_Reflection_MethodInfo,
        system_Reflection_ConstructorInfo,
        system_Reflection_MethodBase,
        system_Reflection_FieldInfo,
        system_Reflection_MemberInfo,
        system_Reflection_Missing,
        system_runtime_compiler_FormattableStringFactory,
        system_runtime_compiler_RuntimeHelpers,
        system_Runtime_ExceptionServices_ExceptionDispatchInfo,
        system_runtime_StructLayoutAttribute,
        system_runtime_UnknownWrapper,
        system_runtime_DispatchWrapper,
        system_runtime_CallingConvention,
        system_runtime_ClassInterfaceAttribute,
        system_runtime_ClassInterfaceType,
        system_runtime_CoClassAttribute,
        system_runtime_ComAwareEventInfo,
        system_runtime_ComEventInterfaceAttribute,
        system_runtime_ComInterfaceType,
        system_runtime_ComSourceInterfacesAttribute,
        system_runtime_ComVisibleAttribute,
        system_runtime_DispIdAttribute,
        system_runtime_GuidAttribute,
        system_runtime_InterfaceTypeAttribute,
        system_runtime_Marshal,
        system_runtime_TypeIdentifierAttribute,
        system_runtime_BestFitMappingAttribute,
        system_runtime_DefaultParameterValueAttribute,
        system_runtime_LCIDConversionAttribute,
        system_runtime_UnmanagedFunctionPointerAttribute,
        system_Activator,
        system_Threading_Tasks_Task,
        system_Threading_Tasks_Task_T,
        system_Threading_Interlocked,
        system_Threading_Monitor,
        system_Threading_Thread,
        Microsoft_CSharp_RuntimeBinder_Binder,
        Microsoft_CSharp_RuntimeBinder_CSharpArgumentInfo,
        Microsoft_CSharp_RuntimeBinder_CSharpArgumentInfoFlags,
        Microsoft_CSharp_RuntimeBinder_CSharpBinderFlags,
        Microsoft_VisualBasic_CallType,
        Microsoft_VisualBasic_Embedded,
        Microsoft_VisualBasic_CompilerServices_Conversions,
        Microsoft_VisualBasic_CompilerServices_Operators,
        Microsoft_VisualBasic_CompilerServices_NewLateBinding,
        Microsoft_VisualBasic_CompilerServices_EmbeddedOperators,
        Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute,
        Microsoft_VisualBasic_CompilerServices_Utils,
        Microsoft_VisualBasic_CompilerServices_LikeOperator,
        Microsoft_VisualBasic_CompilerServices_ProjectData,
        Microsoft_VisualBasic_CompilerServices_ObjectFlowControl,
        Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl,
        Microsoft_VisualBasic_CompilerServices_StaticLocalInitFlag,
        Microsoft_VisualBasic_CompilerServices_StringType,
        Microsoft_VisualBasic_CompilerServices_IncompleteInitialization,
        Microsoft_VisualBasic_CompilerServices_Versioned,
        Microsoft_VisualBasic_CompareMethod,
        Microsoft_VisualBasic_Strings,
        Microsoft_VisualBasic_ErrObject,
        Microsoft_VisualBasic_FileSystem,
        Microsoft_VisualBasic_ApplicationServices_ApplicationBase,
        Microsoft_VisualBasic_ApplicationServices_WindowsFormsApplicationBase,
        Microsoft_VisualBasic_Information,
        Microsoft_VisualBasic_Interaction,

        // standard Func delegates - must be ordered by arity
        system_Func_T,
        system_Func_T2,
        system_Func_T3,
        system_Func_T4,
        system_Func_T5,
        system_Func_T6,
        system_Func_T7,
        system_Func_T8,
        system_Func_T9,
        system_Func_T10,
        system_Func_T11,
        system_Func_T12,
        system_Func_T13,
        system_Func_T14,
        system_Func_T15,
        system_Func_T16,
        system_Func_T17,
        system_Func_TMax = system_Func_T17,

        // standard Action delegates - must be ordered by arity
        system_Action,
        system_Action_T,
        system_Action_T2,
        system_Action_T3,
        system_Action_T4,
        system_Action_T5,
        system_Action_T6,
        system_Action_T7,
        system_Action_T8,
        system_Action_T9,
        system_Action_T10,
        system_Action_T11,
        system_Action_T12,
        system_Action_T13,
        system_Action_T14,
        system_Action_T15,
        system_Action_T16,
        system_Action_TMax = system_Action_T16,

        system_AttributeUsageAttribute,
        system_ParamArrayAttribute,
        system_NonSerializedAttribute,
        system_STAThreadAttribute,
        system_Reflection_DefaultMemberAttribute,
        system_runtime_compiler_DateTimeConstantAttribute,
        system_runtime_compiler_DecimalConstantAttribute,
        system_runtime_compiler_IUnknownConstantAttribute,
        system_runtime_compiler_IDispatchConstantAttribute,
        system_runtime_compiler_ExtensionAttribute,
        system_runtime_compiler_INotifyCompletion,
        system_runtime_compiler_InternalsVisibleToAttribute,
        system_runtime_compiler_CompilerGeneratedAttribute,
        system_runtime_compiler_AccessedThroughPropertyAttribute,
        system_runtime_compiler_CompilationRelaxationsAttribute,
        system_runtime_compiler_RuntimeCompatibilityAttribute,
        system_runtime_compiler_UnsafeValueTypeAttribute,
        system_runtime_compiler_FixedBufferAttribute,
        system_runtime_compiler_DynamicAttribute,
        system_runtime_compiler_CallSiteBinder,
        system_runtime_compiler_CallSite,
        system_runtime_compiler_CallSite_T,

        system_runtime_WindowsRuntime_EventRegistrationToken,
        system_runtime_WindowsRuntime_EventRegistrationTokenTable_T,
        system_runtime_WindowsRuntime_WindowsRuntimeMarshal,

        Windows_Foundation_IAsyncAction,
        Windows_Foundation_IAsyncActionWithProgress_T,
        Windows_Foundation_IAsyncOperation_T,
        Windows_Foundation_IAsyncOperationWithProgress_T2,

        system_Diagnostics_Debugger,
        system_Diagnostics_DebuggerDisplayAttribute,
        system_Diagnostics_DebuggerNonUserCodeAttribute,
        system_Diagnostics_DebuggerHiddenAttribute,
        system_Diagnostics_DebuggerBrowsableAttribute,
        system_Diagnostics_DebuggerStepThroughAttribute,
        system_Diagnostics_DebuggerBrowsableState,
        system_Diagnostics_DebuggableAttribute,
        system_Diagnostics_DebuggableAttribute__DebuggingModes,

        system_ComponentModel_DesignerSerializationVisibilityAttribute,

        system_IEquatable_T,

        system_Collections_IList,
        system_Collections_ICollection,
        system_Collections_Generic_EqualityComparer_T,
        system_Collections_Generic_List_T,
        system_Collections_Generic_IDictionary_KV,
        system_Collections_Generic_IReadOnlyDictionary_KV,
        system_Collections_ObjectModel_Collection_T,
        system_Collections_ObjectModel_ReadOnlyCollection_T,
        system_Collections_Specialized_INotifyCollectionChanged,
        system_ComponentModel_INotifyPropertyChanged,
        system_ComponentModel_EditorBrowsableAttribute,
        system_ComponentModel_EditorBrowsableState,

        system_Linq_Enumerable,
        system_Linq_Expressions_Expression,
        system_Linq_Expressions_Expression_T,
        system_Linq_Expressions_ParameterExpression,
        system_Linq_Expressions_ElementInit,
        system_Linq_Expressions_MemberBinding,
        system_Linq_Expressions_ExpressionType,
        system_Linq_IQueryable,
        system_Linq_IQueryable_T,

        system_Xml_Linq_Extensions,
        system_Xml_Linq_XAttribute,
        system_Xml_Linq_XCData,
        system_Xml_Linq_XComment,
        system_Xml_Linq_XContainer,
        system_Xml_Linq_XDeclaration,
        system_Xml_Linq_XDocument,
        system_Xml_Linq_XElement,
        system_Xml_Linq_XName,
        system_Xml_Linq_XNamespace,
        system_Xml_Linq_XObject,
        system_Xml_Linq_XProcessingInstruction,

        system_Security_UnverifiableCodeAttribute,
        system_Security_Permissions_SecurityAction,
        system_Security_Permissions_SecurityAttribute,
        system_Security_Permissions_SecurityPermissionAttribute,

        system_NotSupportedException,

        system_runtime_compiler_ICriticalNotifyCompletion,
        system_runtime_compiler_IAsyncStateMachine,
        system_runtime_compiler_AsyncVoidMethodBuilder,
        system_runtime_compiler_AsyncTaskMethodBuilder,
        system_runtime_compiler_AsyncTaskMethodBuilder_T,
        system_runtime_compiler_AsyncStateMachineAttribute,
        system_runtime_compiler_IteratorStateMachineAttribute,

        system_Windows_Forms_Form,
        system_Windows_Forms_Application,

        system_Environment,

        system_Runtime_GCLatencyMode,
        system_IFormatProvider,

        CSharp7Sentinel = system_IFormatProvider, // all types that were known before CSharp7 should remain above this sentinel

        system_ValueTuple_T1,
        system_ValueTuple_T2,
        system_ValueTuple_T3,
        system_ValueTuple_T4,
        system_ValueTuple_T5,
        system_ValueTuple_T6,

        ExtSentinel, // Not a real type, just a marker for types above 255 and strictly below 512

        system_ValueTuple_T7,
        system_ValueTuple_TRest,

        system_runtime_compiler_TupleElementNamesAttribute,

        Microsoft_CodeAnalysis_Runtime_Instrumentation,
        system_runtime_compiler_NullableAttribute,
        system_runtime_compiler_ReferenceAssemblyAttribute,

        system_runtime_ReadOnlyAttribute,
        system_runtime_ByRefLikeAttribute,
        system_runtime_InAttribute,
        system_ObsoleteAttribute,
        system_Span_T,
        system_ReadOnlySpan_T,
        system_runtime_UnmanagedType,
        system_runtime_UnmanagedAttribute,

        Microsoft_VisualBasic_Conversion,
        system_runtime_compiler_NonNullTypesAttribute,
        system_AttributeTargets,
        Microsoft_CodeAnalysis_EmbeddedAttribute,
        system_runtime_compiler_ITuple,

        system_Index,
        system_Range,

        system_runtime_compiler_AsyncIteratorStateMachineAttribute,
        system_IAsyncDisposable,
        system_Collections_Generic_IAsyncEnumerable_T,
        system_Collections_Generic_IAsyncEnumerator_T,
        system_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T,
        system_Threading_Tasks_Sources_ValueTaskSourceStatus,
        system_Threading_Tasks_Sources_ValueTaskSourceOnCompletedFlags,
        system_Threading_Tasks_Sources_IValueTaskSource_T,
        system_Threading_Tasks_Sources_IValueTaskSource,
        system_Threading_Tasks_ValueTask_T,
        system_Threading_Tasks_ValueTask,
        system_runtime_compiler_AsyncIteratorMethodBuilder,
        system_Threading_CancellationToken,

        system_InvalidOperationException,
        system_runtime_compiler_SwitchExpressionException,

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
            "system.Math",
            "system.Array",
            "system.Attribute",
            "system.CLSCompliantAttribute",
            "system.Convert",
            "system.Exception",
            "system.FlagsAttribute",
            "system.FormattableString",
            "system.Guid",
            "system.IFormattable",
            "system.RuntimeTypeHandle",
            "system.RuntimeFieldHandle",
            "system.RuntimeMethodHandle",
            "system.MarshalByRefObject",
            "system.Type",
            "system.Reflection.AssemblyKeyFileAttribute",
            "system.Reflection.AssemblyKeyNameAttribute",
            "system.Reflection.MethodInfo",
            "system.Reflection.ConstructorInfo",
            "system.Reflection.MethodBase",
            "system.Reflection.FieldInfo",
            "system.Reflection.MemberInfo",
            "system.Reflection.Missing",
            "system.runtime.compiler.FormattableStringFactory",
            "system.runtime.compiler.RuntimeHelpers",
            "system.Runtime.ExceptionServices.ExceptionDispatchInfo",
            "system.runtime.StructLayoutAttribute",
            "system.runtime.UnknownWrapper",
            "system.runtime.DispatchWrapper",
            "system.runtime.CallingConvention",
            "system.runtime.ClassInterfaceAttribute",
            "system.runtime.ClassInterfaceType",
            "system.runtime.CoClassAttribute",
            "system.runtime.ComAwareEventInfo",
            "system.runtime.ComEventInterfaceAttribute",
            "system.runtime.ComInterfaceType",
            "system.runtime.ComSourceInterfacesAttribute",
            "system.runtime.ComVisibleAttribute",
            "system.runtime.DispIdAttribute",
            "system.runtime.GuidAttribute",
            "system.runtime.InterfaceTypeAttribute",
            "system.runtime.Marshal",
            "system.runtime.TypeIdentifierAttribute",
            "system.runtime.BestFitMappingAttribute",
            "system.runtime.DefaultParameterValueAttribute",
            "system.runtime.LCIDConversionAttribute",
            "system.runtime.UnmanagedFunctionPointerAttribute",
            "system.Activator",
            "system.Threading.Tasks.Task",
            "system.Threading.Tasks.Task`1",
            "system.Threading.Interlocked",
            "system.Threading.Monitor",
            "system.Threading.Thread",
            "Microsoft.CSharp.RuntimeBinder.Binder",
            "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo",
            "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags",
            "Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags",
            "Microsoft.VisualBasic.CallType",
            "Microsoft.VisualBasic.Embedded",
            "Microsoft.VisualBasic.CompilerServices.Conversions",
            "Microsoft.VisualBasic.CompilerServices.Operators",
            "Microsoft.VisualBasic.CompilerServices.NewLateBinding",
            "Microsoft.VisualBasic.CompilerServices.EmbeddedOperators",
            "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute",
            "Microsoft.VisualBasic.CompilerServices.Utils",
            "Microsoft.VisualBasic.CompilerServices.LikeOperator",
            "Microsoft.VisualBasic.CompilerServices.ProjectData",
            "Microsoft.VisualBasic.CompilerServices.ObjectFlowControl",
            "Microsoft.VisualBasic.CompilerServices.ObjectFlowControl+ForLoopControl",
            "Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag",
            "Microsoft.VisualBasic.CompilerServices.StringType",
            "Microsoft.VisualBasic.CompilerServices.IncompleteInitialization",
            "Microsoft.VisualBasic.CompilerServices.Versioned",
            "Microsoft.VisualBasic.CompareMethod",
            "Microsoft.VisualBasic.Strings",
            "Microsoft.VisualBasic.ErrObject",
            "Microsoft.VisualBasic.FileSystem",
            "Microsoft.VisualBasic.ApplicationServices.ApplicationBase",
            "Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase",
            "Microsoft.VisualBasic.Information",
            "Microsoft.VisualBasic.Interaction",

            "system.Func`1",
            "system.Func`2",
            "system.Func`3",
            "system.Func`4",
            "system.Func`5",
            "system.Func`6",
            "system.Func`7",
            "system.Func`8",
            "system.Func`9",
            "system.Func`10",
            "system.Func`11",
            "system.Func`12",
            "system.Func`13",
            "system.Func`14",
            "system.Func`15",
            "system.Func`16",
            "system.Func`17",
            "system.Action",
            "system.Action`1",
            "system.Action`2",
            "system.Action`3",
            "system.Action`4",
            "system.Action`5",
            "system.Action`6",
            "system.Action`7",
            "system.Action`8",
            "system.Action`9",
            "system.Action`10",
            "system.Action`11",
            "system.Action`12",
            "system.Action`13",
            "system.Action`14",
            "system.Action`15",
            "system.Action`16",

            "system.AttributeUsageAttribute",
            "system.ParamArrayAttribute",
            "system.NonSerializedAttribute",
            "system.STAThreadAttribute",
            "system.Reflection.DefaultMemberAttribute",
            "system.runtime.compiler.DateTimeConstantAttribute",
            "system.runtime.compiler.DecimalConstantAttribute",
            "system.runtime.compiler.IUnknownConstantAttribute",
            "system.runtime.compiler.IDispatchConstantAttribute",
            "system.runtime.compiler.ExtensionAttribute",
            "system.runtime.compiler.INotifyCompletion",
            "system.runtime.compiler.InternalsVisibleToAttribute",
            "system.runtime.compiler.CompilerGeneratedAttribute",
            "system.runtime.compiler.AccessedThroughPropertyAttribute",
            "system.runtime.compiler.CompilationRelaxationsAttribute",
            "system.runtime.compiler.RuntimeCompatibilityAttribute",
            "system.runtime.compiler.UnsafeValueTypeAttribute",
            "system.runtime.compiler.FixedBufferAttribute",
            "system.runtime.compiler.DynamicAttribute",
            "system.runtime.compiler.CallSiteBinder",
            "system.runtime.compiler.CallSite",
            "system.runtime.compiler.CallSite`1",

            "system.runtime.WindowsRuntime.EventRegistrationToken",
            "system.runtime.WindowsRuntime.EventRegistrationTokenTable`1",
            "system.runtime.WindowsRuntime.WindowsRuntimeMarshal",

            "Windows.Foundation.IAsyncAction",
            "Windows.Foundation.IAsyncActionWithProgress`1",
            "Windows.Foundation.IAsyncOperation`1",
            "Windows.Foundation.IAsyncOperationWithProgress`2",

            "system.Diagnostics.Debugger",
            "system.Diagnostics.DebuggerDisplayAttribute",
            "system.Diagnostics.DebuggerNonUserCodeAttribute",
            "system.Diagnostics.DebuggerHiddenAttribute",
            "system.Diagnostics.DebuggerBrowsableAttribute",
            "system.Diagnostics.DebuggerStepThroughAttribute",
            "system.Diagnostics.DebuggerBrowsableState",
            "system.Diagnostics.DebuggableAttribute",
            "system.Diagnostics.DebuggableAttribute+DebuggingModes",

            "system.ComponentModel.DesignerSerializationVisibilityAttribute",

            "system.IEquatable`1",

            "system.Collections.IList",
            "system.Collections.ICollection",
            "system.Collections.Generic.EqualityComparer`1",
            "system.Collections.Generic.List`1",
            "system.Collections.Generic.IDictionary`2",
            "system.Collections.Generic.IReadOnlyDictionary`2",
            "system.Collections.ObjectModel.Collection`1",
            "system.Collections.ObjectModel.ReadOnlyCollection`1",
            "system.Collections.Specialized.INotifyCollectionChanged",
            "system.ComponentModel.INotifyPropertyChanged",
            "system.ComponentModel.EditorBrowsableAttribute",
            "system.ComponentModel.EditorBrowsableState",

            "system.Linq.Enumerable",
            "system.Linq.Expressions.Expression",
            "system.Linq.Expressions.Expression`1",
            "system.Linq.Expressions.ParameterExpression",
            "system.Linq.Expressions.ElementInit",
            "system.Linq.Expressions.MemberBinding",
            "system.Linq.Expressions.ExpressionType",
            "system.Linq.IQueryable",
            "system.Linq.IQueryable`1",

            "system.Xml.Linq.Extensions",
            "system.Xml.Linq.XAttribute",
            "system.Xml.Linq.XCData",
            "system.Xml.Linq.XComment",
            "system.Xml.Linq.XContainer",
            "system.Xml.Linq.XDeclaration",
            "system.Xml.Linq.XDocument",
            "system.Xml.Linq.XElement",
            "system.Xml.Linq.XName",
            "system.Xml.Linq.XNamespace",
            "system.Xml.Linq.XObject",
            "system.Xml.Linq.XProcessingInstruction",

            "system.Security.UnverifiableCodeAttribute",
            "system.Security.Permissions.SecurityAction",
            "system.Security.Permissions.SecurityAttribute",
            "system.Security.Permissions.SecurityPermissionAttribute",

            "system.NotSupportedException",

            "system.runtime.compiler.ICriticalNotifyCompletion",
            "system.runtime.compiler.IAsyncStateMachine",
            "system.runtime.compiler.AsyncVoidMethodBuilder",
            "system.runtime.compiler.AsyncTaskMethodBuilder",
            "system.runtime.compiler.AsyncTaskMethodBuilder`1",
            "system.runtime.compiler.AsyncStateMachineAttribute",
            "system.runtime.compiler.IteratorStateMachineAttribute",

            "system.Windows.Forms.Form",
            "system.Windows.Forms.Application",

            "system.Environment",

            "system.Runtime.GCLatencyMode",

            "system.IFormatProvider",

            "system.ValueTuple`1",
            "system.ValueTuple`2",
            "system.ValueTuple`3",
            "system.ValueTuple`4",
            "system.ValueTuple`5",
            "system.ValueTuple`6",

            "", // extension marker

            "system.ValueTuple`7",
            "system.ValueTuple`8",

            "system.runtime.compiler.TupleElementNamesAttribute",

            "Microsoft.CodeAnalysis.Runtime.Instrumentation",

            "system.runtime.compiler.NullableAttribute",
            "system.runtime.compiler.ReferenceAssemblyAttribute",

            "system.runtime.ReadOnlyAttribute",
            "system.runtime.ByRefLikeAttribute",
            "system.runtime.InAttribute",
            "system.ObsoleteAttribute",
            "system.Span`1",
            "system.ReadOnlySpan`1",
            "system.runtime.UnmanagedType",
            "system.runtime.UnmanagedAttribute",

            "Microsoft.VisualBasic.Conversion",
            "system.runtime.compiler.NonNullTypesAttribute",
            "system.AttributeTargets",
            "Microsoft.CodeAnalysis.EmbeddedAttribute",
            "system.runtime.compiler.ITuple",

            "system.Index",
            "system.Range",

            "system.runtime.compiler.AsyncIteratorStateMachineAttribute",
            "system.IAsyncDisposable",
            "system.Collections.Generic.IAsyncEnumerable`1",
            "system.Collections.Generic.IAsyncEnumerator`1",
            "system.Threading.Tasks.Sources.ManualResetValueTaskSourceCore`1",
            "system.Threading.Tasks.Sources.ValueTaskSourceStatus",
            "system.Threading.Tasks.Sources.ValueTaskSourceOnCompletedFlags",
            "system.Threading.Tasks.Sources.IValueTaskSource`1",
            "system.Threading.Tasks.Sources.IValueTaskSource",
            "system.Threading.Tasks.ValueTask`1",
            "system.Threading.Tasks.ValueTask",
            "system.runtime.compiler.AsyncIteratorMethodBuilder",
            "system.Threading.CancellationToken",

            "system.InvalidOperationException",
            "system.runtime.compiler.SwitchExpressionException"
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
                        typeIdName = "system.Math";
                        break;
                    case WellKnownType.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl:
                        typeIdName = "Microsoft.VisualBasic.CompilerServices.ObjectFlowControl+ForLoopControl";
                        break;
                    case WellKnownType.CSharp7Sentinel:
                        typeIdName = "system.IFormatProvider";
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

            Debug.Assert((int)WellKnownType.ExtSentinel == 255);
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
            return typeId >= WellKnownType.system_ValueTuple_T1 && typeId <= WellKnownType.system_ValueTuple_TRest;
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
            return (invokeArgumentCount <= WellKnownType.system_Func_TMax - WellKnownType.system_Func_T) ?
                (WellKnownType)((int)WellKnownType.system_Func_T + invokeArgumentCount) :
                WellKnownType.Unknown;
        }

        // returns WellKnownType.Unknown if given arity isn't available:
        internal static WellKnownType GetWellKnownActionDelegate(int invokeArgumentCount)
        {
            Debug.Assert(invokeArgumentCount >= 0);

            return (invokeArgumentCount <= WellKnownType.system_Action_TMax - WellKnownType.system_Action) ?
                (WellKnownType)((int)WellKnownType.system_Action + invokeArgumentCount) :
                WellKnownType.Unknown;
        }
    }
}
