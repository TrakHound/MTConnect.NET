// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using System;
using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment where the reported value(s) are represented as a set of key-value pairs.
    /// </summary>
    public class DataSetObservation : IDataSetObservation
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
        /// Key-value pairs published as part of a Data Set observation
        /// </summary>
        public IEnumerable<DataSetEntry> Entries { get; set; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// For those DataItem elements that report data that may be periodically reset to an initial value, 
        /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
        /// </summary>
        public DataItemResetTrigger ResetTrigger { get; set; }

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
                    foreach (var entry in Entries) x += $"{entry.Key}={entry.Value}|";
                    return x;
                }

                return null;
            }
        }


        public DataSetObservation() { }

        public DataSetObservation(string key, IEnumerable<DataSetEntry> entries)
        {
            Key = key;
            Entries = entries;
        }

        public DataSetObservation(string key, IEnumerable<DataSetEntry> entries, long timestamp)
        {
            Key = key;
            Entries = entries;
            Timestamp = timestamp;
        }

        public DataSetObservation(string key, IEnumerable<DataSetEntry> entries, DateTime timestamp)
        {
            Key = key;
            Entries = entries;
            Timestamp = timestamp.ToUnixTime();
        }
    }
}
