// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Defines the meaning of Entry and Cell elements associated with the DataItem when the representation is either `DATA` or `TABLE`.
    /// </summary>
    public interface IDataItemDefinition
    {
        /// <summary>
        /// Semantic definition of a Cell.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.ICellDefinition> CellDefinitions { get; }
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        MTConnect.Devices.IDescription Description { get; }
        
        /// <summary>
        /// Semantic definition of an Entry.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.IEntryDefinition> EntryDefinitions { get; }
    }
}