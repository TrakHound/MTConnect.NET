// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment
    /// where the reported value(s) are represented as rows containing sets of key-value pairs given by Cell elements.
    /// </summary>
    public class TableObservation : Observation
    {
        /// <summary>
        /// Key-value pairs published as part of a Data Set observation
        /// </summary>
        public IEnumerable<TableEntry> Entries
        {
            get
            {
                var entries = new List<TableEntry>();

                if (!Values.IsNullOrEmpty())
                {
                    var entryValues = Values.Where(o => o.ValueType != null && o.ValueType.StartsWith(ValueTypes.TablePrefix));
                    if (!entryValues.IsNullOrEmpty())
                    {
                        var keys = entryValues.Select(o => ValueTypes.GetTableKey(o.ValueType)).Distinct();
                        if (!keys.IsNullOrEmpty())
                        {
                            foreach (var key in keys)
                            {
                                var keyValues = entryValues.Where(o => ValueTypes.GetTableKey(o.ValueType) == key);
                                if (!keyValues.IsNullOrEmpty())
                                {
                                    var cells = new List<TableCell>();
                                    foreach (var keyValue in keyValues)
                                    {
                                        cells.Add(new TableCell(ValueTypes.GetTableValue(keyValue.ValueType, key), keyValue.Value));
                                    }

                                    entries.Add(new TableEntry(key, cells));
                                }
                            }
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
                        if (!entry.Cells.IsNullOrEmpty())
                        {
                            foreach (var cell in entry.Cells)
                            {
                                AddValue(new ObservationValue(ValueTypes.CreateTableValueType(entry.Key, cell.Key), cell.Value));
                            }
                        }
                    }
                }
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
