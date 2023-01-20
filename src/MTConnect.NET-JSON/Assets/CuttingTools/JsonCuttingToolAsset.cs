// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonCuttingToolAsset
    {
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }


        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonPropertyName("toolId")]
        public string ToolId { get; set; }

        [JsonPropertyName("manufacturers")]
        public string Manufacturers { get; set; }

        [JsonPropertyName("cuttingToolLifeCycle")]
        public JsonCuttingToolLifeCycle CuttingToolLifeCycle { get; set; }

        [JsonPropertyName("cuttingToolArchetypeReference")]
        public string CuttingToolArchetypeReference { get; set; }


        public JsonCuttingToolAsset() { }

        public JsonCuttingToolAsset(CuttingToolAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp.ToDateTime();
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;
                Description = asset.Description;

                SerialNumber = asset.SerialNumber;
                ToolId = asset.ToolId;
                Manufacturers = asset.Manufacturers;
                if (asset.CuttingToolLifeCycle != null) CuttingToolLifeCycle = new JsonCuttingToolLifeCycle(asset.CuttingToolLifeCycle);
                CuttingToolArchetypeReference = asset.CuttingToolArchetypeReference;
            }
        }


        public CuttingToolAsset ToCuttingToolAsset()
        {
            var asset = new CuttingToolAsset();

            asset.AssetId = AssetId;
            asset.Type = Type;
            asset.Timestamp = Timestamp.ToUnixTime();
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;
            asset.Description = Description;

            asset.SerialNumber = SerialNumber;
            asset.ToolId = ToolId;
            asset.Manufacturers = Manufacturers;
            if (CuttingToolLifeCycle != null) asset.CuttingToolLifeCycle = CuttingToolLifeCycle.ToCuttingToolLifeCycle();
            asset.CuttingToolArchetypeReference = CuttingToolArchetypeReference;
            return asset;
        }
    }
}
