// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    /// <summary>
    /// A Data Set observation reports multiple values as a set of key-value pairs where each key MUST be unique.
    /// </summary>
    public static class DataSetObservation
    {
        /// <summary>
        /// The sentinel value stored for a Data Set key to mark that the key was removed from the set.
        /// </summary>
        public const string EntryRemovedValue = "[!ENTRY_REMOVED!]";


        /// <summary>
        /// Extracts the Data Set entries from a flat collection of Observation values.
        /// </summary>
        /// <param name="values">The Observation values to decode.</param>
        /// <returns>The decoded entries ordered by key, with removed keys flagged.</returns>
        public static IEnumerable<IDataSetEntry> GetEntries(IEnumerable<ObservationValue> values)
        {
            var entries = new List<IDataSetEntry>();

            if (!values.IsNullOrEmpty())
            {
                var entryValues = values.Where(o => o.Key != null && o.Key.StartsWith(ValueKeys.DataSetPrefix));
                if (!entryValues.IsNullOrEmpty())
                {
                    var oValues = entryValues.OrderBy(o => ValueKeys.GetDataSetKey(o.Key));
                    foreach (var value in oValues)
                    {
                        var key = ValueKeys.GetDataSetKey(value.Key);
                        bool removed = value.Value == EntryRemovedValue;

                        if (!removed) entries.Add(new DataSetEntry(key, value.Value));
                        else entries.Add(new DataSetEntry(key, removed));
                    }
                }
            }

            return entries;
        }

        /// <summary>
        /// Encodes a set of Data Set entries into the flat Observation values that represent them.
        /// </summary>
        /// <param name="entries">The Data Set entries to encode.</param>
        /// <returns>The Observation values, with removed entries encoded using the removal sentinel.</returns>
        public static IEnumerable<ObservationValue> SetEntries(IEnumerable<IDataSetEntry> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var values = new List<ObservationValue>();

                foreach (var entry in entries)
                {
                    values.Add(new ObservationValue(ValueKeys.CreateDataSetValueKey(entry.Key), entry.Value));
                }

                return values;
            }

            return null;
        }
    }
}