// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using StarkPlatform.CodeAnalysis.Editing;

namespace StarkPlatform.CodeAnalysis.ConvertLinq.ConvertForEachToLinqQuery
{
    internal interface IConverter<TForEachStatement, TStatement>
    {
        ForEachInfo<TForEachStatement, TStatement> ForEachInfo { get; }

        void Convert(SyntaxEditor editor, bool convertToQuery, CancellationToken cancellationToken);
    }
}
