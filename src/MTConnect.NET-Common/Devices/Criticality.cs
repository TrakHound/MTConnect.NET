// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
