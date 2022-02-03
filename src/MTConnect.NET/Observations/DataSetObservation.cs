// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment where the reported value(s) are represented as a set of key-value pairs.
    /// </summary>
    public class DataSetObservation : Observation
    {
        public const string EntryRemovedValue = "[!ENTRY_REMOVED!]";


        /// <summary>
        /// Key-value pairs published as part of a Data Set observation
        /// </summary>
        public IEnumerable<DataSetEntry> Entries
        {
            get
            {
                var entries = new List<DataSetEntry>();

                if (!Values.IsNullOrEmpty())
                {
                    var entryValues = Values.Where(o => o.ValueType != null && o.ValueType.StartsWith(ValueTypes.DataSetPrefix));
                    if (!entryValues.IsNullOrEmpty())
                    {
                        var oValues = entryValues.OrderBy(o => ValueTypes.GetDataSetKey(o.ValueType));
                        foreach (var value in oValues)
                        {
                            var key = ValueTypes.GetDataSetKey(value.ValueType);
                            bool removed = value.Value == EntryRemovedValue;
                            entries.Add(new DataSetEntry(key, value.Value, removed));
                        }
                    }
                }

                return entries;
            }
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var entry in value)
                    {
                        if (entry.Removed)
                        {
                            AddValue(new ObservationValue(ValueTypes.CreateDataSetValueType(entry.Key), EntryRemovedValue));
                        }
                        else
                        {
                            AddValue(new ObservationValue(ValueTypes.CreateDataSetValueType(entry.Key), entry.Value));
                        }
                    }
                }
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
