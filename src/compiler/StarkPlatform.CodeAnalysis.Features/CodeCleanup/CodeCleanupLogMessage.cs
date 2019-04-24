// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using StarkPlatform.CodeAnalysis.Internal.Log;
using StarkPlatform.CodeAnalysis.Options;

namespace StarkPlatform.CodeAnalysis.CodeCleanup
{
    internal static class CodeCleanupLogMessage
    {
        public static KeyValueLogMessage Create(OptionSet optionSet)
        {
            return KeyValueLogMessage.Create(LogType.UserAction, m =>
            {
                foreach (var option in CodeCleanupOptionsProvider.SingletonOptions)
                {
                    m[option.Name] = optionSet.GetOption((PerLanguageOption<bool>)option, LanguageNames.Stark);
                }
            });
        }
    }
}
