using BenchmarkDotNet.Running;

namespace Chester.Benchmark;

/// <summary>
/// List benchmarks: dotnet run -c Release -f net6.0 -- --list flat
/// Run all benchmarks: dotnet run -c Release -f net6.0 -- -f *
/// </summary>
public class Program
{
    public static void Main(string[] args)
        => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
