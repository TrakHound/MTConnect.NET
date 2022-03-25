// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// Relationship is an XML element that describes the association between two pieces of equipment that function independently but together perform a manufacturing operation. 
    /// Relationship may also be used to define the association between two components within a piece of equipment.
    /// </summary>
    public interface IRelationship
    {
        /// <summary>
        /// The unique identifier for this Relationship.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// A descriptive name associated with this Relationship.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A reference to the related DataItem id.
        /// </summary>
        Criticality Criticality { get; }

        /// <summary>
        /// A reference to the associated component element.
        /// </summary>
        string IdRef { get; }
    }
}
