```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.65GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3


```
| Method   | Mean     | Error   | StdDev  |
|--------- |---------:|--------:|--------:|
| Tiger128 | 331.5 μs | 0.61 μs | 0.54 μs |
| Tiger160 | 331.7 μs | 0.50 μs | 0.45 μs |
| Tiger192 | 331.3 μs | 0.53 μs | 0.50 μs |
