// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// ConfigurationRelationship that describes the association between two pieces of equipment that function independently but together perform a manufacturing operation.
    /// </summary>
    public interface IDeviceRelationship : IConfigurationRelationship
    {
        /// <summary>
        /// Reference to the uuid attribute of the Device element of the associated piece of equipment.
        /// </summary>
        string DeviceUuidRef { get; }
        
        /// <summary>
        /// URI identifying the agent that is publishing information for the associated piece of equipment.
        /// </summary>
        string Href { get; }
        
        /// <summary>
        /// Defines the services or capabilities that the referenced piece of equipment provides relative to this piece of equipment.
        /// </summary>
        MTConnect.Devices.Configurations.RoleType? Role { get; }
        
        /// <summary>
        /// `xlink:type`**MUST** have a fixed value of `locator` as defined in W3C XLink 1.1 https://www.w3.org/TR/xlink11/.
        /// </summary>
        string XLinkType { get; }
    }
}