// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TigerFull.cs" company="Always Elucidated Solution Pioneers, LLC">
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

// Ignore Spelling: ib
namespace Aesop;

using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

/// <inheritdoc />
/// <seealso cref="HashAlgorithm" />
/// <summary>
/// Implements the core of the full (192-bit) Tiger hash algorithm.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TigerFull" /> class.
/// </remarks>
/// <param name="passes">The number of calculation passes.</param>
public abstract class TigerFull(in int passes = TigerFull.DefaultPasses) : HashAlgorithm
{
    /// <summary>
    /// The default number of calculation passes.
    /// </summary>
    protected const int DefaultPasses = 3;

    /// <summary>
    /// The hash size in bytes.
    /// </summary>
    private const int HashSizeInBytes = 24;

    /// <summary>
    /// The block size in bytes.
    /// </summary>
    private const int BlockSizeInBytes = 64;

    /// <summary>
    /// The block size in unsigned longs.
    /// </summary>
    private const int BlockSizeInUlongs = 8;

    private readonly byte[] _byteBuffer = new byte[BlockSizeInBytes];

    private readonly ulong[] _ulongBuffer = new ulong[BlockSizeInUlongs];

    private ulong _a;

    private ulong _aa;

    private ulong _b;

    private ulong _bb;

    private ulong _c;

    private ulong _cc;

    private int _byteBufferPos;

    private ulong _len;

    /// <inheritdoc />
    /// <summary>Gets a value indicating whether multiple blocks can be transformed.</summary>
    /// <returns><c>true</c> if multiple blocks can be transformed; otherwise, <c>false</c>.</returns>
    public override bool CanTransformMultipleBlocks => true;

    /// <summary>
    /// Gets the number of calculation passes.
    /// </summary>
    /// <value>
    /// The number of calculation passes.
    /// </value>
    public int Passes { get; } = passes;

    /// <inheritdoc />
    /// <summary>Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm" />
    /// class.</summary>
    public override void Initialize()
    {
        _a = 0x0123456789abcdef;
        _b = 0xfedcba9876543210;
        _c = 0xf096a5b4c3b2e187;
        _byteBufferPos = 0;
        _len = 0UL;
    }

    /// <summary>
    /// Provides a self-test of the algorithm in derived classes.
    /// </summary>
    /// <returns>The hash code if the self-test succeeds, an empty <see cref="byte" /> array otherwise.</returns>
    public abstract byte[] SelfTest();

    /// <summary>
    /// Provides a self-test of the algorithm in span mode in derived classes.
    /// </summary>
    /// <returns>The hash code if the self-test succeeds, an empty byte <see cref="ReadOnlySpan{T}" /> otherwise.</returns>
    public abstract ReadOnlySpan<byte> SelfTestTry();

    /// <inheritdoc />
    /// <summary>When overridden in a derived class, routes data written to the object into the hash algorithm for
    /// computing the hash.</summary>
    /// <param name="array">The input to compute the hash code for.</param>
    /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
    /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
    /// <exception cref="RankException"> sourceArray and destinationArray have different ranks.</exception>
    /// <exception cref="ArgumentNullException"> sourceArray is <see langword="null" />.-or-
    /// destinationArray is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> sourceIndex is less than the lower bound of the first
    /// dimension of sourceArray.-or- destinationIndex is less than the lower bound of the first dimension of
    /// destinationArray.-or- length is less than zero.</exception>
    /// <exception cref="ArgumentException"> length is greater than the number of elements from sourceIndex to the
    /// end of sourceArray.-or- length is greater than the number of elements from destinationIndex to the end of
    /// destinationArray.</exception>
    /// <exception cref="ArrayTypeMismatchException"> sourceArray and destinationArray" are of incompatible
    /// types.</exception>
    /// <exception cref="InvalidCastException">At least one element in sourceArray" cannot be cast to the type of
    /// destinationArray.</exception>
    // ReSharper disable once MethodTooLong
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        int end = ibStart + cbSize;
        int toCopy = BlockSizeInBytes - _byteBufferPos;

