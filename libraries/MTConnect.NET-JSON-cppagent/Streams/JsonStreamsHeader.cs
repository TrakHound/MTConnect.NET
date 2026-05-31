// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for the <c>Header</c> of an
    /// MTConnectStreams document in the cppagent-compatible shape.
    /// </summary>
    public class JsonStreamsHeader
    {
        /// <summary>
        /// The instance identifier of the agent, which changes whenever the
        /// agent's buffer is cleared.
        /// </summary>
        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

        /// <summary>
        /// The agent's version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// The major and minor number of the MTConnect Standard schema the Response Document conforms to (for example "2.7").
        /// Mirrors the cppagent v2 wire shape that emits `schemaVersion` on every Header.
        /// </summary>
        [JsonPropertyName("schemaVersion")]
        public string SchemaVersion { get; set; }

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
        /// The sequence number of the oldest observation still in the buffer.
        /// </summary>
        [JsonPropertyName("firstSequence")]
        public ulong FirstSequence { get; set; }

        /// <summary>
        /// The sequence number of the newest observation in the buffer.
        /// </summary>
        [JsonPropertyName("lastSequence")]
        public ulong LastSequence { get; set; }

        /// <summary>
        /// The sequence number the next observation will be assigned.
        /// </summary>
        [JsonPropertyName("nextSequence")]
        public ulong NextSequence { get; set; }

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
        /// Indicates if the MTConnect Agent is validating against the normative model.
        /// Mirrors the cppagent v2 wire shape that emits `validation` on every Header.
        /// </summary>
        [JsonPropertyName("validation")]
        public bool Validation { get; set; }

        /// <summary>
        /// The time the document was created.
        /// </summary>
        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonStreamsHeader() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IMTConnectStreamsHeader"/>.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IMTConnectStreamsHeader"/>.
        /// </summary>
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