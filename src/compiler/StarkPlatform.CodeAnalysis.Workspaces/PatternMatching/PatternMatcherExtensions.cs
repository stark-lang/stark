// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.PatternMatching
{
    internal static class PatternMatcherExtensions
    {
        public static PatternMatch? GetFirstMatch(this PatternMatcher matcher, string candidate)
        {
            var matches = ArrayBuilder<PatternMatch>.GetInstance();
            matcher.AddMatches(candidate, matches);

            var result = matches.Any() ? (PatternMatch?)matches.First() : null;
            matches.Free();

            return result;
        }

        public static bool Matches(this PatternMatcher matcher, string candidate)
            => matcher.GetFirstMatch(candidate) != null;
    }
}
