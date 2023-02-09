// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.RawMaterials
{
    public class JsonRawMaterialAsset
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


        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("containerType")]
        public string ContainerType { get; set; }

        [JsonPropertyName("processKind")]
        public string ProcessKind { get; set; }

        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonPropertyName("form")]
        public string Form { get; set; }

        [JsonPropertyName("hasMaterial")]
        public bool HasMaterial { get; set; }

        [JsonPropertyName("manufacturingDate")]
        public DateTime ManufacturingDate { get; set; }

        [JsonPropertyName("firstUseDate")]
        public DateTime FirstUseDate { get; set; }

        [JsonPropertyName("lastUseDate")]
        public DateTime LastUseDate { get; set; }

        [JsonPropertyName("initialVolume")]
        public double InitialVolume { get; set; }

        [JsonPropertyName("initialDimension")]
        public string InitialDimension { get; set; }

        [JsonPropertyName("initialQuantity")]
        public int InitialQuantity { get; set; }

        [JsonPropertyName("currentVolume")]
        public double CurrentVolume { get; set; }

        [JsonPropertyName("currentDimension")]
        public string CurrentDimension { get; set; }

        [JsonPropertyName("currentQuantity")]
        public int CurrentQuantity { get; set; }

        [JsonPropertyName("material")]
        public JsonMaterial Material { get; set; }


        public JsonRawMaterialAsset() { }

        public JsonRawMaterialAsset(RawMaterialAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp.ToDateTime();
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;
                Description = asset.Description;

                Name = asset.Name;
                ContainerType = asset.ContainerType;
                ProcessKind = asset.ProcessKind;
                SerialNumber = asset.SerialNumber;
                Form = asset.Form;
                HasMaterial = asset.HasMaterial;
                ManufacturingDate = asset.ManufacturingDate;
                FirstUseDate = asset.FirstUseDate;
                LastUseDate = asset.LastUseDate;
                InitialVolume = asset.InitialVolume;
                InitialDimension = asset.InitialDimension;
                InitialQuantity = asset.InitialQuantity;
                CurrentVolume = asset.CurrentVolume;
                CurrentDimension = asset.CurrentDimension;
                CurrentQuantity = asset.CurrentQuantity;

                if (asset != null) Material = new JsonMaterial(asset.Material);
            }
        }


        public RawMaterialAsset ToRawMaterialAsset()
        {
            var asset = new RawMaterialAsset();

            asset.AssetId = AssetId;
            asset.Type = Type;
            asset.Timestamp = Timestamp.ToUnixTime();
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;
            asset.Description = Description;

            asset.Name = Name;
            asset.ContainerType = ContainerType;
            asset.ProcessKind = ProcessKind;
            asset.SerialNumber = SerialNumber;
            asset.Form = Form;
            asset.HasMaterial = HasMaterial;
            asset.ManufacturingDate = ManufacturingDate;
            asset.FirstUseDate = FirstUseDate;
            asset.LastUseDate = LastUseDate;
            asset.InitialVolume = InitialVolume;
            asset.InitialDimension = InitialDimension;
            asset.InitialQuantity = InitialQuantity;
            asset.CurrentVolume = CurrentVolume;
            asset.CurrentDimension = CurrentDimension;
            asset.CurrentQuantity = CurrentQuantity;

            if (Material != null)
            {
                asset.Material = Material.ToMaterial();
            }

            return asset;
        }
    }
}