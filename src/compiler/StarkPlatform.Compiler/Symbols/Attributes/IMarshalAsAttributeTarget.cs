// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler
{
    internal interface IMarshalAsAttributeTarget
    {
        MarshalPseudoCustomAttributeData GetOrCreateData();
    }
}
