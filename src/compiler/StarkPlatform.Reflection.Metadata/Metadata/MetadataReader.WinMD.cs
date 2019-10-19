// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using StarkPlatform.Reflection.Metadata.Ecma335;
using SR = StarkPlatform.Reflection.Resources.SR;

namespace StarkPlatform.Reflection.Metadata
{
    partial class MetadataReader
    {
        internal const string ClrPrefix = "<CLR>";

        #region Projection Tables

        [Conditional("DEBUG")]
        private static void AssertSorted(string[] keys)
        {
            for (int i = 0; i < keys.Length - 1; i++)
            {
                Debug.Assert(string.CompareOrdinal(keys[i], keys[i + 1]) < 0);
            }
        }

        #endregion

        private static uint TreatmentAndRowId(byte treatment, int rowId)
        {
            return ((uint)treatment << TokenTypeIds.RowIdBitCount) | (uint)rowId;
        }

        #region TypeDef

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal uint CalculateTypeDefTreatmentAndRowId(TypeDefinitionHandle handle)
        {
            Debug.Assert(_metadataKind != MetadataKind.Ecma335);

            TypeDefTreatment treatment;

            TypeAttributes flags = TypeDefTable.GetFlags(handle);
            EntityHandle extends = TypeDefTable.GetExtends(handle);

            treatment = TypeDefTreatment.None;

            return TreatmentAndRowId((byte)treatment, handle.RowId);
        }


        #endregion

        #region TypeRef

        internal uint CalculateTypeRefTreatmentAndRowId(TypeReferenceHandle handle)
        {
            Debug.Assert(_metadataKind != MetadataKind.Ecma335);

            bool isIDisposable;
            return TreatmentAndRowId((byte)GetSpecialTypeRefTreatment(handle), handle.RowId);
        }

        private TypeRefTreatment GetSpecialTypeRefTreatment(TypeReferenceHandle handle)
        {
            if (StringHeap.EqualsRaw(TypeRefTable.GetNamespace(handle), "System"))
            {
                StringHandle name = TypeRefTable.GetName(handle);

                if (StringHeap.EqualsRaw(name, "MulticastDelegate"))
                {
                    return TypeRefTreatment.SystemDelegate;
                }

                if (StringHeap.EqualsRaw(name, "Attribute"))
                {
                    return TypeRefTreatment.SystemAttribute;
                }
            }

            return TypeRefTreatment.None;
        }

        #endregion

        #region MethodDef

        private uint CalculateMethodDefTreatmentAndRowId(MethodDefinitionHandle methodDef)
        {
            MethodDefTreatment treatment = MethodDefTreatment.Implementation;

            TypeDefinitionHandle parentTypeDef = GetDeclaringType(methodDef);
            TypeAttributes parentFlags = TypeDefTable.GetFlags(parentTypeDef);

            if (treatment == MethodDefTreatment.Other)
            {
                // we want to hide the method if it implements
                // only redirected interfaces
                // We also want to check if the methodImpl is IClosable.Close,
                // so we can change the name
                bool seenRedirectedInterfaces = false;
                bool seenNonRedirectedInterfaces = false;

                bool isIClosableClose = false;

                foreach (var methodImplHandle in new MethodImplementationHandleCollection(this, parentTypeDef))
                {
                    MethodImplementation methodImpl = GetMethodImplementation(methodImplHandle);
                    if (methodImpl.MethodBody == methodDef)
                    {
                        EntityHandle declaration = methodImpl.MethodDeclaration;

                        // See if this MethodImpl implements a redirected interface
                        // In WinMD, MethodImpl will always use MemberRef and TypeRefs to refer to redirected interfaces,
                        // even if they are in the same module.
                        if (declaration.Kind == HandleKind.MemberReference &&
                            ImplementsRedirectedInterface((MemberReferenceHandle)declaration, out isIClosableClose))
                        {
                            seenRedirectedInterfaces = true;
                            if (isIClosableClose)
                            {
                                // This method implements IClosable.Close
                                // Let's rename to IDisposable later
                                // Once we know this implements IClosable.Close, we are done
                                // looking
                                break;
                            }
                        }
                        else
                        {
                            // Now we know this implements a non-redirected interface
                            // But we need to keep looking, just in case we got a methodimpl that
                            // implements the IClosable.Close method and needs to be renamed
                            seenNonRedirectedInterfaces = true;
                        }
                    }
                }

                if (isIClosableClose)
                {
                    treatment = MethodDefTreatment.DisposeMethod;
                }
                else if (seenRedirectedInterfaces && !seenNonRedirectedInterfaces)
                {
                    // Only hide if all the interfaces implemented are redirected
                    treatment = MethodDefTreatment.HiddenInterfaceImplementation;
                }
            }

            // If treatment is other, then this is a non-managed WinRT runtime class definition
            // Find out about various bits that we apply via attributes and name parsing
            if (treatment == MethodDefTreatment.Other)
            {
                treatment |= GetMethodTreatmentFromCustomAttributes(methodDef);
            }

            return TreatmentAndRowId((byte)treatment, methodDef.RowId);
        }

