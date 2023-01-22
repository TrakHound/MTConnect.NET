// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Relationships
{
    /// <summary>
    /// Relationship that describes the association between a Component and an Asset.
    /// </summary>
    public class AssetRelationship : Relationship, IAssetRelationship
    {
        /// <summary>
        /// UUID of the related Asset
        /// </summary>
        public string AssetIdRef { get; set; }

        /// <summary>
        /// Type of Asset being referenced
        /// </summary>
        public string AssetType { get; set; }

        /// <summary>
        /// URI reference to the associated Asset 
        /// </summary>
        public string Href { get; set; }
    }
}