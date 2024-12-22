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

namespace Aesop;

using System;
using System.Linq;

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
    /// <returns>The hash code if the self-test succeeds, an empty <see cref="byte" /> array otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException"> buffer is <see langword="null" />.</exception>
    /// <exception cref="T:System.ObjectDisposedException">The object has already been disposed.</exception>
    /// <exception cref="T:System.Text.EncoderFallbackException">A fall-back occurred (see Character Encoding in
    /// the .NET Framework for complete explanation)-and- <see cref="P:System.Text.Encoding.EncoderFallback" /> is
    /// set to <see cref="T:System.Text.EncoderExceptionFallback" />.</exception>
    public override byte[] SelfTest()
    {
        const string TestData = "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq";
        byte[] testHash =
        [
            0xb7, 0x61, 0x0d, 0xf7,
            0xe8, 0x4f, 0x0a, 0xc3,
            0xa7, 0x1c, 0x63, 0x1e,
            0x7b, 0x53, 0xf7, 0x8e,
        ];

        Initialize();

        byte[] hash = ComputeHash(ASCII.GetBytes(TestData));

        return hash.SequenceEqual(testHash) ? hash : [];
    }

    /// <inheritdoc />
    /// <summary>
    /// Provides a self-test of the algorithm in span mode.
    /// </summary>
    /// <returns>The hash code if the self-test succeeds, an empty byte <see cref="ReadOnlySpan{T}" />
    /// otherwise.</returns>
    /// <exception cref="T:System.ArgumentNullException"> buffer is <see langword="null" />.</exception>
    /// <exception cref="T:System.ObjectDisposedException">The object has already been disposed.</exception>
    /// <exception cref="T:System.Text.EncoderFallbackException">A fall-back occurred (see Character Encoding in
    /// the .NET Framework for complete explanation)-and- <see cref="P:System.Text.Encoding.EncoderFallback" /> is
    /// set to <see cref="T:System.Text.EncoderExceptionFallback" />.</exception>
    public override ReadOnlySpan<byte> SelfTestTry()
    {
        const string TestData = "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq";
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
        if (!success || bytesWritten != hash.Length || !hash.SequenceEqual(testHash))
        {
            return [];
        }

        byte[] hashArray = new byte[hash.Length];

        hash.CopyTo(hashArray);
        return new(hashArray);
    }
}
