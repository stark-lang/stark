// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Ergon.Core;
using StarkPlatform.CodeAnalysis.Ergon.Constants;
using StarkPlatform.CodeAnalysis.PooledObjects;

namespace StarkPlatform.CodeAnalysis.Ergon.ProjectFile
{
    internal static class Extensions
    {
        public static IEnumerable<ITaskItem> GetAdditionalFiles(this ErgonProjectInstance executedProject)
            => executedProject.GetItems(ItemNames.AdditionalFiles);

        public static IEnumerable<ITaskItem> GetAnalyzers(this ErgonProjectInstance executedProject)
            => executedProject.GetItems(ItemNames.Analyzer);

        public static IEnumerable<ITaskItem> GetDocuments(this ErgonProjectInstance executedProject)
            => executedProject.GetItems(ItemNames.Compile);

        public static IEnumerable<ITaskItem> GetMetadataReferences(this ErgonProjectInstance executedProject)
            => executedProject.GetItems(ItemNames.ReferencePath);

        public static IEnumerable<ProjectFileReference> GetProjectReferences(this ErgonProjectInstance executedProject)
            => executedProject
                .GetItems(ItemNames.ProjectReference)
                .Where(i => i.ReferenceOutputAssemblyIsTrue())
                .Select(CreateProjectFileReference);

        /// <summary>
        /// Create a <see cref="ProjectFileReference"/> from a ProjectReference node in the MSBuild file.
        /// </summary>
        private static ProjectFileReference CreateProjectFileReference(ErgonProjectItemInstance reference)
            => new ProjectFileReference(reference.EvaluatedInclude, reference.GetAliases());

        public static ImmutableArray<string> GetAliases(this ITaskItem item)
        {
            var aliasesText = item.GetMetadata(MetadataNames.Aliases);

            return !string.IsNullOrWhiteSpace(aliasesText)
                ? ImmutableArray.CreateRange(aliasesText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()))
                : ImmutableArray<string>.Empty;
        }

        public static bool ReferenceOutputAssemblyIsTrue(this ITaskItem item)
        {
            var referenceOutputAssemblyText = item.GetMetadata(MetadataNames.ReferenceOutputAssembly);

            return !string.IsNullOrWhiteSpace(referenceOutputAssemblyText)
                ? !string.Equals(referenceOutputAssemblyText, bool.FalseString, StringComparison.OrdinalIgnoreCase)
                : true;
        }

        public static string ReadPropertyString(this ErgonProjectInstance executedProject, string propertyName)
            => executedProject.GetProperty(propertyName)?.EvaluatedValue;

        public static bool ReadPropertyBool(this ErgonProjectInstance executedProject, string propertyName)
            => Conversions.ToBool(executedProject.ReadPropertyString(propertyName));

        public static int ReadPropertyInt(this ErgonProjectInstance executedProject, string propertyName)
            => Conversions.ToInt(executedProject.ReadPropertyString(propertyName));

        public static ulong ReadPropertyULong(this ErgonProjectInstance executedProject, string propertyName)
            => Conversions.ToULong(executedProject.ReadPropertyString(propertyName));

        public static TEnum? ReadPropertyEnum<TEnum>(this ErgonProjectInstance executedProject, string propertyName, bool ignoreCase)
            where TEnum : struct
            => Conversions.ToEnum<TEnum>(executedProject.ReadPropertyString(propertyName), ignoreCase);

        public static string ReadItemsAsString(this ErgonProjectInstance executedProject, string itemType)
        {
            var pooledBuilder = PooledStringBuilder.GetInstance();
            var builder = pooledBuilder.Builder;

            foreach (var item in executedProject.GetItems(itemType))
            {
                if (builder.Length > 0)
                {
                    builder.Append(" ");
                }

                builder.Append(item.EvaluatedInclude);
            }

            return pooledBuilder.ToStringAndFree();
        }

        public static IEnumerable<ITaskItem> GetTaskItems(this ErgonProjectInstance executedProject, string itemType)
            => executedProject.GetItems(itemType);
    }
}
