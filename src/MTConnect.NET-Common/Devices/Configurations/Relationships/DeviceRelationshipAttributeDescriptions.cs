// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Relationships
{
    public static class DeviceRelationshipAttributeDescriptions
    {
        /// <summary>
        /// Defines the authority that this piece of equipment has relative to the associated piece of equipment.
        /// </summary>
        public const string Type = "Defines the authority that this piece of equipment has relative to the associated piece of equipment.";

        /// <summary>
        /// A reference to the associated piece of equipment.
        /// </summary>
        public const string DeviceUuidRef = "A reference to the associated piece of equipment.";

        /// <summary>
        /// Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.    
        /// </summary>
        public const string Role = "Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.";

        /// <summary>
        /// A URI identifying the Agent that is publishing information for the associated piece of equipment. href MUST also include the UUID for that specific piece of equipment.    
        /// </summary>
        public const string Href = "A URI identifying the Agent that is publishing information for the associated piece of equipment. href MUST also include the UUID for that specific piece of equipment.";

        /// <summary>
        /// The XLink type attribute MUST have a fixed value of locator as defined in W3C XLink 1.1 https://www.w3.org/TR/xlink11/ section 5.4 Locator Attribute(href).
        /// </summary>
        public const string XLinkType = "The XLink type attribute MUST have a fixed value of locator as defined in W3C XLink 1.1 https://www.w3.org/TR/xlink11/ section 5.4 Locator Attribute(href).";
    }
}
