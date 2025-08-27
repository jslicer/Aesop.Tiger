// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tiger160Tests.cs" company="Always Elucidated Solution Pioneers, LLC">
//   Copyright (c) Always Elucidated Solution Pioneers, LLC. All rights reserved.
// </copyright>
// <summary>
// This file is part of Aesop.Tiger.
//
// Aesop.Tiger implements the three variants of the Tiger algorithm by Ross
// Anderson and Eli Biham (http://www.cs.technion.ac.il/~biham/Reports/Tiger/).
// Copyright (c) Always Elucidated Solution Pioneers, LLC. All rights reserved.
//
// Aesop.Tiger.Tests is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// Aesop.Tiger.Tests is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
//
// Aesop.Tiger.Tests version 1.0.3.0, Copyright (C) 2007 Always Elucidated Solution Pioneers, LLC
// Aesop.Tiger.Tests comes with ABSOLUTELY NO WARRANTY.
// This is free software, and you are welcome to redistribute it
// under certain conditions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aesop.Tiger.Tests;

using System.Security.Cryptography;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static System.Text.Encoding;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

/// <summary>
/// Tests the functionality of the <see cref="Tiger160" /> class.
/// </summary>
[TestClass]
#pragma warning disable CA1515 // Consider making public types internal
public sealed class Tiger160Tests
#pragma warning restore CA1515 // Consider making public types internal
{
    /// <summary>
    /// The default number of calculation passes.
    /// </summary>
    private const int DefaultPasses = 3;

    /// <summary>
    /// Runs the self-test of the <see cref="Tiger160" /> class and asserts its success.
    /// </summary>
    [TestMethod]
    public void TestSelfTest()
    {
        using TigerFull h = new Tiger160();
        IsGreaterThan(0, h.SelfTest().Length);
    }

    /// <summary>
    /// Runs the self-test of the <see cref="Tiger160" /> class in span mode and asserts its success.
    /// </summary>
    [TestMethod]
    public void TestSelfTestTry()
    {
        using TigerFull h = new Tiger160();
        IsGreaterThan(0, h.SelfTestTry().Length);
    }

    /// <summary>
    /// Tests that <see cref="Tiger160" /> supports transforming multiple blocks.
    /// </summary>
    [TestMethod]
    public void TestCanTransformMultipleBlocks()
    {
        using HashAlgorithm h = new Tiger160();
        IsTrue(h.CanTransformMultipleBlocks);
    }

    /// <summary>
    /// Tests <see cref="Tiger160" /> Passes property returns the proper default.
    /// </summary>
    [TestMethod]
    public void TestPasses()
    {
        using TigerFull h = new Tiger160();
        AreEqual(DefaultPasses, h.Passes);
    }

    /// <summary>
    /// Tests <see cref="Tiger160" /> HashSize property returns the proper hash algorithm bit size.
    /// </summary>
    [TestMethod]
    public void TestHashSize()
    {
        using HashAlgorithm h = new Tiger160();
        AreEqual(160, h.HashSize);
    }

    /// <summary>
    /// Tests <see cref="Tiger160" /> for additional passes with a test string.
    /// </summary>
    [TestMethod]
    public void TestExtraPasses()
    {
        const string TestData = "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq12345678";
        byte[] testHash =
        [
            0xc5, 0xad, 0x43, 0xa0,
            0x90, 0x69, 0x95, 0xe9,
            0xaa, 0xbf, 0xb2, 0x4f,
            0x25, 0x5d, 0x20, 0x17,
            0xf3, 0x7d, 0x39, 0xea,
        ];

        using HashAlgorithm h = new Tiger160(DefaultPasses + 1);

        byte[] hash = h.ComputeHash(ASCII.GetBytes(TestData));

        IsTrue(hash.SequenceEqual(testHash));
    }

    /// <summary>
    /// Tests <see cref="Tiger160" /> in span mode for additional passes with a test string.
    /// </summary>
    [TestMethod]
    //// ReSharper disable once TooManyDeclarations
    public void TestExtraPassesTry()
    {
        const string TestData = "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq12345678";
        byte[] testHash =
        [
            0xc5, 0xad, 0x43, 0xa0,
            0x90, 0x69, 0x95, 0xe9,
            0xaa, 0xbf, 0xb2, 0x4f,
            0x25, 0x5d, 0x20, 0x17,
            0xf3, 0x7d, 0x39, 0xea,
        ];
        int inputByteCount = ASCII.GetByteCount(TestData);
        Span<byte> bytes = stackalloc byte[inputByteCount];

        _ = ASCII.GetBytes(TestData, bytes);
        using HashAlgorithm h = new Tiger160(DefaultPasses + 1);

        // ReSharper disable once ComplexConditionExpression
        Span<byte> destination = stackalloc byte[h.HashSize >> 3];
        bool result = h.TryComputeHash(bytes, destination, out int bytesWritten);

        IsTrue(result);
        AreEqual(destination.Length, bytesWritten);
        IsTrue(destination.SequenceEqual(testHash));
    }
}