        _len += (ulong)cbSize;
        while (ibStart < end)
        {
            if (toCopy > cbSize)
            {
                toCopy = cbSize;
            }

            Array.Copy(array, ibStart, _byteBuffer, _byteBufferPos, toCopy);
            ibStart += toCopy;
            cbSize -= toCopy;
            _byteBufferPos += toCopy;
            if (_byteBufferPos < BlockSizeInBytes)
            {
                return;
            }

            (_a, _b, _c) =
                ProcessBlock(_a, _b, _c, _byteBuffer, _ulongBuffer);
            _byteBufferPos = 0;
            toCopy = BlockSizeInBytes;
        }
    }

    /// <inheritdoc />
    /// <summary>When overridden in a derived class, finalizes the hash computation after the last data is
    /// processed by the cryptographic stream object.</summary>
    /// <returns>The computed hash code.</returns>
    /// <exception cref="ArgumentNullException"> sourceArray is <see langword="null" />.-or-
    /// destinationArray is <see langword="null" />.</exception>
    /// <exception cref="IndexOutOfRangeException"> index is less than the lower bound of array.-or-
    /// length is less than zero.-or-The sum of index and length is greater than the size of array.</exception>
    // ReSharper disable once MethodTooLong
    protected override byte[] HashFinal()
    {
        Span<byte> bytes = stackalloc byte[HashSize >> 3];

        return TryHashFinal(bytes, out int _) ? bytes.ToArray() : [];
    }

    /// <summary>
    /// Attempts to finalize the hash computation after the last data is processed by the hash algorithm.
    /// </summary>
    /// <param name="destination">The buffer to receive the hash value.</param>
    /// <param name="bytesWritten">When this method returns, the total number of bytes written into
    /// <paramref name="destination" />. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if <paramref name="destination" /> is long enough to receive the hash
    /// value; otherwise, <see langword="false" />.</returns>
    // ReSharper disable once MethodTooLong
    protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten)
    {
        _byteBuffer[_byteBufferPos] = 1;
        _byteBufferPos++;
        if (_byteBufferPos >= BlockSizeInBytes - 8)
        {
            Array.Clear(_byteBuffer, _byteBufferPos, BlockSizeInBytes - _byteBufferPos);
            (_a, _b, _c) =
                ProcessBlock(_a, _b, _c, _byteBuffer, _ulongBuffer);
            _byteBufferPos = 0;
        }

        Array.Clear(_byteBuffer, _byteBufferPos, BlockSizeInBytes - _byteBufferPos - 8);
        LongToBytes(_len << 3, _byteBuffer, BlockSizeInBytes - 8);
        (_a, _b, _c) =
            ProcessBlock(_a, _b, _c, _byteBuffer, _ulongBuffer);

        byte[] result = new byte[HashSizeInBytes];
        int actualHashSizeInBytes = HashSize >> 3;

        LongToBytes(_a, result, 0);
        LongToBytes(_b, result, 8);
        LongToBytes(_c, result, 16);
        result.AsSpan(HashSizeInBytes - actualHashSizeInBytes, actualHashSizeInBytes).CopyTo(destination);
        bytesWritten = actualHashSizeInBytes;
        return true;
    }

    // ReSharper disable once TooManyArguments
    private static (ulong ap, ulong bp, ulong cp) Pass(
        ulong ap,
        ulong bp,
        ulong cp,
        in ulong[] ulongBuffer,
        in ulong multiplier)
    {
        (ap, bp, cp) = Round(ap, bp, cp, ulongBuffer[0], multiplier);
        (bp, cp, ap) = Round(bp, cp, ap, ulongBuffer[1], multiplier);
        (cp, ap, bp) = Round(cp, ap, bp, ulongBuffer[2], multiplier);
        (ap, bp, cp) = Round(ap, bp, cp, ulongBuffer[3], multiplier);
        (bp, cp, ap) = Round(bp, cp, ap, ulongBuffer[4], multiplier);
        (cp, ap, bp) = Round(cp, ap, bp, ulongBuffer[5], multiplier);
        (ap, bp, cp) = Round(ap, bp, cp, ulongBuffer[6], multiplier);
        (bp, cp, ap) = Round(bp, cp, ap, ulongBuffer[7], multiplier);
        return (ap, bp, cp);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //// ReSharper disable once MethodTooLong
    private static void KeySchedule(in ulong[] ulongBuffer)
    {
        ulongBuffer[0] -= ulongBuffer[7] ^ 0xa5a5a5a5a5a5a5a5;
        ulongBuffer[1] ^= ulongBuffer[0];
        ulongBuffer[2] += ulongBuffer[1];
        //// ReSharper disable once ComplexConditionExpression
        ulongBuffer[3] -= ulongBuffer[2] ^ (~ulongBuffer[1] << 19);
        ulongBuffer[4] ^= ulongBuffer[3];
        ulongBuffer[5] += ulongBuffer[4];
        //// ReSharper disable once ComplexConditionExpression
        ulongBuffer[6] -= ulongBuffer[5] ^ ((~ulongBuffer[4] >> 23) & 0x000001ffffffffff);
        ulongBuffer[7] ^= ulongBuffer[6];
        ulongBuffer[0] += ulongBuffer[7];
        //// ReSharper disable once ComplexConditionExpression
        ulongBuffer[1] -= ulongBuffer[0] ^ (~ulongBuffer[7] << 19);
        ulongBuffer[2] ^= ulongBuffer[1];
        ulongBuffer[3] += ulongBuffer[2];
        //// ReSharper disable once ComplexConditionExpression
        ulongBuffer[4] -= ulongBuffer[3] ^ ((~ulongBuffer[2] >> 23) & 0x000001ffffffffff);
        ulongBuffer[5] ^= ulongBuffer[4];
        ulongBuffer[6] += ulongBuffer[5];
        ulongBuffer[7] -= ulongBuffer[6] ^ 0x0123456789abcdef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //// ReSharper disable once TooManyArguments
    private static (ulong ar, ulong br, ulong cr) Round(
        ulong ar,
        ulong br,
        ulong cr,
        in ulong ulongBuffer,
        in ulong multiplier)
    {
        cr ^= ulongBuffer;
        //// ReSharper disable once ComplexConditionExpression
        ar -= TigerSBox.Table1[cr & 0xff] ^
              TigerSBox.Table2[(cr >> 16) & 0xff] ^
              TigerSBox.Table3[(cr >> 32) & 0xff] ^
              TigerSBox.Table4[(cr >> 48) & 0xff];
        //// ReSharper disable once ComplexConditionExpression
        br += TigerSBox.Table4[(cr >> 8) & 0xff] ^
              TigerSBox.Table3[(cr >> 24) & 0xff] ^
              TigerSBox.Table2[(cr >> 40) & 0xff] ^
              TigerSBox.Table1[(cr >> 56) & 0xff];
        br *= multiplier;
        return (ar, br, cr);
    }

    private static void LongToBytes(ulong ulVal, in byte[] byteBuffer, int nIdxBuffer)
    {
        const int BytesInALong = 8;
        const int BitsInAByte = 8;
        int end = nIdxBuffer + BytesInALong;

        while (nIdxBuffer < end)
        {
            byteBuffer[nIdxBuffer++] = (byte)(ulVal & 0xff);
            ulVal >>= BitsInAByte;
        }
    }

    // ReSharper disable once TooManyArguments
    private (ulong ap, ulong bp, ulong cp) ProcessBlock(
        in ulong ap,
        in ulong bp,
        in ulong cp,
        in byte[] byteBuffer1,
        in ulong[] ulongBuffer1)
    {
        int pos = 0;
        int i = 0;

        while (pos < BlockSizeInBytes)
        {
            //// ReSharper disable once ComplexConditionExpression
            _ulongBuffer[i++] =
                ((ulong)byteBuffer1[pos++] & 0xff) |
                (((ulong)byteBuffer1[pos++] & 0xff) << 8) |
                (((ulong)byteBuffer1[pos++] & 0xff) << 16) |
                (((ulong)byteBuffer1[pos++] & 0xff) << 24) |
                (((ulong)byteBuffer1[pos++] & 0xff) << 32) |
                (((ulong)byteBuffer1[pos++] & 0xff) << 40) |
                (((ulong)byteBuffer1[pos++] & 0xff) << 48) |
                ((ulong)byteBuffer1[pos++] << 56);
        }

        return Compress(ap, bp, cp, ulongBuffer1);
    }

    // ReSharper disable once TooManyArguments
    private (ulong ac, ulong bc, ulong cc) Compress(ulong ac, ulong bc, ulong cc1, in ulong[] ulongBuffer1)
    {
        SaveAbc(ac, bc, cc1);
        (ac, bc, cc1) = Pass(ac, bc, cc1, ulongBuffer1, 5);
        KeySchedule(ulongBuffer1);
        (cc1, ac, bc) = Pass(cc1, ac, bc, ulongBuffer1, 7);
        KeySchedule(ulongBuffer1);
        (bc, cc1, ac) = Pass(bc, cc1, ac, ulongBuffer1, 9);

        for (int passNumber = DefaultPasses; passNumber < Passes; passNumber++)
        {
            (cc1, ac, bc) = Pass(ac, bc, cc1, ulongBuffer1, 9);
        }

        return FeedForward(ac, bc, cc1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SaveAbc(in ulong av, in ulong bv, in ulong cv) => (_aa, _bb, _cc) = (av, bv, cv);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (ulong af, ulong bf, ulong cf) FeedForward(in ulong af, in ulong bf, in ulong cf) =>
        (af ^ _aa, bf - _bb, cf + _cc);
}
