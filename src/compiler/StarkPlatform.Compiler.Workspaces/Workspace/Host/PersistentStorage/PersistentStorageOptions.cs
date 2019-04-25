// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.Compiler.Options;

namespace StarkPlatform.Compiler.Host
{
    internal static class PersistentStorageOptions
    {
        public const string OptionName = "FeatureManager/Persistence";

        public static readonly Option<bool> Enabled = new Option<bool>(OptionName, "Enabled", defaultValue: true);
    }
}
