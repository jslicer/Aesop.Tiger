# Aesop.Tiger
Aesop.Tiger implements the three variants of the Tiger algorithm by Ross Anderson and Eli Biham. Example usage:
```cs
using (HashAlgorithm h = new Tiger192())
{
    string fileName = @"C:\TestFile.dat";
    FileInfo fi = new (fileName);

    await using (Stream s = new FileStream(
        fi.FullName,
        FileMode.Open,
        FileAccess.Read,
        FileShare.Read,
        4096,
        FileOptions.SequentialScan))
    {
        await h.ComputeHashAsync(s).ConfigureAwait(false);
    }
    
    await Out.WriteAsync(string.Format(CurrentCulture, "\"{0}\": ", fileName)).ConfigureAwait(false);
    await OutputHashAsync(h.Hash).ConfigureAwait(false);
}
```   
## Benchmark Results

<!-- BENCHMARK_RESULTS_START -->
```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3


```
| Method   | Mean     | Error   | StdDev  |
|--------- |---------:|--------:|--------:|
| Tiger128 | 279.9 μs | 0.30 μs | 0.24 μs |
| Tiger160 | 278.9 μs | 0.74 μs | 0.66 μs |
| Tiger192 | 278.2 μs | 0.11 μs | 0.10 μs |
<!-- BENCHMARK_RESULTS_END -->
