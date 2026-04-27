// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonStreamsHeader
    {
        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// The major and minor number of the MTConnect Standard schema the Response Document conforms to (for example "2.7").
        /// Mirrors the cppagent v2 wire shape that emits `schemaVersion` on every Header.
        /// </summary>
        [JsonPropertyName("schemaVersion")]
        public string SchemaVersion { get; set; }

        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        [JsonPropertyName("bufferSize")]
        public ulong BufferSize { get; set; }

        [JsonPropertyName("firstSequence")]
        public ulong FirstSequence { get; set; }

        [JsonPropertyName("lastSequence")]
        public ulong LastSequence { get; set; }

        [JsonPropertyName("nextSequence")]
        public ulong NextSequence { get; set; }

        [JsonPropertyName("deviceModelChangeTime")]
        public string DeviceModelChangeTime { get; set; }

        [JsonPropertyName("testIndicator")]
        public bool TestIndicator { get; set; }

        /// <summary>
        /// Indicates if the MTConnect Agent is validating against the normative model.
        /// Mirrors the cppagent v2 wire shape that emits `validation` on every Header.
        /// </summary>
        [JsonPropertyName("validation")]
        public bool Validation { get; set; }

        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }


        public JsonStreamsHeader() { }

        public JsonStreamsHeader(IMTConnectStreamsHeader header)
        {
            if (header != null)
            {
                InstanceId = header.InstanceId;
                Version = header.Version;
                SchemaVersion = header.SchemaVersion;
                Sender = header.Sender;
                BufferSize = header.BufferSize;
                FirstSequence = header.FirstSequence;
                LastSequence = header.LastSequence;
                NextSequence = header.NextSequence;
                DeviceModelChangeTime = header.DeviceModelChangeTime;
                TestIndicator = header.TestIndicator;
                Validation = header.Validation;
                CreationTime = header.CreationTime;
            }
        }


        public virtual IMTConnectStreamsHeader ToStreamsHeader()
        {
            var header = new MTConnectStreamsHeader();
            header.InstanceId = InstanceId;
            header.Version = Version;
            header.SchemaVersion = SchemaVersion;
            header.Sender = Sender;
            header.BufferSize = BufferSize;
            header.FirstSequence = FirstSequence;
            header.LastSequence = LastSequence;
            header.NextSequence = NextSequence;
            header.DeviceModelChangeTime = DeviceModelChangeTime;
            header.TestIndicator = TestIndicator;
            header.Validation = Validation;
            header.CreationTime = CreationTime;
            return header;
        }
    }
}