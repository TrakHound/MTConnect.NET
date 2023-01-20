// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using MTConnect.Observations.Output;
using System;
using System.Collections.Generic;

namespace MTConnect.Streams.Output
{
    /// <summary>
    /// The Streams Information Model provides a representation of the data reported by a piece of equipment used for a manufacturing process, or used for any other purpose.
    /// </summary>
    public class StreamsResponseOutputDocument : IStreamsResponseOutputDocument
    {
        /// <summary>
        /// Contains the Header information in an MTConnect Streams XML document
        /// </summary>
        public IMTConnectStreamsHeader Header { get; set; }

        /// <summary>
        /// Streams is a container type XML element used to group the data reported from one or more pieces of equipment into a single XML document.
        /// </summary>
        public IDeviceStreamOutput[] Streams { get; set; }

        /// <summary>
        /// The MTConnect Version of the Response document
        /// </summary>
        public Version Version { get; set; }

        public long FirstObservationSequence { get; set; }

        public long LastObservationSequence { get; set; }

        public int ObservationCount { get; set; }


        public StreamsResponseOutputDocument() { }

        public StreamsResponseOutputDocument(IStreamsResponseDocument document)  
        {
            if (document != null)
            {
                Header = document.Header;
                Version = document.Version;

                if (document.Streams != null)
                {
                    var streams = new List<IDeviceStreamOutput>();
                    foreach (var stream in document.Streams)
                    {
                        streams.Add(new DeviceStreamOutput(stream));
                    }
                    Streams = streams.ToArray();
                }
            }
        }


        public IEnumerable<IObservationOutput> GetObservations()
        {
            if (!Streams.IsNullOrEmpty())
            {
                var observations = new List<IObservationOutput>();

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