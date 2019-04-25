// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using StarkPlatform.Compiler.Stark.Utilities;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Host.Mef;
using Roslyn.Utilities;

namespace StarkPlatform.Compiler.Stark
{
    [ExportLanguageService(typeof(ICommandLineParserService), LanguageNames.Stark), Shared]
    internal sealed class CSharpCommandLineParserService : ICommandLineParserService
    {
        public CommandLineArguments Parse(IEnumerable<string> arguments, string baseDirectory, bool isInteractive, string sdkDirectory)
        {
#if SCRIPTING
            var parser = isInteractive ? CSharpCommandLineParser.Interactive : CSharpCommandLineParser.Default;
#else
            var parser = CSharpCommandLineParser.Default;
#endif
            return parser.Parse(arguments, baseDirectory, sdkDirectory);
        }
    }
}
