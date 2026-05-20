// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json
{
    /// <summary>
    /// Shared JSON envelope for the common attributes of every asset in
    /// the cppagent-compatible shape. Subclassed by each typed asset
    /// surrogate (CuttingTool, File, RawMaterial, QIFDocumentWrapper),
    /// so the envelope members do not need to be redeclared per type.
    /// </summary>
    public class JsonAsset
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The asset type discriminator (the concrete element name).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The timestamp when the asset was reported.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The UUID of the device the asset is associated with.
        /// </summary>
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// Whether the asset has been marked as removed.
        /// </summary>
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        /// <summary>
        /// Free-form description of the asset.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}