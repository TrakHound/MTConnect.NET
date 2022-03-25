// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// DeviceStream is a XML container that organizes data reported from a single piece of equipment.A DeviceStream element MUST be provided for each piece of equipment reporting data in an MTConnectStreams document.
    /// </summary>
    public class JsonDeviceStream
    {
        /// <summary>
        /// The name of an element or a piece of equipment. The name associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The uuid associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// An XML container type element that organizes data returned from an Agent in response to a current or sample HTTP request.
        /// </summary>
        [JsonPropertyName("componentStream")]
        public List<JsonComponentStream> ComponentStreams { get; set; }


        public JsonDeviceStream() { }

        public JsonDeviceStream(IDeviceStream deviceStream)
        {
            if (deviceStream != null)
            {
                Name = deviceStream.Name;
                Uuid = deviceStream.Uuid;

                var jsonComponentStreams = new List<JsonComponentStream>();
                if (!deviceStream.ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var stream in deviceStream.ComponentStreams)
                    {
                        var jsonStream = new JsonComponentStream(stream);
                        if (jsonStream != null && !jsonStream.Observations.IsNullOrEmpty()) jsonComponentStreams.Add(jsonStream);
                    }
                }

                ComponentStreams = jsonComponentStreams;
            }
        }


        public DeviceStream ToDeviceStream()
        {
            var deviceStream = new DeviceStream();

            deviceStream.Name = Name;
            deviceStream.Uuid = Uuid;

            if (!ComponentStreams.IsNullOrEmpty())
            {
                var componentStreams = new List<ComponentStream>();

                foreach (var xmlComponentStream in ComponentStreams)
                {
                    componentStreams.Add(xmlComponentStream.ToComponentStream());
                }

                deviceStream.ComponentStreams = componentStreams;
            }

            return deviceStream;
        }
    }
}
