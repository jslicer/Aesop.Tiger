```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.79GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4


```
| Method   | Mean     | Error   | StdDev  |
|--------- |---------:|--------:|--------:|
| Tiger128 | 277.2 μs | 0.37 μs | 0.29 μs |
| Tiger160 | 277.2 μs | 0.20 μs | 0.17 μs |
| Tiger192 | 277.2 μs | 0.32 μs | 0.29 μs |
