// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonStreams
    {
        [JsonPropertyName("DeviceStream")]
        public List<JsonDeviceStream> DeviceStreams { get; set; }


        public JsonStreams() { }

        public JsonStreams(IStreamsResponseOutputDocument streamsDocument)
        {
            if (streamsDocument != null)
            {
                var jsonStreams = new List<JsonDeviceStream>();
                if (!streamsDocument.Streams.IsNullOrEmpty())
                {
                    foreach (var stream in streamsDocument.Streams)
                    {
                        var jsonStream = new JsonDeviceStream(stream);
                        if (jsonStream != null) jsonStreams.Add(jsonStream);
                    }
                }

                DeviceStreams = jsonStreams;
            }
        }


        public IEnumerable<IDeviceStream> ToStreams()
        {
            if (!DeviceStreams.IsNullOrEmpty())
            {
                var deviceStreams = new List<DeviceStream>();

                foreach (var jsonStream in DeviceStreams)
                {
                    deviceStreams.Add(jsonStream.ToDeviceStream());
                }

                return deviceStreams;
            }

            return null;
        }
    }
}