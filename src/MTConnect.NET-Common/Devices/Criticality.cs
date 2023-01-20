// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Defines whether the services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.
    /// </summary>
    public enum Criticality
    {
        /// <summary>
        /// Not Specified
        /// </summary>
        NOT_SPECIFIED,

        /// <summary>
        /// The services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.
        /// </summary>
        CRITICAL,

        /// <summary>
        /// The services or functions provided by the associated piece of equipment is not required for the operation of this piece of equipment.
        /// </summary>
        NONCRITICAL
    }
}