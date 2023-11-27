// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonDeviceStream
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("ComponentStream")]
        public List<JsonComponentStream> ComponentStreams { get; set; }


        public JsonDeviceStream() { }

        public JsonDeviceStream(IDeviceStreamOutput deviceStream)
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