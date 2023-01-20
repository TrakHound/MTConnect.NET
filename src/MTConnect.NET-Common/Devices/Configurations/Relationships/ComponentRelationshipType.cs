// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Relationships
{
    /// <summary>
    /// ComponentRelationship describes the association between two components within a piece of equipment that function independently but together perform a capability or service within a piece of equipment.
    /// </summary>
    public enum ComponentRelationshipType
    {
        /// <summary>
        /// This piece of equipment functions as a parent in the relationship with the associated piece of equipment.
        /// </summary>
        PARENT,

        /// <summary>
        /// This piece of equipment functions as a child in the relationship with the associated piece of equipment.
        /// </summary>
        CHILD,

        /// <summary>
        /// This piece of equipment functions as a peer which provides equal functionality and capabilities in the relationship with the associated piece of equipment.
        /// </summary>
        PEER
    }
}
