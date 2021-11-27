using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Chester.Evaluations;
using Chester.Models;
using Chester.Search;

namespace Chester.Benchmark;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
//[InliningDiagnoser]
//[TailCallDiagnoser]
//[EtwProfiler]
//[ConcurrencyVisualizerProfiler]
//[NativeMemoryProfiler]
[ThreadingDiagnoser]
public class SearchLinear
{
    private Evaluator? _evaluator;
    private ISearch? _search;

    [Params(1, 2, 3, 4)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _evaluator = new Evaluator();
        _search = new AlphaBetaMinMaxSearch(new SearchOptions(N), _evaluator, new NullReporter());
    }

    [Benchmark]
    public Evaluation? Search() => _search?.Search(new Board(), Color.White);
}

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
//[InliningDiagnoser]
//[TailCallDiagnoser]
//[EtwProfiler]
//[ConcurrencyVisualizerProfiler]
//[NativeMemoryProfiler]
[ThreadingDiagnoser]
public class SearchParallell
{
    private Evaluator? _evaluator;
    private ISearch? _search;

    [Params(1, 2, 3, 4)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _evaluator = new Evaluator();
        _search = new ParallellAlphaBetaMinMaxSearch(new SearchOptions(N), _evaluator, new NullReporter());
    }

    [Benchmark]
    public Evaluation? Search() => _search?.Search(new Board(), Color.White);
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
