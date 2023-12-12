// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.QIF;
using MTConnect.Devices.Json;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.QIF
{
    public class JsonQIFDocumentWrapperAsset
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


        [JsonPropertyName("qifDocumentType")]
        public string QIFDocumentType { get; set; }

        [JsonPropertyName("qifDocument")]
        public string QIFDocument { get; set; }


        public JsonQIFDocumentWrapperAsset() 
        {
            Type = QIFDocumentWrapperAsset.TypeId;
        }

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

                if (asset.Description != null) Description = new JsonDescription(asset.Description);

                QIFDocumentType = asset.QifDocumentType.ToString();
                QIFDocument = asset.QIFDocument;
            }
        }


        public IQIFDocumentWrapperAsset ToQIFDocumentWrapperAsset()
        {
            var asset = new QIFDocumentWrapperAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description.ToDescription();

            asset.QifDocumentType = QIFDocumentType.ConvertEnum<QIFDocumentType>();
            asset.QIFDocument = QIFDocument;
            return asset;
        }
    }
}