namespace StarkPlatform.CodeAnalysis.SolutionSize
{
    internal interface ISolutionSizeTracker
    {
        long GetSolutionSize(Workspace workspace, SolutionId solutionId);
    }
}