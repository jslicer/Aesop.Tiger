// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Benchmark.cs" company="Always Elucidated Solution Pioneers, LLC">
//   Copyright (c) Always Elucidated Solution Pioneers, LLC. All rights reserved.
// </copyright>
// <summary>
//   Benchmark the Tiger 128, 160, and 192 hashing algorithm variants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aesop.Tiger.Benchmarks;

using System.Security.Cryptography;

using BenchmarkDotNet.Attributes;

/// <summary>
/// Benchmark the Tiger 128, 160, and 192 hashing algorithm variants.
/// </summary>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1515 // Consider making public types internal
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Benchmark : IDisposable
#pragma warning restore CA1515 // Consider making public types internal
#pragma warning restore IDE0079 // Remove unnecessary suppression
{
    /// <summary>
    /// The size of the random byte array to hash.
    /// </summary>
    private const int N = 100000;

    /// <summary>
    /// The random byte array to hash.
    /// </summary>
    private readonly byte[] _data;

    /// <summary>
    /// The Tiger 128 hasher.
    /// </summary>
    private readonly HashAlgorithm _tiger128 = new Tiger128();

    /// <summary>
    /// The Tiger 160 hasher.
    /// </summary>
    private readonly HashAlgorithm _tiger160 = new Tiger160();

    /// <summary>
    /// The Tiger 192 hasher.
    /// </summary>
    private readonly HashAlgorithm _tiger192 = new Tiger192();

    /// <summary>
    /// Initializes a new instance of the <see cref="Benchmark" /> class.
    /// </summary>
    public Benchmark()
    {
        _data = new byte[N];
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA5394 // Do not use insecure randomness
        Random.Shared.NextBytes(_data);
#pragma warning restore CA5394 // Do not use insecure randomness
#pragma warning restore IDE0079 // Remove unnecessary suppression
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Benchmark the Tiger 128 hashing algorithm variant.
    /// </summary>
    /// <returns>The resulting hash value of the random byte array.</returns>
    [Benchmark]
    public byte[] Tiger128() => _tiger128.ComputeHash(_data);

    /// <summary>
    /// Benchmark the Tiger 160 hashing algorithm variant.
    /// </summary>
    /// <returns>The resulting hash value of the random byte array.</returns>
    [Benchmark]
    public byte[] Tiger160() => _tiger160.ComputeHash(_data);

    /// <summary>
    /// Benchmark the Tiger 192 hashing algorithm variant.
    /// </summary>
    /// <returns>The resulting hash value of the random byte array.</returns>
    [Benchmark]
    public byte[] Tiger192() => _tiger192.ComputeHash(_data);

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
    /// only unmanaged resources.</param>
    // ReSharper disable once FlagArgument
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _tiger192.Dispose();
        _tiger160.Dispose();
        _tiger128.Dispose();
    }
}