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

        public string KeyDescription { get; set; }

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