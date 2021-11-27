``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
  Job-CTRQME : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
  .NET 6.0   : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT

Runtime=.NET 6.0  

```
| Method     | Job            | Toolchain  | N     |           Mean |         Error |        StdDev |          Gen 0 | Completed Work Items | Lock Contentions |       Gen 1 |  Allocated |
| ---------- | -------------- | ---------- | ----- | -------------: | ------------: | ------------: | -------------: | -------------------: | ---------------: | ----------: | ---------: |
| **Search** | **Job-CTRQME** | **net6.0** | **1** |   **1.318 ms** | **0.0059 ms** | **0.0052 ms** |   **103.5156** |                **-** |            **-** |       **-** |   **2 MB** |
| Search     | .NET 6.0       | Default    | 1     |       1.326 ms |     0.0062 ms |     0.0052 ms |       103.5156 |                    - |                - |           - |       2 MB |
| **Search** | **Job-CTRQME** | **net6.0** | **2** |  **12.117 ms** | **0.0400 ms** | **0.0374 ms** |   **953.1250** |                **-** |            **-** | **15.6250** |  **15 MB** |
| Search     | .NET 6.0       | Default    | 2     |      12.205 ms |     0.0737 ms |     0.0689 ms |       953.1250 |                    - |                - |     15.6250 |      15 MB |
| **Search** | **Job-CTRQME** | **net6.0** | **3** |  **42.584 ms** | **0.1014 ms** | **0.0899 ms** |  **3333.3333** |                **-** |            **-** |       **-** |  **54 MB** |
| Search     | .NET 6.0       | Default    | 3     |      42.205 ms |     0.2384 ms |     0.2230 ms |      3333.3333 |                    - |                - |           - |      54 MB |
| **Search** | **Job-CTRQME** | **net6.0** | **4** | **488.731 ms** | **1.4674 ms** | **1.3008 ms** | **37000.0000** |                **-** |            **-** |       **-** | **602 MB** |
| Search     | .NET 6.0       | Default    | 4     |     471.578 ms |     2.0989 ms |     1.9633 ms |     37000.0000 |                    - |                - |           - |     602 MB |
