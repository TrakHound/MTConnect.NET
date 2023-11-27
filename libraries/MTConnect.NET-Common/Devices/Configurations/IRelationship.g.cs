// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Association between two pieces of equipment that function independently but together perform a manufacturing operation.
    /// </summary>
    public interface IRelationship
    {
        /// <summary>
        /// Defines whether the services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.
        /// </summary>
        MTConnect.Devices.Configurations.CriticalityType Criticality { get; }
        
        /// <summary>
        /// Unique identifier for this ConfigurationRelationship.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Name associated with this ConfigurationRelationship.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Defines the authority that this piece of equipment has relative to the associated piece of equipment.
        /// </summary>
        MTConnect.Devices.Configurations.RelationshipType Type { get; }
    }
}