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

namespace Aesop.Tiger.Tests
{
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the functionality of the <see cref="Tiger160" /> class.
    /// </summary>
    [TestClass]
    public sealed class Tiger160Tests
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
            using Tiger160 h = new (DefaultPasses);
            Assert.IsNotNull(h.SelfTest());
        }

        /// <summary>
        /// Tests that <see cref="Tiger160" /> supports transforming multiple blocks.
        /// </summary>
        [TestMethod]
        public void TestCanTransformMultipleBlocks()
        {
            using Tiger160 h = new ();
            Assert.IsTrue(h.CanTransformMultipleBlocks);
        }

        /// <summary>
        /// Tests <see cref="Tiger160" /> Passes property returns the proper default.
        /// </summary>
        [TestMethod]
        public void TestPasses()
        {
            using Tiger160 h = new ();
            Assert.AreEqual(DefaultPasses, h.Passes);
        }

        /// <summary>
        /// Tests <see cref="Tiger160" /> for additional passes with a test string.
        /// </summary>
        [TestMethod]
        public void TestExtraPasses()
        {
            const string TestData = "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq12345678";
            byte[] testHash =
            {
                0xc5, 0xad, 0x43, 0xa0,
                0x90, 0x69, 0x95, 0xe9,
                0xaa, 0xbf, 0xb2, 0x4f,
                0x25, 0x5d, 0x20, 0x17,
                0xf3, 0x7d, 0x39, 0xea,
            };

            using Tiger160 h = new (DefaultPasses + 1);
            byte[] hash = h.ComputeHash(Encoding.ASCII.GetBytes(TestData));

            Assert.IsTrue(hash.SequenceEqual(testHash));
        }
    }
}
