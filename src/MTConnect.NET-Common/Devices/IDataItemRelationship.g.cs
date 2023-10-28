// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// AbstractDataItemRelationship that provides a semantic reference to another DataItem described by the type property.
    /// </summary>
    public interface IDataItemRelationship : IAbstractDataItemRelationship
    {
        /// <summary>
        /// Specifies how the DataItem is related.
        /// </summary>
        MTConnect.Devices.DataItemRelationshipType Type { get; }
    }
}