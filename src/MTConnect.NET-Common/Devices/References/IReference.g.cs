// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.References
{
    /// <summary>
    /// Pointer to information that is associated with another entity defined elsewhere in the MTConnectDevices entity for a piece of equipment.
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// Pointer to the id of a DataItem that contains the information to be associated with this entity.
        /// </summary>
        string DataItemId { get; }
        
        /// <summary>
        /// Pointer to the id of an entity that contains the information to be associated with this entity.
        /// </summary>
        string IdRef { get; }
        
        /// <summary>
        /// name of an element or a piece of equipment.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Pointer to the id of a DataItem that contains the information to be associated with this entity.
        /// </summary>
        string RefDataItemId { get; }
    }
}