        private MethodDefTreatment GetMethodTreatmentFromCustomAttributes(MethodDefinitionHandle methodDef)
        {
            MethodDefTreatment treatment = 0;

            foreach (var caHandle in GetCustomAttributes(methodDef))
            {
                StringHandle namespaceHandle, nameHandle;
                if (!GetAttributeTypeNameRaw(caHandle, out namespaceHandle, out nameHandle))
                {
                    continue;
                }

                Debug.Assert(!namespaceHandle.IsVirtual && !nameHandle.IsVirtual);

                if (StringHeap.EqualsRaw(namespaceHandle, "Windows.UI.Xaml"))
                {
                    if (StringHeap.EqualsRaw(nameHandle, "TreatAsPublicMethodAttribute"))
                    {
                        treatment |= MethodDefTreatment.MarkPublicFlag;
                    }

                    if (StringHeap.EqualsRaw(nameHandle, "TreatAsAbstractMethodAttribute"))
                    {
                        treatment |= MethodDefTreatment.MarkAbstractFlag;
                    }
                }
            }

            return treatment;
        }

        #endregion

        #region FieldDef

        /// <summary>
        /// The backing field of a WinRT enumeration type is not public although the backing fields
        /// of managed enumerations are. To allow managed languages to directly access this field,
        /// it is made public by the metadata adapter.
        /// </summary>
        private uint CalculateFieldDefTreatmentAndRowId(FieldDefinitionHandle handle)
        {
            var flags = FieldTable.GetFlags(handle);
            FieldDefTreatment treatment = FieldDefTreatment.None;

            if ((flags & FieldAttributes.RTSpecialName) != 0 && StringHeap.EqualsRaw(FieldTable.GetName(handle), "value__"))
            {
                TypeDefinitionHandle typeDef = GetDeclaringType(handle);

                EntityHandle baseTypeHandle = TypeDefTable.GetExtends(typeDef);
                if (baseTypeHandle.Kind == HandleKind.TypeReference)
                {
                    var typeRef = (TypeReferenceHandle)baseTypeHandle;

                    if (StringHeap.EqualsRaw(TypeRefTable.GetName(typeRef), "Enum") &&
                        StringHeap.EqualsRaw(TypeRefTable.GetNamespace(typeRef), "System"))
                    {
                        treatment = FieldDefTreatment.EnumValue;
                    }
                }
            }

            return TreatmentAndRowId((byte)treatment, handle.RowId);
        }

        #endregion

        #region MemberRef

        private uint CalculateMemberRefTreatmentAndRowId(MemberReferenceHandle handle)
        {
            MemberRefTreatment treatment;

            // We need to rename the MemberRef for IClosable.Close as well
            // so that the MethodImpl for the Dispose method can be correctly shown
            // as IDisposable.Dispose instead of IDisposable.Close
            bool isIDisposable;
            if (ImplementsRedirectedInterface(handle, out isIDisposable) && isIDisposable)
            {
                treatment = MemberRefTreatment.Dispose;
            }
            else
            {
                treatment = MemberRefTreatment.None;
            }

            return TreatmentAndRowId((byte)treatment, handle.RowId);
        }

