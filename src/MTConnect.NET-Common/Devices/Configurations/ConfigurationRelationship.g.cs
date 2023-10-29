// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_28132294_DF39_4e8e_8AE5_B79565F991A2

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Association between two pieces of equipment that function independently but together perform a manufacturing operation.
    /// </summary>
    public abstract class ConfigurationRelationship : IConfigurationRelationship
    {
        public const string DescriptionText = "Association between two pieces of equipment that function independently but together perform a manufacturing operation.";


        /// <summary>
        /// Defines whether the services or functions provided by the associated piece of equipment is required for the operation of this piece of equipment.
        /// </summary>
        public MTConnect.Devices.Configurations.CriticalityType? Criticality { get; set; }
        
        /// <summary>
        /// Unique identifier for this ConfigurationRelationship.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Name associated with this ConfigurationRelationship.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Defines the authority that this piece of equipment has relative to the associated piece of equipment.
        /// </summary>
        public MTConnect.Devices.Configurations.RelationshipType Type { get; set; }
    }
}