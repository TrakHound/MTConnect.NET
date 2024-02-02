// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class DeviceRelationshipDescriptions
    {
        /// <summary>
        /// Reference to the uuid attribute of the Device element of the associated piece of equipment.
        /// </summary>
        public const string DeviceUuidRef = "Reference to the uuid attribute of the Device element of the associated piece of equipment.";
        
        /// <summary>
        /// URI identifying the agent that is publishing information for the associated piece of equipment.
        /// </summary>
        public const string Href = "URI identifying the agent that is publishing information for the associated piece of equipment.";
        
        /// <summary>
        /// Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.
        /// </summary>
        public const string Role = "Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.";
        
        /// <summary>
        /// `xlink:type`**MUST** have a fixed value of `locator` as defined in W3C XLink 1.1 https://www.w3.org/TR/xlink11/.
        /// </summary>
        public const string XLinkType = "`xlink:type`**MUST** have a fixed value of `locator` as defined in W3C XLink 1.1 https://www.w3.org/TR/xlink11/.";
    }
}