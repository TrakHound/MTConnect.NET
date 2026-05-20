// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Input
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment
    /// where the reported value(s) are represented as rows containing sets of key-value pairs given by Cell elements.
    /// </summary>
    public class TableObservationInput : ObservationInput
    {
        /// <summary>
        /// The rows published as part of a Table observation, each containing a set of Cell key-value pairs.
        /// </summary>
        public IEnumerable<ITableEntry> Entries
        {
            get => TableObservation.GetEntries(Values);
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var entry in value)
                    {
                        if (entry.Removed)
                        {
                            AddValue(new ObservationValue(ValueKeys.CreateTableValueKey(entry.Key), TableObservation.EntryRemovedValue));
                        }
                        else
                        {
                            if (!entry.Cells.IsNullOrEmpty())
                            {
                                foreach (var cell in entry.Cells)
                                {
                                    AddValue(new ObservationValue(ValueKeys.CreateTableValueKey(entry.Key, cell.Key), cell.Value));
                                }
                            }
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
        /// Initializes a new, empty Table Observation with no DataItem key or rows.
        /// </summary>
        public TableObservationInput() { }

        /// <summary>
        /// Initializes a new Table Observation for the specified DataItem with the given rows.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="entries">The rows that make up the Table.</param>
        public TableObservationInput(string dataItemKey, IEnumerable<ITableEntry> entries)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
        }

        /// <summary>
        /// Initializes a new Table Observation for the specified DataItem with the given rows and timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="entries">The rows that make up the Table.</param>
        /// <param name="timestamp">The observation timestamp as UnixTime in milliseconds.</param>
        public TableObservationInput(string dataItemKey, IEnumerable<ITableEntry> entries, long timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Initializes a new Table Observation for the specified DataItem with the given rows and timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="entries">The rows that make up the Table.</param>
        /// <param name="timestamp">The observation timestamp, converted to UnixTime in milliseconds.</param>
        public TableObservationInput(string dataItemKey, IEnumerable<ITableEntry> entries, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Entries = entries;
            Timestamp = timestamp.ToUnixTime();
        }

        /// <summary>
        /// Initializes a new Table Observation by copying the Device key, DataItem key, timestamp, and values from an existing Observation.
        /// </summary>
        /// <param name="observation">The source Observation to copy; a <c>null</c> argument leaves the new instance empty.</param>
        public TableObservationInput(IObservationInput observation)
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