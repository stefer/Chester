using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Chester.Evaluations;
using Chester.Models;

namespace Chester.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    //[InliningDiagnoser]
    //[TailCallDiagnoser]
    //[EtwProfiler]
    //[ConcurrencyVisualizerProfiler]
    //[NativeMemoryProfiler]
    [ThreadingDiagnoser]
    public class EvaluatorBench
    {
        private Evaluator? evaluator;
        private Board? board;

        [Params(1000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            evaluator = new Evaluator();
            board = new Board();
        }

        [Benchmark]
        public int? Evaluate() => evaluator?.Evaluate(board);
    }
}