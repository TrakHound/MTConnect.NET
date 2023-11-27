// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_E20AAF35_BE17_40e8_8701_D2D7676EDC69

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// ConfigurationRelationship that describes the association between two pieces of equipment that function independently but together perform a manufacturing operation.
    /// </summary>
    public class DeviceRelationship : ConfigurationRelationship, IDeviceRelationship
    {
        public new const string DescriptionText = "ConfigurationRelationship that describes the association between two pieces of equipment that function independently but together perform a manufacturing operation.";


        /// <summary>
        /// Reference to the uuid attribute of the Device element of the associated piece of equipment.
        /// </summary>
        public string DeviceUuidRef { get; set; }
        
        /// <summary>
        /// URI identifying the agent that is publishing information for the associated piece of equipment.
        /// </summary>
        public string Href { get; set; }
        
        /// <summary>
        /// Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.
        /// </summary>
        public MTConnect.Devices.Configurations.RoleType? Role { get; set; }
        
        /// <summary>
        /// `xlink:type`**MUST** have a fixed value of `locator` as defined in W3C XLink 1.1 https://www.w3.org/TR/xlink11/.
        /// </summary>
        public string XLinkType { get; set; }
    }
}