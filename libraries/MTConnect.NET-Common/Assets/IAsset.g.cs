// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets
{
    /// <summary>
    /// Abstract Asset.
    /// </summary>
    public partial interface IAsset
    {
        /// <summary>
        /// Unique identifier for an Asset.
        /// </summary>
        string AssetId { get; }
        
        /// <summary>
        /// Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.
        /// </summary>
        MTConnect.Devices.Configurations.IConfiguration Configuration { get; }
        
        /// <summary>
        /// Description can contain any descriptive content about the Asset.
        /// </summary>
        MTConnect.Devices.IDescription Description { get; }
        
        /// <summary>
        /// Associated piece of equipment's UUID that supplied the Asset's data.It references to the uuid property of the Device defined in Device Information Model.
        /// </summary>
        string DeviceUuid { get; }
        
        /// <summary>
        /// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
        /// </summary>
        string Hash { get; }
        
        /// <summary>
        /// Indicator that the Asset has been removed from the piece of equipment.
        /// </summary>
        bool Removed { get; }
        
        /// <summary>
        /// Time the Asset data was last modified.
        /// </summary>
        System.DateTime Timestamp { get; }
    }
}