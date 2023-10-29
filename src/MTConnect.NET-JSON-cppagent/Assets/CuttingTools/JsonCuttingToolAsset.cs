// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Devices.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingToolAsset
    {
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("instanceId")]
        public long InstanceId { get; set; }

        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        [JsonPropertyName("description")]
        public JsonDescription Description { get; set; }


        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonPropertyName("toolId")]
        public string ToolId { get; set; }

        [JsonPropertyName("manufacturers")]
        public IEnumerable<string> Manufacturers { get; set; }

        [JsonPropertyName("cuttingToolLifeCycle")]
        public JsonCuttingToolLifeCycle CuttingToolLifeCycle { get; set; }

        [JsonPropertyName("cuttingToolArchetypeReference")]
        public JsonCuttingToolArchetypeReference CuttingToolArchetypeReference { get; set; }


        public JsonCuttingToolAsset() { }

        public JsonCuttingToolAsset(CuttingTool asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp;
                InstanceId = asset.InstanceId;
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;

                if (asset.Description != null) Description = new JsonDescription(asset.Description);

                SerialNumber = asset.SerialNumber;
                ToolId = asset.ToolId;
                Manufacturers = asset.Manufacturers;
                if (asset.CuttingToolLifeCycle != null) CuttingToolLifeCycle = new JsonCuttingToolLifeCycle(asset.CuttingToolLifeCycle);
                if (asset.CuttingToolArchetypeReference != null) CuttingToolArchetypeReference = new JsonCuttingToolArchetypeReference(asset.CuttingToolArchetypeReference);
            }
        }


        public CuttingTool ToCuttingToolAsset()
        {
            var asset = new CuttingTool();

            asset.AssetId = AssetId;
            asset.Type = Type;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description.ToDescription();

            asset.SerialNumber = SerialNumber;
            asset.ToolId = ToolId;
            asset.Manufacturers = Manufacturers;
            if (CuttingToolLifeCycle != null) asset.CuttingToolLifeCycle = CuttingToolLifeCycle.ToCuttingToolLifeCycle();
            if (CuttingToolArchetypeReference != null) asset.CuttingToolArchetypeReference = CuttingToolArchetypeReference.ToCuttingToolArchetypeReference();
            return asset;
        }
    }
}