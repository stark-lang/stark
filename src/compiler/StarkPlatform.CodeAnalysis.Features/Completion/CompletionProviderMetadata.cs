using System.Collections.Generic;
using StarkPlatform.CodeAnalysis.Host.Mef;
using Roslyn.Utilities;

namespace StarkPlatform.CodeAnalysis.Completion.Providers
{
    internal sealed class CompletionProviderMetadata : OrderableLanguageMetadata
    {
        public string[] Roles { get; }

        public CompletionProviderMetadata(IDictionary<string, object> data)
            : base(data)
        {
            this.Roles = (string[])data.GetValueOrDefault("Roles")
                ?? (string[])data.GetValueOrDefault("TextViewRoles");
        }
    }
}
