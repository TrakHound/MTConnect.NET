// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// An Observation representing an MTConnect Sample or EVENT with a Representation of DATA_SET
    /// </summary>
    public class ShdrDataSet : DataSetObservationInput
    {


        /// <summary>
        /// Flag to set whether the Observation has been sent by the adapter or not
        /// </summary>
        internal bool IsSent { get; set; }


        /// <summary>Creates an empty SHDR data-set observation for builder-style population.</summary>
        public ShdrDataSet() { }

        /// <summary>Creates an SHDR data-set observation scoped to a single DataItem key (no entries set).</summary>
        /// <param name="dataItemKey">The DataItem id or name the data-set applies to.</param>
        public ShdrDataSet(string dataItemKey)
        {
            DataItemKey = dataItemKey;
        }

        /// <summary>Creates an SHDR data-set observation with the supplied DataItem key and entry set.</summary>
        public ShdrDataSet(string dataItemKey, IEnumerable<IDataSetEntry> entries)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
        }

        /// <summary>Creates an SHDR data-set observation with an explicit Unix-time <paramref name="timestamp"/> (milliseconds since epoch).</summary>
        public ShdrDataSet(string dataItemKey, IEnumerable<IDataSetEntry> entries, long timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp;
        }

        /// <summary>Creates an SHDR data-set observation with an explicit <paramref name="timestamp"/>; converted to Unix time during construction.</summary>
        public ShdrDataSet(string dataItemKey, IEnumerable<IDataSetEntry> entries, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp.ToUnixTime();
        }

        /// <summary>Clones the supplied <see cref="DataSetObservationInput"/> into an SHDR-flavoured observation.</summary>
        public ShdrDataSet(DataSetObservationInput dataSetObservation)
        {
            if (dataSetObservation != null)
            {
                DeviceKey = dataSetObservation.DeviceKey;
                DataItemKey = dataSetObservation.DataItemKey;
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
            if (!string.IsNullOrEmpty(DataItemKey))
            {
                var target = DataItemKey;
                if (!string.IsNullOrEmpty(DeviceKey)) target = $"{DeviceKey}:{target}";

                var resetTriggered = ResetTriggered != ResetTriggered.NOT_SPECIFIED ? $":{ResetTriggered} " : "";

                if (Timestamp > 0)
                {
                    if (!IsUnavailable)
                    {
                        return $"{GetTimestampString(Timestamp, timeZoneInfo: TimeZoneInfo)}|{target}|{resetTriggered}{PrintEntries(Entries)}";
                    }
                    else
                    {
                        return $"{GetTimestampString(Timestamp, timeZoneInfo: TimeZoneInfo)}|{target}|{Observation.Unavailable}";
                    }
                }
                else
                {
                    if (!IsUnavailable)
                    {
                        return $"|{target}|{resetTriggered}{PrintEntries(Entries)}";
                    }
                    else
                    {
                        return $"|{target}|{Observation.Unavailable}";
                    }
                }
            }

            return null;
        }

        private static string PrintEntries(IEnumerable<IDataSetEntry> entries)
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
                ShdrLine.ParseTimestampField(x, out var timestamp, out var duration);

                var y = ShdrLine.GetNextSegment(input);
                return FromLine(y, timestamp, duration);
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
                    dataSet.Duration = duration;

                    // Set DataItemKey
                    var x = ShdrLine.GetNextValue(input);
                    var y = ShdrLine.GetNextSegment(input);

                    // Get Device Key (if specified). Example : Device01:avail
                    var separator = x.LastIndexOf(':');
                    if (separator >= 0)
                    {
                        dataSet.DeviceKey = x.Substring(0, separator);
                        dataSet.DataItemKey = x.Substring(separator + 1);
                    }
                    else
                    {
                        dataSet.DataItemKey = x;
                    }

                    if (y != null)
                    {
                        x = ShdrLine.GetNextValue(y);
                        if (!string.IsNullOrEmpty(x))
                        {
                            x = ShdrLine.GetNextValue(y);
                            if (x.ToLower() != Observation.Unavailable.ToLower())
                            {
                                // Parse the ResetTriggered (if exists), e.g. ":MANUAL a=1 b=2"
                                var entriesString = ShdrLine.ParseResetTriggered(x, out var resetTriggered, allowSuffix: false);
                                dataSet.ResetTriggered = resetTriggered;

                                var dataSetEntries = new List<ShdrDataSetEntry>();

                                // Get a List of Entries representing DataSetEntry objects
                                var entries = ShdrLine.GetEntries(entriesString);
                                if (!entries.IsNullOrEmpty())
                                {
                                    foreach (var entry in entries)
                                    {
                                        // Create new ShdrDataSetEntry and add to return list
                                        var dataSetEntry = new ShdrDataSetEntry(entry.Key, entry.Value, entry.Removed);
                                        dataSetEntries.Add(dataSetEntry);
                                    }
                                }

                                dataSet.Entries = dataSetEntries;
                            }
                            else
                            {
                                dataSet.Unavailable();
                            }
                        }
                    }

                    return dataSet;
                }
                catch { }
            }

            return null;
        }
    }
}