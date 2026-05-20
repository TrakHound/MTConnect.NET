// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.RawMaterials
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect <c>RawMaterial</c> asset.
    /// Mirrors the on-the-wire shape so the JSON serializer can read and write
    /// it, then converts to and from the strongly-typed
    /// <see cref="RawMaterialAsset"/> model.
    /// </summary>
    public class JsonRawMaterialAsset
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The asset type identifier, <c>RawMaterial</c>.
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
        /// The human-readable name of the raw material.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of container holding the raw material, such as a bottle or
        /// cartridge.
        /// </summary>
        [JsonPropertyName("containerType")]
        public string ContainerType { get; set; }

        /// <summary>
        /// The manufacturing process the raw material is intended for.
        /// </summary>
        [JsonPropertyName("processKind")]
        public string ProcessKind { get; set; }

        /// <summary>
        /// The serial number that uniquely identifies this raw material.
        /// </summary>
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The physical form of the material (for example BAR, BLOCK, or
        /// POWDER), serialized as the enumeration name.
        /// </summary>
        [JsonPropertyName("form")]
        public string Form { get; set; }

        /// <summary>
        /// Whether the container currently holds material.
        /// </summary>
        [JsonPropertyName("hasMaterial")]
        public bool? HasMaterial { get; set; }

        /// <summary>
        /// The date the raw material was manufactured.
        /// </summary>
        [JsonPropertyName("manufacturingDate")]
        public DateTime? ManufacturingDate { get; set; }

        /// <summary>
        /// The date the raw material was first used.
        /// </summary>
        [JsonPropertyName("firstUseDate")]
        public DateTime? FirstUseDate { get; set; }

        /// <summary>
        /// The date the raw material was last used.
        /// </summary>
        [JsonPropertyName("lastUseDate")]
        public DateTime? LastUseDate { get; set; }

        /// <summary>
        /// The volume of unused material when first received.
        /// </summary>
        [JsonPropertyName("initialVolume")]
        public double? InitialVolume { get; set; }

        /// <summary>
        /// The dimension of unused material when first received, serialized as
        /// a millimeter 3D string.
        /// </summary>
        [JsonPropertyName("initialDimension")]
        public string InitialDimension { get; set; }

        /// <summary>
        /// The quantity of unused material when first received.
        /// </summary>
        [JsonPropertyName("initialQuantity")]
        public int? InitialQuantity { get; set; }

        /// <summary>
        /// The volume of material currently remaining.
        /// </summary>
        [JsonPropertyName("currentVolume")]
        public double? CurrentVolume { get; set; }

        /// <summary>
        /// The dimension of material currently remaining, serialized as a
        /// millimeter 3D string.
        /// </summary>
        [JsonPropertyName("currentDimension")]
        public string CurrentDimension { get; set; }

        /// <summary>
        /// The quantity of material currently remaining.
        /// </summary>
        [JsonPropertyName("currentQuantity")]
        public int? CurrentQuantity { get; set; }

        /// <summary>
        /// The material the raw material is made of.
        /// </summary>
        [JsonPropertyName("material")]
        public JsonMaterial Material { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization, defaulting
        /// <see cref="Type"/> to the RawMaterial type identifier.
        /// </summary>
        public JsonRawMaterialAsset()
        {
            Type = RawMaterialAsset.TypeId;
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IRawMaterialAsset"/>, converting enumerations and
        /// dimensions to their string representations.
        /// </summary>
        public JsonRawMaterialAsset(IRawMaterialAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp;
                InstanceId = asset.InstanceId;
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;

                if (asset.Description != null) Description = asset.Description; // v2.5
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IRawMaterialAsset"/>, parsing enumerations and
        /// millimeter 3D dimension strings back into typed values.
        /// </summary>
        public IRawMaterialAsset ToRawMaterialAsset()
        {
            var asset = new RawMaterialAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description; // v2.5
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