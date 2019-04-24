// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    /// <summary>
    /// Information decoded from well-known custom attributes applied on a type.
    /// </summary>
    internal sealed class TypeWellKnownAttributeData : CommonTypeWellKnownAttributeData
    {
        #region CoClassAttribute

        private NamedTypeSymbol _comImportCoClass;
        public NamedTypeSymbol ComImportCoClass
        {
            get
            {
                return _comImportCoClass;
            }
            set
            {
                VerifySealed(expected: false);
                Debug.Assert((object)_comImportCoClass == null);
                Debug.Assert((object)value != null);
                _comImportCoClass = value;
                SetDataStored();
            }
        }

        #endregion
    }
}
