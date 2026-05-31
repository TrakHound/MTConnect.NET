// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a typed container of device
    /// streams inside <c>MTConnectStreams</c>, keyed by the singular
    /// cppagent element name <c>DeviceStream</c>.
    /// </summary>
    public class JsonStreams
    {
        /// <summary>
        /// The device streams in the document.
        /// </summary>
        [JsonPropertyName("DeviceStream")]
        public List<JsonDeviceStream> DeviceStreams { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonStreams() { }

        /// <summary>
        /// Initializes the container from a strongly-typed streams
        /// document.
        /// </summary>
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


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="IDeviceStream"/> sequence.
        /// </summary>
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