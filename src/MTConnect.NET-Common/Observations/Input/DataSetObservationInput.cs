// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;

namespace MTConnect.Observations.Input
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment where the reported value(s) are represented as a set of key-value pairs.
    /// </summary>
    public class DataSetObservationInput : ObservationInput
    {
        /// <summary>
        /// Key-value pairs published as part of a Data Set observation
        /// </summary>
        public IEnumerable<IDataSetEntry> Entries
        {
            get => DataSetObservation.GetEntries(Values);
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var entry in value)
                    {
                        if (entry.Removed)
                        {
                            AddValue(new ObservationValue(ValueKeys.CreateDataSetValueKey(entry.Key), DataSetObservation.EntryRemovedValue));
                        }
                        else
                        {
                            AddValue(new ObservationValue(ValueKeys.CreateDataSetValueKey(entry.Key), entry.Value));
                        }
                    }
                }
            }
        }


        public DataSetObservationInput() { }

        public DataSetObservationInput(string dataItemKey, IEnumerable<IDataSetEntry> entries)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
        }

        public DataSetObservationInput(string dataItemKey, IEnumerable<IDataSetEntry> entries, long timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp;
        }

        public DataSetObservationInput(string dataItemKey, IEnumerable<IDataSetEntry> entries, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp.ToUnixTime();
        }
    }
}
