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
        /// Contains the Header information in an MTConnect Streams Response document
        /// </summary>
        public IMTConnectStreamsHeader Header { get; set; }

        /// <summary>
        /// Streams is a container type Response element used to group the data reported from one or more pieces of equipment into a single Response document.
        /// </summary>
        public IDeviceStreamOutput[] Streams { get; set; }

        /// <summary>
        /// The MTConnect Version of the Response document
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// The sequence number of the earliest observation included in this document.
        /// </summary>
        public ulong FirstObservationSequence { get; set; }

        /// <summary>
        /// The sequence number of the latest observation included in this document.
        /// </summary>
        public ulong LastObservationSequence { get; set; }

        /// <summary>
        /// The total number of observations included in this document.
        /// </summary>
        public uint ObservationCount { get; set; }


        /// <summary>
        /// Initializes an empty streams output document.
        /// </summary>
        public StreamsResponseOutputDocument() { }

        /// <summary>
        /// Builds an output document from a deserialized streams response, copying its header and version and projecting each device stream into its output representation; a null source yields an empty document.
        /// </summary>
        /// <param name="document">The source streams response document.</param>
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


        /// <summary>
        /// Flattens the document's device streams into a single sequence of all contained observation outputs.
        /// </summary>
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