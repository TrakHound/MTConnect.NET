// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of an Observation with a DataItemRepresentation of <see cref="Devices.DataItemRepresentation.TABLE">TABLE</see>.
    /// </summary>
    public interface ITableEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        string Key { get; }

        string KeyDescription { get; }

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