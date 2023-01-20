// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Specifies how the DataItem is related.
    /// </summary>
    public enum DataItemRelationshipType
    {
        /// <summary>
        /// A reference to a DataItem that associates the values with an external entity.
        /// </summary>
        ATTACHMENT,

        /// <summary>
        /// The referenced DataItem provides the id of the effective Coordinate System.
        /// </summary>
        COORDINATE_SYSTEM,

        /// <summary>
        /// The referenced DataItem provides process limits.
        /// </summary>
        LIMIT,

        /// <summary>
        /// The referenced DataItem provides the observed values.
        /// </summary>
        OBSERVATION
    }
}
