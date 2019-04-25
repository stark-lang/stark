// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using StarkPlatform.Compiler.Options;

namespace StarkPlatform.Compiler.Formatting
{
    internal abstract class TriviaDataWithList : TriviaData
    {
        public TriviaDataWithList(OptionSet optionSet, string language)
            : base(optionSet, language)
        {
        }

        public abstract List<SyntaxTrivia> GetTriviaList(CancellationToken cancellationToken);
    }
}
