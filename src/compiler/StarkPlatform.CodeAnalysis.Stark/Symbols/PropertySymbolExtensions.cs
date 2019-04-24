// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Stark.Symbols;
using StarkPlatform.CodeAnalysis.Stark.Syntax;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Stark.Symbols
{
    internal static class PropertySymbolExtensions
    {
        /// <summary>
        /// If the property has a GetMethod, return that.  Otherwise check the overridden
        /// property, if any.  Repeat for each overridden property.
        /// </summary>
        public static MethodSymbol GetOwnOrInheritedGetMethod(this PropertySymbol property)
        {
            while ((object)property != null)
            {
                MethodSymbol getMethod = property.GetMethod;
                if ((object)getMethod != null)
                {
                    return getMethod;
                }

                property = property.OverriddenProperty;
            }

            return null;
        }

        /// <summary>
        /// If the property has a SetMethod, return that.  Otherwise check the overridden
        /// property, if any.  Repeat for each overridden property.
        /// </summary>
        public static MethodSymbol GetOwnOrInheritedSetMethod(this PropertySymbol property)
        {
            while ((object)property != null)
            {
                MethodSymbol setMethod = property.SetMethod;
                if ((object)setMethod != null)
                {
                    return setMethod;
                }

                property = property.OverriddenProperty;
            }

            return null;
        }

        public static bool CanCallMethodsDirectly(this PropertySymbol property)
        {
            if (property.MustCallMethodsDirectly)
            {
                return true;
            }

            // Indexed property accessors can always be called directly, to support legacy code.
            return property.IsIndexedProperty && (!property.IsIndexer || property.HasRefOrOutParameter());
        }

        public static bool HasRefOrOutParameter(this PropertySymbol property)
        {
            foreach (ParameterSymbol param in property.Parameters)
            {
                if (param.RefKind == RefKind.Ref || param.RefKind == RefKind.Out)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
