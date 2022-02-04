// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Text.RegularExpressions;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// Tools for analyzing and extracting data from an SHDR text line
    /// </summary>
    internal static class ShdrLine
    {
        private static Regex _timestampRegex = new Regex(@"(.*)\@([0-9\.]+)");


        /// <summary>
        /// Determine if the SHDR Line represents an MTConnect Asset
        /// </summary>
        /// <param name="line">SHDR Line</param>
        /// <returns>Returns 'true' if the line represents an MTConnect Asset</returns>
        internal static bool IsAsset(string line)
        {
            return !string.IsNullOrEmpty(line) && line.StartsWith("@ASSET@");
        }

        internal static DateTime? GetTimestamp(string s)
        {
            string x = s;

            // Expected Format
            // Without Duration : 2014-09-29T23:59:33.460470Z
            // With Duration : 2014-09-29T23:59:33.460470Z@100.0
            var match = _timestampRegex.Match(s);
            if (match.Success && match.Groups != null && match.Groups.Count > 2)
            {
                // First Group contains Timestamp
                x = match.Groups[1].Value;
            }

            if (DateTime.TryParse(x, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var y))
            {
                return y;
            }

            return null;
        }

        internal static double? GetDuration(string s)
        {
            // Expected Format
            // Without Duration : 2014-09-29T23:59:33.460470Z
            // With Duration : 2014-09-29T23:59:33.460470Z@100.0
            var match = _timestampRegex.Match(s);
            if (match.Success && match.Groups != null && match.Groups.Count > 2)
            {
                // Second group contains Duration
                var x = match.Groups[2].Value;

                if (double.TryParse(x, out var y))
                {
                    return y;
                }
            }

            return null;
        }


        internal static string GetNextValue(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var i = s.IndexOf('|');
                if (i >= 0)
                {
                    return s.Substring(0, i);
                }
                else return s;
            }

            return null;
        }

        internal static string GetNextSegment(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var i = s.IndexOf('|');
                if (i >= 0 && i < s.Length - 1)
                {
                    return s.Substring(i + 1);
                }
            }

            return null;
        }
    }
}
