// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Relationships
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
        /// Defines whether the services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.
        /// </summary>
        Criticality Criticality { get; }

        /// <summary>
        /// A reference to the associated component element.
        /// </summary>
        string IdRef { get; }

		/// <summary>
		/// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
		/// </summary>
		string Hash { get; }
	}
}