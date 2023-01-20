// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MTConnect.Streams.Output;

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
        public MTConnectStreamsHeader Header { get; set; }

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
                var header = new MTConnectStreamsHeader();
                header.InstanceId = streamsDocument.Header.InstanceId;
                header.Version = streamsDocument.Header.Version;
                header.Sender = streamsDocument.Header.Sender;
                header.BufferSize = streamsDocument.Header.BufferSize;
                header.FirstSequence = streamsDocument.Header.FirstSequence;
                header.LastSequence = streamsDocument.Header.LastSequence;
                header.NextSequence = streamsDocument.Header.NextSequence;
                header.DeviceModelChangeTime = streamsDocument.Header.DeviceModelChangeTime;
                header.TestIndicator = streamsDocument.Header.TestIndicator;
                header.CreationTime = streamsDocument.Header.CreationTime;
                Header = header;

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
            streamsDocument.Header = Header;
            
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
