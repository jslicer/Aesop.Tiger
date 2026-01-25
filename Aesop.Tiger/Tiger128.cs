// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tiger128.cs" company="Always Elucidated Solution Pioneers, LLC">
//   Copyright (c) Always Elucidated Solution Pioneers, LLC. All rights reserved.
// </copyright>
// <summary>
// This file is part of Aesop.Tiger.
//
// Aesop.Tiger implements the three variants of the Tiger algorithm by Ross
// Anderson and Eli Biham (http://www.cs.technion.ac.il/~biham/Reports/Tiger/).
// Copyright (c) Always Elucidated Solution Pioneers, LLC. All rights reserved.
//
// Aesop.Tiger is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// Aesop.Tiger is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
//
// Aesop.Tiger version 1.0.3.0, Copyright (C) 2007 Always Elucidated Solution Pioneers, LLC
// Aesop.Tiger comes with ABSOLUTELY NO WARRANTY.
// This is free software, and you are welcome to redistribute it
// under certain conditions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aesop;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using System;
using System.Text;

using static System.Text.Encoding;

/// <inheritdoc />
/// <summary>
/// Tiger hash, 128-bit implementation.
/// </summary>
/// <seealso cref="Tiger160" />
public class Tiger128 : TigerFull
{
    /// <summary>
    /// The hash size in bytes.
    /// </summary>
    private const int HashSizeInBytes = 16;

    /// <summary>
    /// The test data.
    /// </summary>
    private const string TestData = "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq";

    /// <inheritdoc cref="Tiger160"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="Tiger128" /> class.
    /// </summary>
    /// <param name="passes">The number of calculation passes.</param>
    public Tiger128(in int passes = DefaultPasses)
        : base(passes) => HashSizeValue = HashSizeInBytes << 3;

    /// <inheritdoc />
    /// <summary>
    /// Provides a self-test of the algorithm.
    /// </summary>
    /// <returns><see langword="true" /> if the self-test succeeds,<see langword="false" /> otherwise.</returns>
    /// <exception cref="ArgumentNullException"> buffer is <see langword="null" />.</exception>
    /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
    /// <exception cref="EncoderFallbackException">A fall-back occurred (see Character Encoding in
    /// the .NET Framework for complete explanation)-and- <see cref="EncoderFallback" /> is
    /// set to <see cref="EncoderExceptionFallback" />.</exception>
    public override (bool Success, byte[] Hash) SelfTestPass()
    {
        byte[] testHash =
        [
            0xb7, 0x61, 0x0d, 0xf7,
            0xe8, 0x4f, 0x0a, 0xc3,
            0xa7, 0x1c, 0x63, 0x1e,
            0x7b, 0x53, 0xf7, 0x8e,
        ];

        Initialize();

        byte[] hash = ComputeHash(ASCII.GetBytes(TestData));

        return (hash.SequenceEqual(testHash), hash);
    }

    /// <inheritdoc />
    /// <summary>
    /// Provides a self-test of the algorithm in span mode.
    /// </summary>
    /// <returns><see langword="true" /> if the self-test succeeds,<see langword="false" /> otherwise.</returns>
    /// <exception cref="ArgumentNullException"> buffer is <see langword="null" />.</exception>
    /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
    /// <exception cref="EncoderFallbackException">A fall-back occurred (see Character Encoding in
    /// the .NET Framework for complete explanation)-and- <see cref="EncoderFallback" /> is
    /// set to <see cref="EncoderExceptionFallback" />.</exception>
    public override bool SelfTestTryPass()
    {
        byte[] testHash =
        [
            0xb7, 0x61, 0x0d, 0xf7,
            0xe8, 0x4f, 0x0a, 0xc3,
            0xa7, 0x1c, 0x63, 0x1e,
            0x7b, 0x53, 0xf7, 0x8e,
        ];

        Initialize();

        Span<byte> hash = stackalloc byte[HashSizeInBytes];
        bool success = TryComputeHash(ASCII.GetBytes(TestData), hash, out int bytesWritten);

        // ReSharper disable once ComplexConditionExpression
        return success && bytesWritten == hash.Length && hash.SequenceEqual(testHash);
    }

    /// <inheritdoc />
    /// <summary>
    /// Provides a self-test of the algorithm.
    /// </summary>
    /// <returns><see langword="true" /> if the self-test succeeds,<see langword="false" /> otherwise.</returns>
    /// <exception cref="ArgumentNullException"> buffer is <see langword="null" />.</exception>
    /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
    /// <exception cref="EncoderFallbackException">A fall-back occurred (see Character Encoding in
    /// the .NET Framework for complete explanation)-and- <see cref="EncoderFallback" /> is
    /// set to <see cref="EncoderExceptionFallback" />.</exception>
    public override bool SelfTestFail()
    {
        byte[] testHash =
        [
            0xb7, 0x61, 0x0d, 0xf7,
            0xe8, 0x4f, 0x0a, 0xc4,
            0xa8, 0x1c, 0x63, 0x1e,
            0x7b, 0x53, 0xf7, 0x8e,
        ];

        Initialize();

        byte[] hash = ComputeHash(ASCII.GetBytes(TestData));

        return hash.SequenceEqual(testHash);
    }

    /// <inheritdoc />
    /// <summary>
    /// Provides a self-test of the algorithm in span mode.
    /// </summary>
    /// <returns><see langword="true" /> if the self-test succeeds,<see langword="false" /> otherwise.</returns>
    /// <exception cref="ArgumentNullException"> buffer is <see langword="null" />.</exception>
    /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
    /// <exception cref="EncoderFallbackException">A fall-back occurred (see Character Encoding in
    /// the .NET Framework for complete explanation)-and- <see cref="EncoderFallback" /> is
    /// set to <see cref="EncoderExceptionFallback" />.</exception>
    public override bool SelfTestTryFail()
    {
        byte[] testHash =
        [
            0xb7, 0x61, 0x0d, 0xf7,
            0xe8, 0x4f, 0x0a, 0xc4,
            0xa8, 0x1c, 0x63, 0x1e,
            0x7b, 0x53, 0xf7, 0x8e,
        ];

        Initialize();

        Span<byte> hash = stackalloc byte[HashSizeInBytes];
        bool success = TryComputeHash(ASCII.GetBytes(TestData), hash, out int bytesWritten);

        // ReSharper disable once ComplexConditionExpression
        return success && bytesWritten == hash.Length && hash.SequenceEqual(testHash);
    }
}
