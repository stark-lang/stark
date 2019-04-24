// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Options;

namespace StarkPlatform.CodeAnalysis.Recommendations
{
    public static class RecommendationOptions
    {
        public static PerLanguageOption<bool> HideAdvancedMembers { get; } = new PerLanguageOption<bool>(nameof(RecommendationOptions), nameof(HideAdvancedMembers), defaultValue: false);

        public static PerLanguageOption<bool> FilterOutOfScopeLocals { get; } = new PerLanguageOption<bool>(nameof(RecommendationOptions), nameof(FilterOutOfScopeLocals), defaultValue: true);
    }
}
