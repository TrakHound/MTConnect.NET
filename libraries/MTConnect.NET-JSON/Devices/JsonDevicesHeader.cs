// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for the <c>Header</c> of an
    /// MTConnectDevices document. Mirrors the on-the-wire shape so the JSON
    /// serializer can read and write it, then converts to and from the
    /// strongly-typed <see cref="MTConnectDevicesHeader"/> model.
    /// </summary>
    public class JsonDevicesHeader
    {
        /// <summary>
        /// The instance identifier of the agent, which changes whenever the
        /// agent's buffer is cleared.
        /// </summary>
        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

        /// <summary>
        /// The MTConnect schema version of the document.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// The identifier of the agent that produced the document.
        /// </summary>
        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        /// <summary>
        /// The maximum number of observations the agent's buffer can hold.
        /// </summary>
        [JsonPropertyName("bufferSize")]
        public ulong BufferSize { get; set; }

        /// <summary>
        /// The maximum number of assets the agent can store.
        /// </summary>
        [JsonPropertyName("assetBufferSize")]
        public ulong AssetBufferSize { get; set; }

        /// <summary>
        /// The number of assets currently stored by the agent.
        /// </summary>
        [JsonPropertyName("assetCount")]
        public ulong AssetCount { get; set; }

        /// <summary>
        /// The timestamp of the most recent change to the device model.
        /// </summary>
        [JsonPropertyName("deviceModelChangeTime")]
        public string DeviceModelChangeTime { get; set; }

        /// <summary>
        /// Whether the document was produced for testing rather than
        /// production use.
        /// </summary>
        [JsonPropertyName("testIndicator")]
        public bool TestIndicator { get; set; }

        /// <summary>
        /// The time the document was created.
        /// </summary>
        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDevicesHeader() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IMTConnectDevicesHeader"/>.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IMTConnectDevicesHeader"/>.
        /// </summary>
        public virtual IMTConnectDevicesHeader ToDevicesHeader()
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