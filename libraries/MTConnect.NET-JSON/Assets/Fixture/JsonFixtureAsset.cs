// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Fixture;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Fixture
{
    /// <summary>
    /// JSON surrogate for an MTConnect <c>Fixture</c> asset (2.5+).
    /// </summary>
    public class JsonFixtureAsset
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The MTConnect asset type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The time the asset was last modified.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The instance id of the agent that produced the asset.
        /// </summary>
        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

        /// <summary>
        /// The UUID of the device that supplied the asset.
        /// </summary>
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// Whether the asset has been removed.
        /// </summary>
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        /// <summary>
        /// The free-form description of the asset.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }


        /// <summary>
        /// The identifier of the fixture.
        /// </summary>
        [JsonPropertyName("fixtureId")]
        public string FixtureId { get; set; }

        /// <summary>
        /// The number of the fixture.
        /// </summary>
        [JsonPropertyName("fixtureNumber")]
        public int? FixtureNumber { get; set; }

        /// <summary>
        /// The actuation type of the clamping mechanism.
        /// </summary>
        [JsonPropertyName("clampingMethod")]
        public string ClampingMethod { get; set; }

        /// <summary>
        /// The actuation type of the mounting mechanism.
        /// </summary>
        [JsonPropertyName("mountingMethod")]
        public string MountingMethod { get; set; }

        /// <summary>
        /// The date the asset was made; omitted when not set.
        /// </summary>
        [JsonPropertyName("manufactureDate")]
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// The date the asset was last calibrated; omitted when not set.
        /// </summary>
        [JsonPropertyName("calibrationDate")]
        public DateTime? CalibrationDate { get; set; }

        /// <summary>
        /// The date the asset was last inspected; omitted when not set.
        /// </summary>
        [JsonPropertyName("inspectionDate")]
        public DateTime? InspectionDate { get; set; }

        /// <summary>
        /// The date the asset is next due for inspection; omitted when not set.
        /// </summary>
        [JsonPropertyName("nextInspectionDate")]
        public DateTime? NextInspectionDate { get; set; }

        /// <summary>
        /// The measurements of the asset (weight, height, width, length, swing and their loaded counterparts).
        /// </summary>
        [JsonPropertyName("measurements")]
        public List<JsonPhysicalMeasurement> Measurements { get; set; }


        /// <summary>
        /// Creates an empty surrogate for deserialization.
        /// </summary>
        public JsonFixtureAsset()
        {
            Type = FixtureAsset.TypeId;
        }

        /// <summary>
        /// Creates the surrogate from <paramref name="asset"/>.
        /// </summary>
        public JsonFixtureAsset(IFixtureAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp;
                InstanceId = asset.InstanceId;
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;
                Description = asset.Description;

                FixtureId = asset.FixtureId;
                if (asset.FixtureNumber != 0) FixtureNumber = asset.FixtureNumber;
                ClampingMethod = asset.ClampingMethod;
                MountingMethod = asset.MountingMethod;
                ManufactureDate = JsonPhysicalMeasurement.ToNullable(asset.ManufactureDate);
                CalibrationDate = JsonPhysicalMeasurement.ToNullable(asset.CalibrationDate);
                InspectionDate = JsonPhysicalMeasurement.ToNullable(asset.InspectionDate);
                NextInspectionDate = JsonPhysicalMeasurement.ToNullable(asset.NextInspectionDate);
                Measurements = JsonPhysicalMeasurement.Create(asset.Measurements);
            }
        }


        /// <summary>
        /// Converts this surrogate into the strongly-typed model.
        /// </summary>
        public IFixtureAsset ToFixtureAsset()
        {
            var asset = new FixtureAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.InstanceId = InstanceId;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;
            asset.Description = Description;

            asset.FixtureId = FixtureId;
            asset.FixtureNumber = FixtureNumber ?? 0;
            asset.ClampingMethod = ClampingMethod;
            asset.MountingMethod = MountingMethod;
            asset.ManufactureDate = ManufactureDate ?? default;
            asset.CalibrationDate = CalibrationDate ?? default;
            asset.InspectionDate = InspectionDate ?? default;
            asset.NextInspectionDate = NextInspectionDate ?? default;
            asset.Measurements = JsonPhysicalMeasurement.ToMeasurements(Measurements);

            return asset;
        }
    }
}
