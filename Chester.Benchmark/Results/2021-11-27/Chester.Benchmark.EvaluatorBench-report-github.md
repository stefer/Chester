``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
  Job-CTRQME : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
  .NET 6.0   : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT

Runtime=.NET 6.0  

```
| Method   | Job        | Toolchain | N    |     Mean |     Error |    StdDev |  Gen 0 | Completed Work Items | Lock Contentions | Allocated |
| -------- | ---------- | --------- | ---- | -------: | --------: | --------: | -----: | -------------------: | ---------------: | --------: |
| Evaluate | Job-CTRQME | net6.0    | 1000 | 4.119 μs | 0.0231 μs | 0.0216 μs | 0.3281 |                    - |                - |      5 KB |
| Evaluate | .NET 6.0   | Default   | 1000 | 4.126 μs | 0.0142 μs | 0.0133 μs | 0.3281 |                    - |                - |      5 KB |
