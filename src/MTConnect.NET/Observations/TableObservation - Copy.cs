// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment
    /// where the reported value(s) are represented as rows containing sets of key-value pairs given by Cell elements.
    /// </summary>
    public class TableObservation : ITableObservation
    {
        /// <summary>
        /// The name of the Device that the Observation is associated with
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// The (ID, Name, or Source) of the DataItem that the Observation is associated with
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Key-value pairs published as part of a Table observation
        /// </summary>
        public IEnumerable<TableEntry> Entries { get; set; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// A MD5 Hash of the Observation that can be used for comparison
        /// </summary>
        public string ChangeId
        {
            get
            {
                if (!Entries.IsNullOrEmpty())
                {
                    var x = "";
                    foreach (var entry in Entries)
                    {
                        x += $"{entry.Key}|";
                        if (!entry.Cells.IsNullOrEmpty())
                        {
                            foreach (var cell in entry.Cells)
                            {
                                x += $"{cell.Key}={cell.Value}|";
                            }
                        }
                    }
                    return x;
                }

                return null;
            }
        }


        public TableObservation() { }

        public TableObservation(string key, IEnumerable<TableEntry> entries)
        {
            Key = key;
            Entries = entries;
        }

        public TableObservation(string key, IEnumerable<TableEntry> entries, long timestamp)
        {
            Key = key;
            Entries = entries;
            Timestamp = timestamp;
        }

        public TableObservation(string key, IEnumerable<TableEntry> entries, DateTime timestamp)
        {
            Key = key;
            Entries = entries;
            Timestamp = timestamp.ToUnixTime();
        }
    }
}
