// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark
{
    internal partial class Binder
    {
        protected enum OverflowChecks
        {
            /// <summary>
            /// Outside of <c>checked</c>, <c>unchecked</c> expression/block.
            /// </summary>
            Implicit,

            /// <summary>
            /// Within <c>unchecked</c> expression/block.
            /// </summary>
            Disabled,

            /// <summary>
            /// Within <c>checked</c> expression/block.
            /// </summary>
            Enabled
        }
    }
}