        /// <summary>
        /// We want to know if a given method implements a redirected interface.
        /// For example, if we are given the method RemoveAt on a class "A" 
        /// which implements the IVector interface (which is redirected
        /// to IList in .NET) then this method would return true. The most 
        /// likely reason why we would want to know this is that we wish to hide
        /// (mark private) all methods which implement methods on a redirected 
        /// interface.
        /// </summary>
        /// <param name="memberRef">The declaration token for the method</param>
        /// <param name="isIDisposable">
        /// Returns true if the redirected interface is <see cref="IDisposable"/>.
        /// </param>
        /// <returns>True if the method implements a method on a redirected interface.
        /// False otherwise.</returns>
        private bool ImplementsRedirectedInterface(MemberReferenceHandle memberRef, out bool isIDisposable)
        {
            isIDisposable = false;

            EntityHandle parent = MemberRefTable.GetClass(memberRef);

            TypeReferenceHandle typeRef;
            if (parent.Kind == HandleKind.TypeReference)
            {
                typeRef = (TypeReferenceHandle)parent;
            }
            else if (parent.Kind == HandleKind.TypeSpecification)
            {
                BlobHandle blob = TypeSpecTable.GetSignature((TypeSpecificationHandle)parent);
                BlobReader sig = new BlobReader(BlobHeap.GetMemoryBlock(blob));

                if (sig.Length < 2 ||
                    sig.ReadByte() != (byte)CorElementType.ELEMENT_TYPE_GENERICINST ||
                    sig.ReadByte() != (byte)CorElementType.ELEMENT_TYPE_CLASS)
                {
                    return false;
                }

                EntityHandle token = sig.ReadTypeHandle();
                if (token.Kind != HandleKind.TypeReference)
                {
                    return false;
                }

                typeRef = (TypeReferenceHandle)token;
            }
            else
            {
                return false;
            }

            return false;
        }

        #endregion

        #region AssemblyRef

        private int FindMscorlibAssemblyRefNoProjection()
        {
            for (int i = 1; i <= AssemblyRefTable.NumberOfNonVirtualRows; i++)
            {
                if (StringHeap.EqualsRaw(AssemblyRefTable.GetName(i), "mscorlib"))
                {
                    return i;
                }
            }

            throw new BadImageFormatException(SR.WinMDMissingMscorlibRef);
        }

        #endregion

        #region CustomAttribute

        internal CustomAttributeValueTreatment CalculateCustomAttributeValueTreatment(CustomAttributeHandle handle)
        {
            Debug.Assert(_metadataKind != MetadataKind.Ecma335);

            var parent = CustomAttributeTable.GetParent(handle);

            // Check for Windows.Foundation.Metadata.AttributeUsageAttribute.
            // WinMD rules: 
            //   - The attribute is only applicable on TypeDefs.
            //   - Constructor must be a MemberRef with TypeRef.
            if (!IsWindowsAttributeUsageAttribute(parent, handle))
            {
                return CustomAttributeValueTreatment.None;
            }

            var targetTypeDef = (TypeDefinitionHandle)parent;
            if (StringHeap.EqualsRaw(TypeDefTable.GetNamespace(targetTypeDef), "Windows.Foundation.Metadata"))
            {
                if (StringHeap.EqualsRaw(TypeDefTable.GetName(targetTypeDef), "VersionAttribute"))
                {
                    return CustomAttributeValueTreatment.AttributeUsageVersionAttribute;
                }

                if (StringHeap.EqualsRaw(TypeDefTable.GetName(targetTypeDef), "DeprecatedAttribute"))
                {
                    return CustomAttributeValueTreatment.AttributeUsageDeprecatedAttribute;
                }
            }

            bool allowMultiple = HasAttribute(targetTypeDef, "Windows.Foundation.Metadata", "AllowMultipleAttribute");
            return allowMultiple ? CustomAttributeValueTreatment.AttributeUsageAllowMultiple : CustomAttributeValueTreatment.AttributeUsageAllowSingle;
        }

