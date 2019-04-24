// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace StarkPlatform.CodeAnalysis.Stark.Syntax
{
    public partial class IdentifierNameSyntax
    {
        internal override string ErrorDisplayName()
        {
            return Identifier.ValueText;
        }
    }
}
