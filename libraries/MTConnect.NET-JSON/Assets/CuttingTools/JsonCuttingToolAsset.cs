// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect <c>CuttingTool</c> asset.
    /// Mirrors the on-the-wire shape so the JSON serializer can read and write
    /// it, then converts to and from the strongly-typed
    /// <see cref="CuttingToolAsset"/> model.
    /// </summary>
    public class JsonCuttingToolAsset
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The asset type identifier, <c>CuttingTool</c>.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The timestamp at which the asset was last reported.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The instance identifier of the agent that produced this asset.
        /// </summary>
        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

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


        /// <summary>
        /// The serial number that uniquely identifies the cutting tool.
        /// </summary>
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The identifier of the tool's definition.
        /// </summary>
        [JsonPropertyName("toolId")]
        public string ToolId { get; set; }

        /// <summary>
        /// The manufacturers of the cutting tool.
        /// </summary>
        [JsonPropertyName("manufacturers")]
        public IEnumerable<string> Manufacturers { get; set; }

        /// <summary>
        /// The life cycle state of the cutting tool, including its measurements
        /// and remaining life.
        /// </summary>
        [JsonPropertyName("cuttingToolLifeCycle")]
        public JsonCuttingToolLifeCycle CuttingToolLifeCycle { get; set; }

        /// <summary>
        /// The reference to the cutting tool archetype this asset is an
        /// instance of.
        /// </summary>
        [JsonPropertyName("cuttingToolArchetypeReference")]
        public JsonCuttingToolArchetypeReference CuttingToolArchetypeReference { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization, defaulting
        /// <see cref="Type"/> to the CuttingTool type identifier.
        /// </summary>
        public JsonCuttingToolAsset()
        {
            Type = CuttingToolAsset.TypeId;
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICuttingToolAsset"/>, converting the life cycle and
        /// archetype reference.
        /// </summary>
        public JsonCuttingToolAsset(ICuttingToolAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp;
                InstanceId = asset.InstanceId;
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;

                Description = asset.Description; // v2.5
                //if (asset.Description != null) Description = new JsonDescription(asset.Description);

                SerialNumber = asset.SerialNumber;
                ToolId = asset.ToolId;
                Manufacturers = asset.Manufacturers;
                if (asset.CuttingToolLifeCycle != null) CuttingToolLifeCycle = new JsonCuttingToolLifeCycle(asset.CuttingToolLifeCycle);
                if (asset.CuttingToolArchetypeReference != null) CuttingToolArchetypeReference = new JsonCuttingToolArchetypeReference(asset.CuttingToolArchetypeReference);
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ICuttingToolAsset"/>, converting the life cycle and
        /// archetype reference.
        /// </summary>
        public ICuttingToolAsset ToCuttingToolAsset()
        {
            var asset = new CuttingToolAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description; // v2.5
            //if (Description != null) asset.Description = Description.ToDescription();

            asset.SerialNumber = SerialNumber;
            asset.ToolId = ToolId;
            asset.Manufacturers = Manufacturers;
            if (CuttingToolLifeCycle != null) asset.CuttingToolLifeCycle = CuttingToolLifeCycle.ToCuttingToolLifeCycle();
            if (CuttingToolArchetypeReference != null) asset.CuttingToolArchetypeReference = CuttingToolArchetypeReference.ToCuttingToolArchetypeReference();
            return asset;
        }
    }
}