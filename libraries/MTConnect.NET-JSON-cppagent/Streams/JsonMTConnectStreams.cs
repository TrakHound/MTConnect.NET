// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for the top-level
    /// <c>MTConnectStreams</c> document in the cppagent-compatible
    /// shape. Sits inside a <see cref="JsonStreamsResponseDocument"/>
    /// envelope and carries the wire-format version, the agent header,
    /// and the document's device streams. Converts to and from the
    /// strongly-typed <see cref="StreamsResponseDocument"/> model.
    /// </summary>
    public class JsonMTConnectStreams
    {
        /// <summary>
        /// The wire-format version of the cppagent JSON envelope
        /// emitted by this producer.
        /// </summary>
        [JsonPropertyName("jsonVersion")]
        public int JsonVersion { get; set; }

        /// <summary>
        /// Top-level <c>schemaVersion</c> identifies the envelope schema
        /// this DOCUMENT conforms to — the wire format the producer chose
        /// to emit. It is distinct from <c>Header.schemaVersion</c>, which
        /// identifies the AGENT's configured MTConnect Standard release
        /// (what the data inside refers to). The two fields are populated
        /// from independent sources and are not interchangeable.
        /// </summary>
        [JsonPropertyName("schemaVersion")]
        public string SchemaVersion { get; set; }

        /// <summary>
        /// The MTConnect Agent header.
        /// </summary>
        [JsonPropertyName("Header")]
        public JsonStreamsHeader Header { get; set; }

        /// <summary>
        /// The keyed device-stream container holding every device's
        /// observations.
        /// </summary>
        [JsonPropertyName("Streams")]
        public JsonStreams Streams { get; set; }


        /// <summary>
        /// Initializes a fresh container, defaulting
        /// <see cref="JsonVersion"/> to the current emitter version.
        /// </summary>
        public JsonMTConnectStreams()
        {
            JsonVersion = 2;
        }

        /// <summary>
        /// Initializes the container from a strongly-typed
        /// <see cref="IStreamsResponseOutputDocument"/>, capturing the
        /// agent schema version (distinct from
        /// <see cref="JsonVersion"/>).
        /// </summary>
        public JsonMTConnectStreams(IStreamsResponseOutputDocument streamsDocument)
        {
            JsonVersion = 2;

            if (streamsDocument != null)
            {
                SchemaVersion = streamsDocument.Version?.ToString();
                Header = new JsonStreamsHeader(streamsDocument.Header);
                Streams = new JsonStreams(streamsDocument);
            }
        }


        /// <summary>
        /// Converts the container to a strongly-typed
        /// <see cref="StreamsResponseDocument"/>.
        /// </summary>
        public IStreamsResponseDocument ToStreamsDocument()
        {
            var streamsDocument = new StreamsResponseDocument();

            streamsDocument.Header = Header.ToStreamsHeader();
            streamsDocument.Streams = Streams.ToStreams();

            return streamsDocument;
        }
    }
}