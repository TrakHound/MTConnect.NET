// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams
{
    /// <summary>
    /// The Streams Information Model provides a representation of the data reported by a piece of equipment used for a manufacturing process, or used for any other purpose.
    /// </summary>
    [XmlRoot("MTConnectStreams")]
    public class StreamsResponseDocument : IStreamsResponseDocument
    {
        /// <summary>
        /// Contains the Header information in an MTConnect Streams XML document
        /// </summary>
        [XmlElement("Header")]
        [JsonPropertyName("header")]
        public IMTConnectStreamsHeader Header { get; set; }

        /// <summary>
        /// Streams is a container type XML element used to group the data reported from one or more pieces of equipment into a single XML document.
        /// </summary>
        [XmlArray("Streams")]
        [XmlArrayItem("DeviceStream")]
        [JsonPropertyName("streams")]
        public IEnumerable<IDeviceStream> Streams { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Version Version { get; set; }


        public IEnumerable<IObservation> GetObservations()
        {
            if (!Streams.IsNullOrEmpty())
            {
                var observations = new List<IObservation>();

                foreach (var stream in Streams)
                {
                    if (!stream.Observations.IsNullOrEmpty())
                    {
                        observations.AddRange(stream.Observations);
                    }
                }

                return observations;
            }

            return null;
        }
    }
}
