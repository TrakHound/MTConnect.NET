// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of a Table observation.
    /// </summary>
    public class TableEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Key-value pairs published as part of a Table Entry.
        /// </summary>
        public IEnumerable<TableCell> Cells { get; set; }


        public TableEntry() { }

        public TableEntry(string key, IEnumerable<TableCell> cells)
        {
            Key = key;
            Cells = cells;
        }
    }
}
