// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using MTConnect.Devices.Json;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.RawMaterials
{
    public class JsonRawMaterialAsset
    {
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        //[JsonPropertyName("description")]
        //public JsonDescription Description { get; set; }
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

        [JsonPropertyName("Form")]
        public string Form { get; set; }

        [JsonPropertyName("HasMaterial")]
        public bool? HasMaterial { get; set; }

        [JsonPropertyName("ManufacturingDate")]
        public DateTime? ManufacturingDate { get; set; }

        [JsonPropertyName("FirstUseDate")]
        public DateTime? FirstUseDate { get; set; }

        [JsonPropertyName("LastUseDate")]
        public DateTime? LastUseDate { get; set; }

        [JsonPropertyName("InitialVolume")]
        public double? InitialVolume { get; set; }

        [JsonPropertyName("InitialDimension")]
        public string InitialDimension { get; set; }

        [JsonPropertyName("InitialQuantity")]
        public int? InitialQuantity { get; set; }

        [JsonPropertyName("CurrentVolume")]
        public double? CurrentVolume { get; set; }

        [JsonPropertyName("CurrentDimension")]
        public string CurrentDimension { get; set; }

        [JsonPropertyName("CurrentQuantity")]
        public int? CurrentQuantity { get; set; }

        [JsonPropertyName("Material")]
        public JsonMaterial Material { get; set; }


        public JsonRawMaterialAsset() { }

        public JsonRawMaterialAsset(IRawMaterialAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Timestamp = asset.Timestamp;
                InstanceId = asset.InstanceId;
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;

                if (asset.Description != null) Description = asset.Description;
                //if (asset.Description != null) Description = new JsonDescription(asset.Description);

                Name = asset.Name;
                ContainerType = asset.ContainerType;
                ProcessKind = asset.ProcessKind;
                SerialNumber = asset.SerialNumber;
                Form = asset.Form.ToString();
                HasMaterial = asset.HasMaterial;
                ManufacturingDate = asset.ManufacturingDate;
                FirstUseDate = asset.FirstUseDate;
                LastUseDate = asset.LastUseDate;
                InitialVolume = asset.InitialVolume;
                InitialDimension = asset.InitialDimension.ToString();
                InitialQuantity = asset.InitialQuantity;
                CurrentVolume = asset.CurrentVolume;
                CurrentDimension = asset.CurrentDimension.ToString();
                CurrentQuantity = asset.CurrentQuantity;

                if (asset != null) Material = new JsonMaterial(asset.Material);
            }
        }


        public IRawMaterialAsset ToRawMaterialAsset()
        {
            var asset = new RawMaterialAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description;
            //if (Description != null) asset.Description = Description.ToDescription();

            asset.Name = Name;
            asset.ContainerType = ContainerType;
            asset.ProcessKind = ProcessKind;
            asset.SerialNumber = SerialNumber;
            asset.Form = Form.ConvertEnum<Form>();
            asset.HasMaterial = HasMaterial;
            asset.ManufacturingDate = ManufacturingDate;
            asset.FirstUseDate = FirstUseDate;
            asset.LastUseDate = LastUseDate;
            asset.InitialVolume = InitialVolume;
            asset.InitialDimension = Millimeter3D.FromString(InitialDimension);
            asset.InitialQuantity = InitialQuantity;
            asset.CurrentVolume = CurrentVolume;
            asset.CurrentDimension = Millimeter3D.FromString(CurrentDimension);
            asset.CurrentQuantity = CurrentQuantity;

            if (Material != null)
            {
                asset.Material = Material.ToMaterial();
            }

            return asset;
        }
    }
}