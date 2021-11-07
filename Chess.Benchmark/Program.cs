using BenchmarkDotNet.Running;

namespace Chess.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SearchBench>();
        }
    }
}