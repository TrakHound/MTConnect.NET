// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Output;
using System.Collections.Generic;

namespace MTConnect.Streams.Output
{
    /// <summary>
    /// DeviceStream is a XML container that organizes data reported from a single piece of equipment.A DeviceStream element MUST be provided for each piece of equipment reporting data in an MTConnectStreams document.
    /// </summary>
    public class DeviceStreamOutput : IDeviceStreamOutput
    {
        /// <summary>
        /// The name of an element or a piece of equipment. The name associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The uuid associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// An XML container type element that organizes data returned from an Agent in response to a current or sample HTTP request.
        /// </summary>
        public IComponentStreamOutput[] ComponentStreams { get; set; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        public IEnumerable<IObservationOutput> Observations
        {
            get
            {
                var l = new List<IObservationOutput>();

                if (!ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var componentStream in ComponentStreams)
                    {
                        if (!componentStream.Observations.IsNullOrEmpty())
                        {
                            l.AddRange(componentStream.Observations);
                        }
                    }
                }

                return l;
            }
        }


        public DeviceStreamOutput() { }

        public DeviceStreamOutput(IDeviceStream deviceStream)
        {
            if (deviceStream != null)
            {
                Name = deviceStream.Name;
                Uuid = deviceStream.Uuid;

                if (deviceStream.ComponentStreams != null)
                {
                    var componentStreams = new List<IComponentStreamOutput>();
                    foreach (var componentStream in deviceStream.ComponentStreams)
                    {
                        componentStreams.Add(new ComponentStreamOutput(componentStream));
                    }
                    ComponentStreams = componentStreams.ToArray();
                }
            }
        }
    }
}