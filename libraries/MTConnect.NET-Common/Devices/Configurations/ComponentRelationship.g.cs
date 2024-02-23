// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = EAID_2D0A9D8F_2538_4f46_8B83_6B1988818511

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// ConfigurationRelationship that describes the association between two components within a piece of equipment that function independently but together perform a capability or service within a piece of equipment.
    /// </summary>
    public class ComponentRelationship : ConfigurationRelationship, IComponentRelationship
    {
        public new const string DescriptionText = "ConfigurationRelationship that describes the association between two components within a piece of equipment that function independently but together perform a capability or service within a piece of equipment.";


        /// <summary>
        /// Reference to the associated Component.
        /// </summary>
        public string IdRef { get; set; }
    }
}