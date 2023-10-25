// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_C7D39183_23CB_416b_A62D_F60815E08B1A

namespace MTConnect.Assets
{
    /// <summary>
    /// Abstract Asset.
    /// </summary>
    public partial class Asset : IAsset
    {
        public const string DescriptionText = "Abstract Asset.";


        /// <summary>
        /// Unique identifier for an Asset.
        /// </summary>
        public string AssetId { get; set; }
        
        /// <summary>
        /// Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.
        /// </summary>
        public MTConnect.Devices.Configurations.IConfiguration Configuration { get; set; }
        
        /// <summary>
        /// Description can contain any descriptive content about the Asset.
        /// </summary>
        public MTConnect.Devices.IDescription Description { get; set; }
        
        /// <summary>
        /// Associated piece of equipment's UUID that supplied the Asset's data.It references to the uuid property of the Device defined in Device Information Model.
        /// </summary>
        public string DeviceUuid { get; set; }
        
        /// <summary>
        /// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
        /// </summary>
        public string Hash { get; set; }
        
        /// <summary>
        /// Indicator that the Asset has been removed from the piece of equipment.
        /// </summary>
        public bool Removed { get; set; }
        
        /// <summary>
        /// Time the Asset data was last modified.
        /// </summary>
        public System.DateTime Timestamp { get; set; }
    }
}