// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Association between a DataItem and another entity.
    /// </summary>
    public partial interface IAbstractDataItemRelationship
    {
        /// <summary>
        /// Reference to the related entity's `id`.
        /// </summary>
        string IdRef { get; }
        
        /// <summary>
        /// Descriptive name associated with this AbstractDataItemRelationship.
        /// </summary>
        string Name { get; }
    }
}