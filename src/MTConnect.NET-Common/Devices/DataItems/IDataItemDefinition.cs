// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// The Definition provides additional descriptive information for any DataItem representations.
    /// When the representation is either DATA_SET or TABLE, it gives the specific meaning of a key and MAY provide a Description, type, and units for semantic interpretation of data.
    /// </summary>
    public interface IDataItemDefinition
    {
        /// <summary>
        /// The Description of the Definition.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The EntryDefinitions aggregates EntryDefinition.
        /// </summary>
        IEnumerable<IEntryDefinition> EntryDefinitions { get; }

        /// <summary>
        /// The CellDefinitions aggregates CellDefinition.      
        /// </summary>
        IEnumerable<ICellDefinition> CellDefinitions { get; }
    }
}