// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Relationships
{
    /// <summary>
    /// DeviceRelationship describes the association between two pieces of equipment that function independently but together perform a manufacturing operation.
    /// </summary>
    public class DeviceRelationship : Relationship, IDeviceRelationship
    {
        /// <summary>
        /// Defines the authority that this piece of equipment has relative to the associated piece of equipment.
        /// </summary>
        public DeviceRelationshipType Type { get; set; }

        /// <summary>
        /// A reference to the associated piece of equipment.
        /// </summary>
        public string DeviceUuidRef { get; set; }

        /// <summary>
        /// Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.    
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// A URI identifying the Agent that is publishing information for the associated piece of equipment. href MUST also include the UUID for that specific piece of equipment.    
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// The XLink type attribute MUST have a fixed value of locator as defined in W3C XLink 1.1 https://www.w3.org/TR/xlink11/ section 5.4 Locator Attribute(href)
        /// </summary>
        public string XLinkType { get; set; }
    }
}