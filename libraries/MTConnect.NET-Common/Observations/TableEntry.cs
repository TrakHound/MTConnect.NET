// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of an Observation with a DataItemRepresentation of <see cref="Devices.DataItemRepresentation.TABLE">TABLE</see>.
    /// </summary>
    public class TableEntry : ITableEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// An optional human-readable description of the entry's <see cref="Key"/> (row).
        /// </summary>
        public string KeyDescription { get; set; }

        /// <summary>
        /// Boolean removal indicator of a key-value pair that MUST be true or false.
        /// </summary>
        public bool Removed { get; set; }

        /// <summary>
        /// Key-value pairs published as part of a Table Entry.
        /// </summary>
        public IEnumerable<ITableCell> Cells { get; set; }


        /// <summary>
        /// Initializes a new, empty Table entry.
        /// </summary>
        public TableEntry() { }

        /// <summary>
        /// Initializes a new Table entry with the given row key and its cells.
        /// </summary>
        /// <param name="key">The entry's row key.</param>
        /// <param name="cells">The cells (columns) that make up the row.</param>
        public TableEntry(string key, IEnumerable<ITableCell> cells)
        {
            Key = key;
            Cells = cells;
        }

        /// <summary>
        /// Initializes a new Table entry that marks a row key as added or removed.
        /// </summary>
        /// <param name="key">The entry's row key.</param>
        /// <param name="removed">Whether the row was removed from the table.</param>
        public TableEntry(string key, bool removed)
        {
            Key = key;
            Removed = removed;
        }
    }
}