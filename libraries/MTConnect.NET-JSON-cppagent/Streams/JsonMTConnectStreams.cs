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

        [JsonPropertyName("schemaVersion")]
        public string SchemaVersion { get; set; }

        [JsonPropertyName("Header")]
        public JsonStreamsHeader Header { get; set; }

        [JsonPropertyName("Streams")]
        public JsonStreams Streams { get; set; }


        public JsonMTConnectStreams() 
        {
            JsonVersion = 2;
            SchemaVersion = "2.0";
        }

        public JsonMTConnectStreams(IStreamsResponseOutputDocument streamsDocument)
        {
            JsonVersion = 2;
            SchemaVersion = "2.0";

            if (streamsDocument != null)
            {
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