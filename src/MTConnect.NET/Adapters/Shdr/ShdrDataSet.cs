// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MTConnect.Adapters.Shdr
{
    public class ShdrDataSet : DataSetObservation
    {
        public bool IsUnavailable { get; set; }

        public bool IsSent { get; set; }


        public ShdrDataSet() { }

        public ShdrDataSet(string key, IEnumerable<DataSetEntry> entries)
        {
            Key = key;
            Entries = entries;
        }

        public ShdrDataSet(string key, IEnumerable<DataSetEntry> entries, long timestamp)
        {
            Key = key;
            Entries = entries;
            Timestamp = timestamp;
        }

        public ShdrDataSet(string key, IEnumerable<DataSetEntry> entries, DateTime timestamp)
        {
            Key = key;
            Entries = entries;
            Timestamp = timestamp.ToUnixTime();
        }

        public ShdrDataSet(DataSetObservation dataSetObservation)
        {
            if (dataSetObservation != null)
            {
                DeviceName = dataSetObservation.DeviceName;
                Key = dataSetObservation.Key;
                Entries = dataSetObservation.Entries;
                Timestamp = dataSetObservation.Timestamp;
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

        private static string PrintEntries(IEnumerable<DataSetEntry> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var pairs = new List<string>();

                foreach (var entry in entries)
                {
                    pairs.Add(new ShdrDataSetEntry(entry).ToString());
                }

                return string.Join(" ", pairs);
            }

            return "";
        }

        /// <summary>
        /// Read a ShdrDataSet object from an SHDR line
        /// </summary>
        /// <param name="input">SHDR Input String</param>
        public static ShdrDataSet FromString(string input)
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

        private static ShdrDataSet FromLine(string input, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    var dataSet = new ShdrDataSet();
                    dataSet.Timestamp = timestamp;

                    // Set DataItemId
                    var x = ShdrLine.GetNextValue(input);
                    var y = ShdrLine.GetNextSegment(input);
                    dataSet.Key = x;

                    if (y != null)
                    {
                        x = ShdrLine.GetNextValue(y);
                        if (!string.IsNullOrEmpty(x))
                        {
                            var entries = new List<DataSetEntry>();

                            // Regular Expression that matches groups like x=y and takes into account the ' and " characters
                            var regex = new Regex(@"\s*([^\=\s]+)\=([^\s\'\""]+)|\s*([^\=\s]+)\='(.*)'|\s*([^\=\s]+)\=""(.*)""");

                            var matches = regex.Matches(x);
                            if (matches != null && matches.Count > 0)
                            {
                                foreach (Match match in matches)
                                {
                                    if (match.Success && match.Groups != null && match.Groups.Count > 1)
                                    {
                                        var success = false;
                                        string key = null;
                                        string value = null;

                                        // Matches have 6 Groups plus the 0 Group (holding entire Match)
                                        // - Odd Indexes are the Keys
                                        // - Event Indexes are the Values

                                        for (var i = 1; i < match.Groups.Count; i++)
                                        {
                                            var group = match.Groups[i];
                                            if (group.Success)
                                            {
                                                success = true;

                                                if (i.IsOdd()) key = group.Value;
                                                else value = group.Value;
                                            }
                                        }

                                        // Add new DataSet to Entries
                                        if (success) entries.Add(new DataSetEntry(key, value));
                                    }
                                }
                            }

                            if (dataSet.Entries.IsNullOrEmpty())
                            {
                                dataSet.Entries = entries;

                                return dataSet;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        //private static ShdrDataSet FromLine(string input, long timestamp = 0)
        //{
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        try
        //        {
        //            var dataSet = new ShdrDataSet();
        //            dataSet.Timestamp = timestamp;

        //            // Set DataItemId
        //            var x = ShdrLine.GetNextValue(input);
        //            var y = ShdrLine.GetNextSegment(input);
        //            dataSet.Key = x;

        //            if (y != null)
        //            {
        //                x = ShdrLine.GetNextValue(y);
        //                if (!string.IsNullOrEmpty(x))
        //                {
        //                    var entries = new List<DataSetEntry>();
        //                    var entrySegments = x.Split(' ');

        //                    foreach (var entrySegment in entrySegments)
        //                    {
        //                        var entry = ShdrDataSetEntry.FromString(entrySegment);
        //                        if (entry != null)
        //                        {
        //                            entries.Add(entry);
        //                        }                         
        //                    }

        //                    dataSet.Entries = entries;

        //                    return dataSet;
        //                }
        //            }
        //        }
        //        catch { }
        //    }

        //    return null;
        //}
    }
}
