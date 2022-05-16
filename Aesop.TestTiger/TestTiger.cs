// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestTiger.cs" company="Always Elucidated Solution Pioneers, LLC">
//   Copyright (c) Always Elucidated Solution Pioneers, LLC. All rights reserved.
// </copyright>
// <summary>
// This file is part of Aesop.TestTiger.
//
// Aesop.TestTiger tests the three variants of the Tiger algorithm by Ross
// Anderson and Eli Biham (http://www.cs.technion.ac.il/~biham/Reports/Tiger/)
// as implemented in Aesop.Tiger.
// Copyright (c) Always Elucidated Solution Pioneers, LLC. All rights reserved.
//
// Aesop.TestTiger is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// Aesop.TestTiger is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
//
// Aesop.TestTiger version 1.0.3.0, Copyright (C) 2007 Always Elucidated Solution Pioneers, LLC
// Aesop.TestTiger comes with ABSOLUTELY NO WARRANTY.
// This is free software, and you are welcome to redistribute it
// under certain conditions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aesop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    using static System.Console;
    using static System.Globalization.CultureInfo;

    /// <summary>
    /// Holds the program's entry point.
    /// </summary>
    internal static class TestTiger
    {
        /// <summary>
        /// The program's entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static async Task Main(string[] args)
        {
            TigerFull h1 = new Tiger192();
            ////var h2 = new MD5CryptoServiceProvider();
            ////var h3 = new SHA1CryptoServiceProvider();
            ////var h4 = new SHA1Managed();
            ////var h5 = new SHA256Managed();

            // Change h1 to h2, etc. for testing.
            using (TigerFull h = h1)
            {
                await Out.WriteLineAsync(string.Format(
                    CurrentCulture,
                    "IntPtr size is: {0}",
                    IntPtr.Size)).ConfigureAwait(false);
                if (await TestMyTigerAsync(h).ConfigureAwait(false) && (args.Length > 0))
                {
                    await HashFileAsync(args[0], h).ConfigureAwait(false);
                }
            }

            using (TigerFull h = new Tiger160())
            {
                await TestMyTigerAsync(h).ConfigureAwait(false);
            }

            using (TigerFull h = new Tiger128())
            {
                await TestMyTigerAsync(h).ConfigureAwait(false);
            }

            await In.ReadLineAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the Tiger algorithm.
        /// </summary>
        /// <param name="h">The hash algorithm.</param>
        /// <returns><c>true</c> if the test succeeds, <c>false</c> otherwise.</returns>
        private static async Task<bool> TestMyTigerAsync(IDisposable h)
        {
            if (h is not TigerFull h1)
            {
                return true;
            }

            List<byte> hash = (h1.SelfTest() ?? Enumerable.Empty<byte>()).ToList();

            await OutputHashAsync(hash).ConfigureAwait(false);
            await Out.WriteLineAsync(string.Format(
                CurrentCulture,
                "Tiger/{0} {1}",
                h1.HashSize,
                hash.Any() ? "good" : "bad")).ConfigureAwait(false);
            return hash.Any();
        }

        /// <summary>
        /// Gets the hash value for the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="h">The hash algorithm.</param>
        private static async Task HashFileAsync(string fileName, HashAlgorithm h)
        {
            Stopwatch stopwatch;
            FileInfo fi = new (fileName);

            await using (Stream s = new FileStream(
                fi.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                (int)fi.Length,
                FileOptions.SequentialScan))
            {
                stopwatch = Stopwatch.StartNew();
                try
                {
                    await h.ComputeHashAsync(s).ConfigureAwait(false);
                }
                finally
                {
                    stopwatch.Stop();
                }
            }

            await Out.WriteAsync(string.Format(CurrentCulture, "\"{0}\": ", fileName)).ConfigureAwait(false);
            await OutputHashAsync(h.Hash).ConfigureAwait(false);
            await Out.WriteLineAsync(string.Format(
                CurrentCulture,
                "Time: {0}",
                stopwatch.Elapsed)).ConfigureAwait(false);
            ////if (h is TigerFull)
            ////{
            ////    ((TigerFull)h).Timings();
            ////}
        }

        /// <summary>
        /// Outputs the hash.
        /// </summary>
        /// <param name="hash">The hash.</param>
        private static async Task OutputHashAsync(IEnumerable<byte> hash)
        {
            foreach (byte by in hash)
            {
                await Out.WriteAsync(by.ToString("X", InvariantCulture)).ConfigureAwait(false);
            }

            await Out.WriteLineAsync().ConfigureAwait(false);
        }
    }
}
