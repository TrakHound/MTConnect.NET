// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Component composed of a piece of equipment that produces observation about itself.
    /// </summary>
    public partial interface IDevice
    {
        /// <summary>
        /// MTConnect version of the Device Information Model used to configure the information to be published for a piece of equipment in an MTConnect Response Document.
        /// </summary>
        System.Version MTConnectVersion { get; }
        
    }
}