using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Chester.Evaluations;
using Chester.Models;

namespace Chester.Benchmark;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
//[InliningDiagnoser(false, false)]
//[TailCallDiagnoser(logFailuresOnly: false, filterByNamespace: false)]
//[EtwProfiler]
//[NativeMemoryProfiler]
public class EvaluatorBench
{
    private Evaluator? _evaluator;
    private Board? _board;

    [Params(1000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _evaluator = new Evaluator();
        _board = new Board();
    }

    [Benchmark]
    public int? Evaluate() => _evaluator?.Evaluate(_board);
}
