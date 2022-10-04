namespace Stark.Compiler.BenchApp;

internal class Program
{
    static void Main(string[] args)
    {
        //var benchLexer = new BenchLexer();
        //for (int i = 0; i < 100; i++)
        //{
        //    //benchLexer.Roslyn();
        //    benchLexer.Stark();
        //}
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchLexer>();
    }
}