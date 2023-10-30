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
        public string QifDocumentType { get; set; }

        [JsonPropertyName("qifDocument")]
        public string QifDocument { get; set; }


        public JsonQIFDocumentWrapperAsset() { }

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

                QifDocumentType = asset.QifDocumentType.ToString();
                //QifDocument = asset.QIFDocument; ??
            }
        }


        public IQIFDocumentWrapperAsset ToQIFDocumentWrapperAsset()
        {
            var asset = new QIFDocumentWrapperAsset();

            asset.AssetId = AssetId;
            asset.Type = Type;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description.ToDescription();

            asset.QifDocumentType = QifDocumentType.ConvertEnum<QIFDocumentType>();
            //asset.QifDocument = QifDocument; ??
            return asset;
        }
    }
}