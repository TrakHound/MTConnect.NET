// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>DeviceStream</c>, the
    /// observations reported for a single device. Mirrors the on-the-wire
    /// shape so the JSON serializer can read and write it, then converts to
    /// and from the strongly-typed <see cref="DeviceStream"/> model.
    /// </summary>
    public class JsonDeviceStream
    {
        /// <summary>
        /// The name of the device the observations belong to.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The UUID of the device the observations belong to.
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// The per-component observation streams of the device.
        /// </summary>
        [JsonPropertyName("componentStream")]
        public List<JsonComponentStream> ComponentStreams { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDeviceStream() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDeviceStreamOutput"/>, including only component streams
        /// that contain at least one observation.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="DeviceStream"/>, converting each component stream.
        /// </summary>
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