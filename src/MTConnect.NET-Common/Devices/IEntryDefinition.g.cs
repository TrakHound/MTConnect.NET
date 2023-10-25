// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
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
        MTConnect.Devices.ICellDefinition CellDefinition { get; }
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        MTConnect.Devices.IDescription Description { get; }
        
        /// <summary>
        /// Unique identification of the Entry in the Definition. The description applies to all Entry observation having this key.
        /// </summary>
        string Key { get; }
        
        /// <summary>
        /// Dataitem type that defines the meaning of the key.
        /// </summary>
        string KeyType { get; }
        
        /// <summary>
        /// Same as DataItem DataItem::subType. See DataItem.
        /// </summary>
        string SubType { get; }
        
        /// <summary>
        /// Same as DataItem DataItem::type. See DataItem Types.
        /// </summary>
        string Type { get; }
        
        /// <summary>
        /// Same as DataItem DataItem::units. See Value Properties of DataItem.
        /// </summary>
        string Units { get; }
    }
}