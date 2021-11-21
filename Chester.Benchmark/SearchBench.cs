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
            search = new ParallellAlphaBetaMinMaxSearch(new SearchOptions(N), evaluator, new NullReporter());
        }

        [Benchmark]
        public Evaluation? Search() => search?.Search(board, Color.White);
    }

    public class NullReporter : ISearchReporter
    {
        public void BestLine(int depth, int score, IEnumerable<Move> bestLine)
        {
        }

        public void CurrentMove(Move move, long moveNumber, int score)
        {
        }

        public void NodeVisited()
        {
        }

        public void Reset()
        {
        }
    }
}