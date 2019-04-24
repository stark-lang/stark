using System.Collections.Immutable;
using System.Globalization;
using StarkPlatform.CodeAnalysis.Host;
using StarkPlatform.CodeAnalysis.Text;

namespace StarkPlatform.CodeAnalysis.Completion
{
    internal interface ICompletionHelperService : IWorkspaceService
    {
        CompletionHelper GetCompletionHelper(Document document);
    }
}