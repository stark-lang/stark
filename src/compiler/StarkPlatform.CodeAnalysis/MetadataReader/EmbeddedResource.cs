// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Reflection;
using System.Reflection.Stark;

namespace StarkPlatform.CodeAnalysis
{
    internal struct EmbeddedResource
    {
        public readonly uint Offset;
        public readonly ManifestResourceAttributes Attributes;
        public readonly string Name;

        internal EmbeddedResource(uint offset, ManifestResourceAttributes attributes, string name)
        {
            this.Offset = offset;
            this.Attributes = attributes;
            this.Name = name;
        }
    }
}
