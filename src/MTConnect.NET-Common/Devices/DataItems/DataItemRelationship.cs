// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Relationships;

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// A Relationship providing a semantic reference to another DataItem described by the type property.
    /// </summary>
    public class DataItemRelationship : Relationship, IDataItemRelationship
    {
        /// <summary>
        /// Specifies how the DataItem is related.
        /// </summary>
        public DataItemRelationshipType Type { get; set; }
    }
}
