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


        /// <summary>
        /// Initializes a new, empty Data Set Observation with no DataItem key or entries.
        /// </summary>
        public DataSetObservationInput() { }

        /// <summary>
        /// Initializes a new Data Set Observation for the specified DataItem with the given entries.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="entries">The key-value pairs that make up the Data Set.</param>
        public DataSetObservationInput(string dataItemKey, IEnumerable<IDataSetEntry> entries)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
        }

        /// <summary>
        /// Initializes a new Data Set Observation for the specified DataItem with the given entries and timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="entries">The key-value pairs that make up the Data Set.</param>
        /// <param name="timestamp">The observation timestamp as UnixTime in milliseconds.</param>
        public DataSetObservationInput(string dataItemKey, IEnumerable<IDataSetEntry> entries, long timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Initializes a new Data Set Observation for the specified DataItem with the given entries and timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="entries">The key-value pairs that make up the Data Set.</param>
        /// <param name="timestamp">The observation timestamp, converted to UnixTime in milliseconds.</param>
        public DataSetObservationInput(string dataItemKey, IEnumerable<IDataSetEntry> entries, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp.ToUnixTime();
        }

        /// <summary>
        /// Initializes a new Data Set Observation by copying the Device key, DataItem key, timestamp, and values from an existing Observation.
        /// </summary>
        /// <param name="observation">The source Observation to copy; a <c>null</c> argument leaves the new instance empty.</param>
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