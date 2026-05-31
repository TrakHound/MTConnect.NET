// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    /// <summary>
    /// Common JSON serialization shape for an MTConnect asset, providing the
    /// identifier, type, timestamp, association, removal, and description
    /// fields shared by every asset surrogate.
    /// </summary>
    public abstract class JsonAsset
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The asset type identifier.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The timestamp at which the asset was last reported.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The UUID of the device the asset is associated with.
        /// </summary>
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// Whether the asset has been removed from the agent.
        /// </summary>
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        /// <summary>
        /// The free-form description of the asset.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}