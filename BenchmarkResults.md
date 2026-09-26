```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method   | Mean     | Error   | StdDev  |
|--------- |---------:|--------:|--------:|
| Tiger128 | 216.5 μs | 0.20 μs | 0.17 μs |
| Tiger160 | 216.2 μs | 0.10 μs | 0.09 μs |
| Tiger192 | 217.0 μs | 0.09 μs | 0.08 μs |
