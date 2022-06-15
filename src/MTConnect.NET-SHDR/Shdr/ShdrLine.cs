// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// Tools for analyzing and extracting data from an SHDR text line
    /// </summary>
    public static class ShdrLine
    {
        public const char PipeDelimiter = '|';
        public const string LineTerminator = "\r\n";

        private static Regex _timestampRegex = new Regex(@"(.*)\@([0-9\.]+)");

        private const string _emptySingleEntryPattern = @"^([^=\s]+)\={0,1}$";
        private const string _emptyEntryPattern = @"([^=\s]+)\s+";
        private const string _emptyEntryWithEqualsPattern = @"([^=\s]+)\={1}\s+";
        private const string _entryPattern = @"([^=\s]+)\={1}([^\s\'\""\{\}]+)";
        private const string _entrySingleQuotesPattern = @"([^=\s]+)\={ 1}('[^'\\]*(?:\\.[^'\\]*)*')";
        private const string _entryDoubleQuotesPattern = @"([^=\s]+)\={1}(\""[^'\\]*(?:\\.[^'\\]*)*\"")";
        private const string _entryCurlyBracesPattern = @"([^=\s]+)\={1}(\{[^'\}\\]*(?:\\.[^'\{\\]*)*\})";

        private static readonly Regex _entriesRegex = new Regex($"{_emptySingleEntryPattern}|{_emptyEntryPattern}|{_emptyEntryWithEqualsPattern}|{_entryPattern}|{_entrySingleQuotesPattern}|{_entryDoubleQuotesPattern}|{_entryCurlyBracesPattern}");


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
                var i = s.IndexOf(PipeDelimiter);
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
                var i = s.IndexOf(PipeDelimiter);
                if (i >= 0 && i < s.Length - 1)
                {
                    return s.Substring(i + 1);
                }
            }

            return null;
        }

        internal static IEnumerable<ShdrEntry> GetEntries(string s)
        {
            var entries = new List<ShdrEntry>();

            if (!string.IsNullOrEmpty(s))
            {
                // Regular Expression that matches groups like x=y and takes into account the ', ", and {} characters
                // ([^=\s]+)\s+|([^=\s]+)\={1}\s+|([^=\s]+)\={1}([^\s\'\"\{\}]+)|([^=\s]+)\={1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\"[^'\\]*(?:\\.[^'\\]*)*\")|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})
                // ([^=\s]+)\s+|([^=\s]+)\={1}\s+|([^=\s]+)\={1}([^\s\'\"\{\}]+)|([^=\s]+)\={1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})
                // ([^=\s]+)\={1}([^\s\'\"\{\}]+)|([^=\s]+)\={1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})
                // \s*([^\=\s]+)\=?\s+|\s*([^\=\s]+)\={1}([^\s\'\"\{\}])|\s*([^\=\s]+)\={1}'(.*)'|\s*([^\=\s]+)\={1}"(.*)"|\s*([^\=\s]+)\={1}\{(.*)\}
                //var regex = new Regex(@"\s*([^\=\s]+)\=([^\s\'\""]+)|\s*([^\=\s]+)\='(.*)'|\s*([^\=\s]+)\=""(.*)""");
                //var regex = new Regex(@"([^=\s]+)\s+|([^=\s]+)\={1}\s+|([^=\s]+)\={1}([^\s\'\""\{\}]+)|([^=\s]+)\={ 1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\""[^'\\]*(?:\\.[^'\\]*)*\"")|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})");
                //var regex = new Regex(@"^([^=\s]+)\={0,1}$|([^=\s]+)\s+|([^=\s]+)\={1}\s+|([^=\s]+)\={1}([^\s\'\""\{\}]+)|([^=\s]+)\={ 1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\""[^'\\]*(?:\\.[^'\\]*)*\"")|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})");

                var matches = _entriesRegex.Matches(s);
                if (matches != null && matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        if (match.Success && match.Groups != null && match.Groups.Count > 4)
                        {
                            var success = false;
                            string key = null;
                            string value = null;
                            bool removed = false;

                            // Matches have 11 Groups plus the 0 Group (holding entire Match)
                            // Group 1 = v : v2 : Key - Only entry listed
                            // Group 2 = v : v2 : Key - One of Multiple entries
                            // Group 3 = v2= : v2 : Key
                            // Group 4 = v2=53 : v2 : Key
                            // Group 5 = v2=53 : 53 : Value
                            // Group 6 = v2='53' : v2 : Key
                            // Group 7 = v2='53' : 53 : Value
                            // Group 8 = v2="53" : v2 : Key
                            // Group 9 = v2="53" : 53 : Value
                            // Group 10 = v2={53} : v2 : Key
                            // Group 11 = v2={53} : 53 : Value

                            if (match.Groups[1].Success)
                            {
                                success = true;
                                removed = true;
                                key = match.Groups[1].Value;
                            }

                            if (!success & match.Groups[2].Success)
                            {
                                success = true;
                                removed = true;
                                key = match.Groups[2].Value;
                            }

                            if (!success & match.Groups[3].Success)
                            {
                                success = true;
                                removed = true;
                                key = match.Groups[3].Value;
                            }

                            // Above Group 3 :
                            // - Even Indexes are the Keys
                            // - Odd Indexes are the Values

                            if (!success)
                            {
                                for (var i = 4; i < match.Groups.Count; i++)
                                {
                                    var group = match.Groups[i];
                                    if (group.Success)
                                    {
                                        success = true;

                                        if (i.IsOdd()) value = group.Value;
                                        else key = group.Value;
                                    }
                                }
                            }

                            // Add new TableEntry to Entries
                            if (success)
                            {
                                entries.Add(new ShdrEntry(key, value, removed));
                            }
                        }
                    }
                }
            }

            return entries;
        }
    }
}
