// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.QIF;
using MTConnect.Devices.Json;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.QIF
{
    /// <summary>
    /// JSON serialization surrogate for a <c>QIFDocumentWrapper</c> asset in
    /// the cppagent-compatible shape. Carries the standard asset envelope
    /// (asset id, timestamp, instance id, device uuid, removed flag) plus
    /// the embedded QIF (Quality Information Framework) document type and
    /// the document payload itself. Converts to and from the strongly-typed
    /// <see cref="QIFDocumentWrapperAsset"/>.
    /// </summary>
    public class JsonQIFDocumentWrapperAsset
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The asset type discriminator (always
        /// <c>QIFDocumentWrapper</c> for this asset).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The timestamp when the asset was reported.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The MTConnect Agent instance identifier this asset was reported
        /// against.
        /// </summary>
        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

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

        //[JsonPropertyName("description")]
        //public JsonDescription Description { get; set; }

        /// <summary>
        /// Free-form description of the asset.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }


        /// <summary>
        /// The kind of QIF document being wrapped (for example PLAN,
        /// RESULTS, RULES, STATISTICS).
        /// </summary>
        [JsonPropertyName("qifDocumentType")]
        public string QIFDocumentType { get; set; }

        /// <summary>
        /// The serialized QIF document content.
        /// </summary>
        [JsonPropertyName("qifDocument")]
        public string QIFDocument { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonQIFDocumentWrapperAsset() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="QIFDocumentWrapperAsset"/>, serializing the QIF
        /// document type as its enumeration name.
        /// </summary>
        public JsonQIFDocumentWrapperAsset(QIFDocumentWrapperAsset asset)
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

                QIFDocumentType = asset.QifDocumentType.ToString();
                //QifDocument = asset.QifDocument;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="QIFDocumentWrapperAsset"/>, parsing the QIF document
        /// type enumeration from its serialized form.
        /// </summary>
        public QIFDocumentWrapperAsset ToQIFDocumentWrapperAsset()
        {
            var asset = new QIFDocumentWrapperAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description;
            //if (Description != null) asset.Description = Description.ToDescription();

            asset.QifDocumentType = QIFDocumentType.ConvertEnum<QIFDocumentType>();
            //asset.QifDocument = QifDocument;
            return asset;
        }
    }
}