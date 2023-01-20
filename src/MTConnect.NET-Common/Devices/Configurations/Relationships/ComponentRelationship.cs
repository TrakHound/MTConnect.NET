// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Relationships
{
    /// <summary>
    /// ComponentRelationship describes the association between two components within a piece of equipment that function independently but together perform a capability or service within a piece of equipment.
    /// </summary>
    public class ComponentRelationship : Relationship, IComponentRelationship
    {
        /// <summary>
        /// Defines the authority that this component element has relative to the associated component element.
        /// </summary>
        public ComponentRelationshipType Type { get; set; }
    }
}
