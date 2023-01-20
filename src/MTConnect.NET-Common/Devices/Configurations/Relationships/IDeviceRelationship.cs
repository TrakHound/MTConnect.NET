// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Relationships
{
    /// <summary>
    /// DeviceRelationship describes the association between two pieces of equipment that function independently but together perform a manufacturing operation.
    /// </summary>
    public interface IDeviceRelationship : IRelationship
    {
        /// <summary>
        /// Defines the authority that this piece of equipment has relative to the associated piece of equipment.
        /// </summary>
        DeviceRelationshipType Type { get; }

        /// <summary>
        /// A reference to the associated piece of equipment.
        /// </summary>
        string DeviceUuidRef { get; }

        /// <summary>
        /// Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.    
        /// </summary>
        Role Role { get; }

        /// <summary>
        /// A URI identifying the Agent that is publishing information for the associated piece of equipment. href MUST also include the UUID for that specific piece of equipment.    
        /// </summary>
        string Href { get; }

        /// <summary>
        /// The XLink type attribute MUST have a fixed value of locator as defined in W3C XLink 1.1 https://www.w3.org/TR/xlink11/ section 5.4 Locator Attribute(href)
        /// </summary>
        string XLinkType { get; }
    }
}
