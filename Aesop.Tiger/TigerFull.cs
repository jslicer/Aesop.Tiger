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
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    /// <summary>Initializes an implementation of the <see cref="HashAlgorithm" />
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

            array.AsSpan(ibStart, toCopy).CopyTo(_byteBuffer.AsSpan(_byteBufferPos));
            ibStart += toCopy;
            cbSize -= toCopy;
            _byteBufferPos += toCopy;
            if (_byteBufferPos < BlockSizeInBytes)
            {
                return;
            }

            ProcessBlock(ref _a, ref _b, ref _c, _byteBuffer, _ulongBuffer);
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
        _byteBuffer[_byteBufferPos++] = 1;
        if (_byteBufferPos >= BlockSizeInBytes - 8)
        {
            _byteBuffer.AsSpan(_byteBufferPos, BlockSizeInBytes - _byteBufferPos).Clear();
            ProcessBlock(ref _a, ref _b, ref _c, _byteBuffer, _ulongBuffer);
            _byteBufferPos = 0;
        }

        _byteBuffer.AsSpan(_byteBufferPos, BlockSizeInBytes - _byteBufferPos - 8).Clear();
        BinaryPrimitives.WriteUInt64LittleEndian(_byteBuffer.AsSpan(BlockSizeInBytes - 8, 8), _len << 3);
        ProcessBlock(ref _a, ref _b, ref _c, _byteBuffer, _ulongBuffer);

        int actual = HashSize >> 3;

        if (destination.Length < actual)
        {
            bytesWritten = 0;
            return false;
        }

        Span<byte> biggerDestination = stackalloc byte[HashSizeInBytes];

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(biggerDestination), _a);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref MemoryMarshal.GetReference(biggerDestination), 8), _b);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref MemoryMarshal.GetReference(biggerDestination), 16), _c);
        biggerDestination.Slice(HashSizeInBytes - actual, actual).CopyTo(destination);
        bytesWritten = actual;
        return true;
    }

    // ReSharper disable once TooManyArguments
    private static void Pass5(
        ref ulong ap,
        ref ulong bp,
        ref ulong cp,
        in ulong[] ulongBuffer)
    {
        Round(ref ap, ref bp, ref cp, ulongBuffer[0], 5);
        Round(ref bp, ref cp, ref ap, ulongBuffer[1], 5);
        Round(ref cp, ref ap, ref bp, ulongBuffer[2], 5);
        Round(ref ap, ref bp, ref cp, ulongBuffer[3], 5);
        Round(ref bp, ref cp, ref ap, ulongBuffer[4], 5);
        Round(ref cp, ref ap, ref bp, ulongBuffer[5], 5);
        Round(ref ap, ref bp, ref cp, ulongBuffer[6], 5);
        Round(ref bp, ref cp, ref ap, ulongBuffer[7], 5);
    }

    // ReSharper disable once TooManyArguments
    private static void Pass7(
        ref ulong ap,
        ref ulong bp,
        ref ulong cp,
        in ulong[] ulongBuffer)
    {
        Round(ref ap, ref bp, ref cp, ulongBuffer[0], 7);
        Round(ref bp, ref cp, ref ap, ulongBuffer[1], 7);
        Round(ref cp, ref ap, ref bp, ulongBuffer[2], 7);
        Round(ref ap, ref bp, ref cp, ulongBuffer[3], 7);
        Round(ref bp, ref cp, ref ap, ulongBuffer[4], 7);
        Round(ref cp, ref ap, ref bp, ulongBuffer[5], 7);
        Round(ref ap, ref bp, ref cp, ulongBuffer[6], 7);
        Round(ref bp, ref cp, ref ap, ulongBuffer[7], 7);
    }

    // ReSharper disable once TooManyArguments
    private static void Pass9(
        ref ulong ap,
        ref ulong bp,
        ref ulong cp,
        in ulong[] ulongBuffer)
    {
        Round(ref ap, ref bp, ref cp, ulongBuffer[0], 9);
        Round(ref bp, ref cp, ref ap, ulongBuffer[1], 9);
        Round(ref cp, ref ap, ref bp, ulongBuffer[2], 9);
        Round(ref ap, ref bp, ref cp, ulongBuffer[3], 9);
        Round(ref bp, ref cp, ref ap, ulongBuffer[4], 9);
        Round(ref cp, ref ap, ref bp, ulongBuffer[5], 9);
        Round(ref ap, ref bp, ref cp, ulongBuffer[6], 9);
        Round(ref bp, ref cp, ref ap, ulongBuffer[7], 9);
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
    // ReSharper disable once MethodTooLong
    private static void Round(
        ref ulong ar,
        ref ulong br,
        ref ulong cr,
        in ulong ulongBuffer,
        in ulong multiplier)
    {
        cr ^= ulongBuffer;

        ref readonly ulong t1Ref = ref MemoryMarshal.GetReference(TigerSBox.Table1);
        ref readonly ulong t2Ref = ref MemoryMarshal.GetReference(TigerSBox.Table2);
        ref readonly ulong t3Ref = ref MemoryMarshal.GetReference(TigerSBox.Table3);
        ref readonly ulong t4Ref = ref MemoryMarshal.GetReference(TigerSBox.Table4);

        int i0 = (int)(cr & 0xff);
        //// ReSharper disable ComplexConditionExpression
        int i1 = (int)((cr >> 16) & 0xff);
        int i2 = (int)((cr >> 32) & 0xff);
        int i3 = (int)((cr >> 48) & 0xff);
        //// ReSharper restore ComplexConditionExpression

        // ReSharper disable once ComplexConditionExpression
        ar -= Unsafe.Add(ref Unsafe.AsRef(in t1Ref), i0)
            ^ Unsafe.Add(ref Unsafe.AsRef(in t2Ref), i1)
            ^ Unsafe.Add(ref Unsafe.AsRef(in t3Ref), i2)
            ^ Unsafe.Add(ref Unsafe.AsRef(in t4Ref), i3);

        //// ReSharper disable ComplexConditionExpression
        int j0 = (int)((cr >> 8) & 0xff);
        int j1 = (int)((cr >> 24) & 0xff);
        int j2 = (int)((cr >> 40) & 0xff);
        int j3 = (int)((cr >> 56) & 0xff);
        //// ReSharper restore ComplexConditionExpression

        // ReSharper disable once ComplexConditionExpression
        br += Unsafe.Add(ref Unsafe.AsRef(in t4Ref), j0)
            ^ Unsafe.Add(ref Unsafe.AsRef(in t3Ref), j1)
            ^ Unsafe.Add(ref Unsafe.AsRef(in t2Ref), j2)
            ^ Unsafe.Add(ref Unsafe.AsRef(in t1Ref), j3);
        br *= multiplier;
    }

    // ReSharper disable once TooManyArguments
    private void ProcessBlock(
        ref ulong ap,
        ref ulong bp,
        ref ulong cp,
        in byte[] byteBuffer1,
        in ulong[] ulongBuffer1)
    {
        Span<ulong> ulongSpan = MemoryMarshal.Cast<byte, ulong>(byteBuffer1.AsSpan());

        ulongSpan.CopyTo(ulongBuffer1);
        if (!BitConverter.IsLittleEndian)
        {
            for (int i = 0; i < 8; i++)
            {
                ulongBuffer1[i] = BinaryPrimitives.ReverseEndianness(ulongBuffer1[i]);
            }
        }

        Compress(ref ap, ref bp, ref cp, ulongBuffer1);
    }

    // ReSharper disable once TooManyArguments
    private void Compress(ref ulong ac, ref ulong bc, ref ulong cc1, in ulong[] ulongBuffer1)
    {
        SaveAbc(ac, bc, cc1);
        Pass5(ref ac, ref bc, ref cc1, ulongBuffer1);
        KeySchedule(ulongBuffer1);
        Pass7(ref cc1, ref ac, ref bc, ulongBuffer1);
        KeySchedule(ulongBuffer1);
        Pass9(ref bc, ref cc1, ref ac, ulongBuffer1);
        for (int passNumber = DefaultPasses; passNumber < Passes; passNumber++)
        {
            Pass9(ref ac, ref bc, ref cc1, ulongBuffer1);
            (ac, bc, cc1) = (bc, cc1, ac);
        }

        FeedForward(ref ac, ref bc, ref cc1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SaveAbc(in ulong av, in ulong bv, in ulong cv) => (_aa, _bb, _cc) = (av, bv, cv);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FeedForward(ref ulong a, ref ulong b, ref ulong c)
    {
        a ^= _aa;
        b -= _bb;
        c += _cc;
    }
}
