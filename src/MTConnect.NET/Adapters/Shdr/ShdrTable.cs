// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MTConnect.Observations;

namespace MTConnect.Adapters.Shdr
{
    public class ShdrTable : TableObservation
    {
        public bool IsUnavailable { get; set; }

        public bool IsSent { get; set; }


        public ShdrTable() { }

        public ShdrTable(string key, IEnumerable<TableEntry> entries)
        {
            Key = key;
            Entries = entries;
        }

        public ShdrTable(string key, IEnumerable<TableEntry> entries, long timestamp)
        {
            Key = key;
            Entries = entries;
            Timestamp = timestamp;
        }

        public ShdrTable(string key, IEnumerable<TableEntry> entries, DateTime timestamp)
        {
            Key = key;
            Entries = entries;
            Timestamp = timestamp.ToUnixTime();
        }

        public ShdrTable(TableObservation tableObservation)
        {
            if (tableObservation != null)
            {
                DeviceName = tableObservation.DeviceName;
                Key = tableObservation.Key;
                Entries = tableObservation.Entries;
                Timestamp = tableObservation.Timestamp;
            }
        }


        /// <summary>
        /// Convert ShdrDataSet to an SHDR string
        /// </summary>
        /// <returns>SHDR string</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Key) && !Entries.IsNullOrEmpty())
            {
                if (Timestamp > 0)
                {
                    if (!IsUnavailable)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{Key}|{PrintEntries(Entries)}";
                    }
                    else
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{Key}|{Streams.DataItem.Unavailable}";
                    }
                }
                else
                {
                    if (!IsUnavailable)
                    {
                        return $"{Key}|{PrintEntries(Entries)}";
                    }
                    else
                    {
                        return $"{Key}|{Streams.DataItem.Unavailable}";
                    }
                }
            }

            return null;
        }

        private static string PrintEntries(IEnumerable<TableEntry> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var pairs = new List<string>();

                foreach (var entry in entries)
                {
                    pairs.Add(new ShdrTableEntry(entry).ToString());
                }

                return string.Join(" ", pairs);
            }

            return "";
        }

        /// <summary>
        /// Read a ShdrTable object from an SHDR line
        /// </summary>
        /// <param name="input">SHDR Input String</param>
        public static ShdrTable FromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                // Start reading input and read Timestamp first (if specified)
                var x = ShdrLine.GetNextValue(input);

                if (DateTime.TryParse(x, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var timestamp))
                {
                    var y = ShdrLine.GetNextSegment(input);
                    return FromLine(y, timestamp.ToUnixTime());
                }
                else
                {
                    return FromLine(input);
                }
            }

            return null;
        }

        private static ShdrTable FromLine(string input, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    var table = new ShdrTable();
                    table.Timestamp = timestamp;

                    // Set DataItemId
                    var x = ShdrLine.GetNextValue(input);
                    var y = ShdrLine.GetNextSegment(input);
                    table.Key = x;

                    if (y != null)
                    {
                        x = ShdrLine.GetNextValue(y);
                        if (!string.IsNullOrEmpty(x))
                        {
                            var entries = new List<TableEntry>();

                            var regex = new Regex(@"([a-zA-Z0-9\.]*=\{[^{}]*\})");
                            var entrySegments = regex.Matches(x);
                            if (entrySegments.Count > 0)
                            {
                                foreach (var entrySegment in entrySegments)
                                {
                                    var entry = ShdrTableEntry.FromString(entrySegment.ToString());
                                    if (entry != null)
                                    {
                                        entries.Add(entry);
                                    }
                                }
                            }

                            table.Entries = entries;

                            return table;
                        }
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
