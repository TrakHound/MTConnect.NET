// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDevicesHeader
    {
        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("schemaVersion")]
        public string SchemaVersion { get; set; }

        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        [JsonPropertyName("bufferSize")]
        public ulong BufferSize { get; set; }

        [JsonPropertyName("assetBufferSize")]
        public ulong AssetBufferSize { get; set; }

        [JsonPropertyName("assetCount")]
        public ulong AssetCount { get; set; }

        [JsonPropertyName("deviceModelChangeTime")]
        public string DeviceModelChangeTime { get; set; }

        [JsonPropertyName("testIndicator")]
        public bool TestIndicator { get; set; }

        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }


        public JsonDevicesHeader() { }

        public JsonDevicesHeader(IMTConnectDevicesHeader header)
        {
            if (header != null)
            {
                InstanceId = header.InstanceId;
                Version = header.Version;
                Sender = header.Sender;
                BufferSize = header.BufferSize;
                AssetBufferSize = header.AssetBufferSize;
                AssetCount = header.AssetCount;
                DeviceModelChangeTime = header.DeviceModelChangeTime;
                TestIndicator = header.TestIndicator;
                CreationTime = header.CreationTime;
            }
        }


        public IMTConnectDevicesHeader ToDevicesHeader()
        {
            var header = new MTConnectDevicesHeader();
            header.InstanceId = InstanceId;
            header.Version = Version;
            header.Sender = Sender;
            header.BufferSize = BufferSize;
            header.AssetBufferSize = AssetBufferSize;
            header.AssetCount = AssetCount;
            header.DeviceModelChangeTime = DeviceModelChangeTime;
            header.TestIndicator = TestIndicator;
            header.CreationTime = CreationTime;
            return header;
        }
    }
}