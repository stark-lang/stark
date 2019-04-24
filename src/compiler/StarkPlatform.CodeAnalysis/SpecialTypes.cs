// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis
{
    internal static class SpecialTypes
    {
        /// <summary>
        /// Array of names for types from Cor Library.
        /// The names should correspond to ids from TypeId enum so
        /// that we could use ids to index into the array
        /// </summary>
        /// <remarks></remarks>
        private static readonly string[] s_emittedNames = new string[]
        {
            // The following things should be in sync:
            // 1) SpecialType enum
            // 2) names in SpecialTypes.EmittedNames array.
            // 3) languageNames in SemanticFacts.cs
            // 4) languageNames in SemanticFacts.vb
            null, // SpecialType.None
            "system.Object",
            "system.Enum",
            "system.MulticastDelegate",
            "system.Delegate",
            "system.ValueType",
            "system.Void",
            "system.Bool",
            "system.Rune",
            "system.Int8",
            "system.UInt8",
            "system.Int16",
            "system.UInt16",
            "system.Int32",
            "system.UInt32",
            "system.Int64",
            "system.UInt64",
            "system.Decimal",
            "system.Float32",
            "system.Float64",
            "system.String",
            "system.Int",
            "system.UInt",
            "system.Array",
            "system.Collections.IEnumerable",
            "system.Collections.Generic.IEnumerable`1",
            "system.Collections.Generic.IList`1",
            "system.Collections.Generic.ICollection`1",
            "system.Collections.IEnumerator",
            "system.Collections.Generic.IEnumerator`1",
            "system.Collections.Generic.IReadOnlyList`1",
            "system.Collections.Generic.IReadOnlyCollection`1",
            "system.Nullable`1",
            "system.DateTime",
            "system.runtime.compiler.IsVolatile",
            "system.IDisposable",
            "system.TypedReference",
            "system.ArgIterator",
            "system.RuntimeArgumentHandle",
            "system.RuntimeFieldHandle",
            "system.RuntimeMethodHandle",
            "system.RuntimeTypeHandle",
            "system.IAsyncResult",
            "system.AsyncCallback",
        };

        private readonly static Dictionary<string, SpecialType> s_nameToTypeIdMap;

        private static readonly StarkPlatform.Cci.PrimitiveTypeCode[] s_typeIdToTypeCodeMap;
        private static readonly SpecialType[] s_typeCodeToTypeIdMap;

        static SpecialTypes()
        {
            s_nameToTypeIdMap = new Dictionary<string, SpecialType>((int)SpecialType.Count);

            int i;

            for (i = 1; i < s_emittedNames.Length; i++)
            {
                Debug.Assert(s_emittedNames[i].IndexOf('+') < 0); // Compilers aren't prepared to lookup for a nested special type.
                s_nameToTypeIdMap.Add(s_emittedNames[i], (SpecialType)i);
            }

            s_typeIdToTypeCodeMap = new StarkPlatform.Cci.PrimitiveTypeCode[(int)SpecialType.Count + 1];

            for (i = 0; i < s_typeIdToTypeCodeMap.Length; i++)
            {
                s_typeIdToTypeCodeMap[i] = StarkPlatform.Cci.PrimitiveTypeCode.NotPrimitive;
            }

            s_typeIdToTypeCodeMap[(int)SpecialType.System_Boolean] = StarkPlatform.Cci.PrimitiveTypeCode.Boolean;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Char] = StarkPlatform.Cci.PrimitiveTypeCode.Char;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Void] = StarkPlatform.Cci.PrimitiveTypeCode.Void;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_String] = StarkPlatform.Cci.PrimitiveTypeCode.String;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Int64] = StarkPlatform.Cci.PrimitiveTypeCode.Int64;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Int32] = StarkPlatform.Cci.PrimitiveTypeCode.Int32;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Int16] = StarkPlatform.Cci.PrimitiveTypeCode.Int16;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Int8] = StarkPlatform.Cci.PrimitiveTypeCode.Int8;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_UInt64] = StarkPlatform.Cci.PrimitiveTypeCode.UInt64;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_UInt32] = StarkPlatform.Cci.PrimitiveTypeCode.UInt32;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_UInt16] = StarkPlatform.Cci.PrimitiveTypeCode.UInt16;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_UInt8] = StarkPlatform.Cci.PrimitiveTypeCode.UInt8;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Float32] = StarkPlatform.Cci.PrimitiveTypeCode.Float32;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Float64] = StarkPlatform.Cci.PrimitiveTypeCode.Float64;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_Int] = StarkPlatform.Cci.PrimitiveTypeCode.IntPtr;
            s_typeIdToTypeCodeMap[(int)SpecialType.System_UInt] = StarkPlatform.Cci.PrimitiveTypeCode.UIntPtr;

            s_typeCodeToTypeIdMap = new SpecialType[(int)StarkPlatform.Cci.PrimitiveTypeCode.Invalid + 1];

            for (i = 0; i < s_typeCodeToTypeIdMap.Length; i++)
            {
                s_typeCodeToTypeIdMap[i] = SpecialType.None;
            }

            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.Boolean] = SpecialType.System_Boolean;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.Char] = SpecialType.System_Char;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.Void] = SpecialType.System_Void;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.String] = SpecialType.System_String;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.Int64] = SpecialType.System_Int64;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.Int32] = SpecialType.System_Int32;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.Int16] = SpecialType.System_Int16;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.Int8] = SpecialType.System_Int8;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.UInt64] = SpecialType.System_UInt64;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.UInt32] = SpecialType.System_UInt32;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.UInt16] = SpecialType.System_UInt16;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.UInt8] = SpecialType.System_UInt8;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.Float32] = SpecialType.System_Float32;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.Float64] = SpecialType.System_Float64;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.IntPtr] = SpecialType.System_Int;
            s_typeCodeToTypeIdMap[(int)StarkPlatform.Cci.PrimitiveTypeCode.UIntPtr] = SpecialType.System_UInt;
        }

        /// <summary>
        /// Gets the name of the special type as it would appear in metadata.
        /// </summary>
        public static string GetMetadataName(this SpecialType id)
        {
            return s_emittedNames[(int)id];
        }

        public static SpecialType GetTypeFromMetadataName(string metadataName)
        {
            SpecialType id;

            if (s_nameToTypeIdMap.TryGetValue(metadataName, out id))
            {
                return id;
            }

            return SpecialType.None;
        }

        public static SpecialType GetTypeFromMetadataName(StarkPlatform.Cci.PrimitiveTypeCode typeCode)
        {
            return s_typeCodeToTypeIdMap[(int)typeCode];
        }

        public static StarkPlatform.Cci.PrimitiveTypeCode GetTypeCode(SpecialType typeId)
        {
            return s_typeIdToTypeCodeMap[(int)typeId];
        }
    }
}
