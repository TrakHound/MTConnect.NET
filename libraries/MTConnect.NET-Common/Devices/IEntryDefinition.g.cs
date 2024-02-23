// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Semantic definition of an Entry.
    /// </summary>
    public interface IEntryDefinition
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
        /// Unique identification of the Entry in the Definition. key.
        /// </summary>
        string Key { get; }
        
        /// <summary>
        /// Key.
        /// </summary>
        string KeyType { get; }
        
        /// <summary>
        /// SubType. See DataItem.
        /// </summary>
        string SubType { get; }
        
        /// <summary>
        /// Type. See DataItem Types.
        /// </summary>
        string Type { get; }
        
        /// <summary>
        /// Units. See Value Properties of DataItem.
        /// </summary>
        string Units { get; }
    }
}