// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// The Definition provides additional descriptive information for any DataItem representations.
    /// When the representation is either DATA_SET or TABLE, it gives the specific meaning of a key and MAY provide a Description, type, and units for semantic interpretation of data.
    /// </summary>
    public class DataItemDefinition : IDataItemDefinition
    {
        /// <summary>
        /// The Description of the Definition.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The EntryDefinitions aggregates EntryDefinition.
        /// </summary>
        public IEnumerable<IEntryDefinition> EntryDefinitions { get; set; }

        /// <summary>
        /// The CellDefinitions aggregates CellDefinition.      
        /// </summary>
        public IEnumerable<ICellDefinition> CellDefinitions { get; set; }
    }
}
