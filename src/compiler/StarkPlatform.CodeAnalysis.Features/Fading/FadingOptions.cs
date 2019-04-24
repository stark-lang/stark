// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Options;

namespace StarkPlatform.CodeAnalysis.Fading
{
    internal static class FadingOptions
    {
        public static readonly PerLanguageOption<bool> FadeOutUnusedImports = new PerLanguageOption<bool>(
            nameof(FadingOptions), nameof(FadeOutUnusedImports), defaultValue: true,
            storageLocations: new RoamingProfileStorageLocation($"TextEditor.%LANGUAGE%.Specific.{nameof(FadeOutUnusedImports)}"));

        public static readonly PerLanguageOption<bool> FadeOutUnreachableCode = new PerLanguageOption<bool>(
            nameof(FadingOptions), nameof(FadeOutUnreachableCode), defaultValue: true,
            storageLocations: new RoamingProfileStorageLocation($"TextEditor.%LANGUAGE%.Specific.{nameof(FadeOutUnreachableCode)}"));
    }
}
