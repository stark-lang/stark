namespace StarkPlatform.Compiler.SolutionSize
{
    internal interface ISolutionSizeTracker
    {
        long GetSolutionSize(Workspace workspace, SolutionId solutionId);
    }
}