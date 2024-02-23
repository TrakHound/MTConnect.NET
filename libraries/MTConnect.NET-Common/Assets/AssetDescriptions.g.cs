// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets
{
    public static class AssetDescriptions
    {
        /// <summary>
        /// Unique identifier for an Asset.
        /// </summary>
        public const string AssetId = "Unique identifier for an Asset.";
        
        /// <summary>
        /// Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.
        /// </summary>
        public const string Configuration = "Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.";
        
        /// <summary>
        /// Description can contain any descriptive content about the Asset.
        /// </summary>
        public const string Description = "Description can contain any descriptive content about the Asset.";
        
        /// <summary>
        /// Associated piece of equipment's UUID that supplied the Asset's data.uuid defined in Device Information Model.
        /// </summary>
        public const string DeviceUuid = "Associated piece of equipment's UUID that supplied the Asset's data.uuid defined in Device Information Model.";
        
        /// <summary>
        /// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
        /// </summary>
        public const string Hash = "Condensed message digest from a secure one-way hash function. FIPS PUB 180-4";
        
        /// <summary>
        /// Indicator that the Asset has been removed from the piece of equipment.
        /// </summary>
        public const string Removed = "Indicator that the Asset has been removed from the piece of equipment.";
        
        /// <summary>
        /// Time the Asset data was last modified.
        /// </summary>
        public const string Timestamp = "Time the Asset data was last modified.";
    }
}