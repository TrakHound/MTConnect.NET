// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Devices.Json;
using System;
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
        public ulong InstanceId { get; set; }

        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        ////[JsonIgnore]
        //public JsonDescription Description { get; set; }


        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonPropertyName("toolId")]
        public string ToolId { get; set; }

        [JsonPropertyName("manufacturers")]
        public string Manufacturers { get; set; }

        [JsonPropertyName("CuttingToolLifeCycle")]
        public JsonCuttingToolLifeCycle CuttingToolLifeCycle { get; set; }

        [JsonPropertyName("CuttingToolArchetypeReference")]
        public JsonCuttingToolArchetypeReference CuttingToolArchetypeReference { get; set; }


        public JsonCuttingToolAsset() { }

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

                if (asset.Description != null) Description = asset.Description;
                //if (asset.Description != null) Description = new JsonDescription(asset.Description);

                SerialNumber = asset.SerialNumber;
                ToolId = asset.ToolId;
                if (!asset.Manufacturers.IsNullOrEmpty()) Manufacturers = string.Join(", ", asset.Manufacturers);
                if (asset.CuttingToolLifeCycle != null) CuttingToolLifeCycle = new JsonCuttingToolLifeCycle(asset.CuttingToolLifeCycle);
                if (asset.CuttingToolArchetypeReference != null) CuttingToolArchetypeReference = new JsonCuttingToolArchetypeReference(asset.CuttingToolArchetypeReference);
            }
        }


        public ICuttingToolAsset ToCuttingToolAsset()
        {
            var asset = new CuttingToolAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description;
            //if (Description != null) asset.Description = Description.ToDescription();

            asset.SerialNumber = SerialNumber;
            asset.ToolId = ToolId;
            if (!string.IsNullOrEmpty(Manufacturers)) asset.Manufacturers = Manufacturers.Split(',');
            if (CuttingToolLifeCycle != null) asset.CuttingToolLifeCycle = CuttingToolLifeCycle.ToCuttingToolLifeCycle();
            if (CuttingToolArchetypeReference != null) asset.CuttingToolArchetypeReference = CuttingToolArchetypeReference.ToCuttingToolArchetypeReference();
            return asset;
        }
    }
}