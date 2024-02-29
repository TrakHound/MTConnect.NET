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

        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }


        public JsonStreamsHeader() { }

        public JsonStreamsHeader(IMTConnectStreamsHeader header)
        {
            if (header != null)
            {
                InstanceId = header.InstanceId;
                Version = header.Version;
                Sender = header.Sender;
                BufferSize = header.BufferSize;
                FirstSequence = header.FirstSequence;
                LastSequence = header.LastSequence;
                NextSequence = header.NextSequence;
                DeviceModelChangeTime = header.DeviceModelChangeTime;
                TestIndicator = header.TestIndicator;
                CreationTime = header.CreationTime;
            }
        }


        public virtual IMTConnectStreamsHeader ToStreamsHeader()
        {
            var header = new MTConnectStreamsHeader();
            header.InstanceId = InstanceId;
            header.Version = Version;
            header.Sender = Sender;
            header.BufferSize = BufferSize;
            header.FirstSequence = FirstSequence;
            header.LastSequence = LastSequence;
            header.NextSequence = NextSequence;
            header.DeviceModelChangeTime = DeviceModelChangeTime;
            header.TestIndicator = TestIndicator;
            header.CreationTime = CreationTime;
            return header;
        }
    }
}