// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal static class AccessibilityExtensions
    {
        public static bool HasProtected(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                case Accessibility.ProtectedAndInternal:
                    return true;

                default:
                    return false;
            }
        }
    }
}

