// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of a Table observation.
    /// </summary>
    public class TableEntry : ITableEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Boolean removal indicator of a key-value pair that MUST be true or false.
        /// </summary>
        public bool Removed { get; set; }

        /// <summary>
        /// Key-value pairs published as part of a Table Entry.
        /// </summary>
        public IEnumerable<ITableCell> Cells { get; set; }


        public TableEntry() { }

        public TableEntry(string key, IEnumerable<ITableCell> cells)
        {
            Key = key;
            Cells = cells;
        }

        public TableEntry(string key, bool removed)
        {
            Key = key;
            Removed = removed;
        }
    }
}