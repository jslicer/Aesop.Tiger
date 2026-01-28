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

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v3


```
| Method   | Mean     | Error   | StdDev  |
|--------- |---------:|--------:|--------:|
| Tiger128 | 329.4 μs | 0.78 μs | 0.61 μs |
| Tiger160 | 329.9 μs | 0.76 μs | 0.67 μs |
| Tiger192 | 329.6 μs | 0.14 μs | 0.12 μs |
<!-- BENCHMARK_RESULTS_END -->
