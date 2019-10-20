// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace StarkPlatform.Compiler
{
    // Members of well known types
    internal enum WellKnownMember
    {
        System_Math__RoundDouble,
        System_Math__PowDoubleDouble,

        System_Array__Empty,
        System_Array__Copy,

        System_Convert__ToBooleanInt32,
        System_Convert__ToBooleanUInt32,
        System_Convert__ToBooleanInt64,
        System_Convert__ToBooleanUInt64,
        System_Convert__ToBooleanSingle,
        System_Convert__ToBooleanDouble,
        System_Convert__ToSByteDouble,
        System_Convert__ToSByteSingle,
        System_Convert__ToByteDouble,
        System_Convert__ToByteSingle,
        System_Convert__ToInt16Double,
        System_Convert__ToInt16Single,
        System_Convert__ToUInt16Double,
        System_Convert__ToUInt16Single,
        System_Convert__ToInt32Double,
        System_Convert__ToInt32Single,
        System_Convert__ToUInt32Double,
        System_Convert__ToUInt32Single,
        System_Convert__ToInt64Double,
        System_Convert__ToInt64Single,
        System_Convert__ToUInt64Double,
        System_Convert__ToUInt64Single,

        System_CLSCompliantAttribute__ctor,
        System_FlagsAttribute__ctor,
        System_Guid__ctor,

        System_Type__GetTypeFromCLSID,
        System_Type__GetTypeFromHandle,
        System_Type__Missing,

        System_Reflection_AssemblyKeyFileAttribute__ctor,
        System_Reflection_AssemblyKeyNameAttribute__ctor,

        System_Reflection_MethodBase__GetMethodFromHandle,
        System_Reflection_MethodBase__GetMethodFromHandle2,
        System_Reflection_MethodInfo__CreateDelegate,
        System_Delegate__CreateDelegate,
        System_Delegate__CreateDelegate4,
        System_Reflection_FieldInfo__GetFieldFromHandle,
        System_Reflection_FieldInfo__GetFieldFromHandle2,

        System_Reflection_Missing__Value,

        System_IEquatable_T__Equals,

        System_Collections_Generic_EqualityComparer_T__Equals,
        System_Collections_Generic_EqualityComparer_T__GetHashCode,
        System_Collections_Generic_EqualityComparer_T__get_Default,

        System_AttributeUsageAttribute__ctor,
        System_AttributeUsageAttribute__AllowMultiple,
        System_AttributeUsageAttribute__Inherited,

        System_ParamArrayAttribute__ctor,
        System_STAThreadAttribute__ctor,

        System_Reflection_DefaultMemberAttribute__ctor,

        System_Diagnostics_Debugger__Break,
        System_Diagnostics_DebuggerDisplayAttribute__ctor,
        System_Diagnostics_DebuggerDisplayAttribute__Type,
        System_Diagnostics_DebuggerNonUserCodeAttribute__ctor,
        System_Diagnostics_DebuggerHiddenAttribute__ctor,
        System_Diagnostics_DebuggerBrowsableAttribute__ctor,
        System_Diagnostics_DebuggerStepThroughAttribute__ctor,
        System_Diagnostics_DebuggableAttribute__ctorDebuggingModes,
        System_Diagnostics_DebuggableAttribute_DebuggingModes__Default,
        System_Diagnostics_DebuggableAttribute_DebuggingModes__DisableOptimizations,
        System_Diagnostics_DebuggableAttribute_DebuggingModes__EnableEditAndContinue,
        System_Diagnostics_DebuggableAttribute_DebuggingModes__IgnoreSymbolStoreSequencePoints,

        System_Runtime_InteropServices_ClassInterfaceAttribute__ctorClassInterfaceType,
        System_Runtime_InteropServices_CoClassAttribute__ctor,
        System_Runtime_InteropServices_ComAwareEventInfo__ctor,
        System_Runtime_InteropServices_ComAwareEventInfo__AddEventHandler,
        System_Runtime_InteropServices_ComAwareEventInfo__RemoveEventHandler,
        System_Runtime_InteropServices_ComEventInterfaceAttribute__ctor,
        System_Runtime_InteropServices_ComSourceInterfacesAttribute__ctorString,
        System_Runtime_InteropServices_ComVisibleAttribute__ctor,
        System_Runtime_InteropServices_DispIdAttribute__ctor,
        System_Runtime_InteropServices_GuidAttribute__ctor,
        System_Runtime_InteropServices_InterfaceTypeAttribute__ctorComInterfaceType,
        System_Runtime_InteropServices_InterfaceTypeAttribute__ctorInt16,
        System_Runtime_InteropServices_Marshal__GetTypeFromCLSID,
        System_Runtime_InteropServices_TypeIdentifierAttribute__ctor,
        System_Runtime_InteropServices_TypeIdentifierAttribute__ctorStringString,
        System_Runtime_InteropServices_BestFitMappingAttribute__ctor,
        System_Runtime_InteropServices_DefaultParameterValueAttribute__ctor,
        System_Runtime_InteropServices_LCIDConversionAttribute__ctor,
        System_Runtime_InteropServices_UnmanagedFunctionPointerAttribute__ctor,

        System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__AddEventHandler,
        System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__GetOrCreateEventRegistrationTokenTable,
        System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__InvocationList,
        System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__RemoveEventHandler,

        System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__AddEventHandler_T,
        System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveAllEventHandlers,
        System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveEventHandler_T,

        System_Runtime_CompilerServices_ExtensionAttribute__ctor,
        System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor,
        System_Runtime_CompilerServices_AccessedThroughPropertyAttribute__ctor,
        System_Runtime_CompilerServices_CompilationRelaxationsAttribute__ctorInt32,
        System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__ctor,
        System_Runtime_CompilerServices_RuntimeCompatibilityAttribute__WrapNonExceptionThrows,
        System_Runtime_CompilerServices_UnsafeValueTypeAttribute__ctor,
        System_Runtime_CompilerServices_FixedBufferAttribute__ctor,
        System_Runtime_CompilerServices_DynamicAttribute__ctor,
        System_Runtime_CompilerServices_DynamicAttribute__ctorTransformFlags,
        System_Runtime_CompilerServices_CallSite_T__Create,
        System_Runtime_CompilerServices_CallSite_T__Target,

        System_Runtime_CompilerServices_RuntimeHelpers__GetObjectValueObject,
        System_Runtime_CompilerServices_RuntimeHelpers__InitializeArrayArrayRuntimeFieldHandle,
        System_Runtime_CompilerServices_RuntimeHelpers__get_OffsetToStringData,

        System_Runtime_ExceptionServices_ExceptionDispatchInfo__Capture,
        System_Runtime_ExceptionServices_ExceptionDispatchInfo__Throw,

        System_Security_UnverifiableCodeAttribute__ctor,
        System_Security_Permissions_SecurityAction__RequestMinimum,
        System_Security_Permissions_SecurityPermissionAttribute__ctor,
        System_Security_Permissions_SecurityPermissionAttribute__SkipVerification,

        System_Activator__CreateInstance,
        System_Activator__CreateInstance_T,

        System_Threading_Interlocked__CompareExchange_T,

        System_Threading_Monitor__Enter, //Monitor.Enter(object)
        System_Threading_Monitor__Enter2, //Monitor.Enter(object, bool&)
        System_Threading_Monitor__Exit,

        System_Threading_Thread__CurrentThread,
        System_Threading_Thread__ManagedThreadId,

        System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext,
        System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine,

        System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Create,
        System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetException,
        System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetResult,
        System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitOnCompleted,
        System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitUnsafeOnCompleted,
        System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Start_T,
        System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetStateMachine,

        System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Create,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetException,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetResult,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitOnCompleted,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitUnsafeOnCompleted,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Start_T,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetStateMachine,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Task,

        System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Create,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetException,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetResult,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitOnCompleted,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitUnsafeOnCompleted,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Start_T,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetStateMachine,
        System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Task,

        System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor,
        System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor,

        System_Xml_Linq_XElement__ctor,
        System_Xml_Linq_XElement__ctor2,
        System_Xml_Linq_XNamespace__Get,

        System_Windows_Forms_Application__RunForm,

        System_Environment__CurrentManagedThreadId,

        System_ComponentModel_EditorBrowsableAttribute__ctor,

        System_Runtime_GCLatencyMode__SustainedLowLatency,

        System_ValueTuple_T1__Item1,

        System_ValueTuple_T2__Item1,
        System_ValueTuple_T2__Item2,

        System_ValueTuple_T3__Item1,
        System_ValueTuple_T3__Item2,
        System_ValueTuple_T3__Item3,

        System_ValueTuple_T4__Item1,
        System_ValueTuple_T4__Item2,
        System_ValueTuple_T4__Item3,
        System_ValueTuple_T4__Item4,

        System_ValueTuple_T5__Item1,
        System_ValueTuple_T5__Item2,
        System_ValueTuple_T5__Item3,
        System_ValueTuple_T5__Item4,
        System_ValueTuple_T5__Item5,

        System_ValueTuple_T6__Item1,
        System_ValueTuple_T6__Item2,
        System_ValueTuple_T6__Item3,
        System_ValueTuple_T6__Item4,
        System_ValueTuple_T6__Item5,
        System_ValueTuple_T6__Item6,

        System_ValueTuple_T7__Item1,
        System_ValueTuple_T7__Item2,
        System_ValueTuple_T7__Item3,
        System_ValueTuple_T7__Item4,
        System_ValueTuple_T7__Item5,
        System_ValueTuple_T7__Item6,
        System_ValueTuple_T7__Item7,

        System_ValueTuple_TRest__Item1,
        System_ValueTuple_TRest__Item2,
        System_ValueTuple_TRest__Item3,
        System_ValueTuple_TRest__Item4,
        System_ValueTuple_TRest__Item5,
        System_ValueTuple_TRest__Item6,
        System_ValueTuple_TRest__Item7,
        System_ValueTuple_TRest__Rest,

        System_ValueTuple_T1__ctor,
        System_ValueTuple_T2__ctor,
        System_ValueTuple_T3__ctor,
        System_ValueTuple_T4__ctor,
        System_ValueTuple_T5__ctor,
        System_ValueTuple_T6__ctor,
        System_ValueTuple_T7__ctor,
        System_ValueTuple_TRest__ctor,

        System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames,

        System_String__Format_IFormatProvider,
        System_String__Substring,

        Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningSingleFile,
        Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningMultipleFiles,

        System_Runtime_CompilerServices_NullableAttribute__ctorByte,
        System_Runtime_CompilerServices_NullableAttribute__ctorTransformFlags,
        System_Runtime_CompilerServices_ReferenceAssemblyAttribute__ctor,
        System_Runtime_CompilerServices_IsReadOnlyAttribute__ctor,
        System_Runtime_CompilerServices_IsByRefLikeAttribute__ctor,

        System_ObsoleteAttribute__ctor,

        System_Span_T__ctor,
        System_Span_T__get_Item,
        System_Span_T__get_Length,

        System_ReadOnlySpan_T__ctor,
        System_ReadOnlySpan_T__get_Item,
        System_ReadOnlySpan_T__get_Length,

        System_Runtime_CompilerServices_IsUnmanagedAttribute__ctor,

        System_Math__CeilingDouble,
        System_Math__FloorDouble,
        System_Math__TruncateDouble,

        core_Index__ctor,
        core_Index__value,
        core_Range__ctor,
        core_Range__begin,
        core_Range__end,

        System_Runtime_CompilerServices_AsyncIteratorStateMachineAttribute__ctor,

        System_IAsyncDisposable__DisposeAsync,
        System_Collections_Generic_IAsyncEnumerable_T__GetAsyncEnumerator,
        System_Collections_Generic_IAsyncEnumerator_T__MoveNextAsync,
        System_Collections_Generic_IAsyncEnumerator_T__get_Current,

        System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetResult,
        System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetStatus,
        System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__OnCompleted,
        System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__Reset,
        System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetException,
        System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetResult,
        System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__get_Version,
        System_Threading_Tasks_Sources_IValueTaskSource_T__GetResult,
        System_Threading_Tasks_Sources_IValueTaskSource_T__GetStatus,
        System_Threading_Tasks_Sources_IValueTaskSource_T__OnCompleted,
        System_Threading_Tasks_Sources_IValueTaskSource__GetResult,
        System_Threading_Tasks_Sources_IValueTaskSource__GetStatus,
        System_Threading_Tasks_Sources_IValueTaskSource__OnCompleted,
        System_Threading_Tasks_ValueTask_T__ctorSourceAndToken,
        System_Threading_Tasks_ValueTask_T__ctorValue,
        System_Threading_Tasks_ValueTask__ctor,

        System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Create,
        System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__Complete,
        System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitOnCompleted,
        System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__AwaitUnsafeOnCompleted,
        System_Runtime_CompilerServices_AsyncIteratorMethodBuilder__MoveNext_T,

        System_Runtime_CompilerServices_ITuple__get_Item,
        System_Runtime_CompilerServices_ITuple__get_Length,

        System_InvalidOperationException__ctor,
        System_Runtime_CompilerServices_SwitchExpressionException__ctor,
        System_Runtime_CompilerServices_SwitchExpressionException__ctorObject,

        Count

        // Remember to update the AllWellKnownTypeMembers tests when making changes here
    }
}
