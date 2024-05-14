// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using MTConnect.Observations;
using System;
using System.Collections.Generic;

namespace MTConnect.Streams
{
    /// <summary>
    /// Root entity of an MTConnectStreams Response Document that contains the Observation Information Model of one or more Device entities.
    /// </summary>
    public class StreamsResponseDocument : IStreamsResponseDocument
    {
        /// <summary>
        /// Provides information from an agent defining version information, storage capacity, and parameters associated with the data management within the agent.
        /// </summary>
        public IMTConnectStreamsHeader Header { get; set; }

        /// <summary>
        /// Streams groups one or more DeviceStream entities.
        /// </summary>
        public IEnumerable<IDeviceStream> Streams { get; set; }

        /// <summary>
        /// The MTConnect Version of the Response document
        /// </summary>
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