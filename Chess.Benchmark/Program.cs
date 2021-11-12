using BenchmarkDotNet.Running;

namespace Chester.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SearchBench>();
        }
    }
}