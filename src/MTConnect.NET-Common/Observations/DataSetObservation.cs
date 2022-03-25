// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    /// <summary>
    /// 5 Data Set observation reports multiple values as a set of key-value pairs where each key MUST be unique.
    /// </summary>
    internal static class DataSetObservation
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
    }
}
