// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Input
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

                    AddValue(new ObservationValue(ValueKeys.Count, value.Count()));

                }
                else
                {
                    AddValue(new ObservationValue(ValueKeys.Count, 0));
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

        public DataSetObservationInput(IObservationInput observation)
        {
            if (observation != null)
            {
                DeviceKey = observation.DeviceKey;
                DataItemKey = observation.DataItemKey;
                Timestamp = observation.Timestamp;
                Values = observation.Values;
            }
        }
    }
}