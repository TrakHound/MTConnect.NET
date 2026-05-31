// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.QIF;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.QIF
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect
    /// <c>QIFDocumentWrapper</c> asset, which embeds a Quality Information
    /// Framework document. Mirrors the on-the-wire shape so the JSON
    /// serializer can read and write it, then converts to and from the
    /// strongly-typed <see cref="QIFDocumentWrapperAsset"/> model.
    /// </summary>
    public class JsonQIFDocumentWrapperAsset
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The asset type identifier, <c>QIFDocumentWrapper</c>.
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
        /// The QIF document type (for example MEASUREMENT_RESOURCE or PLAN),
        /// serialized as the enumeration name.
        /// </summary>
        [JsonPropertyName("qifDocumentType")]
        public string QIFDocumentType { get; set; }

        /// <summary>
        /// The embedded QIF document content.
        /// </summary>
        [JsonPropertyName("qifDocument")]
        public string QIFDocument { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization, defaulting
        /// <see cref="Type"/> to the QIFDocumentWrapper type identifier.
        /// </summary>
        public JsonQIFDocumentWrapperAsset()
        {
            Type = QIFDocumentWrapperAsset.TypeId;
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IQIFDocumentWrapperAsset"/>, converting the document type
        /// enumeration to a string.
        /// </summary>
        public JsonQIFDocumentWrapperAsset(IQIFDocumentWrapperAsset asset)
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

                QIFDocumentType = asset.QifDocumentType.ToString();
                QIFDocument = asset.QIFDocument;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IQIFDocumentWrapperAsset"/>, parsing the document type
        /// enumeration.
        /// </summary>
        public IQIFDocumentWrapperAsset ToQIFDocumentWrapperAsset()
        {
            var asset = new QIFDocumentWrapperAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description; // v2.5
            //if (Description != null) asset.Description = Description.ToDescription();

            asset.QifDocumentType = QIFDocumentType.ConvertEnum<QIFDocumentType>();
            asset.QIFDocument = QIFDocument;
            return asset;
        }
    }
}