// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public enum DataItemRelationshipType
    {
        /// <summary>
        /// Reference to a DataItem that associates the values with an external entity.
        /// </summary>
        ATTACHMENT,
        
        /// <summary>
        /// Referenced DataItem provides the id of the effective Coordinate System.
        /// </summary>
        COORDINATE_SYSTEM,
        
        /// <summary>
        /// Referenced DataItem provides process limits.
        /// </summary>
        LIMIT,
        
        /// <summary>
        /// Referenced DataItem provides the observed values.
        /// </summary>
        OBSERVATION
    }
}