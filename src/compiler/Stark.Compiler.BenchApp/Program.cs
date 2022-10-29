using BenchmarkDotNet.Running;

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
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        //BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchLexer>();
    }
}