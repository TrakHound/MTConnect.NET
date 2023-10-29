// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// The Streams Information Model provides a representation of the data reported by a piece of equipment used for a manufacturing process, or used for any other purpose.
    /// </summary>
    public class JsonStreamsDocument
    {
        /// <summary>
        /// Contains the Header information in an MTConnect Streams XML document
        /// </summary>
        [JsonPropertyName("header")]
        public JsonStreamsHeader Header { get; set; }

        /// <summary>
        /// Streams is a container type XML element used to group the data reported from one or more pieces of equipment into a single XML document.
        /// </summary>
        [JsonPropertyName("streams")]
        public List<JsonDeviceStream> Streams { get; set; }


        public JsonStreamsDocument() { }

        public JsonStreamsDocument(IStreamsResponseOutputDocument streamsDocument)
        {
            if (streamsDocument != null)
            {
                Header = new JsonStreamsHeader(streamsDocument.Header);

                var xmlStreams = new List<JsonDeviceStream>();
                if (!streamsDocument.Streams.IsNullOrEmpty())
                {
                    foreach (var stream in streamsDocument.Streams)
                    {
                        var xmlStream = new JsonDeviceStream(stream);
                        if (xmlStream != null) xmlStreams.Add(xmlStream);
                    }
                }

                Streams = xmlStreams;
            }
        }


        public IStreamsResponseDocument ToStreamsDocument()
        {
            var streamsDocument = new StreamsResponseDocument();

            streamsDocument.Header = Header.ToStreamsHeader();

            if (!Streams.IsNullOrEmpty())
            {
                var deviceStreams = new List<DeviceStream>();

                foreach (var xmlStream in Streams)
                {
                    deviceStreams.Add(xmlStream.ToDeviceStream());
                }

                streamsDocument.Streams = deviceStreams;
            }

            return streamsDocument;
        }
    }
}