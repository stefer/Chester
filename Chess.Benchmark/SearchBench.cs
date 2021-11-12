using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Chester.Evaluations;
using Chester.Models;
using Chester.Search;

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
    public class SearchBench
    {
        private Evaluator? evaluator;
        private Board? board;
        private ISearch? search;

        [Params(1, 2, 3, 4)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            evaluator = new Evaluator();
            board = new Board();
            search = new ParallellAlphaBetaMinMaxSearch(new SearchOptions(N), evaluator);
        }

        [Benchmark]
        public Evaluation? Search() => search?.Search(board, Color.White);
    }
}