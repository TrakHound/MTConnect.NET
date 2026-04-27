// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonMTConnectStreams
    {
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

        [JsonPropertyName("Header")]
        public JsonStreamsHeader Header { get; set; }

        [JsonPropertyName("Streams")]
        public JsonStreams Streams { get; set; }


        public JsonMTConnectStreams()
        {
            JsonVersion = 2;
        }

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


        public IStreamsResponseDocument ToStreamsDocument()
        {
            var streamsDocument = new StreamsResponseDocument();

            streamsDocument.Header = Header.ToStreamsHeader();
            streamsDocument.Streams = Streams.ToStreams();

            return streamsDocument;
        }
    }
}