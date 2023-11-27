// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonStreamsResponseDocument
    {
        [JsonPropertyName("MTConnectStreams")]
        public JsonMTConnectStreams MTConnectStreams { get; set; }


        public JsonStreamsResponseDocument() { }

        public JsonStreamsResponseDocument(IStreamsResponseOutputDocument streamsDocument)
        {
            if (streamsDocument != null)
            {
                MTConnectStreams = new JsonMTConnectStreams(streamsDocument);
            }
        }


        public IStreamsResponseDocument ToStreamsDocument()
        {
            if (MTConnectStreams != null)
            {
                return MTConnectStreams.ToStreamsDocument();
            }

            return null;
        }
    }
}