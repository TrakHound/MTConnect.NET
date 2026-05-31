// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    /// <summary>
    /// A Table represents two-dimensional sets of key-value pairs where the Entry represents rows containing sets of key-value pairs given by Cell elements.
    /// </summary>
    public static class TableObservation
    {
        /// <summary>
        /// The sentinel value stored for a Table row key to mark that the row was removed from the table.
        /// </summary>
        public const string EntryRemovedValue = "[!ENTRY_REMOVED!]";


        /// <summary>
        /// Extracts the Table entries (rows and their cells) from a flat collection of Observation values.
        /// </summary>
        /// <param name="values">The Observation values to decode.</param>
        /// <returns>The decoded Table entries.</returns>
        public static IEnumerable<ITableEntry> GetEntries(IEnumerable<ObservationValue> values)
        {
            var entries = new List<ITableEntry>();

            if (!values.IsNullOrEmpty())
            {
                var entryValues = values.Where(o => o.Key != null && o.Key.StartsWith(ValueKeys.TablePrefix));
                if (!entryValues.IsNullOrEmpty())
                {
                    var tempEntries = new Dictionary<string, Dictionary<string, string>>();
                    var emptyKeys = new List<string>();

                    foreach (var entryValue in entryValues)
                    {
                        var tableKey = ValueKeys.GetTableKey(entryValue.Key);
                        var tableCellKey = ValueKeys.GetTableCellKey(entryValue.Key);

                        if (tableKey != null && tableCellKey != null)
                        {
                            Dictionary<string, string> entry;
                            if (tempEntries.ContainsKey(tableKey)) entry = tempEntries[tableKey];
                            else
                            {
                                entry = new Dictionary<string, string>();
                                tempEntries[tableKey] = entry;
                            }

                            if (!entry.ContainsKey(tableCellKey))
                            {
                                entry[tableCellKey] = entryValue.Value;
                            }
                        }
                        else if (tableKey != null)
                        {
                            if (entryValue.Value == EntryRemovedValue && !emptyKeys.Contains(tableKey)) emptyKeys.Add(tableKey);
                        }
                    }

                    foreach (var tempEntry in tempEntries)
                    {
                        var tableKey = tempEntry.Key;
                        var removed = emptyKeys.Contains(tempEntry.Key);

                        if (removed)
                        {
                            entries.Add(new TableEntry(tableKey, true));
                        }
                        else
                        {
                            var cells = new List<ITableCell>();
                            foreach (var cell in tempEntry.Value)
                            {
                                cells.Add(new TableCell(cell.Key, cell.Value));
                            }

                            entries.Add(new TableEntry(tableKey, cells));
                        }
                    }
                }
            }

            return entries;
        }

        /// <summary>
        /// Encodes a set of Table entries into the flat Observation values that represent them.
        /// </summary>
        /// <param name="entries">The Table entries to encode.</param>
        /// <returns>The Observation values, with removed rows encoded using the removal sentinel.</returns>
        public static IEnumerable<ObservationValue> SetEntries(IEnumerable<ITableEntry> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var values = new List<ObservationValue>();

                foreach (var entry in entries)
                {
                    foreach (var cell in entry.Cells)
                    {
                        values.Add(new ObservationValue(ValueKeys.CreateTableValueKey(entry.Key, cell.Key), cell.Value));
                    }
                }

                return values;
            }

            return null;
        }
    }
}