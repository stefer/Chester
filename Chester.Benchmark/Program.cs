using BenchmarkDotNet.Running;

namespace Chester.Benchmark
{
    public class Program
    {
        public static void Main() => BenchmarkRunner.Run<SearchBench>();
    }
}