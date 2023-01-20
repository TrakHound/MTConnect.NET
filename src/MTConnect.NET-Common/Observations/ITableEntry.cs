// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of a Table observation.
    /// </summary>
    public interface ITableEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Boolean removal indicator of a key-value pair that MUST be true or false.
        /// </summary>
        bool Removed { get; }

        /// <summary>
        /// Key-value pairs published as part of a Table Entry.
        /// </summary>
        IEnumerable<ITableCell> Cells { get; }
    }
}
