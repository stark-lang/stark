// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Shared.Extensions
{
    internal static class ILanguageServiceProviderExtensions
    {
        public static IEnumerable<Lazy<T, TMetadata>> SelectMatchingExtensions<T, TMetadata>(
            this HostLanguageServices serviceProvider,
            IEnumerable<Lazy<T, TMetadata>> items)
            where TMetadata : ILanguageMetadata
        {
            if (items == null)
            {
                return SpecializedCollections.EmptyEnumerable<Lazy<T, TMetadata>>();
            }

            return items.Where(lazy => lazy.Metadata.Language == serviceProvider.Language);
        }
    }
}
