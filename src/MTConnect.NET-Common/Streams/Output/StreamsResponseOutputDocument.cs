// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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

        public Version Version { get; set; }

        public long FirstObservationSequence { get; set; }

        public long LastObservationSequence { get; set; }

        public int ObservationCount { get; set; }



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
