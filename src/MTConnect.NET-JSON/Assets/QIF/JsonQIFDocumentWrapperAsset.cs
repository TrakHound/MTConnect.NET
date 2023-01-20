// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.QIF
{
    public class JsonQIFDocumentWrapperAsset
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


        [JsonPropertyName("qifDocumentType")]
        public string QifDocumentType { get; set; }

        [JsonPropertyName("qifDocument")]
        public string QifDocument { get; set; }


        public JsonQIFDocumentWrapperAsset() { }

        public JsonQIFDocumentWrapperAsset(QIFDocumentWrapperAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp.ToDateTime();
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;
                Description = asset.Description;

                QifDocumentType = asset.QifDocumentType;
                QifDocument = asset.QifDocument;
            }
        }


        public QIFDocumentWrapperAsset ToQIFDocumentWrapperAsset()
        {
            var asset = new QIFDocumentWrapperAsset();

            asset.AssetId = AssetId;
            asset.Type = Type;
            asset.Timestamp = Timestamp.ToUnixTime();
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;
            asset.Description = Description;

            asset.QifDocumentType = QifDocumentType;
            asset.QifDocument = QifDocument;
            return asset;
        }
    }
}