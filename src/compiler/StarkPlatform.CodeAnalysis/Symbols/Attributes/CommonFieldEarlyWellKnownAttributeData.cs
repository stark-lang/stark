// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using StarkPlatform.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis
{
    /// <summary>
    /// Information decoded from early well-known custom attributes applied on a field.
    /// </summary>
    internal class CommonFieldEarlyWellKnownAttributeData : EarlyWellKnownAttributeData
    {
        #region ObsoleteAttribute
        private ObsoleteAttributeData _obsoleteAttributeData = ObsoleteAttributeData.Uninitialized;
        public ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                VerifySealed(expected: true);
                return _obsoleteAttributeData.IsUninitialized ? null : _obsoleteAttributeData;
            }
            set
            {
                VerifySealed(expected: false);
                Debug.Assert(value != null);
                Debug.Assert(!value.IsUninitialized);

                _obsoleteAttributeData = value;
                SetDataStored();
            }
        }
        #endregion
    }
}
