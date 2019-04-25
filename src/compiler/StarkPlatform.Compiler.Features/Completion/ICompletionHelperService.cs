using System.Collections.Immutable;
using System.Globalization;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.Text;

namespace StarkPlatform.Compiler.Completion
{
    internal interface ICompletionHelperService : IWorkspaceService
    {
        CompletionHelper GetCompletionHelper(Document document);
    }
}