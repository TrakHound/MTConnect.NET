// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// Tools for analyzing and extracting data from an SHDR text line
    /// </summary>
    internal static class ShdrLine
    {
        /// <summary>
        /// Determine if the SHDR Line represents an MTConnect Asset
        /// </summary>
        /// <param name="line">SHDR Line</param>
        /// <returns>Returns 'true' if the line represents an MTConnect Asset</returns>
        internal static bool IsAsset(string line)
        {
            return !string.IsNullOrEmpty(line) && line.StartsWith("@ASSET@");
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
