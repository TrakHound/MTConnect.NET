// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Type.
    /// </summary>
    public interface IDataItemRelationship : IAbstractDataItemRelationship
    {
        /// <summary>
        /// Specifies how the DataItem is related.
        /// </summary>
        MTConnect.Devices.DataItemRelationshipType Type { get; }
    }
}