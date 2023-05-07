// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets
{
    /// <summary>
    /// An Asset XML element is a container type XML element used to organize
    /// information describing an entity that is not a piece of equipment.
    /// </summary>
    public interface IAsset : IMTConnectEntity
    {
        /// <summary>
        /// The unique identifier for the MTConnect Asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        string AssetId { get; set; }

        /// <summary>
        /// The type for the MTConnect Asset
        /// </summary>
        [JsonPropertyName("type")]
        string Type { get; set; }

        /// <summary>
        /// The time this MTConnect Asset was last modified.
        /// </summary>
        [JsonPropertyName("timestamp")]
        long Timestamp { get; set; }

        /// <summary>
        /// The Agent InstanceId of the Agent that produced this Asset
        /// </summary>
        [JsonPropertyName("instanceId")]
        long InstanceId { get; set; }

        /// <summary>
        /// The piece of equipments UUID that supplied this data.
        /// </summary>
        [JsonPropertyName("deviceUuid")]
        string DeviceUuid { get; set; }

        /// <summary>
        /// This is an optional attribute that is an indicator that the MTConnect
        /// Asset has been removed from the piece of equipment.
        /// </summary>
        [JsonPropertyName("removed")]
        bool Removed { get; set; }

        /// <summary>
        /// An optional element that can contain any descriptive content.
        /// </summary>
        [JsonPropertyName("description")]
        string Description { get; set; }


        IAsset Process(Version mtconnectVersion);

        AssetValidationResult IsValid(Version mtconnectVersion);
    }
}