        private bool IsWindowsAttributeUsageAttribute(EntityHandle targetType, CustomAttributeHandle attributeHandle)
        {
            // Check for Windows.Foundation.Metadata.AttributeUsageAttribute.
            // WinMD rules: 
            //   - The attribute is only applicable on TypeDefs.
            //   - Constructor must be a MemberRef with TypeRef.

            if (targetType.Kind != HandleKind.TypeDefinition)
            {
                return false;
            }

            var attributeCtor = CustomAttributeTable.GetConstructor(attributeHandle);
            if (attributeCtor.Kind != HandleKind.MemberReference)
            {
                return false;
            }

            var attributeType = MemberRefTable.GetClass((MemberReferenceHandle)attributeCtor);
            if (attributeType.Kind != HandleKind.TypeReference)
            {
                return false;
            }

            var attributeTypeRef = (TypeReferenceHandle)attributeType;
            return StringHeap.EqualsRaw(TypeRefTable.GetName(attributeTypeRef), "AttributeUsageAttribute") &&
                   StringHeap.EqualsRaw(TypeRefTable.GetNamespace(attributeTypeRef), "Windows.Foundation.Metadata");
        }

        private bool HasAttribute(EntityHandle token, string asciiNamespaceName, string asciiTypeName)
        {
            foreach (var caHandle in GetCustomAttributes(token))
            {
                StringHandle namespaceName, typeName;
                if (GetAttributeTypeNameRaw(caHandle, out namespaceName, out typeName) &&
                    StringHeap.EqualsRaw(typeName, asciiTypeName) &&
                    StringHeap.EqualsRaw(namespaceName, asciiNamespaceName))
                {
                    return true;
                }
            }

            return false;
        }

        private bool GetAttributeTypeNameRaw(CustomAttributeHandle caHandle, out StringHandle namespaceName, out StringHandle typeName)
        {
            namespaceName = typeName = default(StringHandle);

            EntityHandle typeDefOrRef = GetAttributeTypeRaw(caHandle);
            if (typeDefOrRef.IsNil)
            {
                return false;
            }

            if (typeDefOrRef.Kind == HandleKind.TypeReference)
            {
                TypeReferenceHandle typeRef = (TypeReferenceHandle)typeDefOrRef;
                var resolutionScope = TypeRefTable.GetResolutionScope(typeRef);

                if (!resolutionScope.IsNil && resolutionScope.Kind == HandleKind.TypeReference)
                {
                    // we don't need to handle nested types
                    return false;
                }

                // other resolution scopes don't affect full name

                typeName = TypeRefTable.GetName(typeRef);
                namespaceName = TypeRefTable.GetNamespace(typeRef);
            }
            else if (typeDefOrRef.Kind == HandleKind.TypeDefinition)
            {
                TypeDefinitionHandle typeDef = (TypeDefinitionHandle)typeDefOrRef;

                if (TypeDefTable.GetFlags(typeDef).IsNested())
                {
                    // we don't need to handle nested types
                    return false;
                }

                typeName = TypeDefTable.GetName(typeDef);
                namespaceName = TypeDefTable.GetNamespace(typeDef);
            }
            else
            {
                // invalid metadata
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the type definition or reference handle of the attribute type.
        /// </summary>
        /// <returns><see cref="TypeDefinitionHandle"/> or <see cref="TypeReferenceHandle"/> or nil token if the metadata is invalid and the type can't be determined.</returns>
        private EntityHandle GetAttributeTypeRaw(CustomAttributeHandle handle)
        {
            var ctor = CustomAttributeTable.GetConstructor(handle);

            if (ctor.Kind == HandleKind.MethodDefinition)
            {
                return GetDeclaringType((MethodDefinitionHandle)ctor);
            }

            if (ctor.Kind == HandleKind.MemberReference)
            {
                // In general the parent can be MethodDef, ModuleRef, TypeDef, TypeRef, or TypeSpec.
                // For attributes only TypeDef and TypeRef are applicable.
                EntityHandle typeDefOrRef = MemberRefTable.GetClass((MemberReferenceHandle)ctor);
                HandleKind handleType = typeDefOrRef.Kind;

                if (handleType == HandleKind.TypeReference || handleType == HandleKind.TypeDefinition)
                {
                    return typeDefOrRef;
                }
            }

            return default(EntityHandle);
        }
        #endregion
    }
}
