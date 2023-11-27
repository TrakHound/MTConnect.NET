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
        public const string EntryRemovedValue = "[!ENTRY_REMOVED!]";


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