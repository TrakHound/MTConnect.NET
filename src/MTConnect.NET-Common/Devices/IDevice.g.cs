// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Component composed of a piece of equipment that produces observation about itself.
    /// </summary>
    public partial interface IDevice
    {
        /// <summary>
        /// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
        /// </summary>
        string Hash { get; }
        
        /// <summary>
        /// MTConnect version of the Device Information Model used to configure the information to be published for a piece of equipment in an MTConnect Response Document.
        /// </summary>
        System.Version MTConnectVersion { get; }
        
        /// <summary>
        /// Logical or physical entity that provides a capability.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.IComponent> Components { get; }
        
        /// <summary>
        /// Functional part of a piece of equipment contained within a Component.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.IComposition> Compositions { get; }
        
    }
}