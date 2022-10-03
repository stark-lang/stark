namespace Stark.Compiler.BenchApp;

internal class Program
{
    static void Main(string[] args)
    {
        //var benchLexer = new BenchLexer();
        //benchLexer.Roslyn();
        //benchLexer.Stark();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchLexer>();
    }
}