// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace MTConnect.Shdr
{
    /// <summary>
    /// An Observation representing an MTConnect Sample or EVENT with a Representation of TABLE
    /// </summary>
    public class ShdrTable : TableObservationInput
    {
        private static readonly Regex _deviceKeyRegex = new Regex("(.*):(.*)");


        /// <summary>
        /// Flag to set whether the Observation has been sent by the adapter or not
        /// </summary>
        internal bool IsSent { get; set; }


        /// <summary>Creates an empty SHDR table observation for builder-style population.</summary>
        public ShdrTable() { }

        /// <summary>Creates an SHDR table observation scoped to a single DataItem key (no rows set).</summary>
        public ShdrTable(string dataItemKey)
        {
            DataItemKey = dataItemKey;
        }

        /// <summary>Creates an SHDR table observation with the supplied DataItem key and table rows.</summary>
        public ShdrTable(string dataItemKey, IEnumerable<ITableEntry> entries)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
        }

        /// <summary>Creates an SHDR table observation with an explicit Unix-time <paramref name="timestamp"/> (milliseconds since epoch).</summary>
        public ShdrTable(string dataItemKey, IEnumerable<ITableEntry> entries, long timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp;
        }

        /// <summary>Creates an SHDR table observation with an explicit <paramref name="timestamp"/>; converted to Unix time during construction.</summary>
        public ShdrTable(string dataItemKey, IEnumerable<ITableEntry> entries, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp.ToUnixTime();
        }

        /// <summary>Clones the supplied <see cref="TableObservationInput"/> into an SHDR-flavoured observation.</summary>
        public ShdrTable(TableObservationInput tableObservation)
        {
            if (tableObservation != null)
            {
                DeviceKey = tableObservation.DeviceKey;
                DataItemKey = tableObservation.DataItemKey;
                Entries = tableObservation.Entries;
                Timestamp = tableObservation.Timestamp;
            }
        }


        /// <summary>
        /// Convert ShdrTable to an SHDR string
        /// </summary>
        /// <returns>SHDR string</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(DataItemKey))
            {
                var target = DataItemKey;
                if (!string.IsNullOrEmpty(DeviceKey)) target = $"{DeviceKey}:{target}";

                if (Timestamp > 0)
                {
                    if (!IsUnavailable)
                    {
                        return $"{GetTimestampString(Timestamp, timeZoneInfo: TimeZoneInfo)}|{target}|{PrintEntries(Entries)}";
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
                        return $"|{target}|{PrintEntries(Entries)}";
                    }
                    else
                    {
                        return $"|{target}|{Observation.Unavailable}";
                    }
                }
            }

            return null;
        }

        private static string PrintEntries(IEnumerable<ITableEntry> entries)
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
                ShdrLine.ParseTimestampField(x, out var timestamp, out var duration);

                var y = ShdrLine.GetNextSegment(input);
                return FromLine(y, timestamp, duration);
            }

            return null;
        }

        private static ShdrTable FromLine(string input, long timestamp = 0, double duration = 0)
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    var table = new ShdrTable();
                    table.Timestamp = timestamp;
                    table.Duration = duration;

                    // Set DataItemKey
                    var x = ShdrLine.GetNextValue(input);
                    var y = ShdrLine.GetNextSegment(input);

                    // Get Device Key (if specified). Example : Device01:avail
                    var match = _deviceKeyRegex.Match(x);
                    if (match.Success && match.Groups.Count > 2)
                    {
                        table.DeviceKey = match.Groups[1].Value;
                        table.DataItemKey = match.Groups[2].Value;
                    }
                    else
                    {
                        table.DataItemKey = x;
                    }

                    if (y != null)
                    {
                        x = ShdrLine.GetNextValue(y);
                        if (!string.IsNullOrEmpty(x))
                        {
                            x = ShdrLine.GetNextValue(y);
                            if (x.ToLower() != Observation.Unavailable.ToLower())
                            {
                                // Parse the ResetTriggered (if exists), e.g. ":MANUAL row={c=1}"
                                var entriesString = ShdrLine.ParseResetTriggered(x, out var resetTriggered, allowSuffix: false);
                                table.ResetTriggered = resetTriggered;

                                var tableEntries = new List<ShdrTableEntry>();

                                // Get a List of Entries representing TableEntry objects
                                var entries = ShdrLine.GetEntries(entriesString);
                                if (!entries.IsNullOrEmpty())
                                {
                                    foreach (var entry in entries)
                                    {
                                        var tableCells = new List<ShdrTableCell>();

                                        if (!string.IsNullOrEmpty(entry.Value))
                                        {
                                            // Remove the enclosing braces (exactly one pair; cells may contain braces)
                                            var entryValue = entry.Value;
                                            if (entryValue.Length > 1 && entryValue[0] == '{' && entryValue[entryValue.Length - 1] == '}')
                                            {
                                                entryValue = entryValue.Substring(1, entryValue.Length - 2);
                                            }

                                            // Get a list of Entries representing Table Cells
                                            var cells = ShdrLine.GetEntries(entryValue);
                                            if (!cells.IsNullOrEmpty())
                                            {
                                                foreach (var cell in cells)
                                                {
                                                    // Create new ShdrTableCell and add to cells list
                                                    var tableCell = new ShdrTableCell(cell.Key, cell.Value);
                                                    tableCells.Add(tableCell);
                                                }
                                            }
                                        }

                                        // Create new ShdrTableEntry and add to return list
                                        var tableEntry = new ShdrTableEntry(entry.Key, tableCells);
                                        tableEntry.Removed = entry.Removed;
                                        tableEntries.Add(tableEntry);
                                    }
                                }

                                table.Entries = tableEntries;
                            }
                            else
                            {
                                table.Unavailable();
                            }
                        }
                    }

                    return table;
                }
                catch { }
            }

            return null;
        }
    }
}