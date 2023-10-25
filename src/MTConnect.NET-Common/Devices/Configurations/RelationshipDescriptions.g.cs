// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class RelationshipDescriptions
    {
        /// <summary>
        /// Defines whether the services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.
        /// </summary>
        public const string Criticality = "Defines whether the services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.";
        
        /// <summary>
        /// Unique identifier for this ConfigurationRelationship.
        /// </summary>
        public const string Id = "Unique identifier for this ConfigurationRelationship.";
        
        /// <summary>
        /// Name associated with this ConfigurationRelationship.
        /// </summary>
        public const string Name = "Name associated with this ConfigurationRelationship.";
        
        /// <summary>
        /// Defines the authority that this piece of equipment has relative to the associated piece of equipment.
        /// </summary>
        public const string Type = "Defines the authority that this piece of equipment has relative to the associated piece of equipment.";
    }
}