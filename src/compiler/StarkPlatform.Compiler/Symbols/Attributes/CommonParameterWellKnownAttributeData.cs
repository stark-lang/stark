// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler
{
    /// <summary>
    /// Information decoded from well-known custom attributes applied on a parameter.
    /// </summary>
    internal class CommonParameterWellKnownAttributeData : WellKnownAttributeData, IMarshalAsAttributeTarget
    {
        #region OutAttribute
        private bool _hasOutAttribute;
        public bool HasOutAttribute
        {
            get
            {
                VerifySealed(expected: true);
                return _hasOutAttribute;
            }
            set
            {
                VerifySealed(expected: false);
                _hasOutAttribute = value;
                SetDataStored();
            }
        }
        #endregion

        #region InAttribute
        private bool _hasInAttribute;
        public bool HasInAttribute
        {
            get
            {
                VerifySealed(expected: true);
                return _hasInAttribute;
            }
            set
            {
                VerifySealed(expected: false);
                _hasInAttribute = value;
                SetDataStored();
            }
        }
        #endregion

        #region MarshalAsAttribute
        private MarshalPseudoCustomAttributeData _lazyMarshalAsData;

        MarshalPseudoCustomAttributeData IMarshalAsAttributeTarget.GetOrCreateData()
        {
            VerifySealed(expected: false);
            if (_lazyMarshalAsData == null)
            {
                _lazyMarshalAsData = new MarshalPseudoCustomAttributeData();
                SetDataStored();
            }

            return _lazyMarshalAsData;
        }

        /// <summary>
        /// Returns marshalling data or null of MarshalAs attribute isn't applied on the parameter.
        /// </summary>
        public MarshalPseudoCustomAttributeData MarshallingInformation
        {
            get
            {
                VerifySealed(expected: true);
                return _lazyMarshalAsData;
            }
        }
        #endregion
    }
}
