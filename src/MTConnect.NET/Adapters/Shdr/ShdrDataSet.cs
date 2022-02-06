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
        private const string _emptySingleEntryPattern = @"^([^=\s]+)\={0,1}$";
        private const string _emptyEntryPattern = @"([^=\s]+)\s+";
        private const string _emptyEntryWithEqualsPattern = @"([^=\s]+)\={1}\s+";
        private const string _entryPattern = @"([^=\s]+)\={1}([^\s\'\""\{\}]+)";
        private const string _entrySingleQuotesPattern = @"([^=\s]+)\={ 1}('[^'\\]*(?:\\.[^'\\]*)*')";
        private const string _entryDoubleQuotesPattern = @"([^=\s]+)\={1}(\""[^'\\]*(?:\\.[^'\\]*)*\"")";
        private const string _entryCurlyBracesPattern = @"([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})";

        private static readonly Regex _entriesRegex = new Regex($"{_emptySingleEntryPattern}|{_emptyEntryPattern}|{_emptyEntryWithEqualsPattern}|{_entryPattern}|{_entrySingleQuotesPattern}|{_entryDoubleQuotesPattern}|{_entryCurlyBracesPattern}");
        private static readonly Regex _resetTriggeredRegex = new Regex(@":([A-Z_]+)\s+(.*)");


        public bool IsUnavailable { get; set; }

        public bool IsSent { get; set; }


        public ShdrDataSet() { }

        public ShdrDataSet(string key)
        {
            Key = key;
        }

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
            if (!string.IsNullOrEmpty(Key))
            {
                var resetTriggered = ResetTriggered != Streams.ResetTriggered.NOT_SPECIFIED ? $":{ResetTriggered} " : "";

                if (Timestamp > 0)
                {
                    if (!IsUnavailable)
                    {
                        return $"{Timestamp.ToDateTime().ToString("o")}|{Key}|{resetTriggered}{PrintEntries(Entries)}";
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
                        return $"{Key}|{resetTriggered}{PrintEntries(Entries)}";
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
                var timestamp = ShdrLine.GetTimestamp(x);
                var duration = ShdrLine.GetDuration(x);

                if (timestamp.HasValue)
                {
                    var y = ShdrLine.GetNextSegment(input);
                    return FromLine(y, timestamp.Value.ToUnixTime(), duration.HasValue ? duration.Value : 0);
                }
                else
                {
                    return FromLine(input);
                }
            }

            return null;
        }

        private static ShdrDataSet FromLine(string input, long timestamp = 0, double duration = 0)
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
                            dataSet.ResetTriggered = Streams.ResetTriggered.NOT_SPECIFIED;
                            var entriesString = x;

                            // Parse the ResetTriggered (if exists)
                            //var resetRegex = new Regex(@":([A-Z_]+)\s+(.*)");
                            //var resetMatch = resetRegex.Match(x);
                            var resetMatch = _resetTriggeredRegex.Match(x);
                            if (resetMatch.Success && resetMatch.Groups.Count > 2)
                            {
                                dataSet.ResetTriggered = resetMatch.Groups[1].Value.ConvertEnum<Streams.ResetTriggered>();
                                entriesString = resetMatch.Groups[2].Value;
                            }


                            var entries = new List<DataSetEntry>();

                            // Regular Expression that matches groups like x=y and takes into account the ', ", and {} characters
                            // ([^=\s]+)\s+|([^=\s]+)\={1}\s+|([^=\s]+)\={1}([^\s\'\"\{\}]+)|([^=\s]+)\={1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\"[^'\\]*(?:\\.[^'\\]*)*\")|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})
                            // ([^=\s]+)\s+|([^=\s]+)\={1}\s+|([^=\s]+)\={1}([^\s\'\"\{\}]+)|([^=\s]+)\={1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})
                            // ([^=\s]+)\={1}([^\s\'\"\{\}]+)|([^=\s]+)\={1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})
                            // \s*([^\=\s]+)\=?\s+|\s*([^\=\s]+)\={1}([^\s\'\"\{\}])|\s*([^\=\s]+)\={1}'(.*)'|\s*([^\=\s]+)\={1}"(.*)"|\s*([^\=\s]+)\={1}\{(.*)\}
                            //var regex = new Regex(@"\s*([^\=\s]+)\=([^\s\'\""]+)|\s*([^\=\s]+)\='(.*)'|\s*([^\=\s]+)\=""(.*)""");
                            //var regex = new Regex(@"([^=\s]+)\s+|([^=\s]+)\={1}\s+|([^=\s]+)\={1}([^\s\'\""\{\}]+)|([^=\s]+)\={ 1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\""[^'\\]*(?:\\.[^'\\]*)*\"")|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})");
                            //var regex = new Regex(@"^([^=\s]+)\={0,1}$|([^=\s]+)\s+|([^=\s]+)\={1}\s+|([^=\s]+)\={1}([^\s\'\""\{\}]+)|([^=\s]+)\={ 1}('[^'\\]*(?:\\.[^'\\]*)*')|([^=\s]+)\={1}(\""[^'\\]*(?:\\.[^'\\]*)*\"")|([^=\s]+)\={1}(\{[^'\\]*(?:\\.[^'\\]*)*\})");

                            var matches = _entriesRegex.Matches(entriesString);
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

                                        // Add new DataSet to Entries
                                        if (success) entries.Add(new DataSetEntry(key, value, removed));
                                    }
                                }
                            }

                            if (dataSet.ResetTriggered != Streams.ResetTriggered.NOT_SPECIFIED || dataSet.Entries.IsNullOrEmpty())
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
    }
